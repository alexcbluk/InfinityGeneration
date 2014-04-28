using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {

	// Texture Array for the Clouds
	private Texture2D[] textures = new Texture2D[8];

	private Vector2 offset;
	private Vector2 tiling;

	// Texture's Height and Width
	private int width = 512;
	private int height = 512;

	// Perlin Noise Variables
	private float scale = 10f;
	private int octaves = 6;
	private float persistence = 0.9f;
	private int seed = 0;

	// Cloud Contrast And Brightness Offset Variables
	private float contrastLow = 0f;
	private float contrastHigh = 1f;
	private float brightnessOffset = 0f;

	// Scrolling Speed of the Cloud Textures
	private float xSpeed = 0.001f;
	private float ySpeed = 0.001f;

	// Rate transition change between texture 1 and 2
	private float BlendSpeed = 0.1f;
	// Starting at Texture 1
	private float fader = 0f;

	// Cloud initial color
	private Color color;

	// Whether to change the texture or not
	private bool changingTextures;

	// The clouds are raining or not
	public bool raining;
	public int rainLevel = 0;

	// Which texture to start
	private int startingTextureIndex;

	// Random Generator for changing cloud textures
	private float randomGen;

	// GameTime Script
	private GameTime gameTime;
	private float timeOfDay;

	// Colors from GameTime Script
	private Color nightFogColor;
	private Color duskFogColor;
	private Color morningFogColor;
	private Color middayFogColor;

	// Start
	void Start () {
		gameTime = GameObject.Find("GameTime").GetComponent<GameTime>();

		// Get the Clouds' Colors
		nightFogColor = gameTime.NightFogColor;
		duskFogColor = gameTime.DuskFogColor;
		morningFogColor = gameTime.MorningFogColor;
		middayFogColor = gameTime.MiddayFogColor;

		offset = new Vector2 (1, 1);
		tiling = new Vector2 (1, 1);

		// Starting Texture
		startingTextureIndex = 4;
		changingTextures = false;

		// Set the cloud's color
		color = renderer.material.color;
		color = new Color (0.1f, 0.1f, 0.1f, 1f);

		// Generates 8 clouds textures each with different perlin noise value and stores it in a texture array
		for (int i = 0; i < 8; i++) {
			textures[i] = new Texture2D(width, height, TextureFormat.ARGB32, false);
			switch(i){
			case 0:
				octaves = 10;
				persistence = 0.6f;
				contrastLow = 0f;
				contrastHigh = 1f;
				break;
			case 1:
				octaves = 8;
				persistence = 0.6f;
				contrastLow = 0.1f;
				contrastHigh = 1f;
				break;
			case 2:
				octaves = 7;
				persistence = 0.8f;
				contrastLow = 0.25f;
				contrastHigh = 1f;
				break;
			case 3:
				octaves = 8;
				persistence = 0.8f;
				contrastLow = 0.4f;
				contrastHigh = 1f;
				break;
			case 4:
				octaves = 8;
				persistence = 0.8f;
				contrastLow = 0.5f;
				contrastHigh = 1f;
				break;
			case 5:
				octaves = 8;
				persistence = 0.7f;
				contrastLow = 0.6f;
				contrastHigh = 1f;
				break;
			case 6:
				octaves = 8;
				persistence = 0.7f;
				contrastLow = 0.7f;
				contrastHigh = 1f;
				break;
			case 7:
				octaves = 8;
				persistence = 0.7f;
				contrastLow = 0.8f;
				contrastHigh = 1f;
				break;
			}
			GenerateCloudNoise(textures[i], width, height, seed, octaves, persistence);
		}
		// Set the cloud materials and textures
		renderer.material.mainTexture = textures[startingTextureIndex];
		renderer.material.SetTexture ("_Texture2", textures[startingTextureIndex]);	
		renderer.material.SetFloat( "_Blend", 0f );
	}
	
	// Update
	void Update () {
		// GameTime's TimeOfDay
		timeOfDay = gameTime.TOD;

		// Update the two textures with scrolling speed and position
		renderer.material.mainTextureOffset = new Vector2(renderer.material.mainTextureOffset.x + xSpeed, renderer.material.mainTextureOffset.y + ySpeed);
		renderer.material.SetTextureOffset("_Texture2" ,new Vector2(renderer.material.mainTextureOffset.x + xSpeed, renderer.material.mainTextureOffset.y + ySpeed));

		// Fade between textures
		fader += Time.deltaTime * BlendSpeed;		
		renderer.material.SetFloat( "_Blend", fader );

		// Clouds' color changes depending on time of day
		if (timeOfDay < 4) {
			renderer.material.SetColor("_Color", nightFogColor);
		}
		if (timeOfDay > 4 && timeOfDay <= 6) {
			renderer.material.SetColor("_Color", Color.Lerp (nightFogColor, duskFogColor, (timeOfDay/2)-2));
		}
		if (timeOfDay > 6 && timeOfDay < 8) {
			renderer.material.SetColor("_Color", Color.Lerp (duskFogColor, morningFogColor, (timeOfDay/2)-3));
		}
		if (timeOfDay > 8 && timeOfDay < 10) {
			renderer.material.SetColor("_Color", Color.Lerp (morningFogColor, middayFogColor, (timeOfDay/2)-4));
		}

		// If the blend fader reaches max value
		if (fader >= 1.0f) {
			randomGen = Random.Range(0f, 1f);
			GameObject.Find("Clouds2").GetComponent<Clouds>().randomGen = GameObject.Find("Clouds1").GetComponent<Clouds>().randomGen;

			if(changingTextures == false){
				// Decrement texture 2 if random generator is below 20%
				if(randomGen < 0.20f && startingTextureIndex != 0) {
					startingTextureIndex--;
					renderer.material.SetTexture ("_Texture2", textures[startingTextureIndex]);
					changingTextures = true;
				}
				// Decrement texture 2 if random generator is more 80%
				else if (randomGen > 0.80f && startingTextureIndex != 7) {
					startingTextureIndex++;
					renderer.material.SetTexture ("_Texture2", textures[startingTextureIndex]);
					changingTextures = true;
				}
			}

			// Once texture 1 is the same as texture 2, reset changingTextures
			else if (changingTextures == true){
				renderer.material.mainTexture = textures[startingTextureIndex];
				changingTextures = false;
			}

			// The first 3 cloud textures are rain and have different intensity
			if(renderer.material.mainTexture == textures[2]) {
				raining = true;
				rainLevel = 1;
			}
			else if(renderer.material.mainTexture == textures[1]) {
				raining = true;
				rainLevel = 2;
			}
			else if(renderer.material.mainTexture == textures[0]) {
				raining = true;
				rainLevel = 3;
			}
			else {
				raining = false;
			}

			// Set the fade value from 1 to 0
			fader = 0f;
		}
	}
	
	// Generate cloud noise
	public Texture GenerateCloudNoise(Texture2D texture, int noiseWidth, int noiseHeight, int seed, int octaves, float persistence) {
	
		float[,] perlinNoise = PerlinNoiseCloud.GeneratePerlinNoise(seed, octaves, persistence, noiseWidth, noiseHeight);
		float noiseValue;
		
		for(int y = 0; y < noiseWidth; y++) {
			
			for(int x = 0; x < noiseHeight; x++) {
				
				noiseValue = perlinNoise[x, y];

				noiseValue = Mathf.Clamp(noiseValue, contrastLow, contrastHigh + contrastLow) - contrastLow;
				noiseValue = Mathf.Clamp(noiseValue, 0f, 1f);

				if(noiseValue > 0.0f && this.gameObject.name == "Clouds2"){
					color = new Color (1f, 1f, 1f, 1f);
					noiseValue = 1f;
				}

				else if (noiseValue < 0.0f && this.gameObject.name == "Clouds2") {
					color = new Color (1f, 1f, 1f, 1f);
					noiseValue = 0f;
				}

				float r = Mathf.Clamp(color.r + brightnessOffset, 0f, 1f);
				float g = Mathf.Clamp(color.g + brightnessOffset, 0f, 1f);
				float b = Mathf.Clamp(color.b + brightnessOffset, 0f, 1f);
				
				texture.SetPixel(x, y, new Color(r, g, b, noiseValue));
			}
		}
		
		texture.Apply();

		return texture;
	}
}
