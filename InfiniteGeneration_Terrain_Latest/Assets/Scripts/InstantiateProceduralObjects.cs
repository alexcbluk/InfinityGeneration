using UnityEngine;
using System.Collections;

public class InstantiateProceduralObjects : MonoBehaviour {

	private GameObject terrain;

	// Use this for initialization
	void Start () {
		terrain = new GameObject();
		terrain.name = "Terrain";

		terrain.AddComponent<DiamondSquare>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
