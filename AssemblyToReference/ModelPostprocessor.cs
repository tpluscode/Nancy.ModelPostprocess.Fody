using System;
using System.Collections.Generic;
using System.Linq;
using NullGuard;

namespace Nancy.ModelPostprocess
{
    public class ModelPostprocessor : IModelPostprocessor
    {
        private readonly IList<RegisteredInjector> _handlers = new List<RegisteredInjector>();

        public ModelPostprocessor()
        {
            RegisterModelHandler(new NegotiatorHandler(this));
        }

        [return: AllowNull]
        public object Postprocess([AllowNull] object model, NancyModule module)
        {
            foreach (var handler in HandlersFor(model))
            {
                handler.Postprocess((dynamic)model, module);
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