
namespace ForumPlatform.Shared
{
	public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
		where TId : notnull
	{
		public bool IsDeleted { get; set; }

		public void MarkAsDeleted()
		{
			IsDeleted = true;
		}
	}
}
