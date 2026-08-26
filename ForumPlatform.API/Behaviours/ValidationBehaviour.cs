using FluentValidation;
using MediatR;

namespace ForumPlatform.API.Behaviours
{
	public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : notnull, IRequest<TResponse>
	{
		private readonly IEnumerable<IValidator<TRequest>> _validators;

		public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
		{
			_validators = validators;
		}

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			if(!_validators.Any())
			{
				return await next();
			}

			var context = new ValidationContext<TRequest>(request);

			var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

			var failures = validationResults
				.Where(r => r.IsValid == false)
				.SelectMany(result => result.Errors)
				.ToList();

			if (failures.Count() != 0)
			{
				throw new ValidationException(failures);
			}

			return await next();
		}
	}
}
