# This is an add-in for [Fody](https://github.com/Fody/Fody/) 

It extends [Nancy](https://github.com/NancyFx/Nancy/) with a way to modify models after a rout has been executed, but before they are serialized

# Nuget

Nuget package http://nuget.org/packages/Nancy.ModelPostprocess.Fody

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package Nancy.ModelPostprocess.Fody
	
# Why create this package?
	
Nancy is a very flexible framework, which offers a variety of extension points. 
One such extension point is the [AfterRequest](https://github.com/NancyFx/Nancy/wiki/The-Application-Before%2C-After-and-OnError-pipelines) pipeline

	pipelines.AfterRequest += (ctx) => {
		// Modify ctx.Response
	};
	
The problem is that `ctx.Response` holds serialized value (JSON. HTML, etc) of a response ready to be sent back to the client. Currently though there is no way to modify a model outside of a NancyModule. There is a question on StackOverflow about this: http://stackoverflow.com/questions/19095350/nancy-modify-model-in-afterrequest-event

# How this works

Nancy.ModelPostprocess.Fody is a Fody add-in, which means that extra code is injected to the modules so that the models returned from routes can be modified before thay are serialized. 
	
# Usage

## Module

### Your code

You implement your modules as usual

	public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            Get["Model"] = p => new SampleModel { SomeValue = "Set in module" };
        }
    }

## What gets compiled

	public class SampleModule : NancyModule
    {
        public SampleModule(IModelPostprocessor processor)
        {
			var route = new Func<dynamic, dynamic>(p => new SampleModel { SomeValue = "Original value" });
            Get["Model"] = route.WrapRoute(processor, this);
        }
    }

## Bootstrapper

In `Bootstrapper` you register the IModelPostprocessor and handlers for specific model types

	public class SampleBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            var postprocessor = new ModelPostprocessor();
            postprocessor.RegisterModelHandler(new SampleModelHandler());
            container.Register<IModelPostprocessor>(postprocessor);
        }
    }
	
## The model handler

Model handlers are classes implementing `IModelHandler<T>` interface, which has a single method

	public class SampleModelHandler : IModelHandler<SampleModel>
    {
        public void Postprocess(SampleModel model, NancyModule module)
        {
            model.SomeValue = "I can modify my model here :)";
        }
    }
	
## ModelPostprocessor class

The default implementation of `IModelPostprocessor` 

1. holds instances of model handlers and passes matching models for processing,
2. has a built-in handler for the `Negotiator` class, which handles model(s) returned for Content Negotiation,
3. currently requires that handlers are registered manually and as concrete instances
