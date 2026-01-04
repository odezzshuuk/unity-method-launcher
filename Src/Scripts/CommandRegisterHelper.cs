using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Odezzshuuk.Workflow.MethodLauncher {

  public class PlayModeCommandRegisterHelper : MonoBehaviour {
    [SerializeField] private CommandRegistry _playModeCommandRegistry;
    [SerializeField] private GameObject[] _registeredObjects;
  }

}
