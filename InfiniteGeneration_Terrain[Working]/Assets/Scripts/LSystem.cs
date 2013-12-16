using UnityEngine;
using System.Collections;
using System.Collections.Generic;		// This is to use the List Collection


public class LSystem : MonoBehaviour {
	
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
	private List<Vector3> positionStack = new List<Vector3>();
	
	//Road Creation
	private CreateRoads createRoads = new CreateRoads();
	
	public float roadHeight = 10;
	public float roadScale = 30;
	private float roadLength;
	Road road = new Road();
	
	LSystem(string Axiom, int PositionX, int PositionZ, string Rule, int Iteration){
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
	}
	

	
	private void iterate(){
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
	
	private void drawLines()
	{
		int stackCounter = -1;
		
		string currentString = "";
		
		
		for(int i = 0; i < finalString.Length; i++)
		{
			
			char c = finalString[i];
			if(c == 'F')//draw line
			{
				float x_delta = roadLength * Mathf.Sin(position.x);
				float z_delta = roadLength * Mathf.Cos(position.z);
				
				//drawRoad(new Vector3(position.x, 0, position.z), new Vector3(position.x+x_delta, 0, position.z+z_delta));
				road.drawRoad(position, new Vector3(position.x + x_delta, 0, position.z + z_delta));
				
				position.x += x_delta;
				position.z += z_delta;
				
			}
			else if(c == '+')//turn right
			{
				position.y += angle;
			}
			else if(c == '-')//turn left
			{
				position.y -= angle;
			}
			else if(c == '[')
			{
				//Debug.Log("push " + stackCounter);
				positionStack.Add(position);
				stackCounter++;
				
			}
			else if(c == ']')
			{
				//Debug.Log("Current Character: " + c);
				if(stackCounter < 0){
					Debug.Log("We have MINUS!");	
				}
				
				//stackCounter--;
				//Debug.Log("here: " + stackCounter);
				position = positionStack[stackCounter];
				Debug.Log("Before Stack Counter" + stackCounter);
				
				positionStack.RemoveAt(stackCounter);
				Debug.Log("After Stack Counter" + stackCounter);
				

				stackCounter--;
		
			}
			
			currentString += c;
			Debug.Log("Current Character: " + c + "  Current StackCounter: " + stackCounter);
		}
	
		
		
	}
	
	
	
	
	// Use this for initialization
	void Start () {
		LSystem lsystem = new LSystem("F",0,0,"F-[[X]+X]+FF[+FX]-X",0);
		
		lsystem.iterate();
		lsystem.drawLines();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


/*
void drawRoad(Vector3 start, Vector3 end)
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
		meshRender.material = matRoad;
		//meshRender.sharedMaterial.mainTextureScale.x = 
		meshRender.castShadows = false;		//Since the mesh is slightly above the ground, it may cast shadow so lets turn it off
	}
 */