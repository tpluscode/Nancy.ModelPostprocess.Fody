using AssemblyToProcess.Models;
using Nancy.ModelPostprocess;

namespace AssemblyToProcess.ModelPostprocessors
{
    public class SampleModelPostprocessor : IModelHandler<SampleModel>
    {
        public void Postprocess(SampleModel model)
        {
            model.SomeValue = "Set during postprocessing";
        }
    }
}