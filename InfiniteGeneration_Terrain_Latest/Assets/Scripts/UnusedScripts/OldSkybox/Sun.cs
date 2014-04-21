/// <summary>
/// Sun.cs
/// Alex Luk
/// 20/02/2013
/// 
/// This class handles the brightness of the lens flare to stimulate more of a realistic effect
/// </summary>

using UnityEngine;
using System.Collections;

//Customize the Unity menus
[AddComponentMenu("Environment/Sun")]

public class Sun : MonoBehaviour {
	
	public float maxLightBrightness;
	public float minLightBrightness;
	
	public float maxFlareBrightness;
	public float minFlareBrightness;
	
	public bool giveLight = false;
	
	
	//Check to see if there is a light component, otherwise turn it on
	void Start(){
		if(GetComponent<Light>() != null){
			giveLight = true;
		}
	}
}
