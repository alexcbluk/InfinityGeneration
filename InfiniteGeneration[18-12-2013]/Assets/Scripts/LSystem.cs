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
	private List<Vector3> positionStack = new List<Vector3>();
	
	//Road Creation
	private float roadLength;
	
	public LSystem(string Axiom, int PositionX, int PositionZ, string Rule, int Iteration){
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
	
	public void drawPoints()
	{
		int stackCounter = -1;
		
		string currentString = "";
		
		for(int i = 0; i < finalString.Length; i++)
		{
			
			char c = finalString[i];
			roadLength = 50 + Random.Range(20, 50);
			if(c == 'F')//draw line
			{
				float x_delta = roadLength * Mathf.Sin(position.y);
				float z_delta = roadLength * Mathf.Cos(position.y);
				
				
				//road.drawRoad(position, new Vector3(position.x + x_delta, 0, position.z + z_delta));
				drawRoad(new Vector3(position.x, 0, position.z), new Vector3(position.x + x_delta, 0, position.z + z_delta));
				position.x += x_delta;
				position.z += z_delta;
				
			}
			else if(c == '+')//turn right
			{
				position.y += angle + Random.Range(20, 50);
			}
			else if(c == '-')//turn left
			{
				position.y -= angle + Random.Range(20, 50);
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
		//lsystem = gameObject.GetComponent<LSystem>();
		
		
		lsystem.iterate();
		lsystem.drawPoints();

		Debug.Log ("LSystem class!");
		
		//road.drawRoad(position, new Vector3(position.x + 100, 0, position.z + 100));
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
}