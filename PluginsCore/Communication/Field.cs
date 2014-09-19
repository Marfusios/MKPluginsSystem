using System;

namespace PluginsCore.Communication
{
    [Serializable]
    public class Field
    {
        public Type FieldType { get; set; }
        public string FieldTypeName { get { return FieldType != null ? FieldType.FullName : "Unrecognize"; }}
        public string Name { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Name + " : " + FieldTypeName;
        }
    }
}
