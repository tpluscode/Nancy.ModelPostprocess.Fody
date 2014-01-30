using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mono.Cecil;

namespace Nancy.ModelPostprocess.Fody
{
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1512:SingleLineCommentsMustNotBeFollowedByBlankLine", Justification = "Reviewed. Suppression is OK here.")]
    public partial class ModuleWeaver
    {
// ReSharper disable InconsistentNaming
        private TypeReference PostprocessorType;
        private TypeReference RouteBuilderType;
        private MethodReference WrapMethod;
        private MethodReference AsyncWrapMethod;
// ReSharper restore InconsistentNaming

        private void LoadTypes()
        {
            var nancyModuleType = NancyAssembly.MainModule.Types.Single(t => t.Name == "NancyModule");
            var routeBuilderTypeDefinition = nancyModuleType.NestedTypes.Single(nt => nt.Name == "RouteBuilder");

            var postprocessorType = ReferencedAssembly.MainModule.Types.Single(type => type.Name == "ModelPostprocessor");
            PostprocessorType = ModuleDefinition.Import(postprocessorType);
            RouteBuilderType = ModuleDefinition.Import(routeBuilderTypeDefinition);

            var routeHelperType = ReferencedAssembly.MainModule.Types.Single(type => type.Name == "RouteExtensions");
            WrapMethod = ModuleDefinition.Import(routeHelperType.FindMethod("WrapRoute", "Func`2", PostprocessorType.Name, nancyModuleType.Name));
            AsyncWrapMethod = ModuleDefinition.Import(routeHelperType.FindMethod("WrapAsyncRoute", "Func`3", PostprocessorType.Name, nancyModuleType.Name));
        }
    }
}