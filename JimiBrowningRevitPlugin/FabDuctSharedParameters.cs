using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace JimiBrowningRevitPlugin;

internal class FabDuctSharedParameters
{
    private const string FAB_PARAM_PRIMARY_WIDTH = "Main Primary Width";
    private const string FAB_PARAM_PRIMARY_DEPTH = "Main Primary Depth";
    private const string FAB_PARAM_PRIMARY_DIAMETER = "Main Primary Diameter";

    private const string DIMENSION_WIDTH = "Width";
    private const string DIMENSION_DEPTH = "Depth";
    private const string DIMENSION_WIDTH_IN = "Width In";
    private const string DIMENSION_DEPTH_IN = "Depth In";
    private const string DIMENSION_WIDTH_OUT = "Width Out";
    private const string DIMENSION_DEPTH_OUT = "Depth Out";
    private const string DIMENSION_BOTTOM_WIDTH = "Btm Width";
    private const string DIMENSION_BOTTOM_DEPTH = "Btm Depth";
    private const string DIMENSION_TOP_WIDTH = "Top Width";
    private const string DIMENSION_LEFT_WIDTH = "Left Width";
    private const string DIMENSION_RIGHT_WIDTH = "Right Width";
    private const string DIMENSION_INNER_RADIUS = "Inner Radius";
    private const string DIMENSION_OUTER_RADIUS = "Outer Radius";
    private const string DIMENSION_LEFT_RADIUS = "Left Radius";
    private const string DIMENSION_RIGHT_RADIUS = "Right Radius";
    private const string DIMENSION_DIAMETER = "Diameter";
    private const string DIMENSION_TOP_EXTENSION = "Top Extension";
    private const string DIMENSION_TOP_INNER_EXTENSION = "Top Inner Extension";
    private const string DIMENSION_BOTTOM_EXTENSION = "Bottom Extension";
    private const string DIMENSION_DUCT_WIDTH = "Duct Width";
    private const string DIMENSION_DUCT_DEPTH = "Duct Depth";
    private const string DIMENSION_STEP_IN_WIDTH = "Step in Width";
    private const string DIMENSION_STEP_IN_DEPTH = "Step In Depth";
    private const string DIMENSION_BRANCH_WIDTH = "Branch Width";
    private const string DIMENSION_BRANCH_DEPTH = "Branch Depth";
    private const string DIMENSION_BRANCH_WIDTH_1 = "Branch Width #1";
    private const string DIMENSION_BRANCH_DEPTH_1 = "Branch Depth #1";

    private const string SHARED_PARAM_WIDTH_1 = "FAB WIDTH 1";
    private const string SHARED_PARAM_WIDTH_2 = "FAB WIDTH 2";
    private const string SHARED_PARAM_WIDTH_3 = "FAB WIDTH 3";
    private const string SHARED_PARAM_HEIGHT_1 = "FAB HEIGHT 1";
    private const string SHARED_PARAM_HEIGHT_2 = "FAB HEIGHT 2";
    private const string SHARED_PARAM_HEIGHT_3 = "FAB HEIGHT 3";
    private const string SHARED_PARAM_DIAMETER_1 = "FAB DIAMETER 1";
    private const string SHARED_PARAM_INTERNAL_WIDTH = "Internal Width";
    private const string SHARED_PARAM_INTERNAL_HEIGHT = "Internal Height";
    private const string SHARED_PARAM_INTERNAL_DIAMETER = "Internal Diameter";
    private const string SHARED_PARAM_RADIUS = "Actual Radius";
    private const string SHARED_PARAM_RADIUS_1 = "FAB RADIUS 1";
    private const string SHARED_PARAM_RADIUS_2 = "FAB RADIUS 2";
    private const string SHARED_PARAM_RADIUS_3 = "FAB RADIUS 3";
    private const string SHARED_PARAM_SQ_BEND_BE = "Sq Bend BE";
    private const string SHARED_PARAM_SQ_BEND_TE = "Sq Bend TE";

    private static readonly Dictionary<string, List<string>>
        DIMENSION_TO_SHARED_PARAM_MAP = new()
        {
            { DIMENSION_WIDTH, new List<string>{
                SHARED_PARAM_WIDTH_1,
                SHARED_PARAM_WIDTH_2 ,
                SHARED_PARAM_WIDTH_3} },
            { DIMENSION_DEPTH, new List<string>{
                SHARED_PARAM_HEIGHT_1,
                SHARED_PARAM_HEIGHT_2,
                SHARED_PARAM_HEIGHT_3 } },
            { DIMENSION_WIDTH_IN, new List<string>{
                SHARED_PARAM_WIDTH_1 } },
            { DIMENSION_DEPTH_IN, new List<string>{
                SHARED_PARAM_HEIGHT_1 } },
            { DIMENSION_WIDTH_OUT, new List<string>{
                SHARED_PARAM_WIDTH_2 } },
            { DIMENSION_DEPTH_OUT, new List<string>{
                SHARED_PARAM_HEIGHT_2 } },
            { DIMENSION_BOTTOM_WIDTH, new List<string>{
                SHARED_PARAM_WIDTH_1 } },
            { DIMENSION_BOTTOM_DEPTH, new List<string>{
                SHARED_PARAM_HEIGHT_1,
                SHARED_PARAM_HEIGHT_2,
                SHARED_PARAM_HEIGHT_3 } },
            { DIMENSION_TOP_WIDTH, new List<string>{
                SHARED_PARAM_WIDTH_2 } },
            { DIMENSION_LEFT_WIDTH, new List<string>{
                SHARED_PARAM_WIDTH_2 } },
            { DIMENSION_RIGHT_WIDTH, new List<string>{
                SHARED_PARAM_WIDTH_3 } },
            { DIMENSION_INNER_RADIUS, new List<string>{
                SHARED_PARAM_RADIUS,
                SHARED_PARAM_RADIUS_1 } },
            { DIMENSION_OUTER_RADIUS, new List<string>{
                SHARED_PARAM_RADIUS_2 } },
            { DIMENSION_LEFT_RADIUS, new List<string>{
                SHARED_PARAM_RADIUS_2 } },
            { DIMENSION_RIGHT_RADIUS, new List<string>{
                SHARED_PARAM_RADIUS_3 } },
            { DIMENSION_DIAMETER, new List<string>{
                SHARED_PARAM_DIAMETER_1 } },
            { DIMENSION_TOP_EXTENSION, new List<string>{
                SHARED_PARAM_SQ_BEND_TE } },
            { DIMENSION_TOP_INNER_EXTENSION, new List<string>{
                SHARED_PARAM_SQ_BEND_TE } },
            { DIMENSION_BOTTOM_EXTENSION, new List<string>{
                SHARED_PARAM_SQ_BEND_BE } },
            { DIMENSION_DUCT_WIDTH, new List<string>{
                SHARED_PARAM_WIDTH_1 } },
            { DIMENSION_DUCT_DEPTH, new List<string>{
                SHARED_PARAM_HEIGHT_1 } },
            { DIMENSION_STEP_IN_WIDTH, new List<string>{
                SHARED_PARAM_WIDTH_2 } },
            { DIMENSION_STEP_IN_DEPTH, new List<string>{
                SHARED_PARAM_HEIGHT_2 } },
            { DIMENSION_BRANCH_WIDTH, new List<string>{
                SHARED_PARAM_WIDTH_1 } },
            { DIMENSION_BRANCH_DEPTH, new List<string>{
                SHARED_PARAM_HEIGHT_1 } },
            { DIMENSION_BRANCH_WIDTH_1, new List<string>{
                SHARED_PARAM_WIDTH_1 } },
            { DIMENSION_BRANCH_DEPTH_1, new List<string>{
                SHARED_PARAM_HEIGHT_1 } }
        };

    private readonly Dictionary<string, double> sharedParameters = new()
    {
        { SHARED_PARAM_WIDTH_1, 0 },
        { SHARED_PARAM_WIDTH_2, 0 },
        { SHARED_PARAM_WIDTH_3, 0 },
        { SHARED_PARAM_HEIGHT_1, 0 },
        { SHARED_PARAM_HEIGHT_2, 0 },
        { SHARED_PARAM_HEIGHT_3, 0 },
        { SHARED_PARAM_DIAMETER_1, 0 },
        { SHARED_PARAM_INTERNAL_WIDTH, 0 },
        { SHARED_PARAM_INTERNAL_HEIGHT, 0 },
        { SHARED_PARAM_INTERNAL_DIAMETER, 0 },
        { SHARED_PARAM_RADIUS, 0 },
        { SHARED_PARAM_RADIUS_1, 0 },
        { SHARED_PARAM_RADIUS_2, 0 },
        { SHARED_PARAM_RADIUS_3, 0 },
        { SHARED_PARAM_SQ_BEND_BE, 0 },
        { SHARED_PARAM_SQ_BEND_TE, 0 }
    };

    internal IReadOnlyDictionary<string, double> SharedParameters
    {
        get { return sharedParameters; }
    }

    internal void Process(FabricationPart fabDuct)
    {
        MapDimensionsToSharedParameters(fabDuct);
        CalculateInternalSize(fabDuct);
    }

    private void MapDimensionsToSharedParameters(FabricationPart fabDuct)
    {
        IList<FabricationDimensionDefinition> dimensions =
            fabDuct.GetDimensions();
        foreach (FabricationDimensionDefinition dimension in dimensions)
        {
            if (DIMENSION_TO_SHARED_PARAM_MAP.TryGetValue(
                dimension.Name,
                out List<string> sharedParameters))
            {
                double value = fabDuct.GetDimensionValue(dimension);
                foreach (string sharedParameter in sharedParameters)
                {
                    Set(sharedParameter, value);
                }
            }
        }
    }

    private void CalculateInternalSize(FabricationPart fabDuct)
    {
        Parameter widthParameter =
            fabDuct.LookupParameter(FAB_PARAM_PRIMARY_WIDTH);
        Parameter heightParameter =
            fabDuct.LookupParameter(FAB_PARAM_PRIMARY_DEPTH);
        Parameter diameterParameter =
            fabDuct.LookupParameter(FAB_PARAM_PRIMARY_DIAMETER);

        double width =
            widthParameter != null ? widthParameter.AsDouble() : 0;
        double height =
            heightParameter != null ? heightParameter.AsDouble() : 0;
        double diameter =
            diameterParameter != null ? diameterParameter.AsDouble() : 0;
        double totalLiningThickness = 2 * fabDuct.LiningThickness;

        double internalWidth = width;
        double internalHeight = height;
        double internalDiameter = diameter;

        if (width > totalLiningThickness)
            internalWidth -= totalLiningThickness;

        if (height > totalLiningThickness)
            internalHeight -= totalLiningThickness;

        if (diameter > totalLiningThickness)
            internalDiameter -= totalLiningThickness;

        Set(SHARED_PARAM_INTERNAL_WIDTH, internalWidth);
        Set(SHARED_PARAM_INTERNAL_HEIGHT, internalHeight);
        Set(SHARED_PARAM_INTERNAL_DIAMETER, internalDiameter);
    }

    private void Set(string sharedParameter, double value)
    {
        if (sharedParameters.ContainsKey(sharedParameter))
            sharedParameters[sharedParameter] = value;
    }
}