using Nancy;

namespace AssemblyToProcess
{
    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get["Model"] = p => new SampleModel { SomeValue = "Set in module" };
            Get["AString"] = p => "some arbitrary value";
        }
    }
}
