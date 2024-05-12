using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace JimiBrowningRevitPlugin;

[Transaction(TransactionMode.Manual)]
internal class SyncParametersCommand : IExternalCommand
{
    internal const string Name = "Sync Parameters";
    internal const string ToolTip =
        "Syncs inaccessible parameters to shared parameters.";

    internal static readonly string ClassName =
        typeof(SyncParametersCommand).Namespace
        + "."
        + typeof(SyncParametersCommand).Name;

    private readonly SharedParameters sharedParameters = new();
    private readonly ChangeLog changeLog = new();

    private Document document;
    private Elements elements;

    public Result Execute(
        ExternalCommandData externalCommandData,
        ref string message,
        ElementSet elementSet)
    {
        UIDocument uiDocument =
            externalCommandData.Application.ActiveUIDocument;
        document = uiDocument.Document;
        elements = new Elements(uiDocument);

        using var transactionGroup = new TransactionGroup(document);
        transactionGroup.Start(Name);

        SyncFabDuctParameters();
        SyncGrilleSpigotSizes();
        SyncElementLevels();

        transactionGroup.Assimilate();

        if (sharedParameters.HasMissingParameters())
        {
            string missingParameters =
                string.Join(", ", sharedParameters.GetMissingParameters());
            new TaskDialog(Name)
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
            new TaskDialog(Name)
            {
                MainContent = "Warning! Following shared parameter(s) are " +
                "read-only: " + readOnlyParameters,
                CommonButtons = TaskDialogCommonButtons.Ok
            }.Show();
        }

        new TaskDialog(Name)
        {
            MainContent = "Sync complete. " +
            changeLog.ParameterCount.ToString() + " change(s) to " +
            "shared parameters of " +
            changeLog.ElementCount.ToString() + " element(s).",
            CommonButtons = TaskDialogCommonButtons.Ok
        }.Show();

        return Result.Succeeded;
    }

    private void SyncFabDuctParameters()
    {
        var newChangeLog = new ChangeLog();
        IEnumerable<FabricationPart> fabDucts = elements.GetAllFabDucts();

        using var transaction = new Transaction(document);
        transaction.Start("Sync fab duct parameters");

        foreach (FabricationPart fabDuct in fabDucts)
        {
            var fabDuctIdValue = new ElementIdValue(fabDuct.Id);
            var fabDuctParameters = new FabDuctSharedParameters();
            fabDuctParameters.Process(fabDuct);

            foreach (
                KeyValuePair<string, double> sharedParameter
                in fabDuctParameters.SharedParameters)
            {
                Parameter parameter =
                    sharedParameters.GetRequired(fabDuct, sharedParameter.Key);

                if (parameter == null)
                    continue;

                if (!parameter.HasValue
                    || !EqualsWithError(
                        parameter.AsDouble(),
                        sharedParameter.Value))
                {
                    if (sharedParameters.Set(parameter, sharedParameter.Value))
                        newChangeLog.Add(
                            fabDuctIdValue,
                            sharedParameter.Key,
                            sharedParameter.Value.ToString("0.0"));
                }
            }
        }

        if (transaction.Commit() == TransactionStatus.Committed)
        {
            changeLog.Add(newChangeLog);
        }
        else
        {
            new TaskDialog(Name)
            {
                MainContent = "Failed to commit transaction. Fab duct " +
                "parameters not synced.",
                CommonButtons = TaskDialogCommonButtons.Ok
            }.Show();
        }
    }

    private void SyncGrilleSpigotSizes()
    {
        var newChangeLog = new ChangeLog();
        IEnumerable<Element> grilles = elements.GetAllGrilles();

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
                    var grilleIdValue = new ElementIdValue(grille.Id);
                    newChangeLog.Add(
                        grilleIdValue,
                        "Spigot Size on Schedule",
                        spigotSizeOnSchedule);
                }
            }
        }

        if (transaction.Commit() == TransactionStatus.Committed)
        {
            changeLog.Add(newChangeLog);
        }
        else
        {
            new TaskDialog(Name)
            {
                MainContent = "Failed to commit transaction. Spigot sizes " +
                "not synced.",
                CommonButtons = TaskDialogCommonButtons.Ok
            }.Show();
        }
    }

    private void SyncElementLevels()
    {
        var newChangeLog = new ChangeLog();
        IEnumerable<Level> levels = elements.GetAllLevels();

        using var transaction = new Transaction(document);
        transaction.Start("Sync element levels");

        foreach (Level level in levels)
        {
            IEnumerable<Element> elementsInLevel =
                elements.GetAllElementsOfLevel(level.Id);

            foreach (Element element in elementsInLevel)
            {
                var elementIdValue = new ElementIdValue(element.Id);
                Parameter rlNameParameter =
                    sharedParameters.Get(element, "RL Name");
                Parameter rlHeightParameter =
                    sharedParameters.Get(element, "RL Height");

                if (rlNameParameter != null &&
                    rlNameParameter.AsString() != level.Name)
                {
                    if (sharedParameters.Set(rlNameParameter, level.Name))
                    {
                        newChangeLog.Add(elementIdValue, "RL Name", level.Name);
                    }
                }

                if (rlHeightParameter != null)
                {
                    if (!rlHeightParameter.HasValue
                        || !EqualsWithError(
                            rlHeightParameter.AsDouble(),
                            level.Elevation))
                    {
                        if (sharedParameters
                            .Set(rlHeightParameter, level.Elevation))
                        {
                            newChangeLog.Add(
                                elementIdValue,
                                "RL Height",
                                level.Elevation.ToString("0.0"));
                        }
                    }
                }
            }
        }

        if (transaction.Commit() == TransactionStatus.Committed)
        {
            changeLog.Add(newChangeLog);
        }
        else
        {
            new TaskDialog(Name)
            {
                MainContent = "Failed to commit transaction. Element levels " +
                "not synced.",
                CommonButtons = TaskDialogCommonButtons.Ok
            }.Show();
        }
    }

    private int FeetToMM(double feet)
    {
        double roundFactor = (feet >= 0.0) ? 0.5 : -0.5;
        return (int)(feet * 304.8 + roundFactor);
    }

    private bool EqualsWithError(double x, double y)
    {
        return Math.Abs(x - y) < 1E-6;
    }
}