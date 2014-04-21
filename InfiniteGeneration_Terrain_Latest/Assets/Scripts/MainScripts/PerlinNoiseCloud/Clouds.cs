//-----------------------------------------------------------------------------
//
// Title: 		Clouds
// Creator: 	FKM1900
// Based on: 	-
//
//-----------------------------------------------------------------------------


using UnityEngine;
using System.Collections;

public class Clouds : MonoBehaviour {
	
	private Texture2D texture;
	private Texture2D texture2;

	private Vector2 offset;
	private Vector2 tiling;
	
	public int width = 128;
	public int height = 128;
	
	public Color cloudColor = Color.white;
	
	public float scale = 5f;
	public int octaves = 6;
	public float persistence = 0.6f;
	public int seed = 0;
	
	public float contrastLow = 0f;
	public float contrastHigh = 1f;
	public float brightnessOffset = 0f;
	
	public float xSpeed = 0.001f;
	public float ySpeed = 0.0005f;
	
	// Start
	void Start () {

		offset = new Vector2 (1, 1);
		tiling = new Vector2 (1, 1);
	
		texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
		texture2 = new Texture2D(width, height, TextureFormat.ARGB32, false);
		renderer.material.mainTexture = texture;
		renderer.material.SetTexture ("_Texture2", texture2);
		GenerateCloudNoise(texture, width, height, seed, octaves, persistence);
		GenerateCloudNoise(texture2, width, height, 1, 8, persistence);

	}
	
	// Update
	void Update () {
				
		renderer.material.mainTextureOffset = new Vector2(renderer.material.mainTextureOffset.x + xSpeed, renderer.material.mainTextureOffset.y + ySpeed);
		renderer.material.SetTextureOffset("_Texture2" ,new Vector2(renderer.material.mainTextureOffset.x + xSpeed, renderer.material.mainTextureOffset.y + ySpeed));

		//CrossFade bt = gameObject.GetComponent<CrossFade>();
		//bt.CrossFadeTo( texture2, offset, tiling );
	}
	
	// Generate cloud noise
	public Texture GenerateCloudNoise(Texture2D texture, int noiseWidth, int noiseHeight, int seed, int octaves, float persistence) {
	
		float[,] perlinNoise = PerlinNoiseCloud.GeneratePerlinNoise(seed, octaves, persistence, noiseWidth, noiseHeight);
		float noiseValue;
		
		for(int y = 0; y < noiseWidth; y++) {
			
			for(int x = 0; x < noiseHeight; x++) {
				
				noiseValue = perlinNoise[x, y];
				noiseValue *= SimplexNoise.SeamlessNoise((float) x / (float) width, (float) y / (float) height, scale, scale, 0f);

				noiseValue = Mathf.Clamp(noiseValue, contrastLow, contrastHigh + contrastLow) - contrastLow;
				noiseValue = Mathf.Clamp(noiseValue, 0f, 1f);
				
				float r = Mathf.Clamp(cloudColor.r + brightnessOffset, 0f, 1f);
				float g = Mathf.Clamp(cloudColor.g + brightnessOffset, 0f, 1f);
				float b = Mathf.Clamp(cloudColor.b + brightnessOffset, 0f, 1f);
				
				texture.SetPixel(x, y, new Color(r, g, b, noiseValue));
			}
		}
		
		texture.Apply();

		return texture;
	}
}
