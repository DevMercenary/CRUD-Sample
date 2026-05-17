using CrudSample.Api;
using CrudSample.Core.Infrastructure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var sqlite =
    builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=crud-sample.db";

builder.Services
    .AddCrudSampleCore(sqlite)
    .AddControllers().Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddProblemDetails()
    .AddOpenApi()
    .AddHealthChecks().AddDbContextCheck<AppDbContext>().Services
    .AddExceptionHandler<ValidationExceptionHandler>()
    .AddHttpLogging(o => o.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.ResponseStatusCode);

// Translate FluentValidation failures into RFC 7807 ProblemDetails 400 responses.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://crud-sample/errors/validation",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
        };
        return new BadRequestObjectResult(problem);
    };
});

var app = builder.Build();

// Apply migrations / create the SQLite schema on startup so the sample is
// runnable with no extra steps.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opts => opts.WithTitle("CRUD-Sample API"));
}

app.UseStatusCodePages();
app.UseExceptionHandler();
app.UseHttpLogging();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

// A minimal-API counterpart of the controller — illustrates the alternative
// style on the same data, so reviewers can compare ergonomics side-by-side.
app.MapGet("/api/v1/users/count", async (AppDbContext db, CancellationToken ct) =>
{
    var count = await db.Users.CountAsync(ct);
    return Results.Ok(new { count });
}).WithName("UsersCount");

app.Run();

public partial class Program;
