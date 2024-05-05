using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JimiBrowningRevitPlugin;

internal class RenumberFabDuctEventHandler : IExternalEventHandler
{
    internal const string NAME = "Renumber Fab Duct";

    private readonly RenumberFabDuctSettings settings =
        RenumberFabDuctSettings.Instance;
    private GraphicOverrides graphicsOverrides;

    public void Execute(UIApplication uiApplication)
    {
        UIDocument uiDocument = uiApplication.ActiveUIDocument;
        Document document = uiDocument.Document;

        graphicsOverrides = new GraphicOverrides(document);

        RenumberFabDuctForm.DisableSettings();

        using var transactionGroup = new TransactionGroup(document);
        transactionGroup.Start(NAME);

        if (settings.LockAllWithItemNumber)
        {
            if (!graphicsOverrides.FadeFabDuctsWithItemNumber())
            {
                new TaskDialog(NAME)
                {
                    MainContent = "Failed to commit transaction. Element " +
                    "graphic settings not overridden.",
                    CommonButtons = TaskDialogCommonButtons.Ok
                }.Show();
            }
        }

        // TODO: Hide non duct parts

        List<string> itemNumbers = Elements.GetAllFabDuctItemNumbers(document);

        while (true)
        {
            HashSet<ElementIdValue> overridenFabDuctIds =
                graphicsOverrides.GetOverridenFabDuctIds();
            FabricationPart fabDuct = Elements.SelectFabDuct(
                uiDocument,
                settings.LockAllWithItemNumber,
                overridenFabDuctIds);

            if (fabDuct == null)
                break;

            string previousItemNumber = Elements.GetFabDuctItemNumber(fabDuct);
            if (previousItemNumber != "")
                itemNumbers.Remove(previousItemNumber);

            string itemNumber = settings.ItemNumber;
            if (settings.PreventDuplicates
                && itemNumbers.Contains(
                    itemNumber,
                    StringComparer.OrdinalIgnoreCase))
            {
                new TaskDialog(NAME)
                {
                    MainContent = "Item number \"" +
                    itemNumber + "\" exists. Please " +
                    "choose a different item number or turn off duplicate " +
                    "checks.",
                    CommonButtons = TaskDialogCommonButtons.Ok
                }.Show();

                if (previousItemNumber != "")
                    itemNumbers.Add(previousItemNumber);

                continue;
            }

            if (Elements.SetFabDuctItemNumber(document, fabDuct, itemNumber))
            {
                itemNumbers.Add(itemNumber);
            }
            else
            {
                new TaskDialog(NAME)
                {
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
                    new TaskDialog(NAME)
                    {
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
            new TaskDialog(NAME)
            {
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