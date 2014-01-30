using AssemblyToProcess.Models;
using Nancy;
using Nancy.ModelPostprocess;

namespace AssemblyToProcess.ModelPostprocessors
{
    public class NumericModelPostprocessor : IModelHandler<NumericModel>
    {
        public void Postprocess(NumericModel model, NancyModule module)
        {
            model.Number++;
        }
    }
}