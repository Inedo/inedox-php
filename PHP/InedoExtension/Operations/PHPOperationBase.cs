using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Inedo.Agents;
using Inedo.Documentation;
using Inedo.Extensibility;
using Inedo.Extensibility.Operations;

namespace Inedo.Extensions.PHP.Operations
{
    [Tag("php")]
    public abstract class PHPOperationBase : ExecuteOperation
    {
        [Category("Advanced")]
        [DisplayName("PHP path")]
        [ScriptAlias("PHPExePath")]
        [DefaultValue("$DefaultPHPExePath")]
        public string PHPExePath { get; set; }

        [Category("Advanced")]
        [DisplayName("Additional arguments")]
        [ScriptAlias("AdditionalArgs")]
        public string AdditionalArgs { get; set; }

        protected async Task<(List<(bool error, string line)> output, int exitCode)> ExecutePHPAsync(IOperationExecutionContext context, string args)
        {
            var output = new List<(bool error, string line)>();
            var procExec = await context.Agent.GetServiceAsync<IRemoteProcessExecuter>();
            using (var proc = procExec.CreateProcess(new RemoteProcessStartInfo
            {
                FileName = this.PHPExePath,
                Arguments = args + AH.ConcatNE(" ", this.AdditionalArgs),
                WorkingDirectory = context.WorkingDirectory
            }))
            {
                proc.OutputDataReceived += (s, e) => output.Add((false, e.Data));
                proc.ErrorDataReceived += (s, e) => output.Add((true, e.Data));
                proc.Start();

                await proc.WaitAsync(context.CancellationToken);

                return (output, proc.ExitCode ?? -1);
            }
        }
    }
}
