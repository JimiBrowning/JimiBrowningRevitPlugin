using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JimiBrowningRevitPlugin;

internal class Elements
{
    internal static FabricationPart SelectFabDuct(
        UIDocument uiDocument,
        bool lockAllWithItemNumber,
        HashSet<ElementIdValue> lockElementIds)
    {
        try
        {
            var fabDuctSelectionFilter = new FabDuctSelectionFilter(
                lockAllWithItemNumber,
                lockElementIds);
            Reference reference = uiDocument.Selection.PickObject(
                ObjectType.Element,
                fabDuctSelectionFilter,
                "Select a fabrication duct. Press ESC key to cancel.");

            return uiDocument.Document.GetElement(reference) as FabricationPart;
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            return null;
        }
    }

    internal static IEnumerable<FabricationPart>
        GetAllFabDucts(Document document)
    {
        var collector = new FilteredElementCollector(document);
        var filter = new ElementCategoryFilter(
            BuiltInCategory.OST_FabricationDuctwork);
        IEnumerable<FabricationPart> allFabDucts =
            collector
            .WherePasses(filter)
            .WhereElementIsNotElementType()
            .Cast<FabricationPart>();

        return allFabDucts;
    }

    internal static IEnumerable<FabricationPart>
        GetAllFabDuctsWithItemNumber(Document document)
    {
        var collector = new FilteredElementCollector(document);

        var fabDuctFilter = new ElementCategoryFilter(
            BuiltInCategory.OST_FabricationDuctwork);

        var itemNumberValueProvider = new ParameterValueProvider(
            new ElementId(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER));
        var filterStringGreater = new FilterStringGreater();
        var hasItemNumberFilterRule = new FilterStringRule(
                itemNumberValueProvider,
                filterStringGreater,
                "");
        var hasItemNumberFilter =
            new ElementParameterFilter(hasItemNumberFilterRule);

        var combinedFilter =
            new LogicalAndFilter(fabDuctFilter, hasItemNumberFilter);

        IEnumerable<FabricationPart> fabDucts = collector
            .WherePasses(combinedFilter)
            .WhereElementIsNotElementType()
            .Cast<FabricationPart>();

        return fabDucts;
    }

    internal static IEnumerable<FabricationPart>
        GetAllFabDuctsWithItemNumberPrefix(Document document, string prefix)
    {
        var collector = new FilteredElementCollector(document);

        var fabDuctFilter = new ElementCategoryFilter(
            BuiltInCategory.OST_FabricationDuctwork);

        var valueProvider = new ParameterValueProvider(
            new ElementId(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER));
        var evaluator = new FilterStringBeginsWith();
        var filterRule = new FilterStringRule(valueProvider, evaluator, prefix);
        var prefixFilter = new ElementParameterFilter(filterRule);

        var combinedFilter = new LogicalAndFilter(fabDuctFilter, prefixFilter);

        IEnumerable<FabricationPart> fabDucts = collector
            .WherePasses(combinedFilter)
            .WhereElementIsNotElementType()
            .Cast<FabricationPart>();

        return fabDucts;
    }

    internal static IEnumerable<Element> GetAllGrilles(Document document)
    {
        var collector = new FilteredElementCollector(document);
        var filter = new ElementCategoryFilter(
            BuiltInCategory.OST_DuctTerminal);
        IEnumerable<Element> grilles =
            collector
            .WherePasses(filter)
            .WhereElementIsNotElementType();

        return grilles;
    }

    internal static IEnumerable<Level> GetAllLevels(Document document)
    {
        var collector = new FilteredElementCollector(document);
        var filter = new ElementClassFilter(typeof(Level));
        IEnumerable<Level> levels =
            collector
            .WherePasses(filter)
            .ToElements()
            .Cast<Level>();

        return levels;
    }

    internal static IEnumerable<Element>
        GetAllElementsOfLevel(Document document, ElementId levelId)
    {
        var collector = new FilteredElementCollector(document);
        var filter = new ElementLevelFilter(levelId);
        IEnumerable<Element> elements =
            collector
            .WherePasses(filter)
            .WhereElementIsNotElementType();

        return elements;
    }

    internal static string GetFabDuctItemNumber(FabricationPart fabDuct)
    {
        Parameter itemNumberParameter = fabDuct.get_Parameter(
            BuiltInParameter.FABRICATION_PART_ITEM_NUMBER);
        return itemNumberParameter.AsString();
    }

    internal static List<string> GetAllFabDuctItemNumbers(Document document)
    {
        var itemNumbers = new List<string>();
        IEnumerable<FabricationPart> fabDucts =
            GetAllFabDuctsWithItemNumber(document);

        foreach (FabricationPart fabDuct in fabDucts)
        {
            string itemNumber = GetFabDuctItemNumber(fabDuct);

            if (itemNumber != "")
                itemNumbers.Add(itemNumber);
        }

        return itemNumbers;
    }

    internal static int
        GetHighestItemNumberWithPrefix(Document document, string prefix)
    {
        int highestNumber = 0;

        IEnumerable<FabricationPart> fabDucts =
            GetAllFabDuctsWithItemNumberPrefix(document, prefix);

        foreach (FabricationPart fabDuct in fabDucts)
        {
            string itemNumberString = fabDuct
                .get_Parameter(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER)
                .AsString()
                .Substring(prefix.Length);

            if (itemNumberString.Length == 0 || itemNumberString[0] == '0')
                continue;

            bool isItemNumberStringInt = int.TryParse(
                itemNumberString,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out int itemNumber);

            if (isItemNumberStringInt && itemNumber > highestNumber)
            {
                highestNumber = itemNumber;
            }
        }

        return highestNumber;
    }

    internal static bool SetFabDuctItemNumber(
        Document document,
        FabricationPart fabDuct,
        string itemNumber)
    {
        using var transaction = new Transaction(document);
        transaction.Start("Set fab duct Item Number");

        Parameter itemNumberParameter = fabDuct.get_Parameter(
            BuiltInParameter.FABRICATION_PART_ITEM_NUMBER);
        itemNumberParameter.Set(itemNumber);

        return transaction.Commit() == TransactionStatus.Committed;
    }
}