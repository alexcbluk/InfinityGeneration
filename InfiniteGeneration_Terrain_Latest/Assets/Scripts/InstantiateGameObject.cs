using UnityEngine;
using System.Collections;

public class InstantiateGameObject : MonoBehaviour {
	
	private GameObject BuildingPrefab;
	private GameObject StreetLampPrefab;
	private GameObject TreePrefab;
	
	private Vector3[] ListofAllVector3;
	private string[] ListofAllType;
	
	// Use this for initialization
	void Start () {
		BuildingPrefab = Resources.Load("GameAssets/Building/Building") as GameObject;
		StreetLampPrefab = Resources.Load("GameAssets/StreetLamp/StreetLampMod") as GameObject;
		TreePrefab = Resources.Load("GameAssets/Tree") as GameObject;
		
		ListofAllVector3 = new Vector3[1];
		ListofAllVector3[0] = new Vector3(100,0,100);
		
		ListofAllType = new string[1];
		ListofAllType[0] = "StreetLamp";

		
		
		InstantiateAllGameObject(ListofAllVector3, ListofAllType);
				
	}
	
	
	public void InstantiateAllGameObject(Vector3[] position, string[] Type){
		
		for(int i=0; i < position.Length; i++){
			for(int j=0; j < Type.Length; j++){
				if(Type[j] == "Building"){
	  				Instantiate(BuildingPrefab, new Vector3(position[i].x, position[i].y, position[i].z), Quaternion.identity);
				}
				if(Type[j] == "Tree"){
				  Instantiate(TreePrefab, new Vector3(position[i].x, position[i].y, position[i].z), Quaternion.identity);
	    		      
				}
				if(Type[j] == "StreetLamp"){
				  Instantiate(StreetLampPrefab, new Vector3(position[i].x, position[i].y, position[i].z), Quaternion.identity);
	    		      
				}
			}
		}
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	



}
