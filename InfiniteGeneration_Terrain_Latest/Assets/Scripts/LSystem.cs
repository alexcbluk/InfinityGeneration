using UnityEngine;
using System.Collections;
using System.Collections.Generic;  // This is to use the List Collection

public class LSystem : Road {
	
	private string axiom;
	private int rootX,rootZ;
	private string finalString;         //This is the string to output the iteration       
	//private Vector3 [][] positionStack;    //Position stack for branching, this is used for going to previous node
	private int stack_size = 0;
	//private ArrayList<Vector3> positionStack = new ArrayList<Vector3>();
	
	private Vector3 position;
	
	//private ArrayList<string> rules = new ArrayList<string>();
	private List<string> rules = new List<string>();
	private string rule;
	private Color colour;
	private float angle = 22.5f;
	private int iterationNum;
	private int pathIndex = 0;
	private List<Vector3> positionStack = new List<Vector3>();
	private Vector3 terrSize;
	private int scaleFactor;
	//Road Creation
	private float roadLength;
	private List<List<Vector3>> roadPositions = new List<List<Vector3>>();
	
	//Road Segements
	private List<List<Vector3>> roadSegmentList = new List<List<Vector3>>();

	
	public LSystem(string Axiom, int PositionX, int PositionZ, string Rule, int Iteration, Vector3 terrainSize){
		roadPositions.Add(new List<Vector3>());
		//Root Position
		rootX = PositionX;
		rootZ = PositionZ;
		terrSize = terrainSize;
		scaleFactor = 4;
		//Properties
		iterationNum = Iteration;
		axiom = Axiom;
		finalString = Axiom;
		rule = Rule;
		position.x = rootX;
		position.y = 0;
		position.z = rootZ;
		//Appearance
		colour = new Color(0,0,0);
		
		//road = gameObject.GetComponent<Road>();
	}
	
	
	
	public void iterate(){
		for(int i=0; i<iterationNum; i++){
			string string_next = "";
			
			for(int j=0; j<finalString.Length; j++){
				
				char c = finalString[j];
				
				if(c == 'F')
				{
					string_next = string_next + "FF"; 
				} 
				else if (c == 'X')
				{
					string_next = string_next + rule;
				}
				else if (c == 'O')
				{
					string_next = string_next + "Y";
				}
				else if (c == 'R')
				{
					string_next = string_next + "R";
				}
				else
				{
					string_next = string_next + c ;
				}
			}
			
			finalString = string_next;
			
		}
		Debug.Log(finalString); 
	}

	public void drawRoadMesh(){

		Mesh[] meshs = new Mesh[roadPositions.Count];
		for(int i = 0; i < roadPositions.Count; i++)
		{
			meshs[i] = RoadGenerator.GenerateRoadSegments(roadPositions[i].ToArray(), 1);
		}
		Mesh mesh = MeshCombiner.CombineMesh(meshs);
		GameObject road = new GameObject("Road", typeof(MeshFilter), typeof(MeshRenderer));
		road.transform.position = new Vector3(0, .1f, 0);

		MeshFilter meshFilter = road.GetComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		MeshRenderer meshRender = road.GetComponent<MeshRenderer>();
		meshRender.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
		meshRender.material = Resources.Load("RoadMat") as Material;
		meshRender.castShadows = false;		//Since the mesh is slightly above the ground, it may cast shadow so lets turn it off
	}

	public void GenerateRoadPosition()
	{
		int stackCounter = -1;
		
		string currentString = "";

		
		for(int i = 0; i < finalString.Length; i++)
		{
			
			char c = finalString[i];
			roadLength = 50 + Random.Range(20, 30);
			if(c == 'F')//draw line
			{
				float x_delta = roadLength * Mathf.Sin(position.y);
				float z_delta = roadLength * Mathf.Cos(position.y);
				float x1 = position.x;
				float z1 = position.z;
				float x2 = position.x + x_delta;
				float z2 = position.z + z_delta;

				if(x2 >= (terrSize.x-500)/scaleFactor || x2 <= -(terrSize.x-1500)/scaleFactor || z2 >= (terrSize.z-500)/scaleFactor || z2 <= -(terrSize.z-1500)/scaleFactor)//test if road positions go outside terrain
				{
					Debug.Log("Outside terrain: "+position.x + ", " + position.z);
					//position.y = -position.y;//change direction of road
					//do nothing if positions of road go outside terrain
				}
				else//otherwise create road
				{					
					//Debug.Log("x: "+position.x);
					//Debug.Log("z: "+position.z);
					//road.drawRoad(position, new Vector3(position.x + x_delta, 0, position.z + z_delta));
					//drawRoad(new Vector3(position.x, 0, position.z), new Vector3(position.x + x_delta, 0, position.z + z_delta));
					//Store in the Vector3 Position into an array
					roadPositions[pathIndex].Add(new Vector3(position.x, 0, position.z));
					roadPositions[pathIndex].Add(new Vector3(position.x + x_delta, 0, position.z + z_delta));//if intersect change to intersection point
					
					List<Vector3> road = new List<Vector3>();
					road.Add(new Vector3(x1, 0, z1));
					road.Add(new Vector3(x2, 0, z2));
					roadSegmentList.Add( road );
					
					for(int z = 1; z < roadSegmentList.Count; z++)
					{
						Vector3 V3 = roadSegmentList[z-1][0];						
						Vector3 V4 = roadSegmentList[z-1][1];
						
						//Vector3 iPoint = Intersect ((new Vector3(x1, 0, z1)), (new Vector3(x2, 0, z2)), V3, V4);
					}
					
					
						
					position.x += x_delta;
					position.z += z_delta;
					
				}
				
			}
			else if(c == '+')//turn right
			{
				float a = angle + Random.Range(30, 60);
						
				if(a>20 && a<90)
				{
					position.y += a;
				}
			}
			else if(c == '-')//turn left
			{
				float a = angle + Random.Range(30, 60);
				if(a>20 && a<90)
				{
					position.y -= a;
				}
			}
			else if(c == '[')
			{
				//Debug.Log("push " + stackCounter);
				positionStack.Add(position);
				stackCounter++;
				
			}
			else if(c == ']')
			{
				pathIndex++;
				roadPositions.Add(new List<Vector3>());
				//Debug.Log("Current Character: " + c);
				if(stackCounter < 0){
					Debug.Log("We have MINUS!"); 
				}
				
				//stackCounter--;
				//Debug.Log("here: " + stackCounter);
				position = positionStack[stackCounter];
				//Debug.Log("Before Stack Counter" + stackCounter);
				
				positionStack.RemoveAt(stackCounter);
				//Debug.Log("After Stack Counter" + stackCounter);
				
				
				stackCounter--;

			}
			
			currentString += c;
			//Debug.Log("Current Character: " + c + "  Current StackCounter: " + stackCounter);

		}

	}
	
//	public static Vector3 Intersect(Vector3 line1V1, Vector3 line1V2, Vector3 line2V1, Vector3 line2V2)
//    {
//        //Line1
//        float A1 = line1V2.z - line1V1.z;
//        float B1 = line1V2.x - line1V1.x;
//        float C1 = A1*line1V1.x + B1*line1V1.z;
//
//        //Line2
//        float A2 = line2V2.z - line2V1.z;
//        float B2 = line2V2.x - line2V1.x;
//        float C2 = A2 * line2V1.x + B2 * line2V1.z;
//
//        float det = A1*B2 - A2*B1;
//        if (det == 0)
//        {
//            //return null;//parallel lines
//        }
//        else
//        {
//            float x = (B2*C1 - B1*C2)/det;
//            float z = (A1 * C2 - A2 * C1) / det;
//            return new Vector3(x,0,z);
//        }
//    }
	
	// Use this for initialization
	void Start () {
		Terrain terrain = Terrain.activeTerrain;
		TerrainData terrainData = terrain.terrainData;
		Vector3 terrainSize = terrain.terrainData.size;
		Debug.Log("Tx: "+terrainSize.x);
		Debug.Log("Tz: "+terrainSize.z);
		//float posx = Random.Range(-terrainSize.x, terrainSize.x);
		//float posy = Random.Range(-terrainSize.z, terrainSize.z);
		int posx = Random.Range(-500, 1500);
		int posz = Random.Range(-500, 1500);
		//road = gameObject.GetComponent<Road>();
		LSystem lsystem = new LSystem("X",0,0,"F-[[X]+X]+FF[+FX]-X",3, terrainSize);//"-[[-X]+X]+FF[+FX]-X", 4);//"F-[[X]+X]+FF[+FX]-X",4);
		//LSystem lsystemScript = gameObject.GetComponent<LSystem>();
		
		
		lsystem.iterate();

		lsystem.GenerateRoadPosition();
		lsystem.drawRoadMesh();
		
		//road.drawRoad(position, new Vector3(position.x + 100, 0, position.z + 100));
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
}
