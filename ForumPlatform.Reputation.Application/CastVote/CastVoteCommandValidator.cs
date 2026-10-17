using FluentValidation;

namespace ForumPlatform.Reputation.Application.CastVote
{
	public class CastVoteCommandValidator : AbstractValidator<CastVoteCommand>
	{
		public CastVoteCommandValidator()
		{
			RuleFor(x => x.Value).IsInEnum().WithMessage("Vote value must be either Upvote or Downvote.");
			RuleFor(x => x.ContentId).NotEmpty().WithMessage("ContentId must not be empty.");
			RuleFor(x => x.ContentType).IsInEnum().WithMessage("ContentType must be a valid enum value.");
		}
	}
}
