using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace JimiBrowningRevitPlugin;

internal class FabDuctSelectionFilter : ISelectionFilter
{
    private readonly bool lockAllWithItemNumber;
    private readonly HashSet<ElementIdValue> lockElementIds;
    private readonly ElementId fabDuctCategoryId =
        new(BuiltInCategory.OST_FabricationDuctwork);

    internal FabDuctSelectionFilter(
        bool lockAllWithItemNumber,
        HashSet<ElementIdValue> lockElementIds)
    {
        this.lockAllWithItemNumber = lockAllWithItemNumber;
        this.lockElementIds = lockElementIds;
    }

    public bool AllowElement(Element element)
    {
        if (element.Category == null
            || element.Category.Id != fabDuctCategoryId)
        {
            return false;
        }

        if (lockAllWithItemNumber)
        {
            Parameter itemNumberParameter = element.get_Parameter(
                BuiltInParameter.FABRICATION_PART_ITEM_NUMBER);

            if (itemNumberParameter.AsString() != "")
                return false;
        }

        if (lockElementIds != null
            && lockElementIds.Contains(new ElementIdValue(element.Id)))
        {
            return false;
        }

        return true;
    }

    public bool AllowReference(Reference refer, XYZ point)
    {
        return false;
    }
}