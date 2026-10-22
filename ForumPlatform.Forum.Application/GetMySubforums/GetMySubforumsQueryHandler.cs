using ForumPlatform.Forum.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ForumPlatform.Forum.Application.GetMySubforums
{
	public class GetMySubforumsQueryHandler : IRequestHandler<GetMySubforumsQuery, List<MySubforum>>
	{
		private readonly ForumDbContext _db;

		public GetMySubforumsQueryHandler(ForumDbContext db) => _db = db;

		public async Task<List<MySubforum>> Handle(GetMySubforumsQuery request, CancellationToken cancellationToken)
		{
			var rows = await _db.Subforums
				.Where(s => s.CreatedByUserId == request.UserId)
				.OrderByDescending(s => s.CreatedAt)
				.Select(s => new { s.Id, s.Name, s.Description, s.Status, s.Tags, s.RejectionReason, s.CreatedAt })
				.ToListAsync(cancellationToken);

			return rows
				.Select(r => new MySubforum(
					r.Id, r.Name, r.Description, r.Status.ToString(), r.Tags, r.RejectionReason, r.CreatedAt))
				.ToList();
		}
	}
}
