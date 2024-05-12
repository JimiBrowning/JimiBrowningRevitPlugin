using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JimiBrowningRevitPlugin;

internal class ChangeLog
{
    internal int ElementCount
    {
        get { return log.Count; }
    }

    internal int ParameterCount
    {
        get
        {
            int count = 0;

            foreach (
                KeyValuePair<ElementIdValue, Dictionary<string, string>> element
                in log)
            {
                count += element.Value.Count;
            }

            return count;
        }
    }

    private readonly Dictionary<ElementIdValue, Dictionary<string, string>> log
        = new();

    internal void Add(ChangeLog changeLog)
    {
        foreach (
            KeyValuePair<ElementIdValue, Dictionary<string, string>> element
            in changeLog.log)
        {
            Add(element.Key, element.Value);
        }
    }

    internal void Add(
        ElementIdValue elementId,
        Dictionary<string, string> element)
    {
        foreach (KeyValuePair<string, string> parameter in element)
            Add(elementId, parameter.Key, parameter.Value);
    }

    internal void Add(
        ElementIdValue elementId,
        string parameterName,
        string value)
    {
        if (!log.ContainsKey(elementId))
        {
            var parameters = new Dictionary<string, string>
            { { parameterName, value } };
            log.Add(elementId, parameters);
        }
        else
        {
            if (!log[elementId].ContainsKey(parameterName))
                log[elementId].Add(parameterName, value);
            else
                log[elementId][parameterName] = value;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (
            KeyValuePair<ElementIdValue, Dictionary<string, string>> element
            in log.OrderBy(element => element.Key))
        {
            sb.AppendLine("ElementId: " + element.Key.ToString());

            foreach (
                KeyValuePair<string, string> parameter
                in element.Value.OrderBy(parameter => parameter.Key))
            {
                sb.AppendLine("  " + parameter.Key + ": " + parameter.Value);
            }
        }

        return sb.ToString();
    }
}