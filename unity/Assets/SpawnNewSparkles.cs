using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNewSparkles : MonoBehaviour {
  void Start () {
    FFI.connectToServer("0.0.0.0:12345");
    FFI.sendStatusUpdate(FFI.StatusUpdate.Connect);
    Debug.Log("Start");
  }

  void OnDestroy () {
    FFI.sendStatusUpdate(FFI.StatusUpdate.Disconnect);
    FFI.disconnectFromServer();
    Debug.Log("Destroy");
  }

  void Update () {
    FFI.IncomingUpdate update = FFI.readNextUpdate();

    Debug.Log(string.Format("Update: {0} {1} {2}", update.type, update.x, update.y));
  }
}
