
namespace ForumPlatform.Shared
{
	public interface IAggregateRoot<TId> : ISoftDeletable, IEntity<TId>
	{
		protected abstract void MarkAsDeleted();
	}
}
