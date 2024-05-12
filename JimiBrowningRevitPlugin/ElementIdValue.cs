using Autodesk.Revit.DB;
using System;

namespace JimiBrowningRevitPlugin;

internal readonly struct ElementIdValue : IComparable<ElementIdValue>
{
#if REVIT2024_OR_GREATER
    internal readonly long value;
#else
    internal readonly int value;
#endif

    internal ElementIdValue(ElementId elementId)
    {
#if REVIT2024_OR_GREATER
        this.value = elementId.Value;
#else
        this.value = elementId.IntegerValue;
#endif
    }

    public static bool operator ==(ElementIdValue lhs, ElementIdValue rhs)
    {
        return lhs.value == rhs.value;
    }

    public static bool operator !=(ElementIdValue lhs, ElementIdValue rhs)
    {
        return lhs.value != rhs.value;
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is ElementIdValue other && value == other.value;
    }

    public override string ToString()
    {
        return value.ToString();
    }

    public int CompareTo(ElementIdValue other)
    {
        return value.CompareTo(other.value);
    }
}