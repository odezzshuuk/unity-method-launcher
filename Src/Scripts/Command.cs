using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Synaptafin.PlayModeConsole {

  public abstract class CommandBase {
    protected MethodInfo _methodInfo;
    protected object _instance;
    protected string _id;
    protected string _group;
    protected bool _isMemberOfMonoBehaviour;
  }

  public class Command : CommandBase, IEquatable<Command> {

    public string Id => _id;
    public int ParamCount { get; set; }
    public Type[] ParamTypes { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string TargetName {
      get {
        return _isMemberOfMonoBehaviour && _instance is MonoBehaviour mb
          ? mb.gameObject.name
          : string.Empty;
      }
    }

    public bool IsMonoBehaviour => _isMemberOfMonoBehaviour;

    public string Group {
      get => _group;
      set => _group = value;
    }

    public bool IsStatic => _methodInfo.IsStatic;

    public Command(MethodInfo methodInfo, object instance, string group = "all") {

      _methodInfo = methodInfo;
      _id = $"{methodInfo.DeclaringType?.FullName}.{methodInfo.Name}";

      ParameterInfo[] parameters = methodInfo.GetParameters();

      foreach (ParameterInfo param in parameters) {
        if (!(param.ParameterType.IsPrimitive || param.ParameterType == typeof(string))) {
          throw new ParameterParseException("Unsupported parameter type");
        }
      }

      ParamCount = parameters.Length;
      ParamTypes = parameters.Where(static p => p.ParameterType.IsPrimitive || p.ParameterType == typeof(string)).Select(static p => p.ParameterType).ToArray();

      bool hasLambdaLikeName = methodInfo.Name.Contains("b__") || methodInfo.Name.StartsWith('<');
      bool isTypeCompilerGenerated = methodInfo.DeclaringType != null && methodInfo.DeclaringType.IsDefined(typeof(CompilerGeneratedAttribute), false);
      bool isLambdaExpression = hasLambdaLikeName || isTypeCompilerGenerated;

      Name = methodInfo.Name;
      FullName = isLambdaExpression ? "Lambda Expression" : $"{methodInfo.DeclaringType?.Name}.{methodInfo.Name}";
      _isMemberOfMonoBehaviour = methodInfo.DeclaringType != null && methodInfo.DeclaringType.IsSubclassOf(typeof(MonoBehaviour));
      // MonoBehaviour static member doesn't have a Target, so following check is insufficient
      // _isMemberOfMonoBehaviour = handler.Target is MonoBehaviour;

      _instance = instance;
      _group = group;
    }

    public void Execute(string[] args) {
      if (args.Length != ParamCount) {
        Debug.LogWarning($"Command '{Name}' expects {ParamCount} parameters, but got {args.Length}.");
        return;
      }

      object[] parsedParams = new object[ParamCount];
      for (int i = 0; i < ParamCount; i++) {
        object parsed = ParseParameters(args[i], ParamTypes[i]);
        if (parsed == null && ParamTypes[i].IsValueType) {
          return;
        }
        parsedParams[i] = parsed;
      }

      try {
        object result;
        if (IsStatic) {
          result = _methodInfo.Invoke(null, parsedParams);
        } else {
          result = _methodInfo.Invoke(_instance, parsedParams);
        }
        if (result != null) {
          Debug.Log(result);
        }
      } catch (Exception ex) {
        Debug.LogWarning($"Error executing command '{Name}': {ex.Message}");
      }
    }

    private object ParseParameters(string args, Type paramType) {
      try {
        if (paramType == typeof(string)) {
          return args;
        } else if (paramType == typeof(int)) {
          return int.Parse(args);
        } else if (paramType == typeof(float)) {
          return float.Parse(args);
        } else if (paramType == typeof(bool)) {
          return bool.Parse(args);
        } else if (paramType == typeof(double)) {
          return double.Parse(args);
        } else if (paramType == typeof(long)) {
          return long.Parse(args);
        } else if (paramType == typeof(short)) {
          return short.Parse(args);
        } else if (paramType == typeof(byte)) {
          return byte.Parse(args);
        } else if (paramType == typeof(char)) {
          return char.Parse(args);
        } else if (paramType == typeof(uint)) {
          return uint.Parse(args);
        } else if (paramType == typeof(ulong)) {
          return ulong.Parse(args);
        } else if (paramType == typeof(ushort)) {
          return ushort.Parse(args);
        } else if (paramType == typeof(sbyte)) {
          return sbyte.Parse(args);
        }
      } catch (Exception ex) {
        Debug.LogWarning($"Failed to parse parameter '{args}' to type {paramType}: {ex.Message}");
      }
      return null;
    }

    public bool Equals(Command other) {
      if (other == null) {
        return false;
      }

      // For static methods, only compare MethodInfo
      if (IsStatic) {
        return _methodInfo.Equals(other._methodInfo);
      }

      // For non-static methods, compare both MethodInfo and instance
      return _methodInfo.Equals(other._methodInfo) && Equals(_instance, other._instance);
    }

    public override bool Equals(object obj) {
      if (obj == null || GetType() != obj.GetType()) {
        return false;
      }
      return Equals((Command)obj);
    }

    public override int GetHashCode() {
      // For static methods, use only MethodInfo hash
      if (IsStatic) {
        return _methodInfo.GetHashCode();
      }

      // For non-static methods, combine MethodInfo and instance hash codes
      unchecked {
        int hash = 17;
        hash = hash * 31 + (_methodInfo != null ? _methodInfo.GetHashCode() : 0);
        hash = hash * 31 + (_instance != null ? _instance.GetHashCode() : 0);
        return hash;
      }
    }
  }

  public class ParameterParseException : Exception {
    public ParameterParseException(string message) : base(message) { }
  }
}
