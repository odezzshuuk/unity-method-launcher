namespace Synaptafin.PlayModeConsole.Analyzers {
  public class Constants {

    internal const string ATTRIBUTE_NAME = "ConsoleCommandAttribute";
    internal const string ATTRIBUTE_NAMESPACE = "Synaptafin.PlayModeConsole";

    internal const string NOT_MONOBEHAVIOUR_DIAGNOSTIC_ID = "CC01";
    internal const string NOT_MONOBEHAVIOUR_TITLE = "ConsoleCommandAttribute can only be applied to MonoBehaviour methods";
    internal const string NOT_MONOBEHAVIOUR_MESSAGE_FORMAT = "Method '{0}' with [ConsoleCommand] attribute must be a member of a class that inherits from MonoBehaviour";
    internal const string NOT_MONOBEHAVIOUR_DESCRIPTION = "ConsoleCommandAttribute can only be used on methods that are members of MonoBehaviour classes.";
    internal const string CATEGORY = "Usage";

    internal const string NON_PRIMITIVE_PARAMETER_DIAGNOSTIC_ID = "CC02";
    internal const string NON_PRIMITIVE_PARAMETER_TITLE = "ConsoleCommandAttribute methods must have primitive type parameters";
    internal const string NON_PRIMITIVE_PARAMETER_MESSAGE_FORMAT = "Method '{0}' with [ConsoleCommand] attribute has parameter '{1}' of non-primitive type '{2}'";
    internal const string NON_PRIMITIVE_PARAMETER_DESCRIPTION = "Methods decorated with ConsoleCommandAttribute can only have parameters of primitive types.";

    internal const string PRIVATE_UNUSED_MEMBER_SUPPRESSION_ID = "CC001";
    internal const string UNUSED_MEMBER_DIAGNOSTIC_ID = "IDE0051";
  }
}
