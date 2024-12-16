using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using FocusLearn.Repositories.Abstract;
using FocusLearn.Repositories.Implementation;
using FocusLearn.Models.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FocusLearn API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
    googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
    googleOptions.Events.OnCreatingTicket = (context) =>
    {
        var pictureClaim = context.User.TryGetProperty("picture", out var pictureJson)
            ? pictureJson.GetString()
            : "default_photo_url";

        if (!string.IsNullOrEmpty(pictureClaim))
        {
            context.Identity.AddClaim(new Claim("picture", pictureClaim));
        }

        var localeClaim = context.User.TryGetProperty("locale", out var localeJson)
            ? localeJson.GetString()
            : "en";
        if (!string.IsNullOrEmpty(localeClaim))
        {
            context.Identity.AddClaim(new Claim("locale", localeClaim));
        }

        return Task.CompletedTask;
    };
})
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    facebookOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    facebookOptions.Fields.Add("picture");
    facebookOptions.Fields.Add("locale");
    facebookOptions.Events.OnCreatingTicket = (context) =>
    {
        // Отримання URL фото профілю
        var picture = context.User.TryGetProperty("picture", out var pictureJson) &&
                      pictureJson.TryGetProperty("data", out var dataJson) &&
                      dataJson.TryGetProperty("url", out var urlJson)
            ? urlJson.GetString()
            : "default_photo_url";

        if (!string.IsNullOrEmpty(picture))
        {
            context.Identity.AddClaim(new Claim("picture", picture));
        }

        // Отримання мови
        var locale = context.User.TryGetProperty("locale", out var localeJson)
            ? localeJson.GetString()
            : "en";

        if (!string.IsNullOrEmpty(locale))
        {
            context.Identity.AddClaim(new Claim("locale", locale));
        }

        return Task.CompletedTask;
    };

});

// Add database context
builder.Services.AddDbContext<FocusLearnDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add memory cache for sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".FocusLearn.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

    
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IConcentrationMethodService, ConcentrationMethodService>();
builder.Services.AddScoped<ILearningMaterialService, LearningMaterialService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
