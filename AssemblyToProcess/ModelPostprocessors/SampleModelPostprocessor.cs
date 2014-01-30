using AssemblyToProcess.Models;
using Nancy;
using Nancy.ModelPostprocess;

namespace AssemblyToProcess.ModelPostprocessors
{
    public class SampleModelPostprocessor : IModelHandler<SampleModel>
    {
        public void Postprocess(SampleModel model, NancyModule module)
        {
            model.SomeValue = "Set during postprocessing";
        }
    }
}