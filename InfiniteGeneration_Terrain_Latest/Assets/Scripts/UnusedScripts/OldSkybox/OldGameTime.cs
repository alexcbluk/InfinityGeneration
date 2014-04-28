/// <summary>
/// Game time.cs
/// Alex Luk
/// 
/// This script controls the rotation of the sun game object (Directional Light)
/// </summary>
using UnityEngine;
using System.Collections;

//Customize the Unity menus
[AddComponentMenu("Environment/GameTimeBlah")]

public class GameTimeBlah : MonoBehaviour {
	//Different state of a day in the game.
	public enum TimeOfDay{
		Idle,
		SunRise,
		SunSet
	}
	
	public float sunRise;						//This variable is used to assign the enumeration onto a flaot
	public float sunSet;						
	public float skyboxBlendModifier;			//How much to blend the two skybox together.

	public float morningLight;
	public float nightLight; 
	private bool isMorning = false;

	//Ambient
	public Color ambLightMax;
	public Color ambLightMin;

	public Transform[] sun;						//An array to hold all the sun components
	private Sun[] sunScript;					//An array to attach the Sun script to sun components
	
	public float dayCycleInMinutes = 1;
	private float dayCycleInSeconds;
	
	//Variable to keep track of the real world time
	private const float SECOND = 1;
	private const float MINUTE = 60 * SECOND;
	private const float HOUR = 60 * MINUTE;
	private const float DAY = 24 * HOUR;
	
	//Normalise the 360 degree revolution by each second of a day (Turning 240 degree per second)
	private const float DEGREES_PER_SECOND = 360 / DAY;
	
	private float degreeRotation;				//This is the end variable to be use for rotating the sun
	private float timeOfDay;					//This variable is to keep track the time of the day.
	
	private TimeOfDay tod;					//This variable is used to trigger the enumeration variables
	private float noonTime;					//This is the time of day when it is in the afternoon (12:00)
	private float morningLength;
	private float eveningLength;

	// Use this for initialization
	void Start () {
		tod = TimeOfDay.Idle;	//Time of day is set to idle first
		
		dayCycleInSeconds = (dayCycleInMinutes * MINUTE); //Calculating the rotation cycle by seconds
		
		//This grab the skybox material, going to the "_Blend" variable and set its value to 0. 
		//At the most left of the slider, and gradually increase the value to change to night time.
		RenderSettings.skybox.SetFloat("_Blend",0);
		
		InitialiseSunScriptOnToGameObject();
		
		//Start at the beginning
		timeOfDay = 0;	
		//	The range of the rotation:	Degree to turn per second * by the duration (second in a day). 
		// 	The speed of the rotation: Divid by the amount of time needed to cycle one day (Normalise) and move the rotation by each minute.
		degreeRotation = DEGREES_PER_SECOND * DAY / (dayCycleInSeconds);
		
		
		//Everything is run in seconds.
		sunRise *= dayCycleInSeconds;
		sunSet *= dayCycleInSeconds;
		
		noonTime = dayCycleInSeconds / 2;				//Noon time is when the rotation reached half way.
		morningLength = noonTime - sunRise;				//The time length of morning
		eveningLength = sunSet - noonTime;				//The time length of evening
		morningLight *= dayCycleInSeconds;
		nightLight *= dayCycleInSeconds;


		//Setup the lighting to minimum light value.
		SetupLighting();
	}
	
	// Update is called once per frame
	void Update () {
		sun[0].Rotate(new Vector3(degreeRotation,0, 0) * Time.deltaTime);
		
		//How long it took from this frame to the next frame
		timeOfDay += Time.deltaTime;
		//Debug.Log(timeOfDay);

		//Controls the outside lighting
		if(!isMorning && timeOfDay > morningLight && timeOfDay < nightLight){
			isMorning = true;
			Messenger<bool>.Broadcast("Morning Light Time", true);
			Debug.Log("Morning");
		} else if(isMorning && timeOfDay > nightLight){
			isMorning = false;
			Messenger<bool>.Broadcast("Morning Light Time", false);
			Debug.Log("Evenning");
		}


		//Having the slider move left to right.
		//If it reach the threshold then minus the value
		if(timeOfDay > dayCycleInSeconds){
			timeOfDay -= dayCycleInSeconds;	
		}

		if(timeOfDay > sunRise && timeOfDay < noonTime){
			AdjustLighting(true);
		} else if(timeOfDay > noonTime && timeOfDay < sunSet) {
			AdjustLighting(false);
		}

		//If the blend is not maxed then keep blending
		if(timeOfDay > sunRise && timeOfDay < sunSet && RenderSettings.skybox.GetFloat("_Blend") < 1){
			tod = GameTimeBlah.TimeOfDay.SunRise;
			BlendSkybox();
		} else if(timeOfDay > sunSet && RenderSettings.skybox.GetFloat("_Blend") > 0){
			tod = GameTimeBlah.TimeOfDay.SunSet;
			BlendSkybox();
		} else {
			tod = GameTimeBlah.TimeOfDay.Idle;	
		}
		
		
	}
	
	//This function, iterate through all the game object attached to this array
	//Check to see if the Sun Script is attached onto the game object inside the array
	public void InitialiseSunScriptOnToGameObject(){
		//Inititalise the array of suns
		sunScript = new Sun[sun.Length];
		
		//For loop to attachveach sun gameobject to sun script
		for(int i=0; i<sun.Length; i++){
			Sun temp = sun[i].GetComponent<Sun>();
			
			//House keeping practice
			//Does it exist?
			if(temp == null){
				//Initialise it
				Debug.Log("Sun Script not found. Adding it ");
				//To initialise it, Take the game object within the sun array and add the script "Sun" to it.
				sun[i].gameObject.AddComponent<Sun>();
				temp = sun[i].GetComponent<Sun>();
			}
			sunScript[i] = temp;
		}	
	}
	
	//This function tell us how far the sun have rotate, so that we can point out
	//at 50% start changing the skybox to night time.
	private void BlendSkybox(){
		//Multiple by 2 because we want to go from Day to Night and Night back to Day
		float temp = 0;
		
		//Depending on the state of the day, temp will either be increasing for SunRise and 
		//decreasing for SunSet.
		switch(tod){
			case TimeOfDay.SunRise:
			temp = (timeOfDay - sunRise) / dayCycleInSeconds * skyboxBlendModifier;
				break;
			case TimeOfDay.SunSet:
			//If temp is over 1.2, if we subtract 1 then we have 0.2.
			//since we want to go backward then 1 - 0.2 = 0.8
			temp = (timeOfDay - sunSet) / dayCycleInSeconds * skyboxBlendModifier;
			temp = 1 - temp;
				break;
		}
		
		//In the render setting, set the blend factor as the temp value.
		RenderSettings.skybox.SetFloat("_Blend",temp);
	}

	private void SetupLighting(){
		RenderSettings.ambientLight = ambLightMin;
		for(int i=0; i < sunScript.Length; i++){
			if(sunScript[i].giveLight){
				sun[i].GetComponent<Light>().intensity = sunScript[i].minLightBrightness;
			}
		}
	}

	private void AdjustLighting(bool brighten){
		float pos = 0;
		if(brighten){
			pos = (timeOfDay - sunRise) / morningLength;		//Get the position of the sun in the monring sky
		} else {
			pos = (sunSet - timeOfDay) / eveningLength;		//Get the position of the sun in the evening sky
		}

		RenderSettings.ambientLight = ambLightMax * pos;
		for(int i=0; i<sunScript.Length; i++){
			if(sunScript[i].giveLight){
				sunScript[i].GetComponent<Light>().intensity = sunScript[i].maxLightBrightness * pos;
			}
		}
	}
}
