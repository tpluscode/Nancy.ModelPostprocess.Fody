using System.Linq;
using Mono.Cecil;

namespace Nancy.ModelPostprocess.Fody
{
    public static class Extensions
    {
        internal static bool HasNancyModuleAncestor(this TypeDefinition childType)
        {
            return childType.HasAncestorOfType("Nancy.NancyModule");
        }

        internal static bool HasAncestorOfType(this TypeDefinition childType, string typeName)
        {
            var baseType = childType.BaseType;

            while (baseType != null)
            {
                if (baseType.FullName == typeName)
                {
                    return true;
                }

                baseType = baseType.Resolve().BaseType;
            }

            return false;
        }

        internal static FieldDefinition GetField(this TypeDefinition type, string name)
        {
            return type.Fields.SingleOrDefault(f => f.Name == name);
        }
    }
}