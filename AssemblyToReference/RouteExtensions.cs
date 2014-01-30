using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nancy.ModelPostprocess
{
    public static class RouteExtensions
    {
        public static Func<object, object> WrapRoute(this Func<object, object> route, ModelPostprocessor processor, NancyModule module)
        {
            return p => processor.Postprocess(route(p), module);
        }

        public static Func<object, CancellationToken, Task<object>> WrapAsyncRoute(this Func<object, CancellationToken, Task<object>> route, ModelPostprocessor processor, NancyModule module)
        {
            return async (p, token) => processor.Postprocess(await route(p, token), module);
        }
    }
}