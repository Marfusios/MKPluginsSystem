using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using PluginsCore.Communication;

namespace PluginsCore
{
    public class PluginLoader
    {
        public static void SetParametersToPlugin(PluginBase plugin, IEnumerable<Parameter> parameters)
        {
            var fields = plugin.GetFields();
            SetParametersToFields(fields, parameters);
            plugin.SetFields(fields);
        }


        public static void SetParametersToFields(IEnumerable<Field> fields, IEnumerable<Parameter> parameters)
        {
            foreach (Field fi in fields)
            {
                foreach (var param in parameters)
                {
                    if (fi.Name.Equals(param.Name))
                    {
                        if (!(param.Value is DBNull) && !string.IsNullOrEmpty((string) param.Value))
                        {
                            object o = Activator.CreateInstance(param.FieldType, param.Value);
                            fi.Value = o;
                        }
                        break;
                    }
                } 
            }
        }


        public static Field[] GetFields(PluginBase plugin)
        {
            var result = new List<Field>();
            if (plugin == null)
                return result.ToArray();

            FieldInfo[] fields = plugin.GetType().GetFields();

            foreach (FieldInfo fi in fields)
            {
                var oneField = new Field();
                oneField.Value = fi.GetValue(plugin);
                oneField.Name = fi.Name;
                oneField.FieldType = fi.FieldType;
                result.Add(oneField);
            }

            return result.ToArray();
        }

        public static void SetFields(PluginBase plugin, Field[] fields)
        {
            foreach (FieldInfo field in plugin.GetType().GetFields())
            {
                FieldInfo field1 = field;
                if (fields.Any(x => x.Name.Equals(field1.Name)))
                {
                    Field fieldIn = fields.Single(x => x.Name.Equals(field1.Name));
                    field1.SetValue(plugin, fieldIn.Value);
                }
            }
        }

        public static Method[] GetMethods(PluginBase plugin)
        {
            var result = new List<Method>();
            if (plugin == null)
                return result.ToArray();

            var methods = plugin.GetType().GetMethods();

            foreach (var me in methods)
            {
                var method = new Method();
                method.AssemblyName = plugin.GetType().Assembly.ManifestModule.Name;
                method.NameSpace = plugin.GetType().Namespace;
                method.ClassName = plugin.GetType().Name;
                method.MethodName = me.Name;

                method.Parameters = new Dictionary<string, Parameter>();
                foreach (var par in me.GetParameters())
                {
                    var parameter = new Parameter(par.ParameterType, par.Name, par.DefaultValue, ParameterDirection.Input);
                    method.Parameters.Add(par.Name, parameter);
                }

                method.ReturnParameter = new Parameter(me.ReturnType, "ReturnValue", null, ParameterDirection.ReturnValue);

                result.Add(method);
            }
            
            return result.ToArray();
        }    
    }
}