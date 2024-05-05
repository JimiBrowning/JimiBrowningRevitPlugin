using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace JimiBrowningRevitPlugin;

[Transaction(TransactionMode.Manual)]
internal class SyncParametersCommand : IExternalCommand
{
    internal const string NAME = "Sync Parameters";
    internal const string TOOL_TIP =
        "Syncs inaccessible parameters to shared parameters.";
    internal static readonly string CLASS_NAME =
        typeof(SyncParametersCommand).Namespace
        + "."
        + typeof(SyncParametersCommand).Name;

    private Document document;
    private readonly SharedParameters sharedParameters = new();

    public Result Execute(
        ExternalCommandData externalCommandData,
        ref string message,
        ElementSet elementSet)
    {
        document = externalCommandData.Application.ActiveUIDocument.Document;

        using var transactionGroup = new TransactionGroup(document);
        transactionGroup.Start(NAME);

        var changeCount = new ChangeCount(0, 0);
        changeCount += SyncFabDuctParameters();
        changeCount += SyncGrilleSpigotSizes();

        // TODO: Sync RL height and name

        transactionGroup.Assimilate();

        if (sharedParameters.HasMissingParameters())
        {
            string missingParameters =
                string.Join(", ", sharedParameters.GetMissingParameters());
            new TaskDialog(NAME)
            {
                MainContent = "Warning! Following shared parameter(s) are " +
                "missing: " + missingParameters,
                CommonButtons = TaskDialogCommonButtons.Ok
            }.Show();
        }

        if (sharedParameters.HasReadOnlyParameters())
        {
            string readOnlyParameters =
                string.Join(", ", sharedParameters.GetReadOnlyParameters());
            new TaskDialog(NAME)
            {
                MainContent = "Warning! Following shared parameter(s) are " +
                "read-only: " + readOnlyParameters,
                CommonButtons = TaskDialogCommonButtons.Ok
            }.Show();
        }

        new TaskDialog(NAME)
        {
            MainContent = "Sync complete. " +
            changeCount.parameterChangeCount.ToString() + " change(s) to shared " +
            "parameters of " + changeCount.elementChangeCount.ToString() +
            " element(s).",
            CommonButtons = TaskDialogCommonButtons.Ok
        }.Show();

        return Result.Succeeded;
    }

    private ChangeCount SyncFabDuctParameters()
    {
        var changeCount = new ChangeCount(0, 0);

        IEnumerable<FabricationPart> fabDucts =
            Elements.GetAllFabDucts(document);

        using var transaction = new Transaction(document);
        transaction.Start("Sync fab duct parameters");

        foreach (FabricationPart fabDuct in fabDucts)
        {
            var fabDuctParameters = new FabDuctSharedParameters();
            fabDuctParameters.Process(fabDuct);
            bool hasChanges = false;

            foreach (
                KeyValuePair<string, double> sharedParameter
                in fabDuctParameters.SharedParameters)
            {
                Parameter parameter =
                    sharedParameters.GetRequired(fabDuct, sharedParameter.Key);

                if (parameter == null)
                    continue;

                if (!parameter.HasValue
                    || !Equals(
                        parameter.AsDouble(),
                        sharedParameter.Value,
                        0.000001D))
                {
                    if (sharedParameters.Set(parameter, sharedParameter.Value))
                    {
                        changeCount.parameterChangeCount++;

                        if (!hasChanges)
                        {
                            changeCount.elementChangeCount++;
                            hasChanges = true;
                        }
                    }
                }
            }
        }

        if (transaction.Commit() != TransactionStatus.Committed)
        {
            new TaskDialog(NAME)
            {
                MainContent = "Failed to commit transaction. Fab duct " +
                "parameters not synced.",
                CommonButtons = TaskDialogCommonButtons.Ok
            }.Show();

            return new ChangeCount(0, 0);
        }

        return changeCount;
    }

    private ChangeCount SyncGrilleSpigotSizes()
    {
        var changeCount = new ChangeCount(0, 0);

        IEnumerable<Element> grilles = Elements.GetAllGrilles(document);

        using var transaction = new Transaction(document);
        transaction.Start("Sync grilles spigot sizes");

        foreach (Element grille in grilles)
        {
            Parameter spigotSizeOnScheduleParameter =
                sharedParameters.Get(grille, "Spigot Size on Schedule");
            if (spigotSizeOnScheduleParameter == null)
                continue;

            Parameter cuBoxParameter =
                sharedParameters.Get(grille, "Cu_Box");

            bool cuBox =
                cuBoxParameter != null && cuBoxParameter.AsInteger() != 0;
            string spigotSizeOnSchedule = "";

            if (cuBox)
            {
                Parameter spigotSizeParameter =
                    sharedParameters.GetRequired(grille, "Spigot_Size");
                Parameter remarksSpigotsParameter =
                    sharedParameters.GetRequired(grille, "Remarks_Spigot");

                string spigotSize = spigotSizeParameter != null
                    ? FeetToMM(spigotSizeParameter.AsDouble()).ToString()
                    : "";
                string remarksSpigot = remarksSpigotsParameter != null
                    ? remarksSpigotsParameter.AsString()
                    : "";

                spigotSizeOnSchedule = spigotSize + remarksSpigot;
            }

            if (!spigotSizeOnScheduleParameter.HasValue
                || spigotSizeOnScheduleParameter.AsString()
                != spigotSizeOnSchedule)
            {
                if (sharedParameters.Set(
                    spigotSizeOnScheduleParameter,
                    spigotSizeOnSchedule))
                {
                    changeCount.elementChangeCount++;
                    changeCount.parameterChangeCount++;
                }
            }
        }

        if (transaction.Commit() != TransactionStatus.Committed)
        {
            new TaskDialog(NAME)
            {
                MainContent = "Failed to commit transaction. Spigot sizes " +
                "not synced.",
                CommonButtons = TaskDialogCommonButtons.Ok
            }.Show();

            return new ChangeCount(0, 0);
        }

        return changeCount;
    }

    private int FeetToMM(double feet)
    {
        return (int)(feet * 304.8);
    }

    private bool Equals(double x, double y, double precision)
    {
        return Math.Abs(x - y) < precision;
    }
}