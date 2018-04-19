using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizedFlare : MonoBehaviour {
	void Start () {
	}

	void Update () {
		if (Input.GetMouseButton(0)) {
			Vector3 projectedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			gameObject.transform.position = new Vector3(projectedPosition.x, projectedPosition.y, 0);
		}
	}
}
