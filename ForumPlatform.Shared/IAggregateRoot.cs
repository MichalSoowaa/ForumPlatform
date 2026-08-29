using System.ComponentModel.DataAnnotations;

namespace ForumPlatform.Shared
{
	public interface IAggregateRoot<TId> : ISoftDeletable, IEntity<TId>
	{
		[Timestamp]
		byte[] Version { get; }

		protected abstract void MarkAsDeleted();
		protected abstract ulong GetVersion();
	}
}
