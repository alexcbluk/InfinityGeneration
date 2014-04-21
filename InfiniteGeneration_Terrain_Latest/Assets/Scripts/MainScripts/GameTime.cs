using UnityEngine;
using System.Collections;

/// <summary>
/// currentTime is the variable that keeps track of the overall game time. It's value ranges from 0 ~ 1. 
/// TimeOfDay reflects the "currentTime" variable by half. This indicates one transition stage. E.g: Morning to Day Time.
/// TOD is the variable that the script keeps track in order to correctly ouput the lighting of the scene and blending of the skyboxes
/// 
/// To match the sky sphere, you should primarily be comparing the "TOD" variable.
/// </summary>

public class GameTime : MonoBehaviour {

	[Range(0f,1.0f)]
	public float currentTime;
	public float TimeOfDay;
	public float Hour;
	public float TOD;
	public GUIText displayTime;
	private string status;

	public Light sun;
	public float sunLightIntensity = 1.7f;
	public int speed = 50;
	 
	public Color NightFogColor;
	public Color DuskFogColor;
	public Color MorningFogColor;
	public Color MiddayFogColor;
	 
	public Color NightAmbientLight;
	public Color DuskAmbientLight;
	public Color MorningAmbientLight;
	public Color MiddayAmbientLight;
	 
	public Color NightTint;
	public Color DuskTint;
	public Color MorningTint;
	public Color MiddayTint;
	 
	public Material SkyBoxNightToMorning;
	public Material SkyBoxMorningToMidday;
	 
	public Color SunNight;
	public Color SunDay;

	void OnGUI () {
		Hour = currentTime * 24;
		TOD = TimeOfDay * 24;
		sun.transform.eulerAngles = new Vector3((currentTime * 360) - 90, 0, 0);
		currentTime = currentTime + Time.deltaTime / speed;
		sun.color = Color.Lerp (SunNight, SunDay, currentTime * 2);

		//When it reaches the end of the cycle, reset the hour back to 0
		if(currentTime >= 1.0){ currentTime = 0; }
		//Increment
		if(currentTime < 0.5){	TimeOfDay = currentTime;		}
		//Decrement
		if(currentTime > 0.5){	TimeOfDay = (1-currentTime);	}
		//Bring the TimeOfDay back to positive again
		if(currentTime <= 0.0){ TimeOfDay = currentTime; 		}

		//Ambient light, this is reflected by the Sun light intensity
		sun.intensity = (TimeOfDay - 0.2f) * sunLightIntensity;
		
		//Transition to Night Time Setting
		if(TOD<4){
			RenderSettings.skybox = SkyBoxNightToMorning;
			RenderSettings.skybox.SetFloat("_Blend", 0);
			SkyBoxNightToMorning.SetColor ("_Tint", NightTint);
			RenderSettings.ambientLight = NightAmbientLight;
			RenderSettings.fogColor = NightFogColor;

			Messenger<bool>.Broadcast("Morning Light Time", false);
			status = "Night";
			//Debug.Log("Night Time");
		}
		
		//Transition to Sun Set Setting
		if(TOD > 4 && TOD <= 6){
			RenderSettings.skybox = SkyBoxNightToMorning;
			RenderSettings.skybox.SetFloat("_Blend", 0);
			RenderSettings.skybox.SetFloat("_Blend", (TOD/2)-2);
			SkyBoxNightToMorning.SetColor ("_Tint", Color.Lerp (NightTint, DuskTint, (TOD/2)-2) );
			RenderSettings.ambientLight = Color.Lerp (NightAmbientLight, DuskAmbientLight, (TOD/2)-2);
			RenderSettings.fogColor = Color.Lerp (NightFogColor,DuskFogColor, (TOD/2)-2);

			status = "Sun Set";
			//Debug.Log("Sun Set");			
		}
		
		//Transition to Morning Setting
		if(TOD > 6 && TOD < 8){
			RenderSettings.skybox = SkyBoxMorningToMidday;
			RenderSettings.skybox.SetFloat("_Blend", 0);
			RenderSettings.skybox.SetFloat("_Blend", (TOD/2)-3);
			SkyBoxMorningToMidday.SetColor ("_Tint", Color.Lerp (DuskTint,MorningTint,  (TOD/2)-3) );
			RenderSettings.ambientLight = Color.Lerp (DuskAmbientLight, MorningAmbientLight, (TOD/2)-3);
			RenderSettings.fogColor = Color.Lerp (DuskFogColor,MorningFogColor, (TOD/2)-3);

			Messenger<bool>.Broadcast("Morning Light Time", true);
			status = "Morning";
			//Debug.Log("Morning Time");
		}
		
		//Transition to Day Time Setting
		if(TOD > 8 && TOD < 10){
			RenderSettings.ambientLight = MiddayAmbientLight;
			RenderSettings.skybox=SkyBoxMorningToMidday;
			RenderSettings.skybox.SetFloat("_Blend", 1);
			SkyBoxMorningToMidday.SetColor ("_Tint", Color.Lerp (MorningTint,MiddayTint,  (TOD/2)-4) );
			RenderSettings.ambientLight = Color.Lerp (MorningAmbientLight, MiddayAmbientLight, (TOD/2)-4);
			RenderSettings.fogColor = Color.Lerp (MorningFogColor,MiddayFogColor, (TOD/2)-4);
			//Debug.Log("Day Time");

			status = "Day Time";
		}

		//GUI Text:
		displayTime.text = "Time: " + (int)Hour + ":00" + "  Setting: " + status;

		//Keypress INPUT
		if(Input.GetKeyDown(KeyCode.Z)){
			currentTime = 0.3f;
			Debug.Log("Day Time");
		} else if(Input.GetKeyDown(KeyCode.X)){
			currentTime = 0.7f;
			Debug.Log("Day Time");
		}
	}
}
