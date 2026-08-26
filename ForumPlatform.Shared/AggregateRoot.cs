using System.Buffers.Binary;
using System.ComponentModel.DataAnnotations;

namespace ForumPlatform.Shared
{
	public abstract class AggregateRoot<TId> : Entity<TId>, ISoftDeletable
		where TId : notnull
	{
		[Timestamp]
		public byte[] Version { get; protected set; } = null!;

		public bool IsDeleted { get; protected set; }

		public void MarkAsDeleted()
		{
			IsDeleted = true;
		}

		public ulong GetVersion()
		{
			return Version is null ? 0 : BinaryPrimitives.ReadUInt64BigEndian(Version);
		}
	}
}
