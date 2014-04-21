using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {
	//public CrossFade bt;
	private Texture curTexture;
	private Vector2 offset;
	private Vector2 tiling;

	private Clouds clouds;

	// Use this for initialization
	void Start () {
		//curTexture = new Texture2D(512, 512, TextureFormat.ARGB32, false);

		//curTexture = clouds.GenerateCloudNoise (512, 512, 0, 8, 0.6f);
		//renderer.material.SetTexture("_Texture2", curTexture);
		//offset = new Vector2 (1, 1);
		//tiling = new Vector2 (1, 1);
	
	
	}
	
	// Update is called once per frame
	void Update () {

		//CrossFade bt = gameObject.GetComponent<CrossFade>();
		//bt.CrossFadeTo( curTexture, offset, tiling );
	
	}
}
