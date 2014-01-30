﻿using System.Linq;
using Mono.Cecil;

namespace Nancy.ModelPostprocess.Fody
{
    public static class CecilExtensions
    {
        public static MethodDefinition FindMethod(this TypeDefinition typeDefinition, string method, params string[] paramTypes)
        {
            var firstOrDefault = typeDefinition.Methods
                                               .FirstOrDefault(x =>
                                                               !x.HasGenericParameters &&
                                                               x.Name == method &&
                                                               IsMatch(x, paramTypes));
            if (firstOrDefault == null)
            {
                var parameterNames = string.Join(", ", paramTypes);
                throw new WeavingException(string.Format("Expected to find method '{0}({1})' on type '{2}'.", method, parameterNames, typeDefinition.FullName));
            }

            return firstOrDefault;
        }

        public static bool IsMatch(this MethodReference methodReference, params string[] paramTypes)
        {
            if (methodReference.Parameters.Count != paramTypes.Length)
            {
                return false;
            }

            for (var index = 0; index < methodReference.Parameters.Count; index++)
            {
                var parameterDefinition = methodReference.Parameters[index];
                var paramType = paramTypes[index];
                if (parameterDefinition.ParameterType.Name != paramType)
                {
                    return false;
                }
            }

            return true;
        }
    }
}