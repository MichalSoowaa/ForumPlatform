using ForumPlatform.Forum.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ForumPlatform.Forum.Application.GetSubforums
{
	public class GetSubforumsQueryHandler : IRequestHandler<GetSubforumsQuery, List<SubforumSummary>>
	{
		private readonly ForumDbContext _db;

		public GetSubforumsQueryHandler(ForumDbContext db)
		{
			_db = db;
		}

		public async Task<List<SubforumSummary>> Handle(GetSubforumsQuery request, CancellationToken cancellationToken)
		{
			return await _db.Subforums
				.OrderBy(s => s.Name)
				.Select(s => new SubforumSummary(s.Id, s.Name, s.Description, s.Status.ToString()))
				.ToListAsync(cancellationToken);
		}
	}
}
