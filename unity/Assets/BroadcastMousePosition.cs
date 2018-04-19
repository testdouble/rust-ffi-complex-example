using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadcastMousePosition : MonoBehaviour {
  void Start () {
  }

  void Update () {
    if (Input.GetMouseButton(0)) {
      Debug.Log(Input.mousePosition);

      Vector3 projectedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      FFI.sendPositionUpdate(projectedPosition);
    }
  }
}
