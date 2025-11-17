using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Synaptafin.PlayModeConsole {
  public class CommandRegistry : MonoBehaviour {

    public static CommandRegistry Instance { get; private set; }

    public List<Command> Commands { get; private set; }
    public string[] CommandNames => Commands.Select(static c => c.Name).ToArray();

    public void Awake() {
      Commands = new List<Command>();
      if (Instance != null && Instance != this) {
        Destroy(gameObject);
        return;
      }
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }

    public void OnEnable() {
      SceneManager.sceneLoaded += SceneLoadedCallback;
    }

    public async void Start() {
      await RegisterCommandAttributeCommandsAsync();
    }

    public void OnDisable() {
      SceneManager.sceneLoaded -= SceneLoadedCallback;
    }


    public void RegisterCommand(Delegate handler, string name = default, string description = default) {
      RegisterCommand(handler.Method, handler.Target, name, description);
    }

    public void RegisterCommand(Action handler, string name = default, string description = default) {
      RegisterCommand((Delegate)handler, name, description);
    }

    public void RegisterCommand(Action<int> handler, string name = default, string description = default) {
      RegisterCommand((Delegate)handler, name, description);
    }

    public void RegisterCommand(Action<float> handler, string name = default, string description = default) {
      RegisterCommand((Delegate)handler, name, description);
    }

    public void RegisterCommand(Action<string> handler, string name = default, string description = default) {
      RegisterCommand((Delegate)handler, name, description);
    }

    public void RegisterCommand(Action<bool> handler, string name = default, string description = default) {
      RegisterCommand((Delegate)handler, name, description);
    }

    // public void RegisterCommand(Func<IEnumerable<object>> handler, string name = default, string description = default) {
    //   IEnumerator<object> enumerator = handler().GetEnumerator();
    //   RegisterCommand((Delegate)(() => enumerator.MoveNext()), name, description);
    // }

    /// <summary>
    /// register command with any delegate type
    /// </summary>
    public void RegisterCommand<T>(T handler, string name = default, string description = default) where T : Delegate {
      RegisterCommand((Delegate)handler, name, description);
    }

    public Command GetCommandByName(string commandName) {
      return Commands.FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));
    }

    public void RemoveCommand(string name) {
      int num = Commands.RemoveAll(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      if (num > 0) {
        Debug.Log($"Command with id {name} removed.");
      } else {
        Debug.LogWarning($"Command with id {name} not found.");
      }
    }

    private void RegisterCommand(Command command) {
      int index = Commands.FindIndex(c => c.Equals(command));
      if (index >= 0) {
        // update command
        Commands[index] = command;
      } else {
        Commands.Add(command);
      }
    }

    private void RegisterCommand(MethodInfo methodInfo, object instance, string name = default, string description = default) {
      try {
        Command command = new(methodInfo, instance);
        if (!string.IsNullOrEmpty(name)) {
          command.Name = name;
        }
        if (!string.IsNullOrEmpty(description)) {
          command.FullName = description;
        }
        RegisterCommand(command);
      } catch (ParameterParseException ex) {
        Debug.LogWarning($"Failed to register command '{name ?? methodInfo.Name}': {ex.Message}");
      }
    }

    private async Awaitable RegisterCommandAttributeCommandsAsync() {
      await Awaitable.MainThreadAsync();
      MonoBehaviour[] mbs = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
      foreach (MonoBehaviour mb in mbs) {
        Type type = mb.GetType();
        MethodInfo[] methods = type.GetMethods(
            BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Static
        );

        foreach (MethodInfo method in methods) {
          if (Attribute.IsDefined(method, typeof(ConsoleCommandAttribute))) {
            RegisterCommand(method, mb);
          }
        }
      }
    }

    private async void SceneLoadedCallback(Scene scene, LoadSceneMode mode) {
      Commands.Clear();
      await RegisterCommandAttributeCommandsAsync();
    }
  }
}
