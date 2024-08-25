namespace Bon.SourceGeneration
{
    readonly struct ContextClass
    {
        public string NamespaceName { get; }

        public string ClassName { get; }

        public ContextClass(string namespaceName, string className)
        {
            NamespaceName = namespaceName;
            ClassName = className;
        }
    }
}
