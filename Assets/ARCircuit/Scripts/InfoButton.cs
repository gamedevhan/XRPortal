using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButton : MonoBehaviour, ITouchable
{
	public DriverInfoUI DriverInfoUI;	

    public void OnTouch()
    {		
		DriverInfo driverInfo = GetComponent<DriverInfo>();
		string[] driverInfoArray = driverInfo.driverInfoArray;

		DriverInfoUI.gameObject.SetActive(true);
		DriverInfoUI.UpdateDriverInfo(driverInfoArray);
    }
}
