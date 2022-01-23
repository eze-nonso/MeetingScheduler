using MeetingScheduler.Core.Extensions;
using MeetingScheduler.Core.Interfaces;
using MeetingScheduler.Core.Interfaces.Helpers;
using MeetingScheduler.Core.Models.Constants;
using MeetingScheduler.Core.Services;
using MeetingScheduler.Infrastructure.Data;
using MeetingScheduler.Infrastructure.Helpers;
using MeetingScheduler.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ILoggerService, LoggerService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IIntervalUtil, IntervalUtil>();
builder.Services.AddScoped<IRepo, Repo>();
builder.Services.AddDbContext<MeetingContext>(opt =>
    opt.UseInMemoryDatabase("MeetingsList"));
builder.Services.AddAutoMapper(typeof(Maps));

string logPath = $"{Directory.GetCurrentDirectory()}/Logs";
if (!Directory.Exists(logPath))
    Directory.CreateDirectory(logPath);
AppSettings.Logpath = logPath;
AppSettings.StartOfDayHour = Convert.ToInt16(builder.Configuration["AppSettings:StartOfDayHour"]);
AppSettings.EndOfDayHour = Convert.ToInt16(builder.Configuration["AppSettings:EndOfDayHour"]);
AppSettings.MaxMeetingHours = Convert.ToInt16(builder.Configuration["AppSettings:MaxMeetingHours"]);
AppSettings.MinMeetingHours = Convert.ToDouble(builder.Configuration["AppSettings:MinMeetingHours"]);
AppSettings.DateStringFormat = builder.Configuration["AppSettings:DateStringFormat"];
AppSettings.MeetingHourLimitExceptionMessage = builder.Configuration["AppSettings:MeetingHourLimitExceptionMessage"];
AppSettings.InvalidStartTimeExceptionMessage = builder.Configuration["AppSettings:InvalidStartTimeExceptionMessage"];
AppSettings.BadTimeStampFormatExceptionMessage = builder.Configuration["AppSettings:BadTimeStampFormatExceptionMessage"];
AppSettings.OutOfRangeExceptionMessage = builder.Configuration["AppSettings:OutOfRangeExceptionMessage"];
AppSettings.ConflictTimeExceptionMessage = builder.Configuration["AppSettings:ConflictTimeExceptionMessage"];
AppSettings.BadDateStringFormatExceptionMessage = builder.Configuration["AppSettings:BadDateStringFormatExceptionMessage"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
