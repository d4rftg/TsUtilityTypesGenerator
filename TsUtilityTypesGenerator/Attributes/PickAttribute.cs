using System;

namespace TsUtilityTypesGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false)]
    public class PickAttribute : Attribute
    {
        public PickAttribute(Type target, params string[] propertyNames)
        {
            Target = target;
            PropertyNames = propertyNames;
        }

        public Type Target { get; }
        public string[] PropertyNames { get; }
    }
}
