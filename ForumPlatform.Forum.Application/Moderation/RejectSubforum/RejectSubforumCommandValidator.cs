using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Moderation.RejectSubforum
{
	public class RejectSubforumCommandValidator : AbstractValidator<RejectSubforumCommand>
	{
		public RejectSubforumCommandValidator()
		{
			RuleFor(x => x.Reason)
				.NotEmpty().WithMessage("A rejection reason is required.")
				.MaximumLength(500).WithMessage("The reason can be at most 500 characters.");
		}
	}
}
