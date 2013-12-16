using UnityEngine;
using System.Collections;

public class TerrainGeneration : MonoBehaviour {
	
	public Terrain EditorTerrain;	
	private Terrain Ter;
	
	private TerrainData tData;
	private PerlinNoise perlin;  //Instance of the perlin noise script.
	
	private float[,] heights;
	//
	GameObject[,] terrainArray = new GameObject[3,3];
	TerrainData[,] terrainsdataArray = new TerrainData[4,4];
	
	int xResolution;
	int zResolution;

	void Start () {
		
		
		Ter = EditorTerrain;
		perlin = new PerlinNoise();
		//any chages during runtime to the terrain get's saved, so set it back to 0
		FlatenOnEditorTerrain();
		ColorEditorTerrain();
		
		tData = Ter.terrainData;
		xResolution = tData.heightmapWidth;
		zResolution = tData.heightmapHeight;
		//Sampling the height of the terrain starting from the 0,0 (orgin) to the end of the terrain
		heights = tData.GetHeights(0,0,xResolution,zResolution);	
	}

	void Update () {
		/*
		 * Keypress Menu 
		 * R = Generate a new terrain using Perlin Noise, also color it correctly
		 * T = Flattens terrain, notice the texture color are still there. Press Y again to color it correctly
		 * Y = Updates the color on the terrain
		 * 
		 * More than one terrain [ Gride of 9 terrains ]
		 * U = Flatten terrains and create the rest of the terrain grids. Notice if  you keep pressing "U" new terrain objects instantiate
		 * I = Flatten the terrains and generate a new set of terrain using Perlin Noise and then update its color texture
		 * O = Updates the color of the terrains
		 * 
		 * We need to create another function that deletes previous terrain to solve the problem from keypressed "U"
		 * Upcoming key:
		 * Q = Delete all terrains or previous.
		 */
		/*
		if(Input.GetMouseButton(0)){
			RaycastHit hit;
			//Converts from Screen co-ordinante from the mouse into world co-ordinate
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit))
			{
				//Pass in a vector 3 
				raiseTerrain(hit.point);	
			}
		} 
		
		if(Input.GetMouseButton(1)){
			RaycastHit hit;
			//Converts from Screen co-ordinante from the mouse into world co-ordinate
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit))
			{
				//Pass in a vector 3 
				lowerTerrain(hit.point);	
			}
		}
		*/
			if (Input.GetKeyDown (KeyCode.R)){
				GenerateOnEditorTerrain ();
				ColorEditorTerrain();
		    }else if (Input.GetKeyDown (KeyCode.T)){
		   		FlatenOnEditorTerrain ();
		    }else if (Input.GetKeyDown (KeyCode.Y)){
		    	ColorEditorTerrain();
		    }else if (Input.GetKeyDown (KeyCode.U)){
				FlatenOnEditorTerrain();
		    	AddAdditionalTerrains();
		    }else if (Input.GetKeyDown (KeyCode.I)){
				FlatenOnEditorTerrain();
		    	GenerateAdditionalTerrains();
				ColorAdditionalTerrains();
		    }else if (Input.GetKeyDown (KeyCode.O)){
		    	ColorAdditionalTerrains();
		    }else if (Input.GetKeyDown (KeyCode.Q)){
		    	DeleteAdditionalTerrain();
		    }
	}
//######################### This section shows how Terrain generation works with 1 terrain already created in the editor
	void GenerateOnEditorTerrain (){
		print ("Generating On the Editor Terrain ");
		perlin.setseed(0); //randomize seed
		heights = EditorTerrain.terrainData.GetHeights (0, 0, 513, 513);
		float otherheight;  
		for (int i=0; i<=(512); i++) {
			for (int j=0; j<=(512); j++) {
				//generate the ground
				otherheight = (float)(((perlin.Noise (i / 50f, j / 50f, 0.5f) + 1.0f) / 2.0f) * 0.4f) - 0.1f;
				//generate mountains
				heights [i, j] = (float)perlin.Noise (i / 50f, j / 50f, 0.5f);
				//merge them
				if (heights [i, j] < otherheight) {
					heights [i, j] = otherheight;
				}
			}
		} 
		EditorTerrain.terrainData.SetHeights (0, 0, heights); 
	}
	//Flattens the edited terrain back to 0
	void FlatenOnEditorTerrain(){
		print ("Flattening The Editor Terrain");
		heights = EditorTerrain.terrainData.GetHeights (0, 0, 513, 513);
		for (int i=0; i<=(512); i++) {
			for (int j=0; j<=(512); j++) {
				heights [i, j] = 0;
			}
		} 
		EditorTerrain.terrainData.SetHeights (0, 0, heights); 	
	}
	//color the editor terrain
	void ColorEditorTerrain(){
		print ("Coloring The Editor Terrain");
		float[, ,] splatmapData = new float[EditorTerrain.terrainData.alphamapWidth, EditorTerrain.terrainData.alphamapHeight, EditorTerrain.terrainData.alphamapLayers];
		// the colors
		Vector4 grass = new Vector4 (1, 0, 0);
		Vector4 peak = new Vector4 (0, 1, 0);
		Vector4 dirt = new Vector4 (0, 0, 1);
		Vector4 splat = grass;
		//for each 'Pixel'
		for (int i = 0; i < EditorTerrain.terrainData.alphamapHeight; i++) {
			for (int j= 0; j < EditorTerrain.terrainData.alphamapWidth; j++) {
				// read the height at this location
				float height = EditorTerrain.terrainData.GetHeight (i, j);
				// determine the mix of textures 1, 2 & 3 to use 
				// (using a vector3, since it can be lerped & normalized)
				splat = grass;
				if (height > 40) {
					splat = Vector4.Lerp (dirt, peak, ((height - 60) / 40));
				} else if (height > 10) {
					splat = Vector4.Lerp (grass, dirt, ((height - 10) / 30));
				}
				// now assign the values to the correct location in the array
				splat.Normalize ();
				splatmapData [j, i, 0] = splat.x;
				splatmapData [j, i, 1] = splat.y;
				splatmapData [j, i, 2] = splat.z;
			}
		}
		EditorTerrain.terrainData.SetAlphamaps (0, 0, splatmapData);
	}

//######################################################################################
	//now we get onto generating more terrain chunks, 9 chunks, in a grid
//######################################################################################
	
	void AddAdditionalTerrains(){
		
		//If gameobject.CompareTag = "Terrain", call that function
		
		print ("Adding the 9 Terrains");
		TerrainData terdata2;
		//load the terraindata from a file
		terdata2 = Resources.Load("saved terrain")as TerrainData;
		if (terdata2 == null){print ("File not found");}

		for (int i=0; i<(3); i++) {
			for (int j=0; j<(3); j++) {
				//copy the terrain into memory [All 9 terrains]
				terrainsdataArray[i,j] = (TerrainData) Object.Instantiate(terdata2);
				//colourit(terrainsdataArray[i,j]);

				//create the 9 terrains and place them into position
				//they are above the Editor terrain, as the editor terrain is just to show the concepts, and won't be implemented in the final game
				terrainArray[i,j] = Terrain.CreateTerrainGameObject(terrainsdataArray[i,j]);
				terrainArray[i,j].transform.Translate( (Ter.transform.position.x-((1-i)*2000) - terrainArray[i,j].transform.position.x ), (Ter.transform.position.y+5 - terrainArray[i,j].transform.position.y ), (Ter.transform.position.z+((1-j)*2000) - terrainArray[i,j].transform.position.z ), Space.World);
				terrainArray[i,j].transform.name = "Terrain [" + i + "," + j + "]";
				terrainArray[i,j].gameObject.tag = "Terrain";
			}
		}
	}
	
	void DeleteAdditionalTerrain(){
		for (int i=0; i<(3); i++) {
			for (int j=0; j<(3); j++) {
				Destroy(terrainArray[i,j]);
			}
		}
	}
	
	
	void GenerateAdditionalTerrains(){
		print ("Generating Landscape on the Additional Terrains");
		perlin.setseed(0);		//randomize seed
		heights = new float[1537,1537];		//generate noise for a 3x3 slab array
		float[,] heights1 = new float[513,513];	//array for one slab
		float otherheight;  

		//fill the 3x3 array with the terrain
		for (int i=0; i<=(1535); i++) {
			for (int j=0; j<=(1535); j++) {
				//generate the ground
				otherheight = (float)(((perlin.Noise (i / 50f, j / 50f, 0.5f) + 1.0f) / 2.0f) * 0.4f) - 0.1f;
				//generate mountains
				heights [i, j] = (float)perlin.Noise (i / 50f, j / 50f, 0.5f);
				//merge them
				if (heights [i, j] < otherheight) {
					heights [i, j] = otherheight;
				}
			}
		}
		Terrain ter2;
		//split the big 3x3 array into 9 little ones
		for (int k=0; k<(3); k++) { //horiz
			for (int l=0; l<3; l++) { //vertical
				ter2 = terrainArray[k,l].GetComponent<Terrain>();
				
				for (int i=0; i<=(512); i++) {
					for (int j=0; j<=(512); j++) {
						heights1[i,j] = heights[((2-l)*512)+i,(k*512)+j];
					}
				}
				ter2.terrainData.SetHeights (0, 0, heights1); // apply sections to individual slabs
				
			}
		}
	}

	void ColorAdditionalTerrains(){
		print ("Coloring Additional Terrains");
		float[, ,] splatmapData = new float[512, 512, 3];
		float height = 0f;
		TerrainData CurrentSlabData;
		// the colors
		Vector4 peak = new Vector4 (0, 1, 0);
		Vector4 grass = new Vector4 (1, 0, 0);
		Vector4 dirt = new Vector4 (0, 0, 1);
		Vector4 splat = grass;
		
		for (int k=0; k<(3); k++) { //horiz			For each slab
			for (int l=0; l<3; l++) { //vertical	
				//### -- Two methods I could think of, of acsessing the data, both yeild same result
				
				//CurrentSlabData = terrainArray[k,l].GetComponent<Terrain>().terrainData;
				CurrentSlabData = terrainsdataArray[k,l];
				
				//###
				for (int i = 0; i < 512; i++) {			//for Each 'Pixel' within that slab
					for (int j= 0; j < 512; j++) {
						// read the height at this location
						height = CurrentSlabData.GetHeight(i, j);
						// determine the mix of textures 1, 2 & 3 to use 
						// (using a vector3, since it can be lerped & normalized)
						splat = grass;
						if (height > 40) {
							splat = Vector4.Lerp (dirt, peak, ((height - 60) / 40));
						} else if (height > 10) {
							splat = Vector4.Lerp (grass, dirt, ((height - 10) / 30));
						}
						// now assign the values to the correct location in the array
						splat.Normalize ();
						splatmapData [j, i, 0] = splat.x;
						splatmapData [j, i, 1] = splat.y;
						splatmapData [j, i, 2] = splat.z;
					}
				}
				CurrentSlabData.SetAlphamaps (0, 0, splatmapData);	//apply the data to the current slab
			}
		}
	}
	
	private void raiseTerrain(Vector3 point){
		//Width = x
		//Height = y
		//Length/Depth = z
		//Multiple by the resolution to localise the position
		int mouseX = (int) ((point.x / tData.size.x) * xResolution);	
		int mouseZ = (int) ((point.z / tData.size.z) * zResolution);
		
		
		float[,] modHeights = new float [1,1];	//An array to store the modified height, increase the new float value to increase the brush size
		float y = heights[mouseX, mouseZ];		//Assign the current Height(Y) where the mouse is pointing to
		y += 10.05f * Time.deltaTime;			//Increment it 
		
		
		//Setting boundaries			
		//Making sure it does not go higher than the set Height of the Terrain
		if(y > tData.size.y){ y = tData.size.y;	}
		
		modHeights[0,0] = y;					//Save the newly incremented Height value to the modifiedHeight array
		heights[mouseX,mouseZ] = y;				//Assign the newly incremented Height value to the actual height array 
												//(This way you don't need a for loop to copy the value for each element) Do it as you go along
		tData.SetHeights(mouseX, mouseZ, modHeights);	//Make the changes to the terrainData of the current width and height along with the Length(Depth)
	}
	
	private void lowerTerrain(Vector3 point){
		//Width = x
		//Height = y
		//Length/Depth = z
		//Multiple by the resolution to localise the position
		int mouseX = (int) ((point.x / tData.size.x) * xResolution);	
		int mouseZ = (int) ((point.z / tData.size.z) * zResolution);
		
		
		float[,] modHeights = new float [1,1];	//An array to store the modified height, increase the new float value to increase the brush size
		float y = heights[mouseX, mouseZ];		//Assign the current Height(Y) where the mouse is pointing to
		y -= 5.05f * Time.deltaTime;			//Increment it 
		
		
		//Setting boundaries			
		//Making sure it does not go lower than the ground (0f)
		if(y < 0.0f){ y = 0.0f;	}
		
		modHeights[0,0] = y;					//Save the newly incremented Height value to the modifiedHeight array
		heights[mouseX,mouseZ] = y;				//Assign the newly incremented Height value to the actual height array 
												//(This way you don't need a for loop to copy the value for each element) Do it as you go along
		tData.SetHeights(mouseX, mouseZ, modHeights);	//Make the changes to the terrainData of the current width and height along with the Length(Depth)
	}
	
}