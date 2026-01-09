
using HomeFlow.Constants;
using HomeFlow.Features.Core.DataManagement;
using HomeFlow.Features.Core.SystemSettings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace HomeFlow.Components.Pages.Core;

public partial class DataManagement
{
    [Inject]
    IDataManagementService DataManagementService { get; set; } = default!;

    [Inject]
    ISystemSettingService SystemSettingService { get; set; } = default!;

    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    IJSRuntime JS { get; set; } = default!;

    public bool IsLoading
    {
        get { return _isOptimizing || _isImportingData || _isExportingdata || _isRunningSeedDatabase; }
    }

    public bool IsDatabaseSeeded
    {
        get { return IsLoading || _isDatabaseSeeded; }
    }

    private bool _isDatabaseSeeded = false;
    private bool _isOptimizing = false;
    private bool _isImportingData = false;
    private bool _isExportingdata = false;
    private bool _isRunningSeedDatabase = false;

    private IList<IBrowserFile> _files = new List<IBrowserFile>();

    protected override async Task OnInitializedAsync()
    {
        // Convert the string result to a boolean value
        var databaseSeededValue = await SystemSettingService.GetSystemSetting( SystemSettings.DATABASE_SEEDED );
        _isDatabaseSeeded = bool.TryParse( databaseSeededValue, out var isSeeded ) && isSeeded;
    }

    private async Task OptimizeAsync()
    {
        _isOptimizing = true;

        string results = await DataManagementService.OptimizeDataAsync();
        Snackbar.Add( results, MudBlazor.Severity.Normal, config =>
        {
            config.Icon = Icons.Material.Filled.AutoFixHigh;
        } );

        _isOptimizing = false;
    }

    private async Task ImportDataAsync( IBrowserFile file )
    {
        _isImportingData = true;

        try
        {
            if ( file != null )
            {
                using ( var stream = file.OpenReadStream( 20000000 ) )
                {
                    var buffer = new byte[stream.Length];
                    await stream.ReadAsync( buffer, 0, (int) stream.Length );

                    await DataManagementService.ImportDataAsync( new ImportRequest
                    {
                        Data = buffer
                    } );

                    StateHasChanged();
                }
            }
        }
        catch ( Exception ex )
        {
            Snackbar.Add( $"Error {ex.Message}", MudBlazor.Severity.Error );
        }

        _isImportingData = false;
    }

    private async Task ExportDataAsync()
    {
        _isExportingdata = true;

        var fileResponse = await DataManagementService.ExportDataAsync();

        using var memoryStream = new MemoryStream();
        await fileResponse.CopyToAsync( memoryStream );

        // Convert the memory stream to a Base64 string
        var base64Data = Convert.ToBase64String( memoryStream.ToArray() );

        // Trigger browser download via JavaScript
        await JS.InvokeVoidAsync( "downloadFile", $"homeflow-{DateTime.Now:yyyyMMddHHmmss}.zip", "application/zip", base64Data );

        Snackbar.Add( "Database exported.", MudBlazor.Severity.Normal, config =>
        {
            config.Icon = Icons.Material.Filled.AutoFixHigh;
        } );

        _isExportingdata = false;
    }

    private async Task SeedDatabaseAsync()
    {
        _isRunningSeedDatabase = true;

        await DataManagementService.SeedDatabaseAsync();

        Snackbar.Add( "Sample data added.", MudBlazor.Severity.Normal, config =>
        {
            config.Icon = Icons.Material.Filled.AutoFixHigh;
        } );

        _isRunningSeedDatabase = false;
    }
}