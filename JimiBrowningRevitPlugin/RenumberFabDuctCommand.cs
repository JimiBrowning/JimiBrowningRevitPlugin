using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace JimiBrowningRevitPlugin;

[Transaction(TransactionMode.Manual)]
internal class RenumberFabDuctCommand : IExternalCommand
{
    internal const string Name = "Renumber Fab Duct";
    internal const string ToolTip =
        "Renumbers fabrication duct Item Number with prefix and " +
        "auto-incrementing number.";

    internal static readonly string ClassName =
        typeof(RenumberFabDuctCommand).Namespace
        + "."
        + typeof(RenumberFabDuctCommand).Name;

    private readonly ExternalEvent renumberFabDuctEvent =
        ExternalEvent.Create(new RenumberFabDuctEventHandler());

    private Document document;

    public Result Execute(
        ExternalCommandData externalCommandData,
        ref string message,
        ElementSet elementSet)
    {
        if (RenumberFabDuctForm.IsOpened())
            return Result.Failed;

        document = externalCommandData.Application.ActiveUIDocument.Document;

        RenumberFabDuctForm.ShowForm(
            GetHighestItemNumberCallback,
            StartRenumberingCallback);

        return Result.Succeeded;
    }

    private int GetHighestItemNumberCallback(string itemNumberPrefix)
    {
        int highestNumber =
            Elements.GetHighestItemNumberWithPrefix(document, itemNumberPrefix);

        return highestNumber;
    }

    private void StartRenumberingCallback()
    {
        renumberFabDuctEvent.Raise();
    }
}