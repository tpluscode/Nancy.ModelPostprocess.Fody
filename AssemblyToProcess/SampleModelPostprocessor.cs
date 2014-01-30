using Nancy.ModelPostprocess;

namespace AssemblyToProcess
{
    public class SampleModelPostprocessor : IModelHandler<SampleModel>
    {
        public void Postprocess(SampleModel model)
        {
            model.SomeValue = "Set during postprocessing";
        }
    }
}