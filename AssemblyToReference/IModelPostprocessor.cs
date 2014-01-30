namespace Nancy.ModelPostprocess
{
    public interface IModelPostprocessor
    {
        object Postprocess(object model, NancyModule module);
    }
}