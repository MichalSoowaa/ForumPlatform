using ForumPlatform.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Domain.Entities
{
	public class ThreadVisit : Entity<Guid>
	{
		// Opening the thread again within this gap counts as the same reading session.
		public static readonly TimeSpan SessionGap = TimeSpan.FromMinutes(30);

		public Guid UserId { get; private set; }
		public Guid ThreadId { get; private set; }

		/// <summary>When the user most recently opened the thread.</summary>
		public DateTime LastVisitedAt { get; private set; }

		/// <summary>Comments newer than this are "new" on the thread page.</summary>
		public DateTime SessionBaselineAt { get; private set; }

		private ThreadVisit() { }

		public static ThreadVisit CreateFirstVisit(Guid userId, Guid threadId, DateTime now)
			=> new()
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				ThreadId = threadId,
				LastVisitedAt = now,
				SessionBaselineAt = now, // nothing predates a first visit, so nothing is "new"
			};

		/// <summary>Records an open and returns the baseline to highlight against.</summary>
		public DateTime RecordVisit(DateTime now)
		{
			// A new session starts: what the user saw last time becomes the baseline.
			if (now - LastVisitedAt >= SessionGap)
				SessionBaselineAt = LastVisitedAt;

			LastVisitedAt = now;
			UpdatedAt = now;

			return SessionBaselineAt;
		}
	}
}
