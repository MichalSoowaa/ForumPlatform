using ForumPlatform.Forum.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ForumPlatform.Forum.Domain.Entities;
using Hangfire;
using ForumPlatform.Reputation.Application.Scoring;
using ForumPlatform.Reputation.Domain.Entities;

namespace ForumPlatform.Forum.Application.CreateComment
{
	public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CreateCommandResponse>
	{
		private readonly ForumDbContext _context;
		private readonly IBackgroundJobClient _backgroundJobClient;

		public CreateCommentCommandHandler(ForumDbContext context, IBackgroundJobClient backgroundJobClient)
		{
			_context = context;
			_backgroundJobClient = backgroundJobClient;
		}

		public async Task<CreateCommandResponse> Handle(CreateCommentCommand request, CancellationToken token)
		{
			var thread = await 
				_context.Threads.FirstOrDefaultAsync(t => t.Id == request.ThreadId, token);

			if (thread is null)
				throw new InvalidOperationException($"Thread {request.ThreadId} does not exist.");

			Comment comment;

			if (request.ParentCommentId is null)
				comment = Comment.CreateTopLevel(request.Body, request.ThreadId, request.AuthorId);
			else
			{
				// Load only the parent's Depth, not the whole comment or thread tree
				var parentDepth = await _context.Comments
					.Where(c => c.Id == request.ParentCommentId)
					.Select(c => (int?)c.Depth)
					.FirstOrDefaultAsync(token);

				if (parentDepth is null)
					throw new InvalidOperationException(
						$"Parent comment {request.ParentCommentId} does not exist.");

				comment = Comment.CreateReply(
					request.Body, request.ThreadId, request.AuthorId,
					request.ParentCommentId.Value, parentDepth.Value);
			}

			_context.Comments.Add(comment);

			thread.RegisterNewComment();
			await _context.SaveChangesAsync(token);

			_backgroundJobClient.Enqueue<ContentScoringJob>(job =>
				job.Handle(comment.Id, ContentType.Comment, comment.Body));

			return new CreateCommandResponse(comment.Id, comment.ThreadId, comment.ParentCommentId, comment.Depth);
		}
	}
}
