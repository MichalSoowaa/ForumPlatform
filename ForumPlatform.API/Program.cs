using ForumPlatform.Users.Infrastructure;
using ForumPlatform.Users.Infrastructure.Persistance;
using Serilog;
using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using ForumPlatform.Users.Application.Login;
using ForumPlatform.Users.Application.Register;
using ForumPlatform.API.Middleware;
using ForumPlatform.API.Auth;
using ForumPlatform.Users.Application.Abstraction;
using ForumPlatform.API.Behaviours;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
	configuration
		.ReadFrom.Configuration(context.Configuration)
		.Enrich.FromLogContext()
		.WriteTo.Console());

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddScoped<ITokenService, TokenService>();

var jwtSigningKey = builder.Configuration["Jwt:SigningKey"]
	?? throw new InvalidOperationException("Missing Jwt:SigningKey configuration.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
	?? throw new InvalidOperationException("Missing Jwt:Issuer configuration.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
	?? throw new InvalidOperationException("Missing Jwt:Audience configuration.");

builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{ 
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
			ValidIssuer = jwtIssuer,
			ValidAudience = jwtAudience,
			ClockSkew = TimeSpan.Zero // Optional: Set clock skew to zero for more strict token expiration validation
		};
	});

builder.Services.AddAuthorizationBuilder();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapGet("/", () => Results.Ok(new { service = "ForumPlatform.WebApi", status = "running" }))
	.WithName("Root");

app.MapGet("/health/db", async (UserDbContext db) =>
{
	var canConnect = await db.Database.CanConnectAsync();
	return canConnect
		? Results.Ok(new { database = "reachable" })
		: Results.Problem("Database is not reachable.", statusCode: 503);
})
	.WithName("HealthDb");

app.MapPost("/auth/register", async (RegisterUserCommand command, IMediator mediator) =>
{
	try
	{
		var result = await mediator.Send(command);
		return Results.Created("/auth/register", result);
	}
	catch(InvalidOperationException ex)
	{
		return Results.BadRequest(new { error = ex.Message });
	}
	catch (Exception ex)
	{
		return Results.Problem(ex.Message, statusCode: 500);
	}
})
	.WithName("RegisterUser")
	.Produces(StatusCodes.Status201Created)
	.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/auth/login", async (LoginQuery query, IMediator mediator) =>
{
	try
	{
		var result = await mediator.Send(query);
		return Results.Ok(result);
	}
	catch (UnauthorizedAccessException ex)
	{
		return Results.Unauthorized();
	}
	catch (Exception ex)
	{
		return Results.Problem(ex.Message, statusCode: 500);
	}
})
	.WithName("LoginUser")
	.Produces(StatusCodes.Status200OK)
	.Produces(StatusCodes.Status401Unauthorized);

app.Run();
