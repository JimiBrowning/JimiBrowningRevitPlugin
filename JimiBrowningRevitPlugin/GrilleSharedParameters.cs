using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace JimiBrowningRevitPlugin;

internal class GrilleSharedParameters
{
    private const string SharedParamSpigotSize = "Spigot_Size";
    private const string SharedParamRemarksSpigot = "Remarks_Spigot";
    private const string SharedParamSpigotSizeOnSchedule =
        "Spigot Size on Schedule";
    private const string SharedParamCuBox = "Cu_Box";

    internal IReadOnlyDictionary<string, string> SharedParameters
    {
        get => sharedParameters;
    }

    private readonly Dictionary<string, string> sharedParameters;

    internal GrilleSharedParameters(Element grille)
    {
        sharedParameters = new Dictionary<string, string>()
        {
            { SharedParamSpigotSizeOnSchedule, "" }
        };

        SetSpigotSizeOnSchedule(grille);
    }

    private void SetSpigotSizeOnSchedule(Element grille)
    {
        Parameter? spigotSizeOnScheduleParameter =
            grille.LookupParameter(SharedParamSpigotSizeOnSchedule);
        if (spigotSizeOnScheduleParameter == null)
            return;

        Parameter? cuBoxParameter = grille.LookupParameter(SharedParamCuBox);

        bool cuBox =
            cuBoxParameter != null && cuBoxParameter.AsInteger() != 0;
        string spigotSizeOnSchedule = "";

        if (cuBox)
        {
            Parameter? spigotSizeParameter =
                grille.LookupParameter(SharedParamSpigotSize);
            Parameter? remarksSpigotsParameter =
                grille.LookupParameter(SharedParamRemarksSpigot);

            string spigotSize =
                spigotSizeParameter != null
                ? Units.Round(Units.FeetToMm(spigotSizeParameter.AsDouble()))
                .ToString()
                : "";
            string remarksSpigot =
                remarksSpigotsParameter != null
                ? remarksSpigotsParameter.AsString()
                : "";

            spigotSizeOnSchedule = spigotSize + remarksSpigot;
        }

        sharedParameters[SharedParamSpigotSizeOnSchedule] =
            spigotSizeOnSchedule;
    }
}