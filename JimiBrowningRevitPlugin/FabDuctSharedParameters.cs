using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace JimiBrowningRevitPlugin;

internal class FabDuctSharedParameters
{
#if REVIT2023_OR_GREATER
    private const string FabParamBottom = "Lower End Bottom Elevation";
#else
    private const string FabParamBottom = "Bottom";
#endif

    private const string DimensionWidth = "Width";
    private const string DimensionDepth = "Depth";
    private const string DimensionWidthIn = "Width In";
    private const string DimensionDepthIn = "Depth In";
    private const string DimensionWidthOut = "Width Out";
    private const string DimensionDepthOut = "Depth Out";
    private const string DimensionBottomWidth = "Btm Width";
    private const string DimensionBottomDepth = "Btm Depth";
    private const string DimensionTopWidth = "Top Width";
    private const string DimensionLeftWidth = "Left Width";
    private const string DimensionRightWidth = "Right Width";
    private const string DimensionInnerRadius = "Inner Radius";
    private const string DimensionOuterRadius = "Outer Radius";
    private const string DimensionLeftRadius = "Left Radius";
    private const string DimensionRightRadius = "Right Radius";
    private const string DimensionDiameter = "Diameter";
    private const string DimensionTopExtension = "Top Extension";
    private const string DimensionTopInnerExtension = "Top Inner Extension";
    private const string DimensionBottomExtension = "Bottom Extension";
    private const string DimensionDuctWidth = "Duct Width";
    private const string DimensionDuctDepth = "Duct Depth";
    private const string DimensionStepInWidth = "Step in Width";
    private const string DimensionStepInDepth = "Step In Depth";
    private const string DimensionBranchWidth = "Branch Width";
    private const string DimensionBranchDepth = "Branch Depth";
    private const string DimensionBranchWidth1 = "Branch Width #1";
    private const string DimensionBranchDepth1 = "Branch Depth #1";

    private const string SharedParamWidth1 = "FAB WIDTH 1";
    private const string SharedParamWidth2 = "FAB WIDTH 2";
    private const string SharedParamWidth3 = "FAB WIDTH 3";
    private const string SharedParamHeight1 = "FAB HEIGHT 1";
    private const string SharedParamHeight2 = "FAB HEIGHT 2";
    private const string SharedParamHeight3 = "FAB HEIGHT 3";
    private const string SharedParamDiameter1 = "FAB DIAMETER 1";
    private const string SharedParamInternalWidth = "Internal Width";
    private const string SharedParamInternalHeight = "Internal Height";
    private const string SharedParamInternalDiameter = "Internal Diameter";
    private const string SharedParamRadius = "Actual Radius";
    private const string SharedParamRadius1 = "FAB RADIUS 1";
    private const string SharedParamRadius2 = "FAB RADIUS 2";
    private const string SharedParamRadius3 = "FAB RADIUS 3";
    private const string SharedParamSqBendBe = "Sq Bend BE";
    private const string SharedParamSqBendTe = "Sq Bend TE";
    private const string SharedParamBottomLevel = "Bottom Level";

    private static readonly Dictionary<string, List<string>>
        DimensionToSharedParamMap;

    internal IReadOnlyDictionary<string, double> SharedParameters
    {
        get => sharedParameters;
    }

    private readonly Dictionary<string, double> sharedParameters;

    static FabDuctSharedParameters()
    {
        DimensionToSharedParamMap = new Dictionary<string, List<string>>()
        {
            { DimensionWidth, new List<string>{
                SharedParamWidth1,
                SharedParamWidth2 ,
                SharedParamWidth3} },
            { DimensionDepth, new List<string>{
                SharedParamHeight1,
                SharedParamHeight2,
                SharedParamHeight3 } },
            { DimensionWidthIn, new List<string>{
                SharedParamWidth1 } },
            { DimensionDepthIn, new List<string>{
                SharedParamHeight1 } },
            { DimensionWidthOut, new List<string>{
                SharedParamWidth2 } },
            { DimensionDepthOut, new List<string>{
                SharedParamHeight2 } },
            { DimensionBottomWidth, new List<string>{
                SharedParamWidth1 } },
            { DimensionBottomDepth, new List<string>{
                SharedParamHeight1,
                SharedParamHeight2,
                SharedParamHeight3 } },
            { DimensionTopWidth, new List<string>{
                SharedParamWidth2 } },
            { DimensionLeftWidth, new List<string>{
                SharedParamWidth2 } },
            { DimensionRightWidth, new List<string>{
                SharedParamWidth3 } },
            { DimensionInnerRadius, new List<string>{
                SharedParamRadius,
                SharedParamRadius1 } },
            { DimensionOuterRadius, new List<string>{
                SharedParamRadius2 } },
            { DimensionLeftRadius, new List<string>{
                SharedParamRadius2 } },
            { DimensionRightRadius, new List<string>{
                SharedParamRadius3 } },
            { DimensionDiameter, new List<string>{
                SharedParamDiameter1 } },
            { DimensionTopExtension, new List<string>{
                SharedParamSqBendTe } },
            { DimensionTopInnerExtension, new List<string>{
                SharedParamSqBendTe } },
            { DimensionBottomExtension, new List<string>{
                SharedParamSqBendBe } },
            { DimensionDuctWidth, new List<string>{
                SharedParamWidth1 } },
            { DimensionDuctDepth, new List<string>{
                SharedParamHeight1 } },
            { DimensionStepInWidth, new List<string>{
                SharedParamWidth2 } },
            { DimensionStepInDepth, new List<string>{
                SharedParamHeight2 } },
            { DimensionBranchWidth, new List<string>{
                SharedParamWidth1 } },
            { DimensionBranchDepth, new List<string>{
                SharedParamHeight1 } },
            { DimensionBranchWidth1, new List<string>{
                SharedParamWidth1 } },
            { DimensionBranchDepth1, new List<string>{
                SharedParamHeight1 } }
        };
    }

    internal FabDuctSharedParameters()
    {
        sharedParameters = new Dictionary<string, double>()
        {
            { SharedParamWidth1, 0 },
            { SharedParamWidth2, 0 },
            { SharedParamWidth3, 0 },
            { SharedParamHeight1, 0 },
            { SharedParamHeight2, 0 },
            { SharedParamHeight3, 0 },
            { SharedParamDiameter1, 0 },
            { SharedParamInternalWidth, 0 },
            { SharedParamInternalHeight, 0 },
            { SharedParamInternalDiameter, 0 },
            { SharedParamRadius, 0 },
            { SharedParamRadius1, 0 },
            { SharedParamRadius2, 0 },
            { SharedParamRadius3, 0 },
            { SharedParamSqBendBe, 0 },
            { SharedParamSqBendTe, 0 },
            { SharedParamBottomLevel, 0 }
        };
    }

    internal void Process(FabricationPart fabDuct)
    {
        MapDimensionsToSharedParameters(fabDuct);
        CalculateInternalSize(fabDuct);
        SyncBottomLevel(fabDuct);
    }

    private void MapDimensionsToSharedParameters(FabricationPart fabDuct)
    {
        IList<FabricationDimensionDefinition> dimensions =
            fabDuct.GetDimensions();
        foreach (FabricationDimensionDefinition dimension in dimensions)
        {
            if (DimensionToSharedParamMap.TryGetValue(
                dimension.Name,
                out List<string>? sharedParameters))
            {
                double value = fabDuct.GetDimensionValue(dimension);
                foreach (string sharedParameter in sharedParameters)
                    Set(sharedParameter, value);
            }
        }
    }

    private void CalculateInternalSize(FabricationPart fabDuct)
    {
        double totalLiningThickness = 2 * fabDuct.LiningThickness;

        double[] internalWidth = [
            Math.Max(
                sharedParameters[SharedParamWidth1] - totalLiningThickness,
                0),
            Math.Max(
                sharedParameters[SharedParamWidth2] - totalLiningThickness,
                0),
            Math.Max(
                sharedParameters[SharedParamWidth3] - totalLiningThickness,
                0)
        ];
        double[] internalHeight = [
            Math.Max(
                sharedParameters[SharedParamHeight1] - totalLiningThickness,
                0),
            Math.Max(
                sharedParameters[SharedParamHeight2] - totalLiningThickness,
                0),
            Math.Max(
                sharedParameters[SharedParamHeight3] - totalLiningThickness,
                0)
        ];
        double internalDiameter =
            Math.Max(
                sharedParameters[SharedParamDiameter1] - totalLiningThickness,
                0);

        double smallestArea = double.MaxValue;
        int smallestAreaIndex = 0;

        for (int i = 0; i < internalWidth.Length; i++)
        {
            double internalArea = internalWidth[i] * internalHeight[i];

            if (internalArea > 0 && internalArea < smallestArea)
            {
                smallestArea = internalArea;
                smallestAreaIndex = i;
            }
        }

        Set(SharedParamInternalWidth, internalWidth[smallestAreaIndex]);
        Set(SharedParamInternalHeight, internalHeight[smallestAreaIndex]);
        Set(SharedParamInternalDiameter, internalDiameter);
    }

    private void SyncBottomLevel(FabricationPart fabDuct)
    {
        Parameter bottomLevelParameter =
            fabDuct.LookupParameter(FabParamBottom);

        double bottomLevel =
            bottomLevelParameter != null ? bottomLevelParameter.AsDouble() : 0;

        Set(SharedParamBottomLevel, bottomLevel);
    }

    private void Set(string sharedParameter, double value)
    {
        if (sharedParameters.ContainsKey(sharedParameter))
            sharedParameters[sharedParameter] = value;
    }
}