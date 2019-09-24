using System.ComponentModel;
using System.Threading.Tasks;
using Inedo.Diagnostics;
using Inedo.Documentation;
using Inedo.Extensibility;
using Inedo.Extensibility.Operations;

namespace Inedo.Extensions.PHP.Operations.Composer
{
    [DisplayName("Install Composer Packages")]
    [ScriptAlias("Install-Packages")]
    public sealed class InstallPackagesOperation : ComposerOperationBase
    {
        [DisplayName("Use dist packages")]
        [ScriptAlias("PreferDist")]
        [DefaultValue(true)]
        public bool PreferDist { get; set; } = true;

        [DisplayName("Include dev dependencies")]
        [ScriptAlias("IncludeDev")]
        [DefaultValue(false)]
        public bool IncludeDev { get; set; } = false;

        [DisplayName("Run scripts")]
        [Description("Set to false to skip execution of scripts defined in composer.json.")]
        [ScriptAlias("RunScripts")]
        [DefaultValue(true)]
        public bool RunScripts { get; set; } = true;

        public override async Task ExecuteAsync(IOperationExecutionContext context)
        {
            var (output, exitCode) = await this.ExecuteComposeAsync(context, $"install --no-progress {(this.PreferDist ? "--prefer-dist" : "--prefer-source")}{(context.Simulation ? " --dry-run" : string.Empty)} {(this.IncludeDev ? "--dev" : "--no-dev")}{(this.RunScripts ? string.Empty : " --no-scripts")}");
            if (exitCode != 0)
            {
                this.LogComposerError(output, exitCode);
                return;
            }

            foreach (var (_, line) in output)
            {
                this.LogDebug(line);
            }
        }

        protected override ExtendedRichDescription GetDescription(IOperationConfiguration config)
        {
            return new ExtendedRichDescription(new RichDescription("Install Composer packages"));
        }
    }
}
