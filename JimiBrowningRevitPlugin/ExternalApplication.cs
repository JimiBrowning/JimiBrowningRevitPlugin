using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace JimiBrowningRevitPlugin;

internal class ExternalApplication : IExternalApplication
{
    private const string TabName = "Jimi Browning";
    private const string PanelName = "Jimi Browning";
    private const string ResourcesFileName =
        "JimiBrowningRevitPluginCore.dll";
    private const string ResourceNamePrefix =
        "JimiBrowningRevitPluginCore.Resources.";
    private const string SyncIcon16 = "SyncIcon16.png";
    private const string SyncIcon32 = "SyncIcon32.png";
    private const string RenameIcon16 = "RenameIcon16.png";
    private const string RenameIcon32 = "RenameIcon32.png";

    private static readonly string AssemblyName =
        Assembly.GetExecutingAssembly().Location;
    private static readonly string AssemblyFolder =
        Path.GetDirectoryName(AssemblyName)
        ?? throw new DirectoryNotFoundException(
            "Failed to get directory from executing assembly location '"
            + AssemblyName + "'");

    private static BitmapFrame GetEmbeddedImage(string name)
    {
        string fullName = ResourceNamePrefix + name;
        string assemblyPath = Path.Combine(AssemblyFolder, ResourcesFileName);
        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        Stream stream =
            assembly.GetManifestResourceStream(fullName)
            ?? throw new InvalidDataException(
                "Failed to load icon '" + fullName + "'");
        return BitmapFrame.Create(stream);
    }

    public Result OnStartup(UIControlledApplication application)
    {
        application.CreateRibbonTab(TabName);
        RibbonPanel ribbonPanel =
            application.CreateRibbonPanel(TabName, PanelName);

        if (ribbonPanel.AddItem(
            new PushButtonData(
                SyncParametersCommand.Name,
                SyncParametersCommand.Name,
                AssemblyName,
                SyncParametersCommand.ClassName))
            is PushButton syncParametersButton)
        {
            syncParametersButton.Image = GetEmbeddedImage(SyncIcon16);
            syncParametersButton.LargeImage = GetEmbeddedImage(SyncIcon32);
            syncParametersButton.ToolTip = SyncParametersCommand.ToolTip;
        }

        if (ribbonPanel.AddItem(
            new PushButtonData(
                RenumberFabDuctCommand.Name,
                RenumberFabDuctCommand.Name,
                AssemblyName,
                RenumberFabDuctCommand.ClassName))
            is PushButton renumberFabDuctButton)
        {
            renumberFabDuctButton.Image = GetEmbeddedImage(RenameIcon16);
            renumberFabDuctButton.LargeImage = GetEmbeddedImage(RenameIcon32);
            renumberFabDuctButton.ToolTip = RenumberFabDuctCommand.ToolTip;
        }

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        RenumberFabDuctForm.CloseIfOpened();

        return Result.Succeeded;
    }
}