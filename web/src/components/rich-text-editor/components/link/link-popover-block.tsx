import { Separator } from '@/components/ui/separator';
import { ExternalLinkIcon, LinkBreak2Icon } from '@radix-ui/react-icons';
import { ToolbarButton } from '../toolbar-button';

const LinkPopoverBlock = ({
  link,
  onClear,
  onEdit,
}: {
  link: Record<string, unknown>;
  onClear: () => void;
  onEdit: (e: React.MouseEvent<HTMLButtonElement>) => void;
}) => {
  return (
    <div className='flex h-10 overflow-hidden rounded bg-background p-2 shadow-lg'>
      <div className='inline-flex items-center gap-1'>
        <ToolbarButton tooltip='Edit link' onClick={onEdit}>
          Edit link
        </ToolbarButton>
        <Separator orientation='vertical' />
        <ToolbarButton tooltip='Open link in a new tab' onClick={() => window.open(link['href'] as string, '_blank')}>
          <ExternalLinkIcon className='size-4' />
        </ToolbarButton>
        <Separator orientation='vertical' />
        <ToolbarButton tooltip='Clear link' onClick={onClear}>
          <LinkBreak2Icon className='size-4' />
        </ToolbarButton>
      </div>
    </div>
  );
};

export { LinkPopoverBlock };
