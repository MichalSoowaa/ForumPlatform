using System.Buffers.Binary;
using System.ComponentModel.DataAnnotations;

namespace ForumPlatform.Shared
{
	public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
		where TId : notnull
	{
		public byte[] Version { get; set; } = null!;
		public bool IsDeleted { get; set; }

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
