using UnityEngine;
using System.Collections;

public class InstantiateGameObject {
	
	private GameObject BuildingPrefab;
	private GameObject StreetLampPrefab;
	private GameObject TreePrefab;
	
	private Vector3[] ListofAllVector3;
	private string ListofAllType;
	public InstantiateGameObject()
	{

	}
	// Use this for initialization
	void Start () {
		 
		
		ListofAllVector3 = new Vector3[1];
		ListofAllVector3[0] = new Vector3(100,0,100);

		ListofAllType = "Building";

		//InstantiateAllGameObject(ListofAllVector3, ListofAllType);
	}
	
	
	public void InstantiateAllGameObject(Vector3[] position, string Type){

		for(int i=0; i < position.Length; i++){
				if(Type == "Building"){

					MonoBehaviour.Instantiate(Resources.Load("GameAssets/Building") as GameObject, new Vector3(position[i].x, position[i].y, position[i].z), Quaternion.identity);
				}
				if(Type == "Tree"){
				MonoBehaviour.Instantiate(Resources.Load("GameAssets/StreetLamp/StreetLampMod") as GameObject, new Vector3(position[i].x, position[i].y, position[i].z), Quaternion.identity);
	    		      
				}
				if(Type == "StreetLamp"){
				MonoBehaviour.Instantiate(Resources.Load("GameAssets/Tree") as GameObject, new Vector3(position[i].x, position[i].y, position[i].z), Quaternion.identity);
	    		      
				}
		}
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	



}
