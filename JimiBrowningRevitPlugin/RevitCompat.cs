using Autodesk.Revit.DB;

namespace JimiBrowningRevitPlugin;

internal static class RevitCompat
{
    internal static FilterStringRule CreateFilterStringRule(
        FilterableValueProvider valueProvider,
        FilterStringRuleEvaluator evaluator,
        string ruleString)
    {
#if REVIT2022_OR_GREATER
        return new FilterStringRule(valueProvider, evaluator, ruleString);
#else
        return new FilterStringRule(
            valueProvider,
            evaluator,
            ruleString,
            false);
#endif
    }
}