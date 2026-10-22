using ForumPlatform.Forum.Domain.Entities;
using ForumPlatform.Forum.Infrastructure.Persistance;
using ForumPlatform.Reputation.Application.Scoring;
using ForumPlatform.Reputation.Domain.Entities;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Th = ForumPlatform.Forum.Domain.Entities.Thread;

namespace ForumPlatform.Forum.Application.CreateThread
{
	public class CreateThreadCommandHandler : IRequestHandler<CreateThreadCommand, CreateThreadResponse>
	{
		private readonly ForumDbContext _context;
		private readonly IBackgroundJobClient _backgroundJobClient;

		public CreateThreadCommandHandler(
			ForumDbContext context, IBackgroundJobClient backgroundJobClient)
		{
			_context = context;
			_backgroundJobClient = backgroundJobClient;
		}

		public async Task<CreateThreadResponse> Handle(
			CreateThreadCommand request, CancellationToken token)
		{
			var subforum = await _context.Subforums
				.Where(s => s.Id == request.SubforumId)
				.Select(s => new { s.Status })
				.FirstOrDefaultAsync(token);
				
			if (subforum is null)
				throw new InvalidOperationException($"Subforum {request.SubforumId} does not exist.");

			if (subforum.Status != SubforumStatus.Approved)
				throw new InvalidOperationException("Threads can only be created in approved subforums.");

			var thread = Th.Create(request.Title, request.Body, request.SubforumId, request.AuthorId);

			_context.Threads.Add(thread);
			await _context.SaveChangesAsync(token);

			_backgroundJobClient.Enqueue<ContentScoringJob>(job =>
				job.Handle(thread.Id, ContentType.Thread, thread.Body));

			return new CreateThreadResponse(thread.Id, thread.Title, thread.SubforumId);
		}
	}
}
