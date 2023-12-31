using HoneybunBlazorServerExample.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// *** START INTEGRATING HONEYBUN AUTH

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json")
    .Build();

var settings = new HoneybunSettings();
config.GetSection("Honeybun").Bind(settings);

builder.Services
    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opt =>
    {
        opt.Authority = settings.Authority;
        opt.ClientId = settings.ClientId;
        opt.ClientSecret = settings.ClientSecret;
        opt.ResponseType = OpenIdConnectResponseType.Code;
        opt.UseTokenLifetime = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.Email,
            ValidateIssuer = false
        };
        opt.Events = new OpenIdConnectEvents
        {
            OnAccessDenied = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/");
                return Task.CompletedTask;
            }
        };
    });

// *** END INTEGRATING HONEYBUN AUTH

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// *** START INTEGRATING HONEYBUN AUTH

app.UseAuthentication();
app.UseAuthorization();

// *** END INTEGRATING HONEYBUN AUTH

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
