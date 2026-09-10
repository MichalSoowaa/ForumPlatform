using MediatR;
using ForumPlatform.Forum.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using ForumPlatform.Forum.Domain.Entities;

namespace ForumPlatform.Forum.Application.CreateSubforum
{
	public class CreateSubforumCommandHandler : IRequestHandler<CreateSubforumCommand, CreateSubforumResponse>
	{
		private readonly ForumDbContext _context;

		public CreateSubforumCommandHandler(ForumDbContext context)
		{
			_context = context;
		}

		public async Task<CreateSubforumResponse> Handle(CreateSubforumCommand request, CancellationToken token)
		{
			// Name uniqueness is also enforced at the DB level (filtered unique index),
			// but checking here first gives a clean 400 instead of an unhandled DBUpdateException bubbling as a 500.
			var nameTaken = await _context.Subforums.AnyAsync(s => s.Name == request.Name, token);

			if (nameTaken)
				throw new InvalidOperationException($"A subforum named '{request.Name}' already exists.");

			var subforum = Subforum.Create(request.Name, request.Description, request.CreatedByUserId);

			_context.Subforums.Add(subforum);
			await _context.SaveChangesAsync(token);

			return new CreateSubforumResponse(subforum.Id, subforum.Name, subforum.Status.ToString());
		}
	}
}
