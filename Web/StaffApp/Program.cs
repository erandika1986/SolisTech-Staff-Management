using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;
using StaffApp.Application.Contracts;
using StaffApp.Components;
using StaffApp.Components.Account;
using StaffApp.Domain.Entity.Authentication;
using StaffApp.Infrastructure.Data;
using StaffApp.Services;
using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog file logging
builder.Logging.ClearProviders();
builder.Logging.AddFile("Logs/app-errors-{Date}.log");

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    //options.Scope.Add("profile");
    //options.Scope.Add("email");
    //options.Scope.Add(CalendarService.Scope.Calendar);
    //options.SaveTokens = true;
});

//builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();


// Add services to the container.
//builder.Services.AddApplicationService(builder.Configuration);
//builder.Services.AddFOFWebAPIServices(builder.Configuration);
builder.Services.AddInfrastructureService(builder.Configuration);

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();



var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception occurred while processing request");

        // Optional: return a friendly error page
        context.Response.Redirect("/error");
    }
});

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<StaffAppDbContextInitializer>();
    await initializer.InitializeAsync();
    await initializer.SeedAsync();
}

//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXtceHVQR2JcWE11XUNWYUA=");
var licenseKey = builder.Configuration["SyncfusionLicenseKey"];
SyncfusionLicenseProvider.RegisterLicense(licenseKey);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
