namespace Bon.SourceGeneration
{
    internal readonly struct CodeGeneratorOutput
    {
        public string Code { get; }

        public string ClassName { get; }

        public string DebugOutputDirectory { get; }

        public CodeGeneratorOutput(string code, string className, string debugOutputDirectory)
        {
            Code = code;
            ClassName = className;
            DebugOutputDirectory = debugOutputDirectory;
        }
    }
}
