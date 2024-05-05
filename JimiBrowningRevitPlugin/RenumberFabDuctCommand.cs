using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace JimiBrowningRevitPlugin;

[Transaction(TransactionMode.Manual)]
internal class RenumberFabDuctCommand : IExternalCommand
{
    internal const string NAME = "Renumber Fab Duct";
    internal const string TOOL_TIP =
        "Renumbers fabrication duct Item Number with prefix and " +
        "auto-incrementing number.";
    internal static readonly string CLASS_NAME =
        typeof(RenumberFabDuctCommand).Namespace
        + "."
        + typeof(RenumberFabDuctCommand).Name;

    private Document document;
    private readonly ExternalEvent renumberFabDuctEvent =
        ExternalEvent.Create(new RenumberFabDuctEventHandler());

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