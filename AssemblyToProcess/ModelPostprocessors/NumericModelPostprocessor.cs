using AssemblyToProcess.Models;
using Nancy.ModelPostprocess;

namespace AssemblyToProcess.ModelPostprocessors
{
    public class NumericModelPostprocessor : IModelHandler<NumericModel>
    {
        public void Postprocess(NumericModel model)
        {
            model.Number++;
        }
    }
}