using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace JimiBrowningRevitPlugin;

internal class FabDuctSelectionFilter : ISelectionFilter
{
    private static readonly ElementId FabDuctCategoryId =
        new(BuiltInCategory.OST_FabricationDuctwork);

    private readonly bool lockAllWithItemNumber;
    private readonly HashSet<ElementIdValue> lockElementIds;

    internal FabDuctSelectionFilter(
        bool lockAllWithItemNumber,
        HashSet<ElementIdValue> lockElementIds)
    {
        this.lockAllWithItemNumber = lockAllWithItemNumber;
        this.lockElementIds = lockElementIds;
    }

    public bool AllowElement(Element element)
    {
        if (element.Category?.Id != FabDuctCategoryId)
            return false;

        if (lockAllWithItemNumber)
        {
            Parameter itemNumberParameter =
                element.get_Parameter(
                    BuiltInParameter.FABRICATION_PART_ITEM_NUMBER);

            if (itemNumberParameter.AsString() != "")
                return false;
        }

        if (lockElementIds?.Contains(new ElementIdValue(element.Id)) == true)
            return false;

        return true;
    }

    public bool AllowReference(Reference refer, XYZ point)
    {
        return false;
    }
}