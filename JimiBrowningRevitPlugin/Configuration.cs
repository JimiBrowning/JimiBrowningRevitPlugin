using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JimiBrowningRevitPlugin;

internal class Configuration
{
    private const string FabConfigGuidNew =
        "F989FED5-EB36-4F0A-7FE4-BC37A3D1339A";
    private const string FabConfigGuidOld =
        "F93A9CE6-7C0E-4BBF-082E-F17068D04B84";

    private const string FabDimToSharedParamMap104String =
        "Width,Fab Width 1,Fab Width 2,Fab Width 3\n" +
        "Width In,Fab Width 1\n" +
        "Width Out,Fab Width 2\n" +
        "Louvre Width,Fab Width 1\n" +
        "Primary Width,Fab Width 1\n" +
        "Secondary Width,Fab Width 2\n" +
        "Tertiary Width,Fab Width 3\n" +
        "Height,Fab Height 1,Fab Height 2,Fab Height 3\n" +
        "Height In,Fab Height 1\n" +
        "Height Out,Fab Height 2\n" +
        "Split Height,Fab Height 2,Fab Height 3\n" +
        "Louvre Height,Fab Height 1\n" +
        "Primary Height,Fab Height 1\n" +
        "Secondary Height,Fab Height 2\n" +
        "Tertiary Height,Fab Height 3\n" +
        "Diameter,Fab Diameter 1,Fab Diameter 2,Fab Diameter 3\n" +
        "Diameter In,Fab Diameter 1\n" +
        "Diameter Out,Fab Diameter 2\n" +
        "Primary Diameter,Fab Diameter 1\n" +
        "Secondary Diameter,Fab Diameter 2\n" +
        "Tertiary Diameter,Fab Diameter 3\n" +
        "Oval Width,Fab Oval Width 1,Fab Oval Width 2,Fab Oval Width 3\n" +
        "Oval Width In,Fab Oval Width 1\n" +
        "Oval Width Out,Fab Oval Width 2\n" +
        "Primary Oval Width,Fab Oval Width 1\n" +
        "Secondary Oval Width,Fab Oval Width 2\n" +
        "Tertiary Oval Width,Fab Oval Width 3\n" +
        "Oval Height,Fab Oval Height 1,Fab Oval Height 2,Fab Oval Height 3\n" +
        "Oval Height In,Fab Oval Height 1\n" +
        "Oval Height Out,Fab Oval Height 2\n" +
        "Primary Oval Height,Fab Oval Height 1\n" +
        "Secondary Oval Height,Fab Oval Height 2\n" +
        "Tertiary Oval Height,Fab Oval Height 3\n" +
        "Primary Extension,Fab Extension 1\n" +
        "Secondary Extension,Fab Extension 2\n" +
        "Tertiary Extension,Fab Extension 3\n" +
        "Split Extension,Fab Extension 2,Fab Extension 3\n" +
        "Inner Radius,Fab Radius 1\n" +
        "Primary Radius,Fab Radius 1\n" +
        "Secondary Radius,Fab Radius 2\n" +
        "Tertiary Radius,Fab Radius 3\n" +
        "Length,Fab Length";

    private const string FabDimToSharedParamMap000String =
        "Width,FAB WIDTH 1,FAB WIDTH 2,FAB WIDTH 3\n" +
        "Depth,FAB HEIGHT 1,FAB HEIGHT 2,FAB HEIGHT 3\n" +
        "Width In,FAB WIDTH 1\n" +
        "Depth In,FAB HEIGHT 1\n" +
        "Width Out,FAB WIDTH 2\n" +
        "Depth Out,FAB HEIGHT 2\n" +
        "Btm Width,FAB WIDTH 1\n" +
        "Btm Depth,FAB HEIGHT 1,FAB HEIGHT 2,FAB HEIGHT 3\n" +
        "Top Width,FAB WIDTH 2\n" +
        "Left Width,FAB WIDTH 2\n" +
        "Right Width,FAB WIDTH 3\n" +
        "Inner Radius,Actual Radius,FAB RADIUS 1\n" +
        "Outer Radius,FAB RADIUS 2\n" +
        "Left Radius,FAB RADIUS 2\n" +
        "Right Radius,FAB RADIUS 3\n" +
        "Diameter,FAB DIAMETER 1\n" +
        "Top Extension,Sq Bend TE\n" +
        "Top Inner Extension,Sq Bend TE\n" +
        "Bottom Extension,Sq Bend BE\n" +
        "Duct Width,FAB WIDTH 1\n" +
        "Duct Depth,FAB HEIGHT 1\n" +
        "Step in Width,FAB WIDTH 2\n" +
        "Step In Depth,FAB HEIGHT 2\n" +
        "Branch Width,FAB WIDTH 1\n" +
        "Branch Depth,FAB HEIGHT 1\n" +
        "Branch Width #1,FAB WIDTH 1\n" +
        "Branch Depth #1,FAB HEIGHT 1";

    private readonly Dictionary<string, List<string>> fabDimToSharedParamMap =
        [];
    private readonly Dictionary<string, string>
        sharedParamBackCompatMap = new() {
            { "Fab Width 1", "FAB WIDTH 1" },
            { "Fab Width 2", "FAB WIDTH 2" },
            { "Fab Width 3", "FAB WIDTH 3" },
            { "Fab Height 1", "FAB HEIGHT 1" },
            { "Fab Height 2", "FAB HEIGHT 2" },
            { "Fab Height 3", "FAB HEIGHT 3" },
            { "Fab Diameter 1", "FAB DIAMETER 1" },
            { "Fab Extension 1", "Sq Bend TE" },
            { "Fab Extension 2", "Sq Bend BE" },
            { "Fab Radius 1", "Actual Radius" },
            { "Fab Radius 2", "FAB RADIUS 2" },
            { "Fab Radius 3", "FAB RADIUS 3" } };
    private readonly HashSet<string> projectParameters = [];
    private readonly HashSet<string> fabDuctSharedParameters = [];

    private double fabConfigVersion = 0;
    private bool isFabConfigNew = false;
    private bool isFabConfigOld = false;
    private string sharedParamWidth1 = "";
    private string sharedParamWidth2 = "";
    private string sharedParamWidth3 = "";
    private string sharedParamHeight1 = "";
    private string sharedParamHeight2 = "";
    private string sharedParamHeight3 = "";
    private string sharedParamDiameter1 = "";
    private string sharedParamRadius1 = "";

    internal bool IsAirmechDatabase { get => isFabConfigNew || isFabConfigOld; }

    internal string FabDimCentreRadius { get => "Centre Radius"; }
    internal string FabDimDiameter { get => "Diameter"; }
    internal string SharedParamWidth1 { get => sharedParamWidth1; }
    internal string SharedParamWidth2 { get => sharedParamWidth2; }
    internal string SharedParamWidth3 { get => sharedParamWidth3; }
    internal string SharedParamHeight1 { get => sharedParamHeight1; }
    internal string SharedParamHeight2 { get => sharedParamHeight2; }
    internal string SharedParamHeight3 { get => sharedParamHeight3; }
    internal string SharedParamOvalWidth1 { get => "Fab Oval Width 1"; }
    internal string SharedParamOvalWidth2 { get => "Fab Oval Width 2"; }
    internal string SharedParamOvalWidth3 { get => "Fab Oval Width 3"; }
    internal string SharedParamOvalHeight1 { get => "Fab Oval Height 1"; }
    internal string SharedParamOvalHeight2 { get => "Fab Oval Height 2"; }
    internal string SharedParamOvalHeight3 { get => "Fab Oval Height 3"; }
    internal string SharedParamDiameter1 { get => sharedParamDiameter1; }
    internal string SharedParamDiameter2 { get => "Fab Diameter 2"; }
    internal string SharedParamDiameter3 { get => "Fab Diameter 3"; }
    internal string SharedParamInternalWidth { get => "Internal Width"; }
    internal string SharedParamInternalHeight { get => "Internal Height"; }
    internal string SharedParamInternalOvalWidth
    {
        get => "Internal Oval Width";
    }
    internal string SharedParamInternalOvalHeight
    {
        get => "Internal Oval Height";
    }
    internal string SharedParamInternalDiameter { get => "Internal Diameter"; }
    internal string SharedParamRadius1 { get => sharedParamRadius1; }

    internal string FabParamBottomElevation
    {
        get =>
#if REVIT2023_OR_GREATER
        "Lower End Bottom Elevation";
#else
        "Bottom";
#endif
    }

    internal Dictionary<string, List<string>> FabDimToSharedParamMap
    {
        get => fabDimToSharedParamMap;
    }

    internal HashSet<string> ProjectParameters { get => projectParameters; }

    internal HashSet<string> FabDuctSharedParameters
    {
        get => fabDuctSharedParameters;
    }

    internal string SharedParamBottomLevel { get => "Bottom Level"; }

    internal Configuration(Document document)
    {
        GetFabricationConfigurationVersion(document);
        GetRevitProjectParameters(document);
        SetupParameterMapping();
    }

    private void GetFabricationConfigurationVersion(Document document)
    {
        FabricationConfiguration fabConfig =
            FabricationConfiguration.GetFabricationConfiguration(document);
        FabricationConfigurationInfo fabConfigInfo =
            fabConfig.GetFabricationConfigurationInfo();

        if (fabConfigInfo.GUID == new Guid(FabConfigGuidNew))
        {
            isFabConfigNew = true;
            isFabConfigOld = false;
            fabConfigVersion = fabConfigInfo.Version;
        }
        else if (fabConfigInfo.GUID == new Guid(FabConfigGuidOld))
        {
            isFabConfigNew = false;
            isFabConfigOld = true;
            fabConfigVersion = 0;
        }
        else
        {
            isFabConfigNew = false;
            isFabConfigOld = false;
            fabConfigVersion = -1;
        }
    }

    private void GetRevitProjectParameters(Document document)
    {
        BindingMap map = document.ParameterBindings;
        DefinitionBindingMapIterator it = map.ForwardIterator();

        while (it.MoveNext())
        {
            projectParameters.Add(it.Key.Name);
        }
    }

    private void SetupParameterMapping()
    {
        string fabDimToSharedParamMapString;
        if (fabConfigVersion >= 1.04)
            fabDimToSharedParamMapString = FabDimToSharedParamMap104String;
        else
            fabDimToSharedParamMapString = FabDimToSharedParamMap000String;

        foreach (string line in fabDimToSharedParamMapString.Split('\n'))
        {
            string[] words = line.Split(',');
            string fabricationDimension = words[0];
            List<string> sharedParameters = new(words.Skip(1));
            List<string> mappedSharedParameters = [];

            foreach (string mappedSharedParameter in sharedParameters)
            {
                if (projectParameters.Contains(mappedSharedParameter))
                {
                    mappedSharedParameters.Add(mappedSharedParameter);
                    fabDuctSharedParameters.Add(mappedSharedParameter);
                }
                else if (sharedParamBackCompatMap.TryGetValue(
                    mappedSharedParameter,
                    out string compatSharedParameter))
                {
                    if (projectParameters.Contains(compatSharedParameter))
                    {
                        mappedSharedParameters.Add(compatSharedParameter);
                        fabDuctSharedParameters.Add(mappedSharedParameter);
                    }
                }
            }

            fabDimToSharedParamMap.Add(
                fabricationDimension,
                mappedSharedParameters);
        }

        if (projectParameters.Contains(SharedParamBottomLevel))
            fabDuctSharedParameters.Add(SharedParamBottomLevel);
        if (projectParameters.Contains(SharedParamInternalWidth))
            fabDuctSharedParameters.Add(SharedParamInternalWidth);
        if (projectParameters.Contains(SharedParamInternalHeight))
            fabDuctSharedParameters.Add(SharedParamInternalHeight);
        if (projectParameters.Contains(SharedParamInternalOvalWidth))
            fabDuctSharedParameters.Add(SharedParamInternalOvalWidth);
        if (projectParameters.Contains(SharedParamInternalOvalHeight))
            fabDuctSharedParameters.Add(SharedParamInternalOvalHeight);
        if (projectParameters.Contains(SharedParamInternalDiameter))
            fabDuctSharedParameters.Add(SharedParamInternalDiameter);

        if (projectParameters.Contains("Fab Width 1"))
            sharedParamWidth1 = "Fab Width 1";
        else
            sharedParamWidth1 = "FAB WIDTH 1";

        if (projectParameters.Contains("Fab Width 2"))
            sharedParamWidth2 = "Fab Width 2";
        else
            sharedParamWidth2 = "FAB WIDTH 2";

        if (projectParameters.Contains("Fab Width 3"))
            sharedParamWidth3 = "Fab Width 3";
        else
            sharedParamWidth3 = "FAB WIDTH 3";

        if (projectParameters.Contains("Fab Height 1"))
            sharedParamHeight1 = "Fab Height 1";
        else
            sharedParamHeight1 = "FAB HEIGHT 1";

        if (projectParameters.Contains("Fab Height 2"))
            sharedParamHeight2 = "Fab Height 2";
        else
            sharedParamHeight2 = "FAB HEIGHT 2";

        if (projectParameters.Contains("Fab Height 3"))
            sharedParamHeight3 = "Fab Height 3";
        else
            sharedParamHeight3 = "FAB HEIGHT 3";

        if (projectParameters.Contains("Fab Diameter 1"))
            sharedParamDiameter1 = "Fab Diameter 1";
        else
            sharedParamDiameter1 = "FAB DIAMETER 1";

        if (projectParameters.Contains("Fab Radius 1"))
            sharedParamRadius1 = "Fab Radius 1";
        else
            sharedParamRadius1 = "Actual Radius";
    }
}