using FluentValidation;
using ForumPlatform.Forum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumPlatform.Forum.Application.Moderation.ApproveSubforum
{
	public class ApproveSubforumCommandValidator : AbstractValidator<ApproveSubforumCommand>
	{
		public ApproveSubforumCommandValidator()
		{
			RuleFor(x => x.Tags)
				.NotEmpty().WithMessage("At least one tag is required.")
				.Must(t => t.Count <= Subforum.MaxTags).WithMessage($"At most {Subforum.MaxTags} tags are allowed.");

			RuleForEach(x => x.Tags)
				.NotEmpty().WithMessage("Tags cannot be blank")
				.MaximumLength(Subforum.MaxTagLength).WithMessage($"A tag can be at most {Subforum.MaxTagLength} characters.");
		}
	}
}
