using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNewSparkles : MonoBehaviour {
  void Start () {
    FFI.connectToServer("0.0.0.0:12345");
    Debug.Log("Start");
  }

  void OnDestroy () {
    FFI.disconnectFromServer();
    Debug.Log("Destroy");
  }

  void Update () {
    if (Input.GetMouseButton(0)) {
      Debug.Log("DING");
      FFI.sendDing();
    }
  }
}
