using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JimiBrowningRevitPlugin;

internal class RenumberFabDuctEventHandler : IExternalEventHandler
{
    internal const string Name = "Renumber Fab Duct";

    private readonly RenumberFabDuctSettings settings =
        RenumberFabDuctSettings.Instance;

    private Elements elements;
    private GraphicOverrides graphicsOverrides;

    public void Execute(UIApplication uiApplication)
    {
        UIDocument uiDocument = uiApplication.ActiveUIDocument;
        Document document = uiDocument.Document;

        elements = new Elements(uiDocument);
        graphicsOverrides = new GraphicOverrides(document);

        RenumberFabDuctForm.DisableSettings();

        using var transactionGroup = new TransactionGroup(document);
        transactionGroup.Start(Name);

        if (settings.LockAllWithItemNumber)
        {
            if (!graphicsOverrides.FadeFabDuctsWithItemNumber())
            {
                new TaskDialog(Name)
                {
                    MainInstruction = "Warning!",
                    MainContent = "Failed to commit transaction. Element " +
                    "graphic settings not overridden.",
                    CommonButtons = TaskDialogCommonButtons.Ok
                }.Show();
            }
        }

        // TODO: Hide non duct parts

        List<string> itemNumbers = elements.GetAllFabDuctItemNumbers();

        while (true)
        {
            HashSet<ElementIdValue> overridenFabDuctIds =
                graphicsOverrides.GetOverridenFabDuctIds();
            FabricationPart fabDuct = elements.SelectFabDuct(
                settings.LockAllWithItemNumber,
                overridenFabDuctIds);

            if (fabDuct == null)
                break;

            string previousItemNumber = elements.GetFabDuctItemNumber(fabDuct);
            if (previousItemNumber != "")
                itemNumbers.Remove(previousItemNumber);

            string itemNumber = settings.ItemNumber;
            if (settings.PreventDuplicates
                && itemNumbers.Contains(
                    itemNumber,
                    StringComparer.OrdinalIgnoreCase))
            {
                new TaskDialog(Name)
                {
                    MainInstruction = "Duplicate item number",
                    MainContent = "Item number \"" +
                    itemNumber + "\" is already in use. Please " +
                    "choose a different item number or turn off duplicate " +
                    "checks.",
                    CommonButtons = TaskDialogCommonButtons.Ok
                }.Show();

                if (previousItemNumber != "")
                    itemNumbers.Add(previousItemNumber);

                continue;
            }

            if (elements.SetFabDuctItemNumber(fabDuct, itemNumber))
            {
                itemNumbers.Add(itemNumber);
            }
            else
            {
                new TaskDialog(Name)
                {
                    MainInstruction = "Warning!",
                    MainContent = "Failed to commit transaction. Item Number " +
                    "not set.",
                    CommonButtons = TaskDialogCommonButtons.Ok
                }.Show();

                if (previousItemNumber != "")
                    itemNumbers.Add(previousItemNumber);

                continue;
            }

            settings.IncrementItemNumber();

            if (settings.LockCurrent)
            {
                if (!graphicsOverrides.FadeFabDuct(fabDuct))
                {
                    new TaskDialog(Name)
                    {
                        MainInstruction = "Warning!",
                        MainContent = "Failed to commit transaction. Element " +
                        "graphic settings not overridden.",
                        CommonButtons = TaskDialogCommonButtons.Ok
                    }.Show();
                }
            }
        }


        if (graphicsOverrides.Revert())
        {
            transactionGroup.Assimilate();
        }
        else
        {
            new TaskDialog(Name)
            {
                MainInstruction = "Warning!",
                MainContent = "Failed to commit transaction. Could not " +
                "revert graphic overrides. Rolling back renumbering command.",
                CommonButtons = TaskDialogCommonButtons.Ok
            }.Show();

            transactionGroup.RollBack();
        }

        RenumberFabDuctForm.EnableSettings();
    }

    public string GetName()
    {
        return "Renumber fab duct";
    }

    internal delegate void RenumberFabDuctCallback();
}