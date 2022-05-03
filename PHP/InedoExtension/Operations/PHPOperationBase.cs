using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Inedo.Agents;
using Inedo.Documentation;
using Inedo.ExecutionEngine.Executer;
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
            using var proc = procExec.CreateProcess(new RemoteProcessStartInfo
            {
                FileName = await this.GetPHPExePathAsync(context),
                Arguments = args + " " + this.AdditionalArgs,
                WorkingDirectory = context.WorkingDirectory
            });

            proc.OutputDataReceived += (s, e) => output.Add((false, e.Data));
            proc.ErrorDataReceived += (s, e) => output.Add((true, e.Data));
            proc.Start();

            await proc.WaitAsync(context.CancellationToken);

            return (output, proc.ExitCode ?? -1);
        }

        private Task<string> GetPHPExePathAsync(IOperationExecutionContext context) => FindExecutableAsync(context, this.PHPExePath, "php", Environment.SpecialFolder.ProgramFiles, "PHP\\", ".exe");

        protected static async Task<string> FindExecutableAsync(IOperationExecutionContext context, string specifiedName, string defaultName, Environment.SpecialFolder specialFolder, string windowsPrefix = "", string windowsSuffix = "")
        {
            var fileOps = await context.Agent.GetServiceAsync<IFileOperationsExecuter>();

            if (!string.IsNullOrEmpty(specifiedName))
            {
                if (!await fileOps.FileExistsAsync(specifiedName))
                    throw new ExecutionFailureException($"Could not find the program '{defaultName}' at '{specifiedName}'. The path specified for this operation does not exist on the active server.");

                return specifiedName;
            }

            var autoName = fileOps.CombinePath("/usr/bin/", defaultName);
            var remoteMethod = await context.Agent.TryGetServiceAsync<IRemoteMethodExecuter>();
            if (remoteMethod != null)
                autoName = await remoteMethod.InvokeFuncAsync(FindRemotePath, specialFolder, windowsPrefix + defaultName + windowsSuffix);

            if (await fileOps.FileExistsAsync(autoName))
                return autoName;

            throw new ExecutionFailureException($"Could not find the program '{defaultName}' at '{autoName}'. Define the correct path on this operation or as a variable on this server.");
        }

        private static string FindRemotePath(Environment.SpecialFolder specialFolder, string defaultName)
        {
            return Path.GetFullPath(Path.Combine(Environment.GetFolderPath(specialFolder), defaultName));
        }
    }
}
