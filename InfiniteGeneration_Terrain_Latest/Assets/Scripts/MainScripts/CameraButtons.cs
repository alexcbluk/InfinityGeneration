using UnityEngine;
using System.Collections;

public class CameraButtons : MonoBehaviour {
	public GameObject mainCamera;
	public GameObject fixedCamera;

	void OnGUI () {
		// Make a background box
		GUI.Box(new Rect(Screen.width*0.3f,Screen.height*0.9f ,Screen.width*0.55f,50), "Cameras");
		
		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(Screen.width*0.32f,Screen.height*0.94f,80,20), "Main")) {
			mainCamera.SetActive(true);
			fixedCamera.SetActive(false);
		}
		
		// Make the second button.
		if(GUI.Button(new Rect(Screen.width*0.43f,Screen.height*0.94f,80,20), "Fixed 1")) {
			mainCamera.SetActive(false);
			fixedCamera.SetActive(true);
			fixedCamera.transform.position = new Vector3(100f, 65.6f, 1698f);
			fixedCamera.transform.eulerAngles = new Vector3(13f, 142.881f, 0);
		}
		if(GUI.Button(new Rect(Screen.width*0.51f,Screen.height*0.94f,80,20), "Fixed 2")) {
			mainCamera.SetActive(false);
			fixedCamera.SetActive(true);
			fixedCamera.transform.position = new Vector3(36.385f, 31.9776f, 961.225f);
			fixedCamera.transform.eulerAngles = new Vector3(8.00115f, 70.6969f, 4.31083f);
		}
		if(GUI.Button(new Rect(Screen.width*0.59f,Screen.height*0.94f,80,20), "Fixed 3")) {
			mainCamera.SetActive(false);
			fixedCamera.SetActive(true);
			fixedCamera.transform.position = new Vector3(955.977f, 292.727f, 74.8501f);
			fixedCamera.transform.eulerAngles = new Vector3(48.5020f, 12.4486f, 6.44267f);
		}
		if(GUI.Button(new Rect(Screen.width*0.67f,Screen.height*0.94f,80,20), "Fixed 4")) {
			mainCamera.SetActive(false);
			fixedCamera.SetActive(true);
			fixedCamera.transform.position = new Vector3(1274.58f, 53.8f, 474f);
			fixedCamera.transform.eulerAngles = new Vector3(358.002f, 351.6f, 3.33708f);
		}
		if(GUI.Button(new Rect(Screen.width*0.75f,Screen.height*0.94f,80,20), "Fixed 5")) {
			mainCamera.SetActive(false);
			fixedCamera.SetActive(true);
			fixedCamera.transform.position = new Vector3(1652.78f, 119.910f, 1216.33f);
			fixedCamera.transform.eulerAngles = new Vector3(26.7539f, 258.700f, 0.5f);
		}
	}
}
