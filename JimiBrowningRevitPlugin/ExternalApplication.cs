using Autodesk.Revit.UI;
using System.Reflection;

namespace JimiBrowningRevitPlugin;

internal class ExternalApplication : IExternalApplication
{
    private const string TAB_NAME = "Jimi Browning";
    private const string PANEL_NAME = "Jimi Browning";

    public Result OnStartup(UIControlledApplication application)
    {
        // TODO: Add button icons
        application.CreateRibbonTab(TAB_NAME);
        RibbonPanel ribbonPanel =
            application.CreateRibbonPanel(TAB_NAME, PANEL_NAME);

        string assemblyName = Assembly.GetExecutingAssembly().Location;

        PushButton syncParametersButton = ribbonPanel.AddItem(
            new PushButtonData(
                SyncParametersCommand.NAME,
                SyncParametersCommand.NAME,
                assemblyName,
                SyncParametersCommand.CLASS_NAME)) as PushButton;
        syncParametersButton.ToolTip = SyncParametersCommand.TOOL_TIP;

        PushButton renumberFabDuctButton = ribbonPanel.AddItem(
            new PushButtonData(
                RenumberFabDuctCommand.NAME,
                RenumberFabDuctCommand.NAME,
                assemblyName,
                RenumberFabDuctCommand.CLASS_NAME)) as PushButton;
        renumberFabDuctButton.ToolTip = RenumberFabDuctCommand.TOOL_TIP;

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        RenumberFabDuctForm.CloseIfOpened();

        return Result.Succeeded;
    }
}