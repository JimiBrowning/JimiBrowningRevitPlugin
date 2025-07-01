using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace JimiBrowningRevitPlugin;

internal class FabDuctSharedParameters
{
    internal IReadOnlyDictionary<string, double> SharedParameters
    {
        get => sharedParameters;
    }

    private readonly Dictionary<string, double> sharedParameters = [];

    internal FabDuctSharedParameters(
        FabricationPart fabDuct,
        Configuration config)
    {
        InitialiseFabricationSharedParameters(config);
        SyncFabricationDimensionsWithSharedParameters(fabDuct, config);
        SyncFabricationInternalSize(fabDuct, config);
        SyncFabricationBottomLevel(fabDuct, config);
    }

    private void InitialiseFabricationSharedParameters(Configuration config)
    {
        foreach (string sharedParameter in config.FabDuctSharedParameters)
            sharedParameters.Add(sharedParameter, 0);
    }

    private void SyncFabricationDimensionsWithSharedParameters(
        FabricationPart fabDuct,
        Configuration config)
    {
        IList<FabricationDimensionDefinition> dimensions =
            fabDuct.GetDimensions();

        double centreRadius = 0;
        double diameter = 0;

        foreach (FabricationDimensionDefinition dimension in dimensions)
        {
            if (config.FabDimToSharedParamMap.TryGetValue(
                dimension.Name,
                out List<string>? sharedParameters))
            {
                double value = fabDuct.GetDimensionValue(dimension);
                foreach (string sharedParameter in sharedParameters)
                    Set(sharedParameter, value);
            }

            if (dimension.Name == config.FabDimCentreRadius)
                centreRadius = fabDuct.GetDimensionValue(dimension);
            if (dimension.Name == config.FabDimDiameter)
                diameter = fabDuct.GetDimensionValue(dimension);
        }

        if (centreRadius > 0 && diameter > 0)
        {
            double innerRadius = centreRadius - (diameter / 2);

            Set(config.SharedParamRadius1, innerRadius);
        }
    }

    private void SyncFabricationInternalSize(
        FabricationPart fabDuct,
        Configuration config)
    {
        double totalLiningThickness = 2 * fabDuct.LiningThickness;

        if (sharedParameters.ContainsKey(config.SharedParamWidth1)
            && sharedParameters.ContainsKey(config.SharedParamWidth2)
            && sharedParameters.ContainsKey(config.SharedParamWidth3)
            && sharedParameters.ContainsKey(config.SharedParamHeight1)
            && sharedParameters.ContainsKey(config.SharedParamHeight2)
            && sharedParameters.ContainsKey(config.SharedParamHeight3))
        {
            double[] internalWidth = [
                Math.Max(
                    sharedParameters[config.SharedParamWidth1]
                    - totalLiningThickness,
                    0),
                Math.Max(
                    sharedParameters[config.SharedParamWidth2]
                    - totalLiningThickness,
                    0),
                Math.Max(
                    sharedParameters[config.SharedParamWidth3]
                    - totalLiningThickness,
                    0)
                ];
            double[] internalHeight = [
                Math.Max(
                    sharedParameters[config.SharedParamHeight1]
                    - totalLiningThickness,
                    0),
                Math.Max(
                    sharedParameters[config.SharedParamHeight2]
                    - totalLiningThickness,
                    0),
                Math.Max(
                    sharedParameters[config.SharedParamHeight3]
                    - totalLiningThickness,
                    0)
                ];

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

            Set(
                config.SharedParamInternalWidth,
                internalWidth[smallestAreaIndex]);
            Set(
                config.SharedParamInternalHeight,
                internalHeight[smallestAreaIndex]);
        }

        if (sharedParameters.ContainsKey(config.SharedParamOvalWidth1)
            && sharedParameters.ContainsKey(config.SharedParamOvalWidth2)
            && sharedParameters.ContainsKey(config.SharedParamOvalWidth3)
            && sharedParameters.ContainsKey(config.SharedParamOvalHeight1)
            && sharedParameters.ContainsKey(config.SharedParamOvalHeight2)
            && sharedParameters.ContainsKey(config.SharedParamOvalHeight3))
        {
            double[] internalOvalWidth = [
                Math.Max(
                    sharedParameters[config.SharedParamOvalWidth1]
                    - totalLiningThickness,
                    0),
                Math.Max(
                    sharedParameters[config.SharedParamOvalWidth2]
                    - totalLiningThickness,
                    0),
                Math.Max(
                    sharedParameters[config.SharedParamOvalWidth3]
                    - totalLiningThickness,
                    0)
                ];
            double[] internalOvalHeight = [
                Math.Max(
                    sharedParameters[config.SharedParamOvalHeight1]
                    - totalLiningThickness,
                    0),
                Math.Max(
                    sharedParameters[config.SharedParamOvalHeight2]
                    - totalLiningThickness,
                    0),
                Math.Max(
                    sharedParameters[config.SharedParamOvalHeight3]
                    - totalLiningThickness,
                    0)
                ];

            double smallestArea = double.MaxValue;
            int smallestAreaIndex = 0;

            for (int i = 0; i < internalOvalWidth.Length; i++)
            {
                double internalArea =
                    Math.PI * internalOvalHeight[i] * internalOvalHeight[i] / 4
                    + (internalOvalWidth[i] - internalOvalHeight[i])
                    * internalOvalHeight[i];

                if (internalArea > 0 && internalArea < smallestArea)
                {
                    smallestArea = internalArea;
                    smallestAreaIndex = i;
                }
            }

            Set(
                config.SharedParamInternalOvalWidth,
                internalOvalWidth[smallestAreaIndex]);
            Set(
                config.SharedParamInternalOvalHeight,
                internalOvalHeight[smallestAreaIndex]);
        }

        if (sharedParameters.ContainsKey(config.SharedParamDiameter1)
            && sharedParameters.ContainsKey(config.SharedParamDiameter2)
            && sharedParameters.ContainsKey(config.SharedParamDiameter3))
        {
            double[] internalDiameter = [
                Math.Max(
                    sharedParameters[config.SharedParamDiameter1]
                    - totalLiningThickness,
                    0),
                Math.Max(
                    sharedParameters[config.SharedParamDiameter2]
                    - totalLiningThickness,
                    0),
                Math.Max(
                    sharedParameters[config.SharedParamDiameter3]
                    - totalLiningThickness,
                    0)
                ];

            double smallestDiameter = double.MaxValue;
            int smallestDiameterIndex = 0;

            for (int i = 0; i < internalDiameter.Length; i++)
            {
                double diameter = internalDiameter[i];

                if (diameter > 0 && diameter < smallestDiameter)
                {
                    smallestDiameter = diameter;
                    smallestDiameterIndex = i;
                }
            }

            Set(
                config.SharedParamInternalDiameter,
                internalDiameter[smallestDiameterIndex]);
        }
    }

    private void SyncFabricationBottomLevel(
        FabricationPart fabDuct,
        Configuration config)
    {
        Parameter bottomElevationParameter =
            fabDuct.LookupParameter(config.FabParamBottomElevation);

        double bottomElevation = bottomElevationParameter != null
            ? bottomElevationParameter.AsDouble()
            : 0;

        Set(config.SharedParamBottomLevel, bottomElevation);
    }

    private void Set(string sharedParameter, double value)
    {
        if (sharedParameters.ContainsKey(sharedParameter))
            sharedParameters[sharedParameter] = value;
    }
}