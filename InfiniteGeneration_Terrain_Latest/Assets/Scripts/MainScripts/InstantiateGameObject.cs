using UnityEngine;
using System.Collections;

public class InstantiateGameObject
{
	
		private GameObject BuildingPrefab;
		private GameObject StreetLampPrefab;
		private GameObject TreePrefab;
		private Vector3[] ListofAllVector3;
		private string ListofAllType;
		private GameObject buildingContainer = new GameObject ("Buildings");
		private GameObject treeContainer = new GameObject ("Trees");

		public InstantiateGameObject ()
		{

		}
		// Use this for initialization
		void Start ()
		{
		 
		
				ListofAllVector3 = new Vector3[1];
				ListofAllVector3 [0] = new Vector3 (100, 0, 100);

				ListofAllType = "Building";

				//InstantiateAllGameObject(ListofAllVector3, ListofAllType);
		}
	
		public void InstantiateAllGameObject (Vector3[] position, string Type)
		{

				for (int i=0; i < position.Length; i++) {
						float randomWeight = Random.Range (0.0f, 1.0f);
						//if (Type == "Building") {
						if (randomWeight > 0.6 && randomWeight < 0.9 && position[i].y > -20 && position[i].y < 1) {	//30% chance to place building and 10% chance to leave empty	
							GameObject ob = MonoBehaviour.Instantiate (Resources.Load ("GameAssets/Building/skyscraper1") as GameObject, new Vector3 (position [i].x, position [i].y, position [i].z), Quaternion.identity) as GameObject;
							ob.transform.localPosition = position [i];
							ob.transform.localRotation = Quaternion.AngleAxis (Random.Range (0, 360), new Vector3 (0, 1, 0));
							ob.transform.localScale = new Vector3 (0.07f, Random.Range (.1f, .3f), .07f);
							ob.transform.parent = buildingContainer.transform;
						}
						if (Type == "StreetLamp") {
								MonoBehaviour.Instantiate (Resources.Load ("GameAssets/StreetLamp/StreetLampMod") as GameObject, new Vector3 (position [i].x, position [i].y, position [i].z), Quaternion.identity);
	    		      
						}
						//if (Type == "Tree") {
						if (randomWeight <= 0.6 ) {//60% chance to get tree placed
								GameObject tree = MonoBehaviour.Instantiate (Resources.Load ("GameAssets/tree") as GameObject, new Vector3 (position [i].x, position [i].y, position [i].z), Quaternion.AngleAxis (-90, new Vector3 (1, 0, 0))) as GameObject;
								tree.transform.parent = treeContainer.transform;
						}
				}
		
		
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	



}
