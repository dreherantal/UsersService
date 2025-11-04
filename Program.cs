using Microsoft.EntityFrameworkCore;
using UsersService.Middlewares;
using UsersService.Models;
using Serilog;
using UsersService.Endpoints;
using UsersService.Services;
using UsersService.Options;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var log = new LoggerConfiguration().Enrich.FromLogContext();



builder.Services.AddDbContext<UsersDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptionsAction: sqlOptions =>
      {
          sqlOptions.EnableRetryOnFailure(
          maxRetryCount: 3);
      }));

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions<APIOptions>().BindConfiguration("API")
.ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<JWTOptions>().BindConfiguration("Token")
.ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

var app = builder.Build();


app.UseSerilogRequestLogging();

app.UseExceptionHandler(exceptionHandlerApp 
    => exceptionHandlerApp.Run(async context 
        => await Results.Problem()
                     .ExecuteAsync(context)));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<LogMiddleware>();
app.UseMiddleware<AuthMiddleware>();

app.RegisterUsersEndpoints();

app.Run();
