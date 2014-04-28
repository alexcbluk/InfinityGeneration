using UnityEngine;
using System.Collections;

public class CrossFade : MonoBehaviour
{
	private Texture    newTexture;
	private Vector2    newOffset;
	private Vector2    newTiling;
	
	private  float    BlendSpeed = 0.15f;
	
	private bool    trigger = false;
	private float    fader = 0f;

	private Clouds clouds;
	
	void Start ()
	{
		renderer.material.SetFloat( "_Blend", 0f );
	}
	
	void Update ()
	{
		if ( true == trigger )
		{
			fader += Time.deltaTime * BlendSpeed;
			
			renderer.material.SetFloat( "_Blend", fader );
			
			if ( fader >= 1.0f )
			{
				trigger = false;
				fader = 0f;
				
				renderer.material.SetTexture ("_MainTex", newTexture );
				renderer.material.SetTextureOffset ( "_MainTex", newOffset );
				renderer.material.SetTextureScale ( "_MainTex", newTiling );
				renderer.material.SetFloat( "_Blend", 0f );
			}
		}
	}
	
	public void CrossFadeTo( Texture curTexture, Vector2 offset, Vector2 tiling )
	{
		newOffset = offset;
		newTiling = tiling;
		newTexture = curTexture;
		renderer.material.SetTexture( "_Texture2", curTexture );
		renderer.material.SetTextureOffset ( "_Texture2", newOffset );
		renderer.material.SetTextureScale ( "_Texture2", newTiling );
		trigger = true;
	}
}