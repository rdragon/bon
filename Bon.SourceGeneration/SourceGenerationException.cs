using Microsoft.CodeAnalysis;
using System;
using System.Runtime.Serialization;

namespace Bon.SourceGeneration
{
    [Serializable]
    public sealed class SourceGenerationException : Exception
    {
        public Diagnostic Diagnostic { get; }

        public SourceGenerationException(string message) : base(message) { }

        public SourceGenerationException(Diagnostic diagnostic) : base(diagnostic.GetMessage())
        {
            Diagnostic = diagnostic;
        }

        public SourceGenerationException(string message, int number, ISymbol symbol) :
            this(DiagnosticHelper.GetError(number, message, symbol))
        { }

        private SourceGenerationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
