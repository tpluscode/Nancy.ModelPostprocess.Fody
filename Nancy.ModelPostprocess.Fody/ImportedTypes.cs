using System.Linq;
using Mono.Cecil;

namespace Nancy.ModelPostprocess.Fody
{
    public partial class ModuleWeaver
    {
        private TypeReference InjectorType;
        private TypeReference RouteBuilderType;
        private MethodReference InjectMethod;
        private MethodReference AsyncInjectMethod;

        private void LoadTypes()
        {
            var nancyModuleType = NancyAssembly.MainModule.Types.Single(t => t.Name == "NancyModule");
            var routeBuilderTypeDefinition = nancyModuleType.NestedTypes.Single(nt => nt.Name == "RouteBuilder");

            var injectorType = ReferencedAssembly.MainModule.Types.Single(type => type.Name == "ModelPostprocessor");
            InjectorType = ModuleDefinition.Import(injectorType);
            RouteBuilderType = ModuleDefinition.Import(routeBuilderTypeDefinition);

            InjectMethod = ModuleDefinition.Import(injectorType.FindMethod("WrapRoute", "Func`2", InjectorType.Name, nancyModuleType.Name));
            AsyncInjectMethod = ModuleDefinition.Import(injectorType.FindMethod("WrapAsyncRoute", "Func`3", InjectorType.Name, nancyModuleType.Name));
        }
    }
}