using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverInfoUI : MonoBehaviour
{
	public UISprite ProfileImage;
	public UILabel DriverName;
	public UILabel TeamName;
	public UILabel CurrentPosition;
	public UILabel FastestLapTime;
	public UILabel LastLapTime;

	public void UpdateDriverInfo(string[] driverInfoArray)
	{
		ProfileImage.spriteName = driverInfoArray[0];
		DriverName.text = driverInfoArray[1];
		TeamName.text = driverInfoArray[2];
		CurrentPosition.text = driverInfoArray[3];
		FastestLapTime.text = driverInfoArray[4];
		LastLapTime.text = driverInfoArray[5];        
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			gameObject.SetActive(false);
		}
	}
}
