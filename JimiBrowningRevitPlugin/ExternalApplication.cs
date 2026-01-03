using Autodesk.Revit.UI;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace JimiBrowningRevitPlugin;

internal class ExternalApplication : IExternalApplication
{
    private const string TabName = "Jimi Browning";
    private const string PanelName = "Jimi Browning";

    private static BitmapFrame GetEmbeddedImage(Assembly assembly, string name)
    {
        string fullName = 
            assembly
            .GetManifestResourceNames()
            .FirstOrDefault(x => x.EndsWith("." + name))
            ?? throw new InvalidDataException(
                "Failed to find icon '" + name + "'"); ;
        Stream stream =
            assembly.GetManifestResourceStream(fullName)
            ?? throw new InvalidDataException(
                "Failed to load icon '" + fullName + "'");
        return BitmapFrame.Create(stream);
    }

    public Result OnStartup(UIControlledApplication application)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string assemblyName = assembly.Location;
        /*string AssemblyFolder =
            Path.GetDirectoryName(AssemblyName)
            ?? throw new DirectoryNotFoundException(
                "Failed to get directory from executing assembly location '"
                + AssemblyName + "'");
        string assemblyPath = Path.Combine(AssemblyFolder, ResourcesFileName);
        Assembly assembly = Assembly.LoadFrom(assemblyPath);*/

        application.CreateRibbonTab(TabName);
        RibbonPanel ribbonPanel =
            application.CreateRibbonPanel(TabName, PanelName);

        var syncParametersButtonData =
            new PushButtonData(
                SyncParametersCommand.Name,
                SyncParametersCommand.Name,
                assemblyName,
                SyncParametersCommand.ClassName);
        if (ribbonPanel.AddItem(syncParametersButtonData)
            is PushButton syncParametersButton)
        {
            syncParametersButton.Image =
                GetEmbeddedImage(assembly, SyncParametersCommand.Icon16);
            syncParametersButton.LargeImage =
                GetEmbeddedImage(assembly, SyncParametersCommand.Icon32);
            syncParametersButton.ToolTip = SyncParametersCommand.ToolTip;
        }

        var renumberFabDuctButtonData =
            new PushButtonData(
                RenumberFabDuctCommand.Name,
                RenumberFabDuctCommand.Name,
                assemblyName,
                RenumberFabDuctCommand.ClassName);
        if (ribbonPanel.AddItem(renumberFabDuctButtonData)
            is PushButton renumberFabDuctButton)
        {
            renumberFabDuctButton.Image =
                GetEmbeddedImage(assembly, RenumberFabDuctCommand.Icon16);
            renumberFabDuctButton.LargeImage =
                GetEmbeddedImage(assembly, RenumberFabDuctCommand.Icon32);
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