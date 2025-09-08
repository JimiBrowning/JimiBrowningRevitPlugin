using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JimiBrowningRevitPlugin;

internal class Elements
{
    private readonly UIDocument uiDocument;
    private readonly Document document;

    internal Elements(UIDocument uiDocument)
    {
        this.uiDocument = uiDocument;
        document = uiDocument.Document;
    }

    internal FabricationPart? SelectFabDuct(
        bool lockAllWithItemNumber,
        HashSet<ElementIdValue> lockElementIds)
    {
        try
        {
            var fabDuctSelectionFilter =
                new FabDuctSelectionFilter(
                    lockAllWithItemNumber,
                    lockElementIds);
            Reference reference =
                uiDocument.Selection.PickObject(
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

    internal IEnumerable<FabricationPart> GetAllFabDucts()
    {
        var collector = new FilteredElementCollector(document);
        var filter =
            new ElementCategoryFilter(BuiltInCategory.OST_FabricationDuctwork);
        IEnumerable<FabricationPart> allFabDucts =
            collector
            .WherePasses(filter)
            .WhereElementIsNotElementType()
            .Cast<FabricationPart>();

        return allFabDucts;
    }

    internal IEnumerable<FabricationPart> GetAllFabDuctsWithItemNumber()
    {
        var collector = new FilteredElementCollector(document);

        var fabDuctFilter =
            new ElementCategoryFilter(BuiltInCategory.OST_FabricationDuctwork);

        var itemNumberValueProvider =
            new ParameterValueProvider(
                new ElementId(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER));
        var filterStringGreater = new FilterStringGreater();
        var hasItemNumberFilterRule =
            RevitCompat.CreateFilterStringRule(
                itemNumberValueProvider,
                filterStringGreater,
                "");
        var hasItemNumberFilter =
            new ElementParameterFilter(hasItemNumberFilterRule);

        var combinedFilter =
            new LogicalAndFilter(fabDuctFilter, hasItemNumberFilter);

        IEnumerable<FabricationPart> fabDucts =
            collector
            .WherePasses(combinedFilter)
            .WhereElementIsNotElementType()
            .Cast<FabricationPart>();

        return fabDucts;
    }

    internal IEnumerable<FabricationPart>
        GetAllFabDuctsWithItemNumberPrefix(string prefix)
    {
        var collector = new FilteredElementCollector(document);

        var fabDuctFilter =
            new ElementCategoryFilter(BuiltInCategory.OST_FabricationDuctwork);

        var valueProvider =
            new ParameterValueProvider(
                new ElementId(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER));
        var evaluator = new FilterStringBeginsWith();
        var filterRule =
            RevitCompat.CreateFilterStringRule(
                valueProvider,
                evaluator,
                prefix);
        var prefixFilter = new ElementParameterFilter(filterRule);

        var combinedFilter = new LogicalAndFilter(fabDuctFilter, prefixFilter);

        IEnumerable<FabricationPart> fabDucts =
            collector
            .WherePasses(combinedFilter)
            .WhereElementIsNotElementType()
            .Cast<FabricationPart>();

        return fabDucts;
    }

    internal string GetFabDuctItemNumber(FabricationPart fabDuct)
    {
        Parameter itemNumberParameter =
            fabDuct.get_Parameter(
                BuiltInParameter.FABRICATION_PART_ITEM_NUMBER);
        return itemNumberParameter.AsString();
    }

    internal List<string> GetAllFabDuctItemNumbers()
    {
        var itemNumbers = new List<string>();
        IEnumerable<FabricationPart> fabDucts = GetAllFabDuctsWithItemNumber();

        foreach (FabricationPart fabDuct in fabDucts)
        {
            string itemNumber = GetFabDuctItemNumber(fabDuct);

            if (itemNumber != "")
                itemNumbers.Add(itemNumber);
        }

        return itemNumbers;
    }

    internal int GetHighestItemNumberWithPrefix(string prefix)
    {
        int highestNumber = 0;

        IEnumerable<FabricationPart> fabDucts =
            GetAllFabDuctsWithItemNumberPrefix(prefix);

        foreach (FabricationPart fabDuct in fabDucts)
        {
            string itemNumberString =
                fabDuct
                .get_Parameter(BuiltInParameter.FABRICATION_PART_ITEM_NUMBER)
                .AsString()
                .Substring(prefix.Length);

            if (itemNumberString.Length == 0 || itemNumberString[0] == '0')
                continue;

            bool isItemNumberStringInt =
                int.TryParse(
                    itemNumberString,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out int itemNumber);

            if (isItemNumberStringInt && itemNumber > highestNumber)
                highestNumber = itemNumber;
        }

        return highestNumber;
    }

    internal bool SetFabDuctItemNumber(
        FabricationPart fabDuct,
        string itemNumber)
    {
        using var transaction = new Transaction(document);
        transaction.Start("Set fab duct Item Number");

        Parameter itemNumberParameter =
            fabDuct.get_Parameter(
                BuiltInParameter.FABRICATION_PART_ITEM_NUMBER);
        itemNumberParameter.Set(itemNumber);

        return transaction.Commit() == TransactionStatus.Committed;
    }

    internal IEnumerable<Element> GetAllGrilles()
    {
        var collector = new FilteredElementCollector(document);
        var filter =
            new ElementCategoryFilter(BuiltInCategory.OST_DuctTerminal);
        IEnumerable<Element> grilles =
            collector
            .WherePasses(filter)
            .WhereElementIsNotElementType();

        return grilles;
    }

    internal IEnumerable<Level> GetAllLevels()
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

    internal IEnumerable<Element> GetAllElementsOfLevel(ElementId levelId)
    {
        var collector = new FilteredElementCollector(document);
        var filter = new ElementLevelFilter(levelId);
        IEnumerable<Element> elements =
            collector
            .WherePasses(filter)
            .WhereElementIsNotElementType();

        return elements;
    }

    internal IEnumerable<Element> GetAllMechanicalEquipment()
    {
        var collector = new FilteredElementCollector(document);
        var filter =
            new ElementCategoryFilter(BuiltInCategory.OST_MechanicalEquipment);
        IEnumerable<Element> elements =
            collector
            .WherePasses(filter)
            .WhereElementIsNotElementType();

        return elements;
    }
}