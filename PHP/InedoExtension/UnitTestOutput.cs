using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Inedo.Extensions.PHP
{
    internal static class UnitTestOutput
    {
        public static List<TestSuite> Load(Stream stream)
        {
            return (List<TestSuite>)new XmlSerializer(typeof(List<TestSuite>), new XmlRootAttribute("testsuites") { ElementName = "testsuite" }).Deserialize(stream);
        }

        internal sealed class TestSuite
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlAttribute("file")]
            public string File { get; set; }

            [XmlAttribute("tests")]
            public int Tests { get; set; }

            [XmlAttribute("assertions")]
            public int Assertions { get; set; }

            [XmlAttribute("failures")]
            public int Failures { get; set; }

            [XmlAttribute("errors")]
            public int Errors { get; set; }

            [XmlAttribute("time")]
            public decimal Time { get; set; }

            [XmlArray("testcase")]
            public List<TestCase> TestCases { get; set; }
        }

        internal sealed class TestCase
        {
            [XmlAttribute("name")]
            public string Name { get; set; }

            [XmlAttribute("class")]
            public string Class { get; set; }

            [XmlAttribute("file")]
            public string File { get; set; }

            [XmlAttribute("line")]
            public int Line { get; set; }

            [XmlAttribute("assertions")]
            public int Assertions { get; set; }

            [XmlAttribute("time")]
            public decimal Time { get; set; }

            [XmlArrayItem("failure", typeof(Failure))]
            [XmlArrayItem("error", typeof(Error))]
            public List<Assertion> Failures { get; set; }
        }

        internal abstract class Assertion
        {
            public abstract bool IsFailure { get; }
            public abstract bool IsError { get; }

            [XmlAttribute("type")]
            public string Type { get; set; }

            [XmlText]
            public string Log { get; set; }
        }

        internal sealed class Failure : Assertion
        {
            public override bool IsFailure => true;
            public override bool IsError => false;
        }

        internal sealed class Error : Assertion
        {
            public override bool IsFailure => false;
            public override bool IsError => true;
        }
    }
}
