using System;

namespace Synaptafin.PlayModeConsole {

  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
  public class ConsoleCommandAttribute : Attribute {
    public string Id { get; }
    public string Group { get; }
    public string Description { get; }

    public ConsoleCommandAttribute(string id = "abc", string group = "all", string description = "") {
      Id = id;
      Group = group;
      Description = description;
    }
  }

  // public class ConsoleCommandAttributeSyntaxReceiver : ISyntaxReceiver {
  //   public List<MethodDeclarationSyntax> CandidateMethods { get; } = new();
  //
  //   public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
  //     if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax) {
  //       foreach (var attributeList in methodDeclarationSyntax.AttributeLists) {
  //         foreach (var attribute in attributeList.Attributes) {
  //           var name = attribute.Name.ToString();
  //           if (name == "ConsoleCommand" || name == "ConsoleCommandAttribute") {
  //             CandidateMethods.Add(methodDeclarationSyntax);
  //           }
  //         }
  //       }
  //     }
  //   }
  // }

}
