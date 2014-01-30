using AssemblyToProcess.Models;
using Nancy;
using Nancy.Responses.Negotiation;

namespace AssemblyToProcess.Modules
{
    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get["Model"] = p => new SampleModel { SomeValue = "Set in module" };
            Get["AString"] = p => "some arbitrary value";
            Get["Negotiated"] = NegotiatedRoute;
        }

        private dynamic NegotiatedRoute(dynamic p)
        {
            return Negotiate.WithModel(new NumericModel(10)).WithMediaRangeModel(MediaRange.FromString("text/html"), new NumericModel(20));
        }
    }
}
