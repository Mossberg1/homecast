using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StreamingApplication.Data;
using StreamingApplication.Interfaces;
using StreamingApplication.Data.Entities;
using StreamingApplication.Data.Repositories;
using StreamingApplication.Forms;
using StreamingApplication.Helpers;
using StreamingApplication.Services;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseSqlite("Data Source=Database.db"); });


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// https://stackoverflow.com/questions/42471866/how-to-create-roles-in-asp-net-core-and-assign-them-to-users
// https://www.youtube.com/watch?v=Y6DCP-yH-9Q


var secretKey = Encoding.ASCII.GetBytes(
    builder.Configuration["JwtAuthentication:SecretKey"]
    ?? throw new NullReferenceException("SecretKey is null.")
);


// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtAuthentication:Issuer"]
                      ?? throw new NullReferenceException("Issuer is null."),
        ValidAudience = builder.Configuration["JwtAuthentication:Audience"]
                        ?? throw new NullReferenceException("Audience is null."),
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        RoleClaimType = ClaimTypes.Role
    };
});


// Add authorization by default, to be sure that all controllers are protected.
builder.Services.AddAuthorization(options => {
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers();


builder.Services.AddLogging();


builder.Services.AddAutoMapper(typeof(Program));


// Add repositories
builder.Services.AddScoped<IRepository<Media>, MediaRepository>();
builder.Services.AddScoped<IRepository<Movie>, MovieRepository>();


// Add scoped services
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();


// Add singleton services
builder.Services.AddSingleton<FFprobeService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


var sizeLimit = 5L * 1024 * 1024 * 1024; // 5 GB

// Configure kestrel to allow larger requests to be able to upload media files.
builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = sizeLimit; });


// Configure FormOptions to allow larger multipart body size
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = sizeLimit; });


var app = builder.Build();


using (var scope = app.Services.CreateScope()) {
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Register roles.
    var roles = new[] { UserRole.Admin, UserRole.Base };
    foreach (var role in roles) {
        if (!await roleManager.RoleExistsAsync(role)) {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // create a default admin user if there is no user in the database.
    if (!userManager.Users.Any()) {
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        var result = await userService.CreateUserAsync(new RegisterForm {
            UserName = "admin",
            Password = "Admin123!"
        }, UserRole.Admin);

        if (!result.Succeeded) {
            foreach (var err in result.Errors) {
                logger.LogCritical($"Code: {err.Code}, Desc: {err.Description}");
            }

            return;
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Create media storage for development testing.
    using (var scope = app.Services.CreateScope()) {
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        var movieDirectory = Path.Combine(env.ContentRootPath, "MediaStorage", "Movies");
        var showDirectory = Path.Combine(env.ContentRootPath, "MediaStorage", "Shows");
    
        if (!Directory.Exists(movieDirectory)) {
            Directory.CreateDirectory(movieDirectory);
            logger.LogInformation($"Created directory: {movieDirectory}");
        } else {
            logger.LogInformation($"Directory already exists: {movieDirectory}");
        }

        if (!Directory.Exists(showDirectory)) {
            Directory.CreateDirectory(showDirectory);
            logger.LogInformation($"Created directory: {showDirectory}");
        } else {
            logger.LogInformation($"Directory already exists: {showDirectory}");
        }
    }
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();