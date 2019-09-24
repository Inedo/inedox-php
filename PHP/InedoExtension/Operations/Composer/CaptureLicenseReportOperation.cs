using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Inedo.Diagnostics;
using Inedo.Documentation;
using Inedo.Extensibility;
using Inedo.Extensibility.Operations;

namespace Inedo.Extensions.PHP.Operations.Composer
{
    [DisplayName("Capture Composer License Report")]
    [Description("Creates a report containing a list of PHP dependency licenses.")]
    [ScriptAlias("Capture-LicenseReport")]
    public sealed class CaptureLicenseReportOperation : ComposerOperationBase
    {
        [Required]
        [DisplayName("Report name")]
        [ScriptAlias("Name")]
        public string ReportName { get; set; }

        [DisplayName("Include dev dependencies")]
        [ScriptAlias("IncludeDev")]
        [DefaultValue(true)]
        public bool IncludeDev { get; set; } = true;

        public override async Task ExecuteAsync(IOperationExecutionContext context)
        {
            var reportRecorder = await context.TryGetServiceAsync<IBuildReportRecorder>();
            if (reportRecorder == null)
            {
                this.LogError("This operation requires support for build reports.");
                return;
            }

            var (output, exitCode) = await this.ExecuteComposeAsync(context, $"licenses --format=text{(this.IncludeDev ? string.Empty : " --no-dev")}");
            if (exitCode != 0)
            {
                this.LogComposerError(output, exitCode);
                return;
            }

            await reportRecorder.AddReportAsync(this.ReportName, string.Join("\n", output.Where(l => !l.error).Select(l => l.line)), true);
        }

        protected override ExtendedRichDescription GetDescription(IOperationConfiguration config)
        {
            return new ExtendedRichDescription(
                new RichDescription("Create Composer license report ", new Hilite(config[nameof(this.ReportName)])),
                new RichDescription(string.Equals(config[nameof(this.IncludeDev)], bool.FalseString, StringComparison.OrdinalIgnoreCase) ? null : "including dev dependencies")
            );
        }
    }
}
