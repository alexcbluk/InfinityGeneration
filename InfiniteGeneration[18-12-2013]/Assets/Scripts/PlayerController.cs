using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	//Improvements: Expose the xPos,yPos and zPos to be edited on the editor window
	
	public Transform playerTransform;
	private Transform myTransform;
	
	// Use this for initialization
	void Start () {	
		myTransform = playerTransform; 	//Cache in the transform
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Home)){
			myTransform.position = new Vector3(88, 150, 270);
		}
	}
}
