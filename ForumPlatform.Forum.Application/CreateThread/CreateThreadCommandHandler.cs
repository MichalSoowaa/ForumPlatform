using ForumPlatform.Forum.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Th = ForumPlatform.Forum.Domain.Entities.Thread;

namespace ForumPlatform.Forum.Application.CreateThread
{
	public class CreateThreadCommandHandler : IRequestHandler<CreateThreadCommand, CreateThreadResponse>
	{
		private readonly ForumDbContext _context;

		public CreateThreadCommandHandler(ForumDbContext context)
		{
			_context = context;
		}

		public async Task<CreateThreadResponse> Handle(CreateThreadCommand request, CancellationToken token)
		{
			var subforumExists = await _context.Subforums.AnyAsync(s => s.Id == request.SubforumId, token);

			if (!subforumExists)
				throw new InvalidOperationException($"Subforum {request.SubforumId} does not exist.");

			// NOTE — deliberate scoped simplification: not checking Subforum.Status here.
			// Per the Week 1-4 plan, moderator approval doesn't exist yet, so requiring
			// Status == Approved would make it impossible to ever create a thread until
			// Week 4 ships. Revisit once the approval workflow exists — this check
			// should be added back in then.

			var thread = Th.Create(request.Title, request.Body, request.SubforumId, request.AuthorId);

			_context.Threads.Add(thread);
			await _context.SaveChangesAsync(token);

			return new CreateThreadResponse(thread.Id, thread.Title, thread.SubforumId);
		}
	}
}
