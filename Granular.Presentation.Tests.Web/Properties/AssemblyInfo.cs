using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Granular.Presentation.Tests;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Granular.Presentation.Tests.Web")]
[assembly: AssemblyDescription("")]

[assembly: Bridge.Reflectable(Bridge.MemberAccessibility.All)]

[assembly: ApplicationHost(typeof(TestApplicationHost))]
