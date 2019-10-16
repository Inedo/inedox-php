using System.ComponentModel;
using Inedo.Extensibility;
using Inedo.Extensibility.VariableFunctions;

namespace Inedo.Extensions.PHP.VariableFunctions
{
    [ScriptAlias("PHPExePath")]
    [Description("Full path of php.exe. The default is /usr/bin/php on Linux and C:\\Program Files\\PHP\\php.exe on Windows.")]
    [ExtensionConfigurationVariable(Required = false)]
    public sealed class DefaultPHPExePathVariableFunction : ScalarVariableFunction
    {
        protected override object EvaluateScalar(IVariableFunctionContext context) => string.Empty;
    }
}
