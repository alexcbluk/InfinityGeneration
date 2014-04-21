using UnityEngine;
using System.Collections;

public class TimedLighting : MonoBehaviour {

	public void OnEnable(){
		Messenger<bool>.AddListener("Morning Light Time",OnToggleLight);
	}

	public void OnDisable(){
		Messenger<bool>.RemoveListener("Morning Light Time",OnToggleLight);
	}

	void OnToggleLight(bool b){
		if(b){
			GetComponent<Light>().enabled = false;
		} else {
			GetComponent<Light>().enabled = true;
		}
	}
}
