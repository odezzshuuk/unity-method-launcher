using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Odezzshuuk.Workflow.MethodLauncher.Analyzers.Constants;

namespace Odezzshuuk.Workflow.MethodLauncher.Analyzer {
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class LaunchableAttributeSuppressor : DiagnosticSuppressor {

    // Define the suppression descriptor
    private static readonly SuppressionDescriptor s_Rule = new SuppressionDescriptor(
        id: PRIVATE_UNUSED_MEMBER_SUPPRESSION_ID,
        suppressedDiagnosticId: UNUSED_MEMBER_DIAGNOSTIC_ID,
        justification: "Private methods marked with [Launchable] are used via reflection/framework logic.");

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(s_Rule);

    public override void ReportSuppressions(SuppressionAnalysisContext context) {
      foreach (Diagnostic diagnostic in context.ReportedDiagnostics) {
        // Check if the diagnostic ID matches the one we want to suppress
        if (diagnostic.Id == UNUSED_MEMBER_DIAGNOSTIC_ID || diagnostic.Id == "CS0628") {
          SyntaxTree? syntaxTree = diagnostic.Location.SourceTree;
          if (syntaxTree == null) continue;
          SyntaxNode root = syntaxTree.GetRoot(context.CancellationToken);
          SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);

          // Get the semantic model to check symbols/attributes
          SemanticModel semanticModel = context.GetSemanticModel(syntaxTree);
          ISymbol? memberSymbol = semanticModel.GetDeclaredSymbol(node);

          if (memberSymbol != null && HasCommandAttribute(memberSymbol)) {
            // If it has the attribute, suppress the diagnostic
            context.ReportSuppression(Suppression.Create(s_Rule, diagnostic));
          }
        }
      }
    }

    private static bool HasCommandAttribute(ISymbol memberSymbol) {
      // Replace "Command" with the actual full name of your attribute if it's in a namespace
      // e.g., "YourFramework.CommandAttribute"

      foreach (AttributeData attribute in memberSymbol.GetAttributes()) {
        if (attribute.AttributeClass?.Name == ATTRIBUTE_NAME
            && attribute.AttributeClass.ContainingNamespace?.ToDisplayString() == ATTRIBUTE_NAMESPACE) {
          return true;
        }
      }
      return false;
    }

  }

}
