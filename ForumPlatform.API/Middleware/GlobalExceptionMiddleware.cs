using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ForumPlatform.API.Middleware
{
	/// <summary>
	/// Global exception handler middleware. Catches all unhandled exceptions and returns a standardized error response.
	/// </summary>
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;

		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch(Exception exception)
			{
				_logger.LogError(exception, "An unhandled exception occurred.");
				await HandleExceptionAsync(context, exception);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/problem+json";

			var response = new ProblemDetails();

			switch(exception)
			{
				case ValidationException validationException:
					context.Response.StatusCode = StatusCodes.Status400BadRequest;
					response.Type = "http://tools.ietf.org/html/rfc7231#section-6.5.1";
					response.Status = StatusCodes.Status400BadRequest;
					response.Title = "Validation Error";
					response.Detail = validationException.Message;

					var errors = validationException.Errors
						.GroupBy(e => e.PropertyName)
						.ToDictionary(
							g => g.Key,
							g => g.Select(e => e.ErrorMessage).ToArray());

					response.Extensions.Add("errors", errors);
					break;

				default:
					context.Response.StatusCode = StatusCodes.Status500InternalServerError;
					response.Type = "http://tools.ietf.org/html/rfc7231#section-6.6.1";
					response.Title = "An error occurred while processing your request.";
					response.Status = StatusCodes.Status500InternalServerError;
					response.Detail = exception.Message;
					break;
			}
			
			return context.Response.WriteAsJsonAsync(response);
		}
	}

	/// <summary>
	/// Minimal ProblemDetails model for serialization.
	/// Real ASP.Net Core has this built-in, but we define it here for clarity.
	/// </summary>
	public class ProblemDetails
	{
		public string? Title { get; set; }
		public int? Status { get; set; }
		public string? Detail { get; set; }
		public string? Type { get; set; }
		public IDictionary<string, object> Extensions { get; } = new Dictionary<string, object>();

	}
}
