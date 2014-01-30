using System;
using System.Collections.Generic;
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
        public void Should_process_model_returned_directly()
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

        [Test]
        public void Should_process_model_negotiated_model_when_overriden_for_media_range()
        {
            // given
            var bootstrapper = (INancyBootstrapper)Activator.CreateInstance(_assembly.GetType("AssemblyToProcess.SampleBootstrapper"));
            var browser = new Browser(bootstrapper);

            // when
            var response = browser.Get("Negotiated", bc => bc.Accept(MediaRange.FromString("application/json")));

            // then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            dynamic model = JsonConvert.DeserializeObject(response.Body.AsString());
            Assert.That((string)model.number, Is.EqualTo("11"));
        }

        [Test]
        public void Should_process_model_negotiated_model_when_returning_default_model()
        {
            // given
            var bootstrapper = (INancyBootstrapper)Activator.CreateInstance(_assembly.GetType("AssemblyToProcess.SampleBootstrapper"));
            var browser = new Browser(bootstrapper);

            // when
            var response = browser.Get("Negotiated");

            // then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Body.AsString(), Is.EqualTo("21"));
        }

        [TestCaseSource("GetMediaRanges")]
        public void Should_not_process_multiple_times_when_negotiated_model_is_used_for_multiple_media_ranges(MediaRange mediaRange)
        {
            // given
            var bootstrapper = (INancyBootstrapper)Activator.CreateInstance(_assembly.GetType("AssemblyToProcess.SampleBootstrapper"));
            var browser = new Browser(bootstrapper);

            // when
            var response = browser.Get("ReusedInNegotiation", bc => bc.Accept(mediaRange));

            // then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Body.AsString(), Is.StringContaining("11"));
        }

        [Test]
        public void Should_not_weave_doubled_processing_when_route_is_already()
        {
            // given
            var bootstrapper = (INancyBootstrapper)Activator.CreateInstance(_assembly.GetType("AssemblyToProcess.SampleBootstrapper"));
            var browser = new Browser(bootstrapper);

            // when
            var response = browser.Get("aNumber", bc => bc.Accept(MediaRange.FromString("application/json")));

            // then
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            dynamic model = JsonConvert.DeserializeObject(response.Body.AsString());
            Assert.That((string)model.number, Is.EqualTo("2"));
        }

#if(DEBUG)
        [Test]
        public void PeVerify()
        {
            Verifier.Verify(_assembly.CodeBase.Remove(0, 8));
        }
#endif

        private static IEnumerable<MediaRange> GetMediaRanges()
        {
            yield return MediaRange.FromString("text/html");
            yield return MediaRange.FromString("application/json");
            yield return MediaRange.FromString("application/xml");
        }
    }
}