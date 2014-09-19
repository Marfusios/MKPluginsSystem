using System;
using System.Data;

namespace PluginsCore.Communication
{
    [Serializable]
    public class Parameter
    {
        public Type FieldType { get; set; }
        public string FieldTypeName { get { return FieldType != null ? FieldType.FullName : "Unrecognize"; } }
        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterDirection Direction { get; set; }

        public Parameter(Type fieldType, string name, object value, ParameterDirection direction)
        {
            FieldType = fieldType;
            Name = name;
            Value = value;
            Direction = direction;
        }
    }
}
