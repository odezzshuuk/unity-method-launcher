using System;
using System.Linq;
using Odezzshuuk.Workflow.MethodLauncher;

namespace UnityEngine.UIElements {

  [UxmlElement("LaunchableItem")]
  public partial class LaunchableItemElement : VisualElement {

    private readonly Label _commandName;
    private readonly Label _commandDescription;
    private readonly Label _commandTarget;

    public event Action<Vector2> OnHover;
    public event Action OnLeave;

    public Command Command { get; private set; }
    public string CommandName => _commandName.text;
    public string CommandDetail { get; set; }

    public LaunchableItemElement() { }
    public LaunchableItemElement(VisualElement root) : this() {
      Add(root);
      _commandName = root.Q<Label>("name");
      _commandDescription = root.Q<Label>("description");
      _commandTarget = root.Q<Label>("target");

      OnHoverManipulator hoverManipulator = new(this);
      hoverManipulator.OnHover += pos => {
        OnHover?.Invoke(pos);
      };
      hoverManipulator.OnLeave += () => OnLeave?.Invoke();
      this.AddManipulator(hoverManipulator);
    }

    public void SetData(Command command) {
      Command = command;
      _commandName.text = command.Name;
      _commandDescription.text = command.FullName;
      _commandTarget.text = command.TargetName;

      if (command.ParamTypes == null) {
        Debug.Log($"Command {command.Name} ParamTypes is null");
      }

      CommandDetail = $"Parameters: {string.Join(", ", command.ParamTypes.Select(static t => t.Name))}\n"
        + $"Group: {command.Group}\n"
        + $"IsMonoBehaviour: {command.IsMonoBehaviour}\n"
        + (command.IsMonoBehaviour ? $"GameObject: {command.TargetName}" : string.Empty);
    }
  }
}
