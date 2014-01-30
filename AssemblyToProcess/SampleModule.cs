using Nancy;
using Nancy.ModelPostprocess;

namespace AssemblyToProcess
{
    public class SampleModule : NancyModule
    {
        public SampleModule(ModelPostprocessor postprocessor)
        {
            Get["Model"] = ModelPostprocessor.WrapRoute(p => new SampleModel { SomeValue = "Set in module" }, postprocessor, this);
            Get["AString"] = p => "some arbitrary value";
        }
    }
}
