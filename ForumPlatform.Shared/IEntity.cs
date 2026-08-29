namespace ForumPlatform.Shared
{
	public interface IEntity<TId>
	{
		TId Id { get; }
		DateTime CreatedAt { get; }
		DateTime UpdatedAt { get; }
		bool IsNew { get; }
		protected abstract void SetCreationDate(DateTime date);
		protected abstract void SetModifiedDate(DateTime date);
	}
}
