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
	
	//Road Creation
	private float roadLength;
	private List<List<Vector3>> roadPositions = new List<List<Vector3>>();
	
	public LSystem(string Axiom, int PositionX, int PositionZ, string Rule, int Iteration){
		roadPositions.Add(new List<Vector3>());
		//Root Position
		rootX = PositionX;
		rootZ = PositionZ;
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
		float offset = 0.1f;
		for(int i = 0; i < roadPositions.Count; i++)
		{
			GameObject road = new GameObject("Road", typeof(MeshFilter), typeof(MeshRenderer));
			road.transform.position = new Vector3(0, offset, 0);
			offset += .0001f;

			//Vector3[] vertices = 
			//{	
			//	
			//	new Vector3(-5, 0, -5),
			//	new Vector3(0, 0, 0),
			//	new Vector3(5, 0, -5),
			//	new Vector3(0,0,-10),
			//	
			//	
			//};

			Vector3[] vertices = (Vector3[])roadPositions[i].ToArray();


			//lsystem = GameObject.Find

			//Vector3[] vertices = lsystemScript.Road


			Mesh mesh = RoadGenerator.GenerateRoadSegments(vertices);

			MeshFilter meshFilter = road.GetComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			MeshRenderer meshRender = road.GetComponent<MeshRenderer>();
			meshRender.material = Resources.Load("RoadMat") as Material;
			meshRender.castShadows = false;		//Since the mesh is slightly above the ground, it may cast shadow so lets turn it off
		}
	}

	public void GenerateRoadPosition()
	{
		int stackCounter = -1;
		
		string currentString = "";
		
		for(int i = 0; i < finalString.Length; i++)
		{
			
			char c = finalString[i];
			roadLength = 5;// + Random.Range(2, 5);
			if(c == 'F')//draw line
			{
				float x_delta = roadLength * Mathf.Sin(position.y);
				float z_delta = roadLength * Mathf.Cos(position.y);
				
				
				//road.drawRoad(position, new Vector3(position.x + x_delta, 0, position.z + z_delta));
				//drawRoad(new Vector3(position.x, 0, position.z), new Vector3(position.x + x_delta, 0, position.z + z_delta));
				//Store in the Vector3 Position into an array
				roadPositions[pathIndex].Add(new Vector3(position.x, 0, position.z));
				roadPositions[pathIndex].Add(new Vector3(position.x + x_delta, 0, position.z + z_delta));

				position.x += x_delta;
				position.z += z_delta;
				
			}
			else if(c == '+')//turn right
			{
				position.y += angle;// + Random.Range(20, 50);
			}
			else if(c == '-')//turn left
			{
				position.y -= angle;// + Random.Range(20, 50);
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
	
	// Use this for initialization
	void Start () {
		
		//road = gameObject.GetComponent<Road>();
		LSystem lsystem = new LSystem("X",0,0,"F-[[X]+X]+FF[+FX]-X",3);
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
