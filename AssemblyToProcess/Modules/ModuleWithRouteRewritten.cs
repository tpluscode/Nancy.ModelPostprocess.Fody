using System;
using AssemblyToProcess.Models;
using Nancy;
using Nancy.ModelPostprocess;

namespace AssemblyToProcess.Modules
{
    public class ModuleWithRouteRewritten : NancyModule
    {
        public ModuleWithRouteRewritten(IModelPostprocessor processor)
        {
            Get["aNumber"] = new Func<object, object>(p => new NumericModel(1)).WrapRoute(processor, this);
        }
    }
}