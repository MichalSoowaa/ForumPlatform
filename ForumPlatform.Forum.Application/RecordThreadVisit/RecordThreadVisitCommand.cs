using MediatR;

namespace ForumPlatform.Forum.Application.RecordThreadVisit
{
	public record RecordThreadVisitCommand(Guid ThreadId, Guid UserId) : IRequest<RecordThreadVisitResult>;

	/// <param name="BaselineAt">Comments created after this moment are new to the user.</param>
	public record RecordThreadVisitResult(DateTime BaselineAt);
}
