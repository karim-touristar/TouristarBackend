using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TouristarBackend.Contracts;
using TouristarBackend.Filters;
using TouristarBackend.Models;
using TouristarBackend.Repositories;
using TouristarBackend.Services;
using Newtonsoft.Json;
using TouristarModels.Models;
using Npgsql;
using TouristarModels.Enums;
using TouristarBackend.Constants;

var AllowedOrigins = "AllowedOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: AllowedOrigins,
        policy =>
        {
            policy.WithOrigins("*");
        }
    );
});

builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection("Auth"));
builder.Services.Configure<RadarConfig>(builder.Configuration.GetSection("Radar"));
builder.Services.Configure<ConnectionConfig>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<GooglePlacesConfig>(builder.Configuration.GetSection("GooglePlaces"));
builder.Services.Configure<InvitationConfig>(builder.Configuration.GetSection("Invitation"));

var connectionConfig = builder.Configuration
    .GetSection("ConnectionStrings")
    .Get<ConnectionConfig>();
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionConfig?.DbConnection);
dataSourceBuilder.MapEnum<ActivityType>();
dataSourceBuilder.MapEnum<TicketLeg>();
dataSourceBuilder.MapEnum<TripDocumentType>();
var dataSource = dataSourceBuilder.Build();
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseNpgsql(dataSource);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

var authConfig = builder.Configuration.GetSection("Auth").Get<AuthConfig>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        x =>
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(authConfig!.JwtTokenKey)
                ),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                // TODO: enable the following validations
                ValidateAudience = false,
                ValidateIssuer = false,
            }
    );

// Services
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IChecklistService, ChecklistService>();
builder.Services.AddScoped<IChecklistTaskService, ChecklistTaskService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IActivityLocationService, ActivityLocationService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IFlightAlertService, FlightAlertService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<ITripInvitationService, TripInvitationService>();

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<AppExceptionFilter>();
    })
    .AddNewtonsoftJson(x =>
    {
        x.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        x.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(AllowedOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
