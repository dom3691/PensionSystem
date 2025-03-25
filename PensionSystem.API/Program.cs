using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.Features.Commands;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.Repositories.Interfaces;
using PensionSystem.Infrastructure.Repositories;
using MediatR;
using Hangfire;
using PensionSystem.API;
using static PensionSystem.Application.Features.BackgroundJobs.BackgroundJobs;
using PensionSystem.Application.Features.BackgroundJobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IContributionRepository, ContributionRepository>();
builder.Services.AddMediatR(typeof(RegisterMemberCommandHandler).Assembly);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

RecurringJob.AddOrUpdate<ContributionValidationJob>(
    "ContributionValidationJob",  // Unique job ID
    job => job.ValidateContributionsAsync(),
    Cron.Monthly,
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

RecurringJob.AddOrUpdate<BenefitEligibilityJob>(
    "BenefitEligibilityJob",  // Unique job ID
    job => job.UpdateBenefitEligibilityAsync(),
    Cron.Monthly,
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

RecurringJob.AddOrUpdate<InterestCalculationJob>(
    "InterestCalculationJob",  // Unique job ID
    job => job.CalculateInterestAsync(),
    Cron.Monthly,
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });


app.Run();
