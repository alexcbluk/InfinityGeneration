using UnityEngine;
using System.Collections;

public class Road : MonoBehaviour {

	public int roadHeight = 10;
	public int roadScale = 30;
	//public Material matRoad;
	
	
	// Use this for initialization
	void Start () {
		//Instantiate(Resources.Load("RoadCube"),new Vector3(0,0,0),Quaternion.identity);
		for(int i=0; i<1000; i+=100){
			drawRoad(new Vector3(0,10,0), new Vector3(100 + i,10,100 + i));
			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void drawRoad(Vector3 start, Vector3 end)
	{
		
		GameObject road = new GameObject("Road", typeof(MeshFilter), typeof(MeshRenderer));
		road.transform.position = start + new Vector3(0, roadHeight, 0);
		road.transform.localScale += new Vector3(0, 0, roadScale);
		road.transform.rotation = Quaternion.FromToRotation(Vector3.right, end - start);
		
		float meshLength = Vector3.Distance(start, end);
		float meshWidth = 1;
		
		Vector3[] vertices = 
		{
			new Vector3(0, 			0, meshWidth/2),
			new Vector3(meshLength, 0, meshWidth/2),
			new Vector3(meshLength, 0, -meshWidth/2),
			new Vector3(0, 			0, -meshWidth/2)
		};
		
		int[] triangles = 
		{
			0, 1, 2, 
			2, 3, 0
		};
		
		Vector2[] uv = 
		{
			new Vector2(0			,0),	
			new Vector2(meshLength	,0),	
			new Vector2(meshLength	,meshWidth),	
			new Vector2(0			,meshWidth)
			
		};
			
		Vector3[] normals = 
		{
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.up
		};
		
		Mesh mesh = new Mesh();
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.normals = normals;
		
		MeshFilter meshFilter = road.GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshRenderer meshRender = road.GetComponent<MeshRenderer>();
		meshRender.material = Resources.Load("RoadMat") as Material;
		//meshRender.sharedMaterial.mainTextureScale.x = 
		meshRender.castShadows = false;		//Since the mesh is slightly above the ground, it may cast shadow so lets turn it off
	}
}
