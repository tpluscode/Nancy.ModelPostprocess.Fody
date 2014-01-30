using System;
using System.Collections.Generic;
using System.Linq;
using Nancy.Responses.Negotiation;

namespace Nancy.ModelPostprocess
{
    public class ModelPostprocessor : IModelPostprocessor
    {
        private readonly IList<RegisteredInjector> _handlers = new List<RegisteredInjector>();

        public ModelPostprocessor()
        {
            RegisterModelHandler(new NegotiatorHandler(this));
        }

        public object Postprocess(object model, NancyModule module)
        {
            dynamic actualModel = model;

            var negotiator = model as Negotiator;
            if (negotiator != null)
            {
                actualModel = negotiator.NegotiationContext.DefaultModel;
            }

            foreach (var handler in HandlersFor(actualModel))
            {
                handler.Postprocess(actualModel, module);
            }

            return model;
        }

        /// <summary>
        /// Registers a type-specific injector
        /// </summary>
        public void RegisterModelHandler<T>(IModelHandler<T> handler)
        {
            _handlers.Add(new RegisteredInjector(typeof(T), handler));
        }

        private IEnumerable<dynamic> HandlersFor(object model)
        {
            return from registration in _handlers
                   where registration.ModelType.IsInstanceOfType(model)
                   select registration.Injector;
        } 

        private struct RegisteredInjector
        {
            public RegisteredInjector(Type modelType, object injector)
                : this()
            {
                ModelType = modelType;
                Injector = injector;
            }

            public Type ModelType { get; private set; }

            public object Injector { get; private set; }
        }
    }
}