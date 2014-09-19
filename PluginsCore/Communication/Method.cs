using System;
using System.Collections.Generic;

namespace PluginsCore.Communication
{
    [Serializable]
    public class Method
    {
        public string AssemblyName { get; set; }
        public string NameSpace { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public Dictionary<string, Parameter> Parameters { get; set; }

        public Parameter ReturnParameter { get; set; }
        public Dictionary<string, Parameter> Result { get; set; }


        public override string ToString()
        {
            return AssemblyName + " - " + NameSpace + " - " + ClassName + " - " + MethodName;
        }

        #region EQUALS

        protected bool Equals(Method other)
        {
            return string.Equals(AssemblyName.ToLower(), other.AssemblyName.ToLower()) && string.Equals(NameSpace, other.NameSpace) &&
                   string.Equals(ClassName, other.ClassName) && string.Equals(MethodName, other.MethodName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((Method) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (AssemblyName != null ? AssemblyName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (NameSpace != null ? NameSpace.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ClassName != null ? ClassName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MethodName != null ? MethodName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Method left, Method right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Method left, Method right)
        {
            return !Equals(left, right);
        }

        #endregion       
    }
}
