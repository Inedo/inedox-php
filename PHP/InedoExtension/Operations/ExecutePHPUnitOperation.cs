using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Inedo.Agents;
using Inedo.Diagnostics;
using Inedo.Documentation;
using Inedo.Extensibility;
using Inedo.Extensibility.Operations;

namespace Inedo.Extensions.PHP.Operations
{
    [DisplayName("Execute PHPUnit Tests")]
    [Description("Runs PHPUnit tests on the specified test, test file, or all tests in the specified directory.")]
    [ScriptNamespace("PHP")]
    [ScriptAlias("Execute-PHPUnit")]
    public sealed class ExecutePHPUnitOperation : PHPOperationBase
    {
        [DisplayName("PHPUnit path")]
        [ScriptAlias("PHPUnitPath")]
        [DefaultValue("$DefaultPHPUnitPath")]
        public string PHPUnitPath { get; set; }

        [DisplayName("Test to run")]
        [ScriptAlias("TestToRun")]
        [DefaultValue(".")]
        public string TestToRun { get; set; } = ".";

        public override async Task ExecuteAsync(IOperationExecutionContext context)
        {
            var fileOps = await context.Agent.GetServiceAsync<IFileOperationsExecuter>();
            var tmpXmlPath = fileOps.CombinePath(context.WorkingDirectory, $"{Guid.NewGuid().ToString()}.xml");

            var testStart = DateTimeOffset.Now;

            await this.ExecutePHPAsync(context, $"{await this.GetPHPUnitPathAsync(context)} --log-junit {tmpXmlPath} {this.TestToRun}");
            if (!await fileOps.FileExistsAsync(tmpXmlPath))
            {
                this.LogWarning("PHPUnit did not generate an output XML file, therefore no tests were run. This can be caused if there are no test cases in the action's source directory, or the test file names do not end with \"Test\" (case-sensitive).");
                return;
            }

            List<UnitTestOutput.TestSuite> suites;
            try
            {
                using (var stream = await fileOps.OpenFileAsync(tmpXmlPath, FileMode.Open, FileAccess.Read))
                {
                    suites = UnitTestOutput.Load(stream);
                }
            }
            finally
            {
                await fileOps.DeleteFileAsync(tmpXmlPath);
            }

            var testRecorder = await context.TryGetServiceAsync<IUnitTestRecorder>();
            foreach (var suite in suites)
            {
                foreach (var test in suite.TestCases)
                {
                    await testRecorder.RecordUnitTestAsync(
                        groupName: suite.Name,
                        testName: test.Name,
                        testStatus: test.Failures.Any() ? UnitTestStatus.Failed : UnitTestStatus.Passed,
                        testResult: string.Empty,
                        startTime: testStart,
                        duration: TimeSpan.FromSeconds((double)test.Time)
                    );
                }
            }
        }

        private Task<string> GetPHPUnitPathAsync(IOperationExecutionContext context) =>
            this.FindExecutableAsync(context, this.PHPUnitPath, "phpunit", Environment.SpecialFolder.CommonProgramFiles, "..\\bin\\", ".phar");

        protected override ExtendedRichDescription GetDescription(IOperationConfiguration config)
        {
            return new ExtendedRichDescription(new RichDescription("Run PHPUnit tests"));
        }
    }
}
