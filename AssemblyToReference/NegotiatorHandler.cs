using System;
using System.Collections.Generic;
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
            var modelsProcessed = new List<object>(negotiationContext.MediaRangeModelMappings.Count + 1)
                                      {
                                          negotiationContext.DefaultModel
                                      };

            var mappingProcessed =
                negotiationContext.MediaRangeModelMappings.Select(
                    mapping =>
                        {
                            var origModel = mapping.Value();

                            // ensure that same object is not processed multiple times
                            if (modelsProcessed.Any(model => object.ReferenceEquals(model, origModel)))
                            {
                                return new { mapping.Key, Value = origModel };
                            }

                            modelsProcessed.Add(origModel);
                            return new { mapping.Key, Value = _postprocessor.Postprocess(origModel, module) };
                        });

            negotiationContext.MediaRangeModelMappings = mappingProcessed.ToDictionary(p => p.Key, p => new Func<dynamic>(() => p.Value));
        }
    }
}