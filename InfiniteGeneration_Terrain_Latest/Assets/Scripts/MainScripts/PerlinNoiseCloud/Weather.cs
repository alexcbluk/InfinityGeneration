using UnityEngine;
using System.Collections;

public class Weather : MonoBehaviour {

	private bool isRaining;
	private int rainLevelIntensity;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		isRaining = GameObject.Find ("Clouds1").GetComponent<Clouds> ().raining;
		rainLevelIntensity = GameObject.Find ("Clouds1").GetComponent<Clouds> ().rainLevel;

		if (isRaining == true) {
			ParticleSystem particlesystem = (ParticleSystem)gameObject.GetComponent ("ParticleSystem");
			particlesystem.enableEmission = true;

			if (rainLevelIntensity == 1){
				particlesystem.maxParticles = 1500;
				particlesystem.emissionRate = 1500;
			}
			else if (rainLevelIntensity == 2){
				particlesystem.maxParticles = 4000;
				particlesystem.emissionRate = 4000;
			}
			else if (rainLevelIntensity == 3){
				particlesystem.maxParticles = 10000;
				particlesystem.emissionRate = 10000;
			}

		} 
		else {
			ParticleSystem particlesystem = (ParticleSystem)gameObject.GetComponent ("ParticleSystem");
			particlesystem.enableEmission = false;
		}
	}
}
