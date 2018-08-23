using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogger : MonoBehaviour {

	public GameObject ARCamera;
	public GameObject Portal;
	
	// Update is called once per frame
	void Update () {
		if (Portal.activeInHierarchy)
		{
			Debug.Log("Portal located at: " + Portal.transform.position + ", CameraPosition: " + ARCamera.transform.position);
		}
	}
}
