using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace JimiBrowningRevitPlugin;

internal class ExternalApplication : IExternalApplication
{
    private const string TabName = "Jimi Browning";
    private const string PanelName = "Jimi Browning";

    private static readonly string AssemblyName =
        Assembly.GetExecutingAssembly().Location;
    private static readonly string AssemblyFolder =
        Path.GetDirectoryName(AssemblyName);

    public Result OnStartup(UIControlledApplication application)
    {
        application.CreateRibbonTab(TabName);
        RibbonPanel ribbonPanel =
            application.CreateRibbonPanel(TabName, PanelName);


        PushButton syncParametersButton = ribbonPanel.AddItem(
            new PushButtonData(
                SyncParametersCommand.Name,
                SyncParametersCommand.Name,
                AssemblyName,
                SyncParametersCommand.ClassName)) as PushButton;
        syncParametersButton.Image = GetEmbeddedImage("SyncIcon16.png");
        syncParametersButton.LargeImage = GetEmbeddedImage("SyncIcon32.png");
        syncParametersButton.ToolTip = SyncParametersCommand.ToolTip;

        PushButton renumberFabDuctButton = ribbonPanel.AddItem(
            new PushButtonData(
                RenumberFabDuctCommand.Name,
                RenumberFabDuctCommand.Name,
                AssemblyName,
                RenumberFabDuctCommand.ClassName)) as PushButton;
        renumberFabDuctButton.Image = GetEmbeddedImage("RenameIcon16.png");
        renumberFabDuctButton.LargeImage = GetEmbeddedImage("RenameIcon32.png");
        renumberFabDuctButton.ToolTip = RenumberFabDuctCommand.ToolTip;

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        RenumberFabDuctForm.CloseIfOpened();

        return Result.Succeeded;
    }

    private BitmapSource GetEmbeddedImage(string name)
    {
        string assemblyPath =
            Path.Combine(AssemblyFolder, "JimiBrowningRevitPluginCore.dll");
        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        Stream stream = assembly.GetManifestResourceStream(
            "JimiBrowningRevitPluginCore.Resources." + name);
        return BitmapFrame.Create(stream);
    }
}