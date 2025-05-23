﻿using Module.Answers.Mappers;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Module.Forms.Mappers;
using AttachmentModel = Feature.CitizenReports.Models.AttachmentModel;
using NoteModel = Feature.CitizenReports.Models.NoteModel;

namespace Feature.CitizenReports.GetById;

public class Endpoint(
    VoteMonitorContext context,
    IAuthorizationService authorizationService,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(s => { s.Summary = "Gets citizen report by id including notes and attachments"; });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new CitizenReportingNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }
        
        var citizenReport = await context
            .CitizenReports
            .Include(x => x.Attachments)
            .Include(x => x.Notes)
            .Include(x => x.Location)
            .Where(x =>
                x.ElectionRoundId == req.ElectionRoundId
                && x.Form.MonitoringNgo.NgoId == req.NgoId
                && x.Form.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                && x.Id == req.CitizenReportId)
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (citizenReport == null)
        {
            return TypedResults.NotFound();
        }

        var form = await context
            .Forms
            .Where(x =>
                x.ElectionRoundId == req.ElectionRoundId
                && x.Id == citizenReport.FormId)
            .FirstAsync(ct);

        var tasks = citizenReport.Attachments
            .Select(AttachmentModel.FromEntity)
            .Select(async attachment =>
            {
                var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                    attachment.FilePath,
                    attachment.UploadedFileName);

                return attachment with
                {
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0
                };
            }).ToArray();

        var attachments = await Task.WhenAll(tasks);
        var response = new Response
        {
            CitizenReportId = citizenReport.Id,
            FormId = form.Id,
            FormCode = form.Code,
            FormName = form.Name,
            FormDefaultLanguage = form.DefaultLanguage,
            Answers = citizenReport.Answers.Select(AnswerMapper.ToModel).ToArray(),
            Notes = citizenReport.Notes.Select(NoteModel.FromEntity).ToArray(),
            Attachments = attachments,
            Questions = form.Questions.Select(QuestionsMapper.ToModel).ToArray(),

            TimeSubmitted = citizenReport.LastModifiedOn ?? citizenReport.CreatedOn,
            FollowUpStatus = citizenReport.FollowUpStatus,

            LocationId = citizenReport.Location.Level1,
            LocationLevel1 = citizenReport.Location.Level1,
            LocationLevel2 = citizenReport.Location.Level2,
            LocationLevel3 = citizenReport.Location.Level3,
            LocationLevel4 = citizenReport.Location.Level4,
            LocationLevel5 = citizenReport.Location.Level5
        };

        return TypedResults.Ok(response);
    }
}
