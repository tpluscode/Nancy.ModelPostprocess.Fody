namespace Nancy.ModelPostprocess
{
    public interface IModelHandler<in T>
    {
        void Postprocess(T model);
    }
}