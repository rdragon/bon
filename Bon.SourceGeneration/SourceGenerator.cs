using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Bon.SourceGeneration
{
    [Generator]
    internal sealed class SourceGenerator : IIncrementalGenerator
    {
        private readonly DefinitionFactory _factory = new DefinitionFactory();

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            DebugOutput.AppendLine("Initialize");

            var maybeDefinitions = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Bon.Serializer.BonObjectAttribute",
                (_, _1) => true,
                TransformBonObject);

            var maybeContextClasses = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Bon.Serializer.BonSerializerContextAttribute",
                (_, _1) => true,
                TransformBonSerializerContext);

            var definitions = maybeDefinitions.Where(maybe => maybe.HasData).Select((maybe, _) => maybe.Data).Collect();
            var contextClasses = maybeContextClasses.Where(maybe => maybe.HasData).Select((maybe, _) => maybe.Data);
            var maybeOutputs = contextClasses.Combine(definitions).Select(TryGetOutput);
            var outputs = maybeOutputs.Where(maybe => maybe.HasData).Select((maybe, _) => maybe.Data);

            var diagnostics1 = maybeDefinitions.Where(maybe => !maybe.HasData).Select((maybe, _) => maybe.Diagnostic);
            var diagnostics2 = maybeContextClasses.Where(maybe => !maybe.HasData).Select((maybe, _) => maybe.Diagnostic);
            var diagnostics3 = maybeOutputs.Where(maybe => !maybe.HasData).Select((maybe, _) => maybe.Diagnostic);

            context.RegisterImplementationSourceOutput(outputs, WriteFile);
            context.RegisterImplementationSourceOutput(diagnostics1, ReportDiagnostic);
            context.RegisterImplementationSourceOutput(diagnostics2, ReportDiagnostic);
            context.RegisterImplementationSourceOutput(diagnostics3, ReportDiagnostic);
        }

        private Maybe<IDefinition> TransformBonObject(GeneratorAttributeSyntaxContext context, CancellationToken _)
        {
            return TryExecute((symbol, sourceGenerator) =>
            {
                sourceGenerator._factory.Clear();

                return sourceGenerator._factory.GetDefinition((INamedTypeSymbol)symbol);
            }, context.TargetSymbol);
        }

        private Maybe<ContextClass> TransformBonSerializerContext(GeneratorAttributeSyntaxContext context, CancellationToken _)
        {
            var isPartialClass = ((ClassDeclarationSyntax)context.TargetNode).Modifiers.Any(SyntaxKind.PartialKeyword);

            if (!isPartialClass)
            {
                return new Maybe<ContextClass>(DiagnosticHelper.GetError(
                    9622,
                    $"The class '{context.TargetSymbol}' must be a partial class.",
                    context.TargetSymbol));
            }

            return TryExecute((symbol, _1) =>
            {
                return new ContextClass(symbol.ContainingNamespace.ToString(), symbol.Name);
            }, context.TargetSymbol);
        }

        private Maybe<CodeGeneratorOutput> TryGetOutput((ContextClass ContextClass, ImmutableArray<IDefinition> Definitions) tuple, CancellationToken _)
        {
            return TryExecute((data, _1) =>
            {
                return CodeGeneratorHelper.GetCode(data.Definitions, data.ContextClass);
            }, tuple);
        }

        private static void WriteFile(SourceProductionContext context, CodeGeneratorOutput output)
        {
            DebugOutput.WriteAllText(output.Code);
            context.AddSource($"{output.ClassName}.g.cs", output.Code);
        }

        private static void ReportDiagnostic(SourceProductionContext context, Diagnostic diagnostic)
        {
            context.ReportDiagnostic(diagnostic);
        }

        private Maybe<TOut> TryExecute<TIn, TOut>(Func<TIn, SourceGenerator, TOut> func, TIn parameter)
        {
            try
            {
                return new Maybe<TOut>(func(parameter, this));
            }
            catch (Exception ex)
            {
                DebugOutput.WriteException(ex);

                return new Maybe<TOut>(GetDiagnostic(ex));
            }
        }

        private static Diagnostic GetDiagnostic(Exception exception)
        {
            if ((exception as SourceGenerationException)?.Diagnostic is Diagnostic diagnostic)
            {
                return diagnostic;
            }

            return DiagnosticHelper.GetError(9999, exception.ToString(), null);
        }
    }
}
