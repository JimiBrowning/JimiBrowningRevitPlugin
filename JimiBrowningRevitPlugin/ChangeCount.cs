namespace JimiBrowningRevitPlugin;

internal struct ChangeCount
{
    internal long parameterChangeCount;
    internal long elementChangeCount;

    internal ChangeCount(long parameterChangeCount, long elementChangeCount)
    {
        this.parameterChangeCount = parameterChangeCount;
        this.elementChangeCount = elementChangeCount;
    }

    public static ChangeCount operator +(ChangeCount lhs, ChangeCount rhs)
    {
        return new ChangeCount(
            lhs.parameterChangeCount + rhs.parameterChangeCount,
            lhs.elementChangeCount + rhs.elementChangeCount);
    }
}