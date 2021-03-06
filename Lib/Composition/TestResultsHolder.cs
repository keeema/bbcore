﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Lib.Composition
{
    public class TestResultsHolder : SuiteOrTest
    {
        public string UserAgent;
        public bool Running;
        public int TestsFailed;
        public int TestsSkipped;
        public int TestsFinished;
        public int TotalTests;

        internal new TestResultsHolder Clone()
        {
            return new TestResultsHolder
            {
                Id = Id,
                ParentId = ParentId,
                IsSuite = IsSuite,
                Name = Name,
                Skipped = Skipped,
                Failure = Failure,
                Duration = Duration,
                Failures = Failures.ToList(),
                Nested = new List<SuiteOrTest>(Nested.Select(n => n.Clone())),
                Logs = Logs.ToList(),
                UserAgent = UserAgent,
                Running = Running,
                TestsFailed = TestsFailed,
                TestsFinished = TestsFinished,
                TestsSkipped = TestsSkipped,
                TotalTests = TotalTests
            };
        }

        static void WriteJUnitSystemOut(XmlWriter w, SuiteOrTest test)
        {
            if (test.Logs == null || test.Logs.Count == 0)
                return;
            w.WriteStartElement("system-out");
            w.WriteAttributeString("xml", "space", null, "preserve");
            test.Logs.ForEach(m =>
            {
                w.WriteString(m.Message + "\n  " + string.Join("\n  ", m.Stack) + "\n");
            });
            w.WriteEndElement();
        }

        static void RecursiveWriteJUnit(XmlWriter w, SuiteOrTest suite, string name)
        {
            var duration = 0d;
            var count = 0;
            var flat = true;
            suite.Nested.ForEach(n =>
            {
                if (n.IsSuite)
                {
                    flat = false;
                    return;
                }
                count++;
                duration += n.Duration;
            });
            if (flat)
                duration = suite.Duration;
            if (count > 0)
            {
                w.WriteStartElement("testsuite");
                w.WriteAttributeString("name", string.IsNullOrEmpty(name) ? "root" : name);
                w.WriteAttributeString("time", (duration * 0.001).ToString("F4", CultureInfo.InvariantCulture));
                suite.Nested.ForEach(test =>
                {
                    if (test.IsSuite)
                        return;
                    w.WriteStartElement("testcase");
                    w.WriteAttributeString("name", test.Name);
                    w.WriteAttributeString("time", (test.Duration * 0.001).ToString("F4", CultureInfo.InvariantCulture));
                    if (test.Skipped)
                    {
                        w.WriteStartElement("skipped");
                        w.WriteEndElement();
                    }
                    else if (test.Failure)
                    {
                        test.Failures.ForEach(fail =>
                        {
                            w.WriteStartElement("failure");
                            w.WriteAttributeString("message", fail.Message + "\n" + string.Join("\n", fail.Stack));
                            w.WriteEndElement();
                        });
                    }
                    WriteJUnitSystemOut(w, test);
                    w.WriteEndElement();
                });
                WriteJUnitSystemOut(w, suite);
                w.WriteEndElement();
            }
            if (!flat)
            {
                suite.Nested.ForEach(n =>
                {
                    if (n.IsSuite)
                    {
                        RecursiveWriteJUnit(w, n, (!string.IsNullOrEmpty(name) ? name + "." : "") + n.Name);
                    }
                });
            }
        }

        public string ToJUnitXml()
        {
            var sw = new StringWriter();
            var w = new XmlTextWriter(sw);
            w.WriteStartDocument();
            w.WriteStartElement("testsuites");
            w.WriteAttributeString("errors", "0");
            w.WriteAttributeString("failures", "" + TestsFailed);
            w.WriteAttributeString("tests", "" + TotalTests);
            w.WriteAttributeString("time", (Duration * 0.001).ToString("F4", CultureInfo.InvariantCulture));
            RecursiveWriteJUnit(w, this, "");
            w.WriteEndElement();
            w.WriteEndDocument();
            return sw.ToString();
        }
    }
}
