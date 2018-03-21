using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Yahvol.Services")]
[assembly: AssemblyDescription("Library for saving and running service commands. Service commands are saved immediatly to the database, and return Ok (or Exception). The library then runs 1 to many workloads asyncronously associated with each command, " +
    "these workloads have a retry mechanism, to handle intermittent issues. " +
    "When all workloads are completed the service command is marked as completed")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Anderton Engineered Solutions")]
[assembly: AssemblyProduct("Yahvol.Services")]
[assembly: AssemblyCopyright("Copyright ©  Anderton Engineered Solutions   2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("92ed1290-ddbc-469b-aca9-6297c3b723f9")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.5.3")]
[assembly: AssemblyFileVersion("2.0.5.3")]
[assembly: InternalsVisibleTo("Yahvol.Services.Tests1")]
[assembly: InternalsVisibleTo("Yahvol.Services.Explorables")]