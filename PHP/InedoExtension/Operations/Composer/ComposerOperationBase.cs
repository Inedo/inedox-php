using System;
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

        protected async Task<(List<(bool error, string line)> output, int exitCode)> ExecuteComposeAsync(IOperationExecutionContext context, string args)
        {
            return await this.ExecutePHPAsync(context, await this.GetComposerExePathAsync(context) + " " + args);
        }

        private Task<string> GetComposerExePathAsync(IOperationExecutionContext context) =>
            FindExecutableAsync(context, this.ComposerExePath, "composer", Environment.SpecialFolder.CommonApplicationData, "ComposerSetup\\bin\\");

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
