using System;
using System.Linq;
using Mono.Cecil;

namespace Nancy.ModelPostprocess.Fody
{
    public partial class ModuleWeaver
    {
        private AssemblyDefinition NancyAssembly;
        private AssemblyDefinition ReferencedAssembly;

        private void LoadDependencies()
        {
            NancyAssembly = LoadAssemblyReference("Nancy");
            ReferencedAssembly = LoadAssemblyReference("Nancy.ModelPostprocess");
        }

        private AssemblyDefinition LoadAssemblyReference(string assemblyFullName)
        {
            var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == assemblyFullName);

            if (existingReference != null)
            {
                return AssemblyResolver.Resolve(existingReference);
            }

            var reference = AssemblyResolver.Resolve(assemblyFullName);
            if (reference != null)
            {
                return reference;
            }

            throw new Exception(string.Format("Could not resolve a reference to {0}.", assemblyFullName));
        }
    }
}