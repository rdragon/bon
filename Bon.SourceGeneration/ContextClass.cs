namespace Bon.SourceGeneration
{
    readonly struct ContextClass
    {
        public string NamespaceName { get; }

        public string ClassName { get; }

        public string DebugOutputDirectory { get; }

        public ContextClass(string namespaceName, string className, string debugOutputDirectory)
        {
            NamespaceName = namespaceName;
            ClassName = className;
            DebugOutputDirectory = debugOutputDirectory;
        }
    }
}
