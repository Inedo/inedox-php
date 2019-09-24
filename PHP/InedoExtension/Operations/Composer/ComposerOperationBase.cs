using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Inedo.Diagnostics;
using Inedo.Extensibility;
using Inedo.Extensibility.Operations;

namespace Inedo.Extensions.PHP.Operations.Composer
{
    [ScriptNamespace("Composer")]
    public abstract class ComposerOperationBase : PHPOperationBase
    {
        [Category("Advanced")]
        [DisplayName("Composer path")]
        [ScriptAlias("ComposerExePath")]
        [DefaultValue("$DefaultComposerExePath")]
        public string ComposerExePath { get; set; }

        protected Task<(List<(bool error, string line)> output, int exitCode)> ExecuteComposeAsync(IOperationExecutionContext context, string args)
        {
            return this.ExecutePHPAsync(context, this.ComposerExePath + " " + args);
        }

        protected void LogComposerError(List<(bool error, string line)> output, int exitCode)
        {
            this.LogError($"Composer exited with code {exitCode}");
            foreach (var (error, line) in output)
            {
                if (error)
                    this.LogWarning(line);
                else
                    this.LogInformation(line);
            }
        }
    }
}
