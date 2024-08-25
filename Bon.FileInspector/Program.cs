using Bon.FileInspector;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Bon.FileInspector.Test")]

try
{
    await new Inspector(new FileSystem()).Run(args);
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    Console.ReadLine();
}
