namespace Bon.SourceGeneration
{
    internal readonly struct CodeGeneratorOutput
    {
        public string Code { get; }

        public string ClassName { get; }

        public CodeGeneratorOutput(string code, string className)
        {
            Code = code;
            ClassName = className;
        }
    }
}
