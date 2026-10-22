using ForumPlatform.Forum.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ForumPlatform.Forum.Application.Moderation.RejectSubforum
{
	public class RejectSubforumCommandHandler : IRequestHandler<RejectSubforumCommand, SubforumReviewResult>
	{
		private readonly ForumDbContext _db;

		public RejectSubforumCommandHandler(ForumDbContext db) => _db = db;

		public async Task<SubforumReviewResult> Handle(RejectSubforumCommand request, CancellationToken cancellationToken)
		{
			var subforum = await _db.Subforums.FirstOrDefaultAsync(s => s.Id == request.SubforumId, cancellationToken)
				?? throw new InvalidOperationException($"Subforum {request.SubforumId} does not exist.");

			subforum.Reject(request.ModeratorId, request.Reason);

			try
			{
				await _db.SaveChangesAsync(cancellationToken);
			}
			catch (DbUpdateConcurrencyException)
			{
				throw new InvalidOperationException("This subforum was just reviewed by another moderator.");
			}

			return new SubforumReviewResult(subforum.Id, subforum.Status.ToString(), subforum.Tags);
		}
	}
}
