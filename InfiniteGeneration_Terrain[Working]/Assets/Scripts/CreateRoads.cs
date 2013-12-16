using UnityEngine;
using System.Collections;

public class CreateRoads : MonoBehaviour {
	
	public Material matRoad;
	//public GameObject roadObject;
	
	public float roadHeight;
	public float roadScale;
	
	Vector3 road_Start;
	
	private Vector3 translateVector3;
	private float angleX,angleY,angleZ;
	
	// Use this for initialization
	void Start () {
		//roadHeight = 0.01f;			//Slightly above the terrain ground
		//meshWidth = 5;
	 	//meshHeight = 1;
	}
	
	// Update is called once per frame
	void Update () {
		checkMouseClick();
	}
	
	void checkMouseClick(){
		if(Input.GetMouseButtonDown(0)){
			//Output the Vector3 position of the roadStart			
			ClickLocation(out road_Start);	
			
		}
		
		if(Input.GetMouseButtonUp(0)){
			//Temp variable to store the position of the release drag
			Vector3 road_End;	
			
			if(ClickLocation(out road_End)){
				RoadCreation(road_Start, road_End);
			}
		}
		
	}
	
	//This boolean method returns true if the raycast hit something
	//and it returns a vector3 point of the mouse position
	bool ClickLocation(out Vector3 point){		
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//Debug.Log (ray);
			
		RaycastHit hit = new RaycastHit();
		//If the collider shoots a raycast and it hit an object (Our terrain) then create the road
			
		if(Physics.Raycast(ray, out hit, Mathf.Infinity) ) {
			
			point = hit.point;	//Raycast.point is the actual vector3 position
			return true;
		}
		
		point = new Vector3(0,0,0);
		return false;
	}
	
	public Vector3 TranslateVector3{
		get{return translateVector3;}
		set{translateVector3 = value;}
	}
	
	public float AngleX{
		get{return angleX;}
		set{angleX = value;}
	}
	
	public float AngleY{
		get{return angleY;}
		set{angleY = value;}
	}
	
	public float AngleZ{
		get{return angleZ;}
		set{angleZ = value;}
	}
	
	public void RoadCreation(Vector3 roadStart, Vector3 roadEnd){
			GameObject road = new GameObject("Road",typeof(MeshFilter), typeof(MeshRenderer) );
			//road.transform.position = new Vector3(0,0.1f,0);
			road.transform.position = roadStart + new Vector3(0, roadHeight, 0);	//Slightly off the terrain
			road.transform.localScale += new Vector3(0,0,roadScale);	//Scaling the road to be bigger
			road.transform.rotation = Quaternion.FromToRotation( Vector3.right, roadEnd - roadStart);
			Debug.Log(road.transform.rotation);
			
			
			//float meshWidth = 30;						//Change this number base on your segemented road.
			float meshWidth = Vector3.Distance(roadStart,roadEnd);			//Change this number base on your segemented road.
			float meshHeight = 1;

			//Initalise your Quad Vertices
			Vector3[] vertices = {
				//If we want our road to be 3D with some depth, add 1 to the Y Axis
				new Vector3(0			,0	,meshHeight/2),	//Top left
				new Vector3(meshWidth	,0	,meshHeight/2),	//Top Right
				new Vector3(meshWidth	,0	,-meshHeight/2),//Bottom Right
				new Vector3(0			,0	,-meshHeight/2)	//Bottom Left
			};
			
			int[] triangles = {
				0,1,2,			//Triangle 1
				2,3,0			//Triangle 2
			};
			
			//UV Mapping
			
			Vector2[] uv = {
				new Vector2(0			,0),	
				new Vector2(meshWidth	,0),	
				new Vector2(meshWidth	,meshHeight),	
				new Vector2(0			,meshHeight)
			
			};
			
			Vector3[] normals = {
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up
			};
			
			Mesh mesh = new Mesh();
			
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uv;					//Apply the texture onto the game object
			mesh.normals = normals;			//Apply the normals for the texture
			
			//Initialise the Mesh Filter and Mesh Render component that we are going to modified
			MeshFilter meshFilter = road.GetComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			MeshRenderer meshRender = road.GetComponent<MeshRenderer>();
			meshRender.material = matRoad;
			//meshRender.sharedMaterial.mainTextureScale.x = 
			meshRender.castShadows = false;		//Since the mesh is slightly above the ground, it may cast shadow so lets turn it off
			
	}
}
