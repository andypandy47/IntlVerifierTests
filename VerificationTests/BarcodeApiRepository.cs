using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace VerificationTests
{
    public class BarcodeApiRepository
    {
        public BarcodeApiRepository()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceStream = assembly.GetManifestResourceStream(Path);

            if (resourceStream == null)
            {
                throw new Exception($"Resource [{Path}] was not found!");
            }

            using (var reader = new StreamReader(resourceStream))
            {
                var data = reader.ReadToEnd();

                this.ExampleData = JsonConvert.DeserializeObject<IEnumerable<TestExample>>(data);
            }
        }

        private const string Path = "VerificationTests.Examples.BarcodeApi.json";

        private IEnumerable<TestExample> ExampleData { get; }

        public IEnumerable<TestExample> GetTestExamples()
        {
            return this.ExampleData;
        }
    }
}