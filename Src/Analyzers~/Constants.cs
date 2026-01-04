namespace Odezzshuuk.Workflow.MethodLauncher.Analyzers {
  public class Constants {

    internal const string ATTRIBUTE_NAME = "LaunchableAttribute";
    internal const string ATTRIBUTE_NAMESPACE = "Odezzshuuk.Workflow.MethodLauncher";

    internal const string NOT_MONOBEHAVIOUR_DIAGNOSTIC_ID = "LC01";
    internal const string NOT_MONOBEHAVIOUR_TITLE = "LaunchableAttribute can only be applied to MonoBehaviour methods";
    internal const string NOT_MONOBEHAVIOUR_MESSAGE_FORMAT = "Method '{0}' with [Launchable] attribute must be a member of a class that inherits from MonoBehaviour";
    internal const string NOT_MONOBEHAVIOUR_DESCRIPTION = "LaunchableAttribute can only be used on methods that are members of MonoBehaviour classes.";
    internal const string CATEGORY = "Usage";

    internal const string NON_PRIMITIVE_PARAMETER_DIAGNOSTIC_ID = "LC02";
    internal const string NON_PRIMITIVE_PARAMETER_TITLE = "LaunchableAttribute  methods must have primitive type parameters";
    internal const string NON_PRIMITIVE_PARAMETER_MESSAGE_FORMAT = "Method '{0}' with [Launchable] attribute has parameter '{1}' of non-primitive type '{2}'";
    internal const string NON_PRIMITIVE_PARAMETER_DESCRIPTION = "Methods decorated with LaunchableAttribute can only have parameters of primitive types.";

    internal const string PRIVATE_UNUSED_MEMBER_SUPPRESSION_ID = "LC001";
    internal const string UNUSED_MEMBER_DIAGNOSTIC_ID = "IDE0051";
  }
}
