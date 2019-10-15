using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Inedo.Extensibility;

[assembly: AssemblyTitle("PHP")]
[assembly: AssemblyDescription("Contains operations to help with CI/CD with PHP-based applications.")]
[assembly: AssemblyCompany("Inedo")]
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: CLSCompliant(false)]
[assembly: ComVisible(false)]

[assembly: AppliesTo(InedoProduct.BuildMaster | InedoProduct.Otter | InedoProduct.Hedgehog)]
