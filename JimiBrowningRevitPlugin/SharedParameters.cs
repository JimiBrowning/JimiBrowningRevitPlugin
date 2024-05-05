using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JimiBrowningRevitPlugin
{
    internal class SharedParameters
    {
        private readonly List<string> missingParameters = new();
        private readonly List<string> readOnlyParameters = new();

        internal Parameter GetRequired(Element element, string parameterName)
        {
            Parameter parameter = Get(element, parameterName);

            if (parameter == null)
            {
                if (!missingParameters.Contains(parameterName))
                    missingParameters.Add(parameterName);

                return null;
            }

            return parameter;
        }

        internal Parameter Get(Element element, string parameterName)
        {
            return element.LookupParameter(parameterName);
        }

        internal bool Set(Parameter parameter, string value)
        {
            if (IsReadonly(parameter))
                return false;

            parameter.Set(value);

            return true;
        }

        internal bool Set(Parameter parameter, double value)
        {
            if (IsReadonly(parameter))
                return false;

            parameter.Set(value);

            return true;
        }

        private bool IsReadonly(Parameter parameter)
        {
            if (parameter.IsReadOnly)
            {
                if (!readOnlyParameters.Contains(parameter.Definition.Name))
                    readOnlyParameters.Add(parameter.Definition.Name);

                return true;
            }

            return false;
        }

        internal bool HasMissingParameters()
        {
            return missingParameters.Count > 0;
        }

        internal bool HasReadOnlyParameters()
        {
            return readOnlyParameters.Count > 0;
        }

        internal ReadOnlyCollection<string> GetMissingParameters()
        {
            return missingParameters.AsReadOnly();
        }

        internal ReadOnlyCollection<string> GetReadOnlyParameters()
        {
            return readOnlyParameters.AsReadOnly();
        }
    }
}