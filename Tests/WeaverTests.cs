using System;
using System.Reflection;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Responses.Negotiation;
using Nancy.Testing;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class WeaverTests
    {
        private Assembly _assembly;

        [TestFixtureSetUp]
        public void Setup()
        {
            _assembly = WeaverHelper.WeaveAssembly();
        }

        [Test]
        public void Modified_ordinary_route_should_execute_postprocessor()
        {
            // given
            var bootstrapper = (INancyBootstrapper)Activator.CreateInstance(_assembly.GetType("AssemblyToProcess.SampleBootstrapper"));
            var browser = new Browser(bootstrapper);

            // when
            var response = browser.Get("Model", bc => bc.Accept(MediaRange.FromString("application/json")));

            // then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            dynamic model = JsonConvert.DeserializeObject(response.Body.AsString());
            Assert.That((string)model.someValue, Is.EqualTo("Set during postprocessing"));
        }

#if(DEBUG)
        [Test]
        public void PeVerify()
        {
            Verifier.Verify(_assembly.CodeBase.Remove(0, 8));
        }
#endif
    }
}