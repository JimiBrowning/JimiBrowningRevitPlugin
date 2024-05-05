using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace JimiBrowningRevitPlugin;

internal class GraphicOverrides
{
    private readonly Document document;
    private readonly View view;
    private readonly OverrideGraphicSettings overrideGraphicSettings = new();
    private readonly Dictionary<ElementIdValue, OverrideGraphicSettings>
        originalOverrideGraphicSettingsList = new();

    internal GraphicOverrides(Document document)
    {
        this.document = document;
        view = document.ActiveView;

        overrideGraphicSettings.SetProjectionLineColor(new Color(0, 0, 0));
        overrideGraphicSettings.SetCutLineColor(new Color(0, 0, 0));
        overrideGraphicSettings.SetCutLineWeight(1);
        overrideGraphicSettings.SetProjectionLineWeight(1);
        overrideGraphicSettings.SetHalftone(true);
        overrideGraphicSettings.SetSurfaceTransparency(100);
    }

    internal HashSet<ElementIdValue> GetOverridenFabDuctIds()
    {
        return new HashSet<ElementIdValue>(
            originalOverrideGraphicSettingsList.Keys);
    }

    internal bool FadeFabDuct(FabricationPart fabDuct)
    {
        var fabDuctId = new ElementIdValue(fabDuct.Id);

        if (originalOverrideGraphicSettingsList.ContainsKey(fabDuctId))
            return true;

        OverrideGraphicSettings originalOverrideGraphicSettings =
            view.GetElementOverrides(fabDuct.Id);

        using var transaction = new Transaction(document);
        transaction.Start("Override graphic settings");

        view.SetElementOverrides(fabDuct.Id, overrideGraphicSettings);

        if (transaction.Commit() != TransactionStatus.Committed)
            return false;

        this.originalOverrideGraphicSettingsList.Add(
            fabDuctId,
            originalOverrideGraphicSettings);

        return true;
    }

    internal bool FadeFabDuctsWithItemNumber()
    {
        var tempOriginalOverrideGraphicSettingsList =
            new Dictionary<ElementIdValue, OverrideGraphicSettings>();
        var collector = new FilteredElementCollector(document);
        var filter = new ElementCategoryFilter(
            BuiltInCategory.OST_FabricationDuctwork);
        IEnumerable<FabricationPart> allFabDuct =
            collector
            .WherePasses(filter)
            .WhereElementIsNotElementType()
            .Cast<FabricationPart>();

        using var transaction = new Transaction(document);
        transaction.Start("Override graphic settings");

        foreach (FabricationPart fabDuct in allFabDuct)
        {
            var fabDuctId = new ElementIdValue(fabDuct.Id);

            if (originalOverrideGraphicSettingsList.ContainsKey(fabDuctId))
                continue;

            Parameter itemNumberParameter = fabDuct.get_Parameter(
                BuiltInParameter.FABRICATION_PART_ITEM_NUMBER);

            if (itemNumberParameter.AsString() == "")
                continue;

            OverrideGraphicSettings originalOverrideGraphicSettings
                    = view.GetElementOverrides(fabDuct.Id);
            view.SetElementOverrides(fabDuct.Id, overrideGraphicSettings);
            tempOriginalOverrideGraphicSettingsList.Add(
                fabDuctId,
                originalOverrideGraphicSettings);
        }

        if (transaction.Commit() != TransactionStatus.Committed)
            return false;

        foreach (
            KeyValuePair<ElementIdValue, OverrideGraphicSettings>
            originalOverrideGraphicSettings
            in tempOriginalOverrideGraphicSettingsList)
        {
            originalOverrideGraphicSettingsList.Add(
                originalOverrideGraphicSettings.Key,
                originalOverrideGraphicSettings.Value);
        }

        return true;
    }

    internal bool Revert()
    {
        using var transaction = new Transaction(document);
        transaction.Start("Revert graphic overrides");

        foreach (
            KeyValuePair<ElementIdValue, OverrideGraphicSettings>
            revertGraphicSettings
            in originalOverrideGraphicSettingsList)
        {
            var id = new ElementId(revertGraphicSettings.Key.value);
            view.SetElementOverrides(id, revertGraphicSettings.Value);
        }

        originalOverrideGraphicSettingsList.Clear();

        return transaction.Commit() == TransactionStatus.Committed;
    }
}