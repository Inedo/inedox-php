using System.ComponentModel;
using Inedo.Extensibility;
using Inedo.Extensibility.VariableFunctions;

namespace Inedo.Extensions.PHP.VariableFunctions
{
    [ScriptAlias("ComposerExePath")]
    [Description("Full path of composer. The default is /usr/bin/composer on Linux and C:\\ProgramData\\ComposerSetup\\bin\\composer on Windows.")]
    [ExtensionConfigurationVariable(Required = false)]
    public sealed class DefaultComposerExePathVariableFunction : ScalarVariableFunction
    {
        protected override object EvaluateScalar(IVariableFunctionContext context) => string.Empty;
    }
}
