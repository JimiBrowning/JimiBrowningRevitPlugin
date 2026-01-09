using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JimiBrowningRevitPlugin;

[Transaction(TransactionMode.Manual)]
internal class SyncParametersCommand : IExternalCommand
{
    internal const string Name = "Sync Parameters";
    internal const string ToolTip =
        "Syncs inaccessible parameters to shared parameters.";
    internal const string Icon16 = "SyncIcon16.png";
    internal const string Icon32 = "SyncIcon32.png";

    private const string SharedParamRlName = "RL Name";
    private const string SharedParamRlHeight = "RL Height";
    private const string SharedParamSystemId = "System ID";

    private const string ProjectParamScheduleSortId = "Schedule Sort ID";

    internal static readonly string ClassName =
        typeof(SyncParametersCommand).Namespace
        + "."
        + typeof(SyncParametersCommand).Name;

    private readonly SharedParameters sharedParameters = new();
    private readonly ChangeLog changeLog = new();

    private Document? document;
    private Elements? elements;
    private Configuration? config;

    public Result Execute(
        ExternalCommandData externalCommandData,
        ref string message,
        ElementSet elementSet)
    {
        UIDocument uiDocument =
            externalCommandData.Application.ActiveUIDocument;
        document = uiDocument.Document;

        if (document.IsFamilyDocument)
        {
            new TaskDialog(Name)
            {
                MainInstruction = "Family document",
                MainContent =
                    "This feature can't be used in a family.",
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();

            return Result.Failed;
        }

        config = new Configuration(document);

        if (!config.IsAirmechDatabase)
        {
            new TaskDialog(Name)
            {
                MainInstruction = "Fabrication configuration not supported",
                MainContent =
                    "This feature is only intended for Airmech Fabrication " +
                    "configurations.",
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();

            return Result.Failed;
        }

        elements = new Elements(uiDocument);

        using var transactionGroup = new TransactionGroup(document);
        transactionGroup.Start(Name);
        SyncFabDuctParameters();
        SyncGrilleSpigotSizes();
        SyncElementLevels();
        SyncScheduleSortIds();
        transactionGroup.Assimilate();

        ShowDialogs();

        return Result.Succeeded;
    }

    private void SyncFabDuctParameters()
    {
        var newChangeLog = new ChangeLog();
        IEnumerable<FabricationPart> fabDucts = elements!.GetAllFabDucts();

        using var transaction = new Transaction(document);
        transaction.Start("Sync fab duct parameters");

        foreach (FabricationPart fabDuct in fabDucts)
        {
            var fabDuctIdValue = new ElementIdValue(fabDuct.Id);
            var fabDuctParameters =
                new FabDuctSharedParameters(fabDuct, config!);

            foreach (
                KeyValuePair<string, double> sharedParameter
                in fabDuctParameters.SharedParameters)
            {
                Parameter? parameter =
                    sharedParameters.GetRequired(fabDuct, sharedParameter.Key);

                if (parameter == null)
                    continue;

                if (!parameter.HasValue
                    || !Units.EqualsWithError(
                        parameter.AsDouble(),
                        sharedParameter.Value))
                {
                    if (sharedParameters.Set(parameter, sharedParameter.Value))
                        newChangeLog.Add(
                            fabDuctIdValue,
                            sharedParameter.Key,
                            Units
                            .FeetToMm(sharedParameter.Value)
                            .ToString("0.0"));
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
                MainInstruction = "Warning!",
                MainContent =
                    "Failed to commit transaction. Fab duct parameters not " +
                    "synced.",
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();
        }
    }

    private void SyncGrilleSpigotSizes()
    {
        var newChangeLog = new ChangeLog();
        IEnumerable<Element> grilles = elements!.GetAllGrilles();

        using var transaction = new Transaction(document);
        transaction.Start("Sync grilles spigot sizes");

        foreach (Element grille in grilles)
        {
            var grilleIdValue = new ElementIdValue(grille.Id);
            var grilleParameters = new GrilleSharedParameters(grille);

            foreach (
                KeyValuePair<string, string> sharedParameter
                in grilleParameters.SharedParameters)
            {
                Parameter? parameter =
                    sharedParameters.GetRequired(grille, sharedParameter.Key);

                if (parameter == null)
                    continue;

                if (!parameter.HasValue
                    || parameter.AsString() != sharedParameter.Value)
                {
                    if (sharedParameters.Set(parameter, sharedParameter.Value))
                        newChangeLog.Add(
                            grilleIdValue,
                            sharedParameter.Key,
                            sharedParameter.Value);
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
                MainInstruction = "Warning!",
                MainContent =
                    "Failed to commit transaction. Spigot sizes not synced.",
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();
        }
    }

    private void SyncElementLevels()
    {
        var newChangeLog = new ChangeLog();
        IEnumerable<Level> levels = elements!.GetAllLevels();

        using var transaction = new Transaction(document);
        transaction.Start("Sync element levels");

        foreach (Level level in levels)
        {
            IEnumerable<Element> elementsInLevel =
                elements.GetAllElementsOfLevel(level.Id);

            foreach (Element element in elementsInLevel)
            {
                var elementIdValue = new ElementIdValue(element.Id);
                Parameter? rlNameParameter =
                    sharedParameters.Get(element, SharedParamRlName);
                Parameter? rlHeightParameter =
                    sharedParameters.Get(element, SharedParamRlHeight);

                if (rlNameParameter != null &&
                    rlNameParameter.AsString() != level.Name)
                {
                    if (sharedParameters.Set(rlNameParameter, level.Name))
                        newChangeLog.Add(
                            elementIdValue,
                            SharedParamRlName,
                            level.Name);
                }

                if (rlHeightParameter != null)
                {
                    if (!rlHeightParameter.HasValue
                        || !Units.EqualsWithError(
                            rlHeightParameter.AsDouble(),
                            level.Elevation))
                    {
                        if (sharedParameters.Set(
                            rlHeightParameter,
                            level.Elevation))
                        {
                            newChangeLog.Add(
                                elementIdValue,
                                SharedParamRlHeight,
                                Units.
                                FeetToMm(level.Elevation)
                                .ToString("0.0"));
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
                MainInstruction = "Warning!",
                MainContent =
                    "Failed to commit transaction. Element levels not synced.",
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();
        }
    }

    private void SyncScheduleSortIds()
    {
        IEnumerable<Element> allMechanicalEquipment =
            elements!.GetAllMechanicalEquipment();

        Element? firstMechanicalEquipment =
            allMechanicalEquipment.FirstOrDefault();

        if (firstMechanicalEquipment == null)
            return;

        Parameter firstScheduleSortIdParameter =
            firstMechanicalEquipment
            .LookupParameter(ProjectParamScheduleSortId);

        if (firstScheduleSortIdParameter == null)
            return;

        var newChangeLog = new ChangeLog();

        using var transaction = new Transaction(document);
        transaction.Start("Sync schedule sort IDs");

        foreach (Element mechanicalEquipment in allMechanicalEquipment)
        {
            Parameter systemIdParameter =
                mechanicalEquipment.LookupParameter(SharedParamSystemId);
            Parameter scheduleSortIdParameter =
                mechanicalEquipment.LookupParameter(ProjectParamScheduleSortId);

            string? systemId = systemIdParameter?.AsString();
            string scheduleSortId = "";
            string currentScheduleSortId = scheduleSortIdParameter.AsString();

            if (systemId != null && !string.IsNullOrEmpty(systemId))
            {
                string pattern = "[^a-zA-Z\\d]";
                Match match = Regex.Match(systemId, pattern);
                if (match.Success && match.Index + 1 < systemId.Length)
                {
                    int index = match.Index;

                    string systemType = systemId.Substring(0, index);
                    string systemNumber = systemId.Substring(index + 1);
                    string systemSortSuffix = "";

                    switch (systemType)
                    {
                        case "CU":
                        case "VRV":
                        case "VRF":
                        case "ACCU":
                            systemSortSuffix = " [1 - " + systemType + "]";
                            break;

                        case "FCU":
                        case "AC":
                            systemSortSuffix = " [2 - " + systemType + "]";
                            break;

                        default:
                            systemNumber = systemId;
                            break;
                    }

                    scheduleSortId = systemNumber + systemSortSuffix;
                }
                else
                {
                    scheduleSortId = systemId;
                }
            }

            if (scheduleSortId != currentScheduleSortId)
            {
                scheduleSortIdParameter.Set(scheduleSortId);

                var elementIdValue = new ElementIdValue(mechanicalEquipment.Id);
                changeLog.Add(
                    elementIdValue,
                    ProjectParamScheduleSortId,
                    scheduleSortId);
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
                MainInstruction = "Warning!",
                MainContent =
                    "Failed to commit transaction. Schedule sort IDs not " +
                    "synced.",
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();
        }
    }

    private void ShowDialogs()
    {
        if (sharedParameters.HasMissingParameters()
            || sharedParameters.HasReadOnlyParameters())
        {
            var stringBuilder = new StringBuilder();

            if (sharedParameters.HasMissingParameters())
            {
                string missingParameters =
                    string.Join(", ", sharedParameters.GetMissingParameters());
                stringBuilder.AppendLine(
                    "Following shared parameter(s) are missing: " +
                    missingParameters + ".");
            }

            if (sharedParameters.HasReadOnlyParameters())
            {
                string readOnlyParameters =
                    string.Join(", ", sharedParameters.GetReadOnlyParameters());
                stringBuilder.AppendLine(
                    "Following shared parameter(s) are read-only: " +
                    readOnlyParameters + ".");
            }

            new TaskDialog(Name)
            {
                MainInstruction = "Warning!",
                MainContent = stringBuilder.ToString(),
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();
        }

        var syncCompleteDialog = new TaskDialog(Name)
        {
            MainInstruction = "Sync complete",
            MainContent =
                changeLog.ParameterCount.ToString() + " change(s) to shared " +
                "parameters of " + changeLog.ElementCount.ToString() +
                " element(s).",
            CommonButtons = TaskDialogCommonButtons.Close
        };

        if (changeLog.ElementCount > 0)
        {
            syncCompleteDialog.AddCommandLink(
                TaskDialogCommandLinkId.CommandLink1,
                "View changes");
        }

        TaskDialogResult taskDialogResult = syncCompleteDialog.Show();

        if (taskDialogResult == TaskDialogResult.CommandLink1)
        {
            new TaskDialog(Name)
            {
                MainInstruction = "Change log",
                MainContent = changeLog.ToString(),
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();
        }
    }
}