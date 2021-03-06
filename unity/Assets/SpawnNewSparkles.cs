using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNewSparkles : MonoBehaviour {
  public Transform sparklePrefab;
  public Dictionary<UInt32, Transform> sparkles;

  void Start () {
    sparkles = new Dictionary<UInt32, Transform>();

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
    FFI.Update update = FFI.readNextUpdate();

    Debug.Log(string.Format("Update: {0} {1} {2}", update.type, update.id, update.position));

    switch (update.type) {
      case FFI.UpdateType.Connect:
        sparkles.Add(update.id, Instantiate(sparklePrefab));
        break;
      case FFI.UpdateType.Disconnect:
        Destroy(sparkles[update.id].gameObject);
        sparkles.Remove(update.id);
        break;
      case FFI.UpdateType.Position:
        sparkles[update.id].position = update.position;
        break;
      default:
        break;
    }
  }
}
