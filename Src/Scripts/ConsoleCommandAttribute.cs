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


}
