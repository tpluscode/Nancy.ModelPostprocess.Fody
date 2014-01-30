using System;
using System.Linq;
using Nancy.Responses.Negotiation;

namespace Nancy.ModelPostprocess
{
    internal class NegotiatorHandler : IModelHandler<Negotiator>
    {
        private readonly IModelPostprocessor _postprocessor;

        public NegotiatorHandler(IModelPostprocessor postprocessor)
        {
            _postprocessor = postprocessor;
        }

        public void Postprocess(Negotiator model, NancyModule module)
        {
            var negotiationContext = model.NegotiationContext;
            _postprocessor.Postprocess(negotiationContext.DefaultModel, module);
            ProcessMediaRangeMappings(negotiationContext, module);
        }

        private void ProcessMediaRangeMappings(NegotiationContext negotiationContext, NancyModule module)
        {
            var mappingProcessed = from mapping in negotiationContext.MediaRangeModelMappings
                                   select new
                                           {
                                               mapping.Key,
                                               Value = new Func<dynamic>(() => _postprocessor.Postprocess(mapping.Value(), module))
                                           };

            negotiationContext.MediaRangeModelMappings = mappingProcessed.ToDictionary(p => p.Key, p => p.Value);
        }
    }
}