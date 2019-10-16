using System.ComponentModel;
using Inedo.Extensibility;
using Inedo.Extensibility.VariableFunctions;

namespace Inedo.Extensions.PHP.VariableFunctions
{
    [ScriptAlias("PHPUnitPath")]
    [Description("Full path of phpunit. The default is /usr/bin/phpunit on Linux and ??? on Windows.")]
    [ExtensionConfigurationVariable(Required = false)]
    public sealed class DefaultPHPUnitPathVariableFunction : ScalarVariableFunction
    {
        protected override object EvaluateScalar(IVariableFunctionContext context) => string.Empty;
    }
}
