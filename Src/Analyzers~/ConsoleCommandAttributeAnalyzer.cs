using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using static Synaptafin.PlayModeConsole.Analyzers.Constants;

namespace Synaptafin.PlayModeConsole.Analyzer {

  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class ConsoleCommandAttributeAnalyzer : DiagnosticAnalyzer {

    private static readonly DiagnosticDescriptor s_notMonoBehaviourRule = new DiagnosticDescriptor(
        NOT_MONOBEHAVIOUR_DIAGNOSTIC_ID,
        NOT_MONOBEHAVIOUR_TITLE,
        NOT_MONOBEHAVIOUR_MESSAGE_FORMAT,
        CATEGORY,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: NOT_MONOBEHAVIOUR_DESCRIPTION
    );

    private static readonly DiagnosticDescriptor s_nonPrimitiveParameterRule = new DiagnosticDescriptor(
        NON_PRIMITIVE_PARAMETER_DIAGNOSTIC_ID,
        NON_PRIMITIVE_PARAMETER_TITLE,
        NON_PRIMITIVE_PARAMETER_MESSAGE_FORMAT,
        CATEGORY,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: NON_PRIMITIVE_PARAMETER_DESCRIPTION);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(s_notMonoBehaviourRule, s_nonPrimitiveParameterRule);

    public override void Initialize(AnalysisContext context) {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
    }

    private void AnalyzeMethod(SymbolAnalysisContext context) {
      var methodSymbol = (IMethodSymbol)context.Symbol;

      // Check if the method has ConsoleCommandAttribute
      bool hasConsoleCommandAttribute = methodSymbol.GetAttributes()
          .Any(attr => attr.AttributeClass?.Name == ATTRIBUTE_NAME
                       && attr.AttributeClass
                       .ContainingNamespace?
                       .ToDisplayString() == ATTRIBUTE_NAMESPACE
          );

      if (!hasConsoleCommandAttribute) {
        return;
      }

      // Check 1: Verify the containing type inherits from MonoBehaviour
      INamedTypeSymbol containingType = methodSymbol.ContainingType;
      if (!InheritsFromMonoBehaviour(containingType, context.Compilation)) {
        var diagnostic = Diagnostic.Create(
            s_notMonoBehaviourRule,
            methodSymbol.Locations[0],
            methodSymbol.Name);
        context.ReportDiagnostic(diagnostic);
      }

      // Check 2: Verify all parameters are primitive types
      foreach (IParameterSymbol parameter in methodSymbol.Parameters) {
        if (!IsPrimitiveType(parameter.Type)) {
          var diagnostic = Diagnostic.Create(
              s_nonPrimitiveParameterRule,
              parameter.Locations[0],
              methodSymbol.Name,
              parameter.Name,
              parameter.Type.ToDisplayString());
          context.ReportDiagnostic(diagnostic);
        }
      }
    }

    private bool InheritsFromMonoBehaviour(INamedTypeSymbol typeSymbol, Compilation compilation) {
      INamedTypeSymbol currentType = typeSymbol;

      while (currentType != null) {
        if (currentType.Name == "MonoBehaviour"
            && currentType.ContainingNamespace?.ToDisplayString() == "UnityEngine") {
          return true;
        }
        currentType = currentType.BaseType;
      }

      return false;
    }

    private bool IsPrimitiveType(ITypeSymbol typeSymbol) {
      if (typeSymbol.SpecialType != SpecialType.None) {
        // Check for primitive special types
        return typeSymbol.SpecialType == SpecialType.System_Boolean ||
               typeSymbol.SpecialType == SpecialType.System_Byte ||
               typeSymbol.SpecialType == SpecialType.System_SByte ||
               typeSymbol.SpecialType == SpecialType.System_Char ||
               typeSymbol.SpecialType == SpecialType.System_Decimal ||
               typeSymbol.SpecialType == SpecialType.System_Double ||
               typeSymbol.SpecialType == SpecialType.System_Single ||
               typeSymbol.SpecialType == SpecialType.System_Int32 ||
               typeSymbol.SpecialType == SpecialType.System_UInt32 ||
               typeSymbol.SpecialType == SpecialType.System_Int64 ||
               typeSymbol.SpecialType == SpecialType.System_UInt64 ||
               typeSymbol.SpecialType == SpecialType.System_Int16 ||
               typeSymbol.SpecialType == SpecialType.System_UInt16 ||
               typeSymbol.SpecialType == SpecialType.System_String;
      }

      return false;
    }
  }
}
