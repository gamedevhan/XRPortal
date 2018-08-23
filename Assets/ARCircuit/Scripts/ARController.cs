
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class ARController : MonoBehaviour
{
	[SerializeField] private Camera m_firstPersonCamera;	
	[SerializeField] private GameObject m_Portal;
	[SerializeField] private GameObject m_Circuit;	
	[SerializeField] private GameObject m_DetectedPlanePrefab;
	[SerializeField] private GameObject m_SkyBox;
	[SerializeField] private GameObject m_DriverInfoUI;
		
	private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();
	private bool m_IsPortalActive = false;
	private bool m_IsQuitting = false;

	public void Update()
	{
		_UpdateApplicationLifecycle();

		Session.GetTrackables<DetectedPlane>(m_AllPlanes, TrackableQueryFilter.New);

		if (!m_IsPortalActive)
		{
			for (int i = 0; i < m_AllPlanes.Count; i++)
			{
				GameObject grid = Instantiate(m_DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
				grid.GetComponent<DetectedPlaneVisualizer>().Initialize(m_AllPlanes[i]);
			}
		}

		Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

		// Raycast against the location the player touched to search for planes.
		TrackableHit hit;		

		if (Frame.Raycast(touch.position.x, touch.position.y, TrackableHitFlags.PlaneWithinPolygon, out hit))
        {			
			if (!m_Portal.activeInHierarchy)
			{
				Vector3 portalPosition = new Vector3(hit.Pose.position.x, 0, hit.Pose.position.z);
				m_Portal.transform.position = portalPosition;
				Vector3 cameraPosition = m_firstPersonCamera.transform.position;				
				m_Portal.transform.LookAt(new Vector3(cameraPosition.x, portalPosition.y, cameraPosition.z));
				m_Portal.SetActive(true);
				Debug.Log("Spawning Portal at : " + m_Portal.transform.position);

				m_Circuit.SetActive(true);
				m_SkyBox.SetActive(true);
				m_IsPortalActive = true;

				var anchor = hit.Trackable.CreateAnchor(hit.Pose);
				m_Portal.transform.parent = anchor.transform;
			}
		}
	}

	/// <summary>
	/// Check and update the application lifecycle.
	/// </summary>
	private void _UpdateApplicationLifecycle()
	{
		// Only allow the screen to sleep when not tracking.
		if (Session.Status != SessionStatus.Tracking)
		{
			const int lostTrackingSleepTimeout = 15;
			Screen.sleepTimeout = lostTrackingSleepTimeout;
		}
		else
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}

		if (m_IsQuitting)
		{
			return;
		}

		// Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
		if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
		{
			_ShowAndroidToastMessage("Camera permission is needed to run this application.");
			m_IsQuitting = true;
			Invoke("_DoQuit", 0.5f);
		}
		else if (Session.Status.IsError())
		{
			_ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
			m_IsQuitting = true;
			Invoke("_DoQuit", 0.5f);
		}
	}

	/// <summary>
	/// Actually quit the application.
	/// </summary>
	private void _DoQuit()
	{
		Application.Quit();
	}

	/// <summary>
	/// Show an Android toast message.
	/// </summary>
	/// <param name="message">Message string to show in the toast.</param>
	private void _ShowAndroidToastMessage(string message)
	{
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

		if (unityActivity != null)
		{
			AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
			unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			{
				AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
					message, 0);
				toastObject.Call("show");
			}));
		}
	}
}