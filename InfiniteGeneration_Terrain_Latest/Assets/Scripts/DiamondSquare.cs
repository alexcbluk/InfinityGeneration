﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Point
{
	public int x;
	public float y;
	public int z;
	public Point(int x_, float y_, int z_)
	{
		x = x_;
		y = y_;
		z = z_;
	}
}

public class DiamondSquare : ProceduralBase {
	
	public static int terrainSize = 16;
	public float[,] heightMap = new float[terrainSize,terrainSize];
	public float[] currentHeights;
	public float startingHeight = 1;
	public int heightIncrement = 50;
	//Vector3 point1,point2,point3,point4;		
	
	//Create a new mesh builder:
	MeshBuilder meshBuilder = new MeshBuilder();
	
	// Use this for initialization
	//void Start () {
		//currentHeights = new float[4]{1,1,1,1};
		//DiamondSquareAlgorithm(0, 0, terrainSize);
		//SampleHeightMap(0.49f, 1.0f);
	//}

	public void initializeDiamondSquare(int s)
	{
		//terrainSize = s;
		//heightMap = new float[terrainSize,terrainSize];
		currentHeights = new float[4]{1,1,1,1};
		DiamondSquareAlgorithm(0, 0, terrainSize);
		SampleHeightMap(0.49f, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public float SampleHeightMap(float x, float z)
	{
		float length = 1.0f / terrainSize;
		int indexX = (int)(x / length);
		int indexZ = (int)(z / length);
		if(indexX >= terrainSize)
			indexX = terrainSize - 1;
		if(indexZ >= terrainSize)
			indexZ = terrainSize - 1;
		///*
		float offsetX = (x - indexX * length) / length;
		float offsetZ = (z - indexZ * length) / length;
		int indexX1 = indexX + 1;
		int indexZ1 = indexZ + 1;
		if(indexX1 >= terrainSize)
			indexX1 = terrainSize - 1;
		if(indexZ1 >= terrainSize)
			indexZ1 = terrainSize - 1;
		float v0 = heightMap[indexX, indexZ] * (1 - offsetX) + heightMap[indexX1, indexZ] * offsetX;
		float v1 = heightMap[indexX, indexZ1] * (1 - offsetX) + heightMap[indexX1, indexZ1] * offsetX;
		float v2 = v0 * (1 - offsetZ) + v1 * offsetZ;
		return v2;
		//*/
	}
	
	//Build the mesh:
	public override Mesh BuildMesh()
	{
		currentHeights = new float[4]{10,10,10,10};
		//DiamondSquareAlgorithm(4,0, 0, 128, 128);
		
		//create the Unity mesh:
		Mesh mesh = meshBuilder.CreateMesh();
		
		//have the mesh calculate its own normals:
		mesh.RecalculateNormals();
		
		MeshFilter meshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		meshFilter.mesh = mesh;
		
		MeshRenderer meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		meshRenderer.material.mainTexture = Resources.Load("Terrain/Grass&Rock") as Texture;
		
		MeshCollider meshcollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		meshcollider.sharedMesh = mesh;
		
		return mesh;
	}
	
	/*
	void createTerrain(Vector3 offset, int segCount){
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();
		Vector2 uv = new Vector2(offset.x, offset.y);
		bool buildTriangles = true;

		BuildQuadForGrid(meshBuilder, offset, uv, buildTriangles, segCount);
	}
	*/
	
	public void DiamondSquareAlgorithm(int startX, int startZ, int length) 
	{
		if(length == 0){
			return;
		}
		//Initialise the temp height variable
		float h1 = currentHeights[0];
		float h2 = currentHeights[1];
		float h3 = currentHeights[2];
		float h4 = currentHeights[3];
		
		Point point1 = new Point(startX, h1,startZ);
		Point point2 = new Point(startX, h2,startZ + length);
		Point point3 = new Point(startX + length, h3,startZ + length);
		Point point4 = new Point(startX + length, h4,startZ);
		
		Point midpoint;
		
		float sideMidPoint1 =0;
		float sideMidPoint2 =0;
		float sideMidPoint3 =0;
		float sideMidPoint4 =0;
		
		
		//Finding the midpoint from Point 1 and 3
		midpoint = new Point(startX + length / 2, 0, startZ + length / 2);
		//Midpoint.y is the average of the 4 points
		midpoint.y = (point1.y + point2.y + point3.y + point4.y) / 4;
		
		//randomise height of the midpoint(s)
		midpoint.y += (float)Random.Range(heightIncrement, -heightIncrement);
		sideMidPoint1 += (float)Random.Range(heightIncrement, -heightIncrement);
		sideMidPoint2 += (float)Random.Range(heightIncrement, -heightIncrement);
		sideMidPoint3 += (float)Random.Range(heightIncrement, -heightIncrement);
		sideMidPoint4 += (float)Random.Range(heightIncrement, -heightIncrement);
		
		if(length == 1){
			
			heightMap[point1.x, point1.z] = point1.y;
			//BuildQuad(meshBuilder, point1,point2,point3,point4);
			Debug.Log("Blah");
		}
		
		//recursively call Partitioning
		//p1
		currentHeights[0] = h1;
		currentHeights[1] = sideMidPoint1;
		currentHeights[2] = midpoint.y;
		currentHeights[3] = sideMidPoint4;
		DiamondSquareAlgorithm(startX, startZ, length/2);
		//p2
		currentHeights[0] = sideMidPoint1;
		currentHeights[1] = h2;
		currentHeights[2] = sideMidPoint2;
		currentHeights[3] = midpoint.y;
		DiamondSquareAlgorithm(startX, startZ + length/2, length/2);
		//p3
		currentHeights[0] = midpoint.y;
		currentHeights[1] = sideMidPoint2;
		currentHeights[2] = h3;
		currentHeights[3] = sideMidPoint3;
		DiamondSquareAlgorithm(startX + length/2, startZ + length/2, length/2);
		//p4
		currentHeights[0] = sideMidPoint4;
		currentHeights[1] = midpoint.y;
		currentHeights[2] = sideMidPoint3;
		currentHeights[3] = h4;
		DiamondSquareAlgorithm(startX + length/2, startZ, length/2);
	}
}
