using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PluginsCore.Communication;
using PluginsCore.Utils;

namespace PluginsCore
{
    public class PluginExecuter
    {

        public static Parameter ExecuteMethod(PluginBase plugin, Method method)
        {
            var equalsMethodsInClass = plugin.GetMethods().Where(x => x.Equals(method)).OrderByDescending(x => x.Parameters.Count).ToList();
            if (equalsMethodsInClass.Count <= 0)
                throw new PluginsException(string.Format("Method {0} in class {1} was not found", method.MethodName, plugin.ToString()));

            var onlyInputParams =
                method.Parameters.Where(x => x.Value.Direction == ParameterDirection.Input).ToList();

            foreach (var methodInClass in equalsMethodsInClass)
            {
                var methodInClassOnlyInput = methodInClass.Parameters.Where(x => x.Value.Direction == ParameterDirection.Input).ToList();
                object[] parameters;
                Type[] types;
                if (sameParameters(methodInClassOnlyInput, onlyInputParams, out parameters, out types))
                {
                    var result = plugin.GetType().GetMethod(method.MethodName, types).Invoke(plugin, parameters);
                    return getResultFromMethod(method, result);
                }
            }

            throw new PluginsException(string.Format("Method {0} in class {1} was found, but it has different parameters", method.MethodName, plugin.ToString()));
        }



        private static bool sameParameters(List<KeyValuePair<string, Parameter>> paramsInClass, List<KeyValuePair<string, Parameter>> paramsConfigured, out object[] par, out Type[] parTypes)
        {
            par = new object[paramsInClass.Count];
            parTypes = new Type[paramsInClass.Count];

            if (paramsInClass.Count != paramsConfigured.Count)
                return false;
            for (int i = 0; i < paramsInClass.Count; i++)
            {
                var param1 = paramsInClass[i];
                if (paramsConfigured.Any(x => x.Key == param1.Key))
                {
                    var paramConfigured = paramsConfigured.First(x => x.Key == param1.Key);
                    par[i] = paramConfigured.Value.Value;
                    parTypes[i] = param1.Value.FieldType;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private static Parameter getResultFromMethod(Method method, object result)
        {
            method.ReturnParameter = method.Parameters.Where(x => x.Value.Direction == ParameterDirection.ReturnValue).Select(x => x.Value).FirstOrDefault();
            if (method.ReturnParameter != null)
                method.ReturnParameter.Value = result;
            return method.ReturnParameter;
        }
    }
}
