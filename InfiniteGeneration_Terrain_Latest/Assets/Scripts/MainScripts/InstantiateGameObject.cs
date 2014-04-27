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
	
		public void InstantiateAllGameObject (Vector3[] position, string Type, string area_type)
		{

				for (int i=0; i < position.Length; i++) 
				{
			//industrial areas have a lot of skyscrapers and less trees
							if(area_type == "Industrial")
							{
										float randomWeight = Random.Range (0.0f, 1.0f);
										if (randomWeight > 0.3 && randomWeight < 0.9 /*&& position[i].y > -20 && position[i].y < 1*/) {	//30% chance to place building and 10% chance to leave empty	
											GameObject ob = MonoBehaviour.Instantiate (Resources.Load ("GameAssets/Building/skyscraper1") as GameObject, new Vector3 (position [i].x, position [i].y, position [i].z), Quaternion.identity) as GameObject;
											ob.transform.localPosition = position [i];
											ob.transform.localRotation = Quaternion.AngleAxis (Random.Range (0, 360), new Vector3 (0, 1, 0));
											ob.transform.localScale = new Vector3 (0.07f, Random.Range (.1f, .3f), .07f);
											ob.transform.parent = buildingContainer.transform;
										}
										if (randomWeight <= 0.3 ) {//60% chance to get tree placed
												GameObject tree = MonoBehaviour.Instantiate (Resources.Load ("GameAssets/tree") as GameObject, new Vector3 (position [i].x, position [i].y, position [i].z), Quaternion.AngleAxis (-90, new Vector3 (1, 0, 0))) as GameObject;
												tree.transform.parent = treeContainer.transform;
										}
							}
			//commercial areas have a lot of shops, leisure centers and  some trees
							else if(area_type == "Commercial")
							{
								float randomWeight = Random.Range (0.0f, 1.0f);
								if (randomWeight > 0.4 && randomWeight < 0.9/* && position[i].y > -20 && position[i].y < 1*/) {	//30% chance to place building and 10% chance to leave empty	
					GameObject ob = MonoBehaviour.Instantiate (Resources.Load ("GameAssets/Building/Build1") as GameObject, new Vector3 (position [i].x, position [i].y, position [i].z), Quaternion.identity) as GameObject;
									ob.transform.localPosition = position [i];
									ob.transform.localRotation = Quaternion.AngleAxis (Random.Range (0, 360), new Vector3 (0, 1, 0));
									ob.transform.localScale = new Vector3 (1, Random.Range (1.1f, 2.0f), 1);
									ob.transform.parent = buildingContainer.transform;
								}
								if (randomWeight <= 0.4 ) {//60% chance to get tree placed
									GameObject tree = MonoBehaviour.Instantiate (Resources.Load ("GameAssets/tree") as GameObject, new Vector3 (position [i].x, position [i].y, position [i].z), Quaternion.AngleAxis (-90, new Vector3 (1, 0, 0))) as GameObject;
									tree.transform.parent = treeContainer.transform;
								}
							}
			//residential areas have small cosy houses and  quite a lot of  trees
							else if(area_type == "Residential")
							{
								float randomWeight = Random.Range (0.0f, 1.0f);
								if (randomWeight > 0.3 && randomWeight < 0.9/* && position[i].y > -20 && position[i].y < 1*/) {	//30% chance to place building and 10% chance to leave empty	
									GameObject ob = MonoBehaviour.Instantiate (Resources.Load ("GameAssets/Building/Build2") as GameObject, new Vector3 (position [i].x, position [i].y, position [i].z), Quaternion.identity) as GameObject;
									ob.transform.localPosition = position [i];
									ob.transform.localRotation = Quaternion.AngleAxis (Random.Range (0, 360), new Vector3 (0, 1, 0));
									ob.transform.localScale = new Vector3 (1, Random.Range (1.1f, 2.0f), 1);
									ob.transform.parent = buildingContainer.transform;
								}
								if (randomWeight <= 0.3 ) {//60% chance to get tree placed
									GameObject tree = MonoBehaviour.Instantiate (Resources.Load ("GameAssets/tree") as GameObject, new Vector3 (position [i].x, position [i].y, position [i].z), Quaternion.AngleAxis (-90, new Vector3 (1, 0, 0))) as GameObject;
									tree.transform.parent = treeContainer.transform;
								}
							}
			//park areas have only  lots of  different trees
							else if(area_type == "Parks")
							{
									GameObject tree = MonoBehaviour.Instantiate (Resources.Load ("GameAssets/tree") as GameObject, new Vector3 (position [i].x, position [i].y, position [i].z), Quaternion.AngleAxis (-90, new Vector3 (1, 0, 0))) as GameObject;
									tree.transform.localScale = new Vector3 (1, 1, Random.Range (2.0f, 4.0f));					
									tree.transform.parent = treeContainer.transform;
									
							}
				}
		
		
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	



}
