using System;

namespace JimiBrowningRevitPlugin;

internal class Units
{
    internal static double FeetToMm(double feet)
    {
        return feet * 304.8;
    }

    internal static long Round(double value)
    {
        double roundFactor = (value >= 0.0) ? 0.5 : -0.5;
        return (long)(value + roundFactor);
    }

    internal static bool EqualsWithError(double x, double y)
    {
        return Math.Abs(x - y) < 1E-6;
    }
}