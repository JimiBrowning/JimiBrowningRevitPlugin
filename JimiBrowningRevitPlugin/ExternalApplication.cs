using Autodesk.Revit.UI;
using System.Reflection;

namespace JimiBrowningRevitPlugin;

internal class ExternalApplication : IExternalApplication
{
    private const string TabName = "Jimi Browning";
    private const string PanelName = "Jimi Browning";

    public Result OnStartup(UIControlledApplication application)
    {
        // TODO: Add button icons
        application.CreateRibbonTab(TabName);
        RibbonPanel ribbonPanel =
            application.CreateRibbonPanel(TabName, PanelName);

        string assemblyName = Assembly.GetExecutingAssembly().Location;

        PushButton syncParametersButton = ribbonPanel.AddItem(
            new PushButtonData(
                SyncParametersCommand.Name,
                SyncParametersCommand.Name,
                assemblyName,
                SyncParametersCommand.ClassName)) as PushButton;
        syncParametersButton.ToolTip = SyncParametersCommand.ToolTip;

        PushButton renumberFabDuctButton = ribbonPanel.AddItem(
            new PushButtonData(
                RenumberFabDuctCommand.Name,
                RenumberFabDuctCommand.Name,
                assemblyName,
                RenumberFabDuctCommand.ClassName)) as PushButton;
        renumberFabDuctButton.ToolTip = RenumberFabDuctCommand.ToolTip;

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        RenumberFabDuctForm.CloseIfOpened();

        return Result.Succeeded;
    }
}