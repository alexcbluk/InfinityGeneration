using UnityEngine;
using System.Collections;
using System.Collections.Generic;  // This is to use the List Collection

public class SpacePartitioning : MonoBehaviour {
	
	
	// Use this for initialization
	void Start () {
		Terrain terrain = Terrain.activeTerrain;
		TerrainData terrainData = terrain.terrainData;
		Vector3 terrainSize = terrain.terrainData.size;
		Vector3[] pointsList = {
								new Vector3(0, 0, 0),
								new Vector3(0, 0, terrainSize.z),
								new Vector3(terrainSize.x, 0, terrainSize.z),
								new Vector3(terrainSize.x, 0, 0)};
		Partition (pointsList, 4);
	}
	
	void Partition(Vector3[] verticesList, int counter)
	{
		Vector3[] roadPoints = new Vector3[2];
		int index1, index2, index3, index4;
		if(counter == 0)
			return;
		//while(counter>0)
		//{
			//finding random indexes of points from vertices list
		 	index1 = Random.Range(0, verticesList.Length-1);
		
		 	while((index2 = Random.Range(0, verticesList.Length-1)) == index1) ;
			//index1 = 2;
			//index2 = 1;
			index3 = (index1+1)%verticesList.Length;
			index4 = (index2+1)%verticesList.Length;
			
			//finding parametiric equation parameter t1
		    float t1 = Random.Range(0.0f, 1.0f);	
			t1 = .5f;
			//finding first random point
			Vector3 p1 = (1-t1)*verticesList[index1] + t1*verticesList[index3];
			//finding parametiric equation parameter t
		    float t2 = Random.Range(0.0f, 1.0f);
			t2 = .5f;
			//finding second random point
			Vector3 p2 = (1-t2)*verticesList[index2] + t2*verticesList[index4];
			
			//draw road segment
			roadPoints[0] = p1;
			roadPoints[1] = p2;
			drawRoadMesh(roadPoints);
		
			//Vector3 [] arr = 
			//int i = index1, j = index2;
			Vector3 [] collection1;
			Vector3 [] collection2;
			int count1 = index2 - index1;
			if(count1 > 0)
			{
				collection1 = new Vector3 [count1 + 2];
				collection2 = new Vector3 [verticesList.Length - count1 + 2];
			}
			else
			{
				collection1 = new Vector3 [verticesList.Length + count1 + 2];
				collection2 = new Vector3 [-count1 + 2];
			}

			collection1[0] = p1;
			collection2[0] = p2;
			for(int i = 1; i < collection1.Length - 1 ; i++)
			{
				collection1[i] = verticesList[(index3 + i - 1) % verticesList.Length];
			}
			collection1[collection1.Length - 1] = p2;
		Partition (collection1, counter - 1);
			for(int i = 1; i < collection2.Length - 1; i++)
			{
				collection2[i] = verticesList[(index4 + i - 1) % verticesList.Length];
			}
			collection2[collection2.Length - 1] = p1;
			Partition (collection2, counter - 1);
		
		
		
		//}
	}
	
	public void drawRoadMesh(Vector3[]roadPoints){

		Mesh mesh = RoadGenerator.GenerateRoadSegments(roadPoints);
		//Mesh mesh = MeshCombiner.CombineMesh(meshs);
		GameObject road = new GameObject("Road", typeof(MeshFilter), typeof(MeshRenderer));
		road.transform.position = new Vector3(0, .1f, 0);

		MeshFilter meshFilter = road.GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshRenderer meshRender = road.GetComponent<MeshRenderer>();
		//meshRender.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
		meshRender.material = Resources.Load("RoadMat") as Material;
		meshRender.castShadows = false;		//Since the mesh is slightly above the ground, it may cast shadow so lets turn it off
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
