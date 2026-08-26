namespace ForumPlatform.Shared
{
	public abstract class Entity<TId>
	{
		public TId Id { get; protected set; } = default!;
		public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; protected set; }
		public bool IsNew => EqualityComparer<TId>.Default.Equals(Id, default);

		public void SetCreationDate(DateTime createdAt)
		{
			CreatedAt = createdAt;
		}

		public void SetUpdateDate(DateTime updatedAt)
		{
			UpdatedAt = updatedAt;
		}
	}
}
