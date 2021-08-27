using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using TsUtilityTypesGenerator.Attributes;
using TsUtilityTypesGenerator.Templates;

namespace TsUtilityTypesGenerator
{
    [Generator]
    public partial class SourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = context.SyntaxReceiver as SyntaxReceiver;
            if (receiver == null) return;

            context.AddSource($"{nameof(PickAttribute)}.cs", _pickAttributeText);

            foreach (var (type, attr) in receiver.Targets)
            {
                var model = context.Compilation.GetSemanticModel(type.SyntaxTree);
                var destTypeSymbol = model.GetDeclaredSymbol(type)
                    ?? throw new Exception("failed to get type symbol.");

                var pickTargetType = attr.ArgumentList!.Arguments[0]; // PickAttribute(Type target, params string[] propertyNames)
                if (pickTargetType.Expression is not TypeOfExpressionSyntax typeOfExp)
                    throw new Exception($"require {nameof(PickAttribute)} attribute and ctor.");
                var pickTypeSymbol = model.GetSymbolInfo(typeOfExp.Type).Symbol as INamedTypeSymbol
                    ?? throw new Exception("require type-symbol.");
                var pickTargetProps = attr.ArgumentList!.Arguments.Skip(1).Select(x =>
                {
                    var literal = x.Expression as LiteralExpressionSyntax;
                    return literal?.Token.ValueText ?? "";
                });

                var template = new PickTemplate()
                {
                    PickBaseType = pickTypeSymbol,
                    PickTargetProperties = pickTargetProps,
                    DestTypeNamespace = destTypeSymbol.ContainingNamespace.ToDisplayString(),
                    DestTypeName = destTypeSymbol.Name,
                };
                var source = template.TransformText();

                context.AddSource($"{template.DestTypeNamespace}.{template.DestTypeName}.Generated.cs", source);
            }
        }

    }

    internal class SyntaxReceiver : ISyntaxReceiver
    {
        public List<(TypeDeclarationSyntax type, AttributeSyntax attr)> Targets { get; set; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax type && type.AttributeLists.Count > 0)
            {
                var attr = type.AttributeLists.SelectMany(x => x.Attributes)
                    .FirstOrDefault(x => x.Name.ToString() is "Pick" or nameof(PickAttribute));
                if (attr != null)
                    Targets.Add((type, attr));
            }

        }
    }
}
