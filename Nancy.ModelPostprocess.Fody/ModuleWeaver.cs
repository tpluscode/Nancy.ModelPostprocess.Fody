using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Nancy.ModelPostprocess.Fody
{
    public partial class ModuleWeaver
    {
        public IAssemblyResolver AssemblyResolver { get; set; }

        public ModuleDefinition ModuleDefinition { get; set; }

        public void Execute()
        {
            LoadDependencies();
            LoadTypes();

            foreach (var typeDefinition in this.GetModuleTypes().ToList())
            {
                ProcessType(typeDefinition);
            }
        }

        private IEnumerable<TypeDefinition> GetModuleTypes()
        {
            return from type in this.ModuleDefinition.Types
                   where type.HasNancyModuleAncestor()
                   select type;
        }
    }
}