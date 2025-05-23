﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.AttachmentAggregate;
using Vote.Monitor.Domain.Entities.CitizenGuideAggregate;
using Vote.Monitor.Domain.Entities.CitizenNotificationAggregate;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.CitizenReportAttachmentAggregate;
using Vote.Monitor.Domain.Entities.CitizenReportNoteAggregate;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.Entities.FeedbackAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportAttachmentAggregate;
using Vote.Monitor.Domain.Entities.IncidentReportNoteAggregate;
using Vote.Monitor.Domain.Entities.LocationAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAdminAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;
using Vote.Monitor.Domain.Entities.NoteAggregate;
using Vote.Monitor.Domain.Entities.NotificationAggregate;
using Vote.Monitor.Domain.Entities.NotificationStubAggregate;
using Vote.Monitor.Domain.Entities.NotificationTokenAggregate;
using Vote.Monitor.Domain.Entities.ObserverAggregate;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Vote.Monitor.Domain;

public class VoteMonitorContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public VoteMonitorContext(DbContextOptions<VoteMonitorContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Ngo> Ngos { get; set; }
    public DbSet<NgoAdmin> NgoAdmins { get; set; }
    public DbSet<Observer> Observers { get; set; }
    public DbSet<PollingStation> PollingStations { get; set; }
    public DbSet<ElectionRound> ElectionRounds { get; set; }
    public DbSet<ImportValidationErrors> ImportValidationErrors { set; get; }
    public DbSet<Trail> AuditTrails => Set<Trail>();
    public DbSet<FormTemplate> FormTemplates { set; get; }
    public DbSet<Entities.FormAggregate.Form> Forms { set; get; }
    public DbSet<FormSubmission> FormSubmissions { set; get; }
    public DbSet<PollingStationInformationForm> PollingStationInformationForms { set; get; }
    public DbSet<PollingStationInformation> PollingStationInformation { set; get; }
    public DbSet<MonitoringNgo> MonitoringNgos { set; get; }
    public DbSet<MonitoringObserver> MonitoringObservers { set; get; }
    public DbSet<ObserverGuide> ObserversGuides { set; get; }
    public DbSet<NotificationToken> NotificationTokens { set; get; }
    public DbSet<Notification> Notifications { set; get; }
    public DbSet<Attachment> Attachments { set; get; }
    public DbSet<Note> Notes { set; get; }
    public DbSet<NotificationStub> NotificationStubs { get; set; }
    public DbSet<ExportedData> ExportedData { get; set; }
    public DbSet<QuickReport> QuickReports { get; set; }
    public DbSet<QuickReportAttachment> QuickReportAttachments { get; set; }
    public DbSet<Feedback> UserFeedback { get; set; }
    public DbSet<CitizenReport> CitizenReports { get; set; }
    public DbSet<CitizenReportNote> CitizenReportNotes { get; set; }
    public DbSet<CitizenReportAttachment> CitizenReportAttachments { get; set; }
    public DbSet<CitizenNotification> CitizenNotifications { get; set; }
    public DbSet<MonitoringObserverNotification> MonitoringObserverNotification { get; set; }
    public DbSet<CitizenGuide> CitizenGuides { get; set; }
    public DbSet<Location> Locations { get; set; }

    public DbSet<IncidentReport> IncidentReports { get; set; }
    public DbSet<IncidentReportNote> IncidentReportNotes { get; set; }
    public DbSet<IncidentReportAttachment> IncidentReportAttachments { get; set; }
    public DbSet<Coalition> Coalitions { get; set; }
    public DbSet<CoalitionMembership> CoalitionMemberships { get; set; }
    public DbSet<CoalitionFormAccess> CoalitionFormAccess { get; set; }
    public DbSet<CoalitionGuideAccess> CoalitionGuideAccess { get; set; }
    public DbSet<ElectionRoundFormTemplate> ElectionRoundFormTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasPostgresExtension("uuid-ossp");

        var jsonbObjectKeys = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.ObjectKeys), new[] { typeof(JsonDocument) });

        var unnest = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.Unnest), new[] { typeof(string[]) });

        var arrayUnique = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.ArrayUnique), new[] { typeof(string[]) });

        var arrayRemove = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.ArrayRemove), new[] { typeof(string[]), typeof(string) });

        var arrayDiff = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.ArrayDiff), new[] { typeof(string[]), typeof(string[]) });

        builder
            .HasDbFunction(jsonbObjectKeys!)
            .HasName("jsonb_object_keys");

        builder
            .HasDbFunction(unnest!)
            .HasName("unnest");

        builder
            .HasDbFunction(arrayUnique!)
            .HasName(CustomDBFunctions.ArrayUnique);

        builder
            .HasDbFunction(arrayRemove!)
            .HasName("array_remove");

        builder
            .HasDbFunction(arrayDiff!)
            .HasName(CustomDBFunctions.ArrayDiff);

        builder.ApplyConfiguration(new ApplicationUserConfiguration());
        builder.ApplyConfiguration(new NgoAdminConfiguration());
        builder.ApplyConfiguration(new ObserverConfiguration());
        builder.ApplyConfiguration(new CountryConfiguration());
        builder.ApplyConfiguration(new LanguageConfiguration());
        builder.ApplyConfiguration(new NgoConfiguration());
        builder.ApplyConfiguration(new ElectionRoundConfiguration());
        builder.ApplyConfiguration(new MonitoringNgoConfiguration());
        builder.ApplyConfiguration(new MonitoringObserverConfiguration());
        builder.ApplyConfiguration(new PollingStationConfiguration());
        builder.ApplyConfiguration(new ImportValidationErrorsConfiguration());
        builder.ApplyConfiguration(new FormTemplateConfiguration());
        builder.ApplyConfiguration(new NotificationConfiguration());
        builder.ApplyConfiguration(new NotificationTokenConfiguration());
        builder.ApplyConfiguration(new AttachmentConfiguration());
        builder.ApplyConfiguration(new NoteConfiguration());
        builder.ApplyConfiguration(new PollingStationInformationFormConfiguration());
        builder.ApplyConfiguration(new PollingStationInformationConfiguration());
        builder.ApplyConfiguration(new ObserverGuideConfiguration());
        builder.ApplyConfiguration(new FormConfiguration());
        builder.ApplyConfiguration(new FormSubmissionConfiguration());
        builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfiguration(new NotificationStubConfiguration());
        builder.ApplyConfiguration(new ExportedDataConfiguration());
        builder.ApplyConfiguration(new QuickReportConfiguration());

        builder.ApplyConfiguration(new CitizenReportConfiguration());
        builder.ApplyConfiguration(new CitizenReportNoteConfiguration());
        builder.ApplyConfiguration(new CitizenReportAttachmentConfiguration());
        builder.ApplyConfiguration(new CitizenGuideConfiguration());
        builder.ApplyConfiguration(new CitizenNotificationConfiguration());

        builder.ApplyConfiguration(new IncidentReportConfiguration());
        builder.ApplyConfiguration(new IncidentReportNoteConfiguration());
        builder.ApplyConfiguration(new IncidentReportAttachmentConfiguration());

        builder.ApplyConfiguration(new LocationConfiguration());

        builder.ApplyConfiguration(new CoalitionConfiguration());
        builder.ApplyConfiguration(new CoalitionMembershipConfiguration());
        builder.ApplyConfiguration(new CoalitionFormAccessConfiguration());
        builder.ApplyConfiguration(new CoalitionGuideAccessConfiguration());
        builder.ApplyConfiguration(new ElectionRoundFormTemplateConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }
}
