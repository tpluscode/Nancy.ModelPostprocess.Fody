using AssemblyToProcess.ModelPostprocessors;
using Nancy;
using Nancy.ModelPostprocess;

namespace AssemblyToProcess
{
    public class SampleBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            var postprocessor = new ModelPostprocessor();
            postprocessor.RegisterModelHandler(new SampleModelPostprocessor());
            postprocessor.RegisterModelHandler(new NumericModelPostprocessor());
            container.Register<IModelPostprocessor>(postprocessor);
        }
    }
}