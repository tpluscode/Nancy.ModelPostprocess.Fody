using System;
using System.Collections.Generic;
using System.Linq;

namespace Nancy.ModelPostprocess
{
    public class ModelPostprocessor
    {
        private readonly IList<RegisteredInjector> _injectors = new List<RegisteredInjector>();

        public object Postprocess(object model, NancyModule context)
        {
            foreach (var handler in HandlersFor(model))
            {
                handler.Postprocess((dynamic)model);
            }

            return model;
        }

        /// <summary>
        /// Registers a type-specific injector
        /// </summary>
        public void RegisterModelHandler<T>(IModelHandler<T> handler)
        {
            _injectors.Add(new RegisteredInjector(typeof(T), handler));
        }

        private IEnumerable<dynamic> HandlersFor(object model)
        {
            return from registration in _injectors
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