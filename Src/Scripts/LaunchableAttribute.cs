using System;

namespace Odezzshuuk.Workflow.MethodLauncher {

  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
  public class LaunchableAttribute : Attribute {
    public string Id { get; }
    public string Group { get; }
    public string Description { get; }

    public LaunchableAttribute(string id = "abc", string group = "all", string description = "") {
      Id = id;
      Group = group;
      Description = description;
    }
  }


}
