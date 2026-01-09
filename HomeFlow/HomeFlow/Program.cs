using System.Reflection;
using System.Text.Json.Serialization;
// using BitzArt.Blazor.Auth.Server;
using HomeFlow.Behaviors;
using HomeFlow.Components;
using HomeFlow.Data;
using HomeFlow.Extensions;
using HomeFlow.Infrastructure.FileStorage;
using Microsoft.Extensions.FileProviders;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder( args );

// Configure environment variables and configuration
builder.Configuration
    .SetBasePath( builder.Environment.ContentRootPath )
    .AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true )
    .AddJsonFile( $"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true )
    .AddEnvironmentVariables()
    .AddEnvironmentVariables( prefix: "JwtSettings__" )
    .AddEnvironmentVariables( prefix: "ConnectionStrings__" )
    .AddEnvironmentVariables( prefix: "Logging__" );

builder.Services.ConfigureApplicationCookie( options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
} );

// Add MudBlazor services
builder.Services.AddMudServices( config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
} );

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor( options =>
{
    options.DetailedErrors = true;
} ).AddHubOptions( options =>
{
    // Increase the maximum message size to 1 MB (default is 32 KB)
    // to support saving images over SignalR
    options.MaximumReceiveMessageSize = 1024 * 1024;
} );

builder.Services.AddDbContext<HomeFlowDbContext>( options => options.UseSqlite( builder.Configuration.GetConnectionString( "SQLiteConnection" ) ) );
builder.Services.AddHostedService<BackupService>();
builder.Services.AddMediatR( configuration => { configuration.RegisterServicesFromAssembly( typeof( Program ).Assembly ); } );
builder.Services.AddAutoMapper( cfg => cfg.AddMaps( typeof( Program ).Assembly ) );

// Register AI services
builder.Services.AddScoped<HomeFlow.Infrastructure.AI.IAIService, HomeFlow.Infrastructure.AI.AIService>();

// register homeflow services
builder.Services.AddScoped<IHomeFlowDbContext, HomeFlowDbContext>();
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();

// Register services from the main assembly
builder.Services.AddAllFeatureServices( typeof( Program ).Assembly );

// add controllers and swagger
builder.Services.AddControllers().AddJsonOptions( options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
} );

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly( Assembly.GetExecutingAssembly() );
builder.Services.AddTransient( typeof( IPipelineBehavior<,> ), typeof( LoggingBehavior<,> ) );
builder.Services.AddTransient( typeof( IPipelineBehavior<,> ), typeof( ValidationBehavior<,> ) );

// Create Directories
var localStoragePath = "/LocalStorage";

Directory.CreateDirectory( Path.Combine( localStoragePath, "Backups" ) );
Directory.CreateDirectory( Path.Combine( localStoragePath, "Database" ) );
Directory.CreateDirectory( Path.Combine( localStoragePath, "ImageFiles" ) );
Directory.CreateDirectory( Path.Combine( localStoragePath, "Logs" ) );

builder.Host.UseSerilog( ( ctx, lc ) => lc
    .MinimumLevel.Is( builder.Environment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information )
    .MinimumLevel.Override( "HomeFlow.Features.Core.Authentication", LogEventLevel.Information )
    .Filter.ByIncludingOnly( logEvent =>
        logEvent.Properties.TryGetValue( "SourceContext", out var context ) &&
        context.ToString().Contains( "HomeFlow" ) )
    .Enrich.FromLogContext()
    .WriteTo.Console( theme: AnsiConsoleTheme.Code )
    .WriteTo.File( Path.Combine( localStoragePath, "Logs", "log.txt" ), rollingInterval: RollingInterval.Month )
);

var app = builder.Build();

// Global Exceptions
app.Use( async ( context, next ) =>
{
    try
    {
        await next();
    }
    catch ( Exception ex )
    {
        Log.Error( ex, "Unhandled exception" );
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync( "Something went wrong" );
    }
} );

// Configure the HTTP request pipeline.
if ( !app.Environment.IsDevelopment() )
{
    app.UseExceptionHandler( "/Error", createScopeForErrors: true );
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// controllers and swagger
if ( app.Environment.IsDevelopment() )
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAntiforgery();
app.MapControllers();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseStaticFiles( new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider( Path.Combine( app.Environment.ContentRootPath, "LocalStorage" ) ),
    RequestPath = "/LocalStorage"
} );

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Apply migrations on startup
using ( var scope = app.Services.CreateScope() )
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HomeFlowDbContext>();

    try
    {
        // Ensure the database directory exists
        var dbDirectory = Path.Combine( localStoragePath, "Database" );
        Directory.CreateDirectory( dbDirectory );

        Console.WriteLine( "Applying database migrations..." );
        dbContext.Database.Migrate();
        Console.WriteLine( "Migrations applied successfully." );
    }
    catch ( Exception ex )
    {
        Console.WriteLine( $"An error occurred while migrating the database: {ex.Message}" );
    }
}

app.Run();
