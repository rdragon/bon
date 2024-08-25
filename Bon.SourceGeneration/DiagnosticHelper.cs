using Microsoft.CodeAnalysis;

namespace Bon.SourceGeneration
{
    internal static class DiagnosticHelper
    {
        public static Diagnostic GetError(int number, string message, ISymbol symbol)
        {
            var descriptor = new DiagnosticDescriptor(
                $"BON{number:0000}",
                message,
                message,
                "Bon.SourceGeneration",
                DiagnosticSeverity.Error,
                true);

            return Diagnostic.Create(descriptor, symbol?.TryGetLocation());
        }
    }
}
