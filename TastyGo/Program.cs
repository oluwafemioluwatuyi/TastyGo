using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TastyGo.Constants;
using TastyGo.Data;
using TastyGo.Helpers;
using TastyGo.Interfaces.IRepositories;
using TastyGo.Interfaces.Other;
using TastyGo.Interfaces.Services;
using TastyGo.Repositories;
using TastyGo.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://localhost:3000", "https://borrowhub.vercel.app", "https://borrowhub.com", "https://borrowhub-v1.vercel.app", "https://borrowhub-v1.com")
                .AllowAnyHeader()
                .WithMethods("GET", "POST", "PATCH", "PUT", "DELETE")
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers(options =>
{
    // TODO: Find out why this is not working and causes an error to be returned to the client
    // options.Filters.Add<ValidateMonnifySignatureAttribute>();

})
       .AddJsonOptions(options =>
{
    // System.Text.Json configuration
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter()); // Custom DateTime converter for System.Text.Json
});
// .AddNewtonsoftJson(options =>
// {
//     // Newtonsoft.Json configuration
//     options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter { AllowIntegerValues = true });
//     options.SerializerSettings.Converters.Add(new MultiFormatDateTimeConverter()); // Custom DateTime converter for Newtonsoft.Json
// });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Check if the endpoint requires authentication
                var endpoint = context.HttpContext.GetEndpoint();
                var requiresAuth = endpoint?.Metadata?.GetMetadata<IAuthorizeData>() != null;

                if (!requiresAuth)
                {
                    // Skip authentication if the endpoint does not require it
                    context.NoResult();
                }

                return System.Threading.Tasks.Task.CompletedTask;
            },
            OnAuthenticationFailed = async context =>
            {
                // This should handle cases like invalid tokens or malformed tokens.
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var responseBody = JsonConvert.SerializeObject(new ServiceResponse<object>(ResponseStatus.Unauthorized, "Token expired", AppStatusCode.Unauthorized, null));
                    await context.Response.WriteAsync(responseBody);
                }
                else
                {
                    context.Response.Headers.Append("Authentication-Failed", "true");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var responseBody = JsonConvert.SerializeObject(new ServiceResponse<object>(ResponseStatus.Unauthorized, "Authentication failed", AppStatusCode.Unauthorized, null));
                    await context.Response.WriteAsync(responseBody);
                }
                return;
            },
            OnChallenge = async context =>
            {
                // This should handle cases where no authentication was provided.
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var responseBody = JsonConvert.SerializeObject(new ServiceResponse<object>(ResponseStatus.Unauthorized, "Unauthorized", AppStatusCode.Unauthorized, null));
                await context.Response.WriteAsync(responseBody);
                return;
            },
            OnForbidden = async context =>
            {
                // This handles cases where authentication succeeded but the user lacks permissions.
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var responseBody = JsonConvert.SerializeObject(new ServiceResponse<object>(ResponseStatus.Forbidden, "You are not allowed to use this feature", AppStatusCode.Unauthorized, null));
                await context.Response.WriteAsync(responseBody);
                return;
            }
        };

    });


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpContextAccessor();
;

builder.Services.AddDbContext<TastyGoDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerDatabase"));
});


// builder.Services
// .AddFluentValidationAutoValidation();

// builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();


// Seeders


//Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();



//services
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IAuthService, AuthService>();


// builder.Services.AddSingleton<IEmailService>(provider =>
// {
//     var logger = provider.GetRequiredService<ILogger<EmailService>>();
//     var environment = provider.GetRequiredService<IWebHostEnvironment>();

//     var templatesFolderPath = Path.Combine(environment.ContentRootPath, "Emails");

//     return new EmailService(templatesFolderPath, logger, builder.Configuration);
// });


//Constants
builder.Services.AddScoped<IConstants, Constants>();

// Wallet Providers: Ensure specific wallet provider services are registered



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();




app.Run();

