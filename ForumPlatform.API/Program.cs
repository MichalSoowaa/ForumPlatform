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
using ForumPlatform.Forum.Application.CreateThread;
using ForumPlatform.Forum.Application.CreateSubforum;
using ForumPlatform.Forum.Application.CreateComment;
using ForumPlatform.Forum.Infrastructure;
using ForumPlatform.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
	configuration
		.ReadFrom.Configuration(context.Configuration)
		.Enrich.FromLogContext()
		.WriteTo.Console());

builder.Services.AddOpenApi();
builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddUsersInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginQuery).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(LoginQueryValidator).Assembly);

builder.Services.AddForumInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateSubforumCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateThreadCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCommentCommand).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(CreateSubforumCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateThreadCommandValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CreateCommentCommandValidator).Assembly);

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

app.MapForumEndpoints();
app.MapAuthEndpoints();

app.Run();
