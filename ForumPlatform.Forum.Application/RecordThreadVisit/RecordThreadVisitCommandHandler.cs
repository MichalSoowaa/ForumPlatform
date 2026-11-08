using ForumPlatform.Forum.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ForumPlatform.Forum.Domain.Entities;

namespace ForumPlatform.Forum.Application.RecordThreadVisit
{
	public class RecordThreadVisitCommandHandler : IRequestHandler<RecordThreadVisitCommand, RecordThreadVisitResult>
	{
		private readonly ForumDbContext _db;

		public RecordThreadVisitCommandHandler(ForumDbContext db)
		{
			_db = db;
		}

		public async Task<RecordThreadVisitResult> Handle(RecordThreadVisitCommand request, CancellationToken ct)
		{
			var threadExists = await _db.Threads.AnyAsync(t => t.Id == request.ThreadId, ct);

			if (!threadExists)
				throw new InvalidOperationException($"Thread {request.ThreadId} does not exist.");

			var now = DateTime.UtcNow;

			for (var attempt = 1; ; attempt++)
			{
				var visit = await _db.ThreadVisits
					.FirstOrDefaultAsync(v => v.UserId == request.UserId && v.ThreadId == request.ThreadId, ct);

				DateTime baseline;

				if (visit is null)
				{
					visit = ThreadVisit.CreateFirstVisit(request.UserId, request.ThreadId, now);
					_db.ThreadVisits.Add(visit);
					baseline = visit.SessionBaselineAt;
				}
				else
				{
					baseline = visit.RecordVisit(now);
				}

				try
				{
					await _db.SaveChangesAsync(ct);

					return new RecordThreadVisitResult(baseline);
				}
				catch (DbUpdateException) when (attempt > 2)
				{
					// Two requests created the first visit at the same moment (the unique index
					// rejected one). Start over: the row now exists, so this becomes an update.
					_db.ChangeTracker.Clear();
				}
			}
		}
	}
}
