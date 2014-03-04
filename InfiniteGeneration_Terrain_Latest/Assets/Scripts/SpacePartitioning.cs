//modified code for better partitioning (line 32) initial index not random, but always switches betwen 0 and 1, so there are no parallel lines like before
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// This is to use the List Collection

public class Pair
{
	public Pair (Vector3 s, Vector3 e)
	{
		start = s;
		end = e;
	}
	
	public Vector3 start;
	public Vector3 end;
}

public class Curve
{
	public Curve (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float t0, float t1)
	{
		v1_ = v1;
		v2_ = v2;
		v3_ = v3;
		v4_ = v4;
		t0_ = t0;
		t1_ = t1;
	}

	public float GetRandomT()
	{
		float t = Random.Range (.3f, .6f);
		return t0_ * (1 - t) + t1_ * t;
	}

	public Vector3 GetT0()
	{
		return CalculateBezierPoint(t0_, v1_, v2_, v3_, v4_); 
	}

	public Vector3 GetT1()
	{
		return CalculateBezierPoint(t1_, v1_, v2_, v3_, v4_); 
	}

	public Vector3 GetPoint(float t)
	{
		return CalculateBezierPoint(t, v1_, v2_, v3_, v4_); 
	}

	public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3){
		float u = 1-t;
		float tt = t*t;
		float uu = u*u;
		float uuu = uu * u;
		float ttt = tt * t;
		
		// formula for cubic bezier - http://en.wikipedia.org/wiki/B%C3%A9zier_curve
		Vector3 p = uuu * p0; //first term (first part of formula), apply float calculation to point p0
		p += 3 * uu * t * p1; //second term (append second part of formula to first), apply float calculation to point p1
		p += 3 * u * tt * p2; //third term (append third part of formula to second and first), apply float calculation to point p2
		p += ttt * p3; //fourth term (append fourth part of formula to third, second and first), apply float calculation to point p3
		
		return p; //return concatinated formula
	}
	
	public Vector3 v1_;
	public Vector3 v2_;
	public Vector3 v3_;
	public Vector3 v4_;
	public float t0_;
	public float t1_;
}


public class Cell
{
	public Cell (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
	{
		v1_ = v1;
		v2_ = v2;
		v3_ = v3;
		v4_ = v4;
	}
	
	public Vector3 v1_;
	public Vector3 v2_;
	public Vector3 v3_;
	public Vector3 v4_;
}

public class Segment{
	public Segment(Vector3 s, Vector3 e)
	{
		segStart = s;
		segEnd = e;
	}
	public Vector3 segStart;
	public Vector3 segEnd;
}


public class SpacePartitioning : MonoBehaviour
{
	List <Pair> listOfPoints = new List<Pair> ();
	List <Cell> listOfCells = new List<Cell> ();
	List <Cell> listOfBuildingCells = new List<Cell> ();
	List <Segment> listofSegments = new List<Segment>();
	bool placeRoads;
	float current_offset = .2f;

	Vector3[] GenerateBezierControlPoints(Vector3 start, Vector3 end)
	{
		Vector3 findP1 = Vector3.Lerp(start, end, Random.Range (0.05f,0.45f));
		Vector3 findP2 = Vector3.Lerp(end, start, Random.Range (0.05f,0.45f));
		Vector3 [] controlPoints = {new Vector3(findP1.x+(Random.Range (-30, 30)), findP1.y, findP1.z+(Random.Range (-30, 30))), new Vector3(findP2.x+(Random.Range (-30, 30)), findP2.y, findP2.z+(Random.Range (-30, 30)))};
		return controlPoints;
	}
	
	// Use this for initialization
	public void initializeSpacePartitioning (float mWidth, float mLength)
	{
		//Terrain terrain = Terrain.activeTerrain;
		//TerrainData terrainData = terrain.terrainData;
		//Vector3 terrainSize = terrain.terrainData.size;
		Vector3 terrainSize = new Vector3(mWidth, 0, mLength);
		Vector3[] pointsList = {
			new Vector3 (0, 0, 0),
			new Vector3 (0, 0, terrainSize.z),
			new Vector3 (terrainSize.x, 0, terrainSize.z),
			new Vector3 (terrainSize.x, 0, 0)};
		Vector3 [] controlPoints0 = GenerateBezierControlPoints(pointsList[0], pointsList[1]);
		Vector3 [] controlPoints1 = GenerateBezierControlPoints(pointsList[1], pointsList[2]);
		Vector3 [] controlPoints2 = GenerateBezierControlPoints(pointsList[2], pointsList[3]);
		Vector3 [] controlPoints3 = GenerateBezierControlPoints(pointsList[3], pointsList[0]);
		Curve [] curves = {
			new Curve(pointsList[0], controlPoints0[0], controlPoints0[1], pointsList[1], 0, 1),
			new Curve(pointsList[1], controlPoints1[0], controlPoints1[1], pointsList[2], 0, 1),
			new Curve(pointsList[2], controlPoints2[0], controlPoints2[1], pointsList[3], 0, 1),
			new Curve(pointsList[3], controlPoints3[0], controlPoints3[1], pointsList[0], 0, 1)
		};
		placeRoads = true;
		PartitionForCurve(curves, 7, 1);
		placeRoads = false;
		Vector3 [] cell = new Vector3[4];
		for (int i = 0; i < listOfCells.Count; i++) {
			int listOfBuildingCells = Random.Range (3, 5);
			cell [0] = listOfCells [i].v1_;
			cell [1] = listOfCells [i].v2_;
			cell [2] = listOfCells [i].v3_;
			cell [3] = listOfCells [i].v4_;
			Partition (cell, listOfBuildingCells, 1);
		}
		
		///*
		Vector3 [] slots;// = new Vector3[1];
		int num = 3;
		float padding = 30;
		float offset = 20;
		GetObjectSlots (out slots, num, padding, offset);
		
		InstantiateGameObject ob = new InstantiateGameObject ();
		ob.InstantiateAllGameObject (slots, "Tree");
		//*/
		
	}
	
	void PartitionForCurve (Curve[] CurveList, int counter, int ind)
	{
		Vector3[] roadPoints = new Vector3[2];
		Vector3[] roadPointsDivided = new Vector3[2];
		int index1, index2, index3, index4;
		if (counter == 0) {
			listOfCells.Add (new Cell (CurveList[0].GetT0(), CurveList[1].GetT0(), CurveList[2].GetT0(), CurveList[3].GetT0()));
			return;
		}
		//finding random indexes of points from vertices list
		index1 = 1 - ind;
		index2 = index1 + 2;
		index3 = index1 + 1;
		index4 = index2 + 1;
		
		float t1 = CurveList[index1].GetRandomT();
		float t2 = CurveList[index2].GetRandomT();
		//finding first random point
		Vector3 p1 = CurveList[index1].GetPoint(t1);
		//finding second random point
		Vector3 p2 = CurveList[index2].GetPoint(t2);
		
		//draw road segment
		roadPoints [0] = p1;
		roadPoints [1] = p2;
		
		//add 4 line segments to the list
		if (counter < 3 && placeRoads) {
			//        /*
			listOfPoints.Add (new Pair (CurveList [index1].GetT0(), p1));
			listOfPoints.Add (new Pair (p1, CurveList [index3].GetT1()));
			
			listOfPoints.Add (new Pair (CurveList [index2].GetT0(), p2));
			listOfPoints.Add (new Pair (p2, CurveList [index4 % 4].GetT1()));
			//        */
		}
		if (counter == 1 && placeRoads) {
			listOfPoints.Add (new Pair (p1, p2));
			
		}
		
		Curve [] collection1 = new Curve [4];
		Curve [] collection2 = new Curve [4];
		
		Vector3 [] controlPoints = GenerateBezierControlPoints(p2, p1);
		collection1 [0] = new Curve(CurveList[index1].v1_, CurveList[index1].v2_, CurveList[index1].v3_, CurveList[index1].v4_, t1, CurveList[index1].t1_);
		collection1 [1] = CurveList[index3];
		collection1 [2] = new Curve(CurveList[index2].v1_, CurveList[index2].v2_, CurveList[index2].v3_, CurveList[index2].v4_, CurveList[index2].t0_, t2);
		collection1 [3] = new Curve(p2, controlPoints[0], controlPoints[1], p1, 0, 1);
		collection2 [0] = new Curve(CurveList[index2].v1_, CurveList[index2].v2_, CurveList[index2].v3_, CurveList[index2].v4_, t2, CurveList[index2].t1_);
		collection2 [1] = CurveList[index4 % 4];
 		collection2 [2] = new Curve(CurveList[index1].v1_, CurveList[index1].v2_, CurveList[index1].v3_, CurveList[index1].v4_, CurveList[index1].t0_, t1);
		collection2 [3] = new Curve(p1, controlPoints[1], controlPoints[0], p2, 0, 1);
		PartitionForCurve (collection1, counter - 1, index1);
		PartitionForCurve (collection2, counter - 1, index1);
		Vector3 [] curvePoints = {p1, controlPoints[1], controlPoints[0], p2};
		if (placeRoads) {
			//drawRoadMesh (roadPoints);
			curvedSegments(curvePoints); // Uses curve segments method
		}
	}

	void Partition (Vector3[] verticesList, int counter, int ind)
	{
		Vector3[] roadPoints = new Vector3[2];
		Vector3[] roadPointsDivided = new Vector3[2];
		int index1, index2, index3, index4;
		if (counter == 0) {
			if (placeRoads) {
				listOfCells.Add (new Cell (verticesList [0], verticesList [1], verticesList [2], verticesList [3]));
			} else {
				listOfBuildingCells.Add (new Cell (verticesList [0], verticesList [1], verticesList [2], verticesList [3]));
			}
			return;
			
		}
		//finding random indexes of points from vertices list
		//index1 = Random.Range(0, verticesList.Length-1);
		index1 = Mathf.Abs (1 - ind);
		//while((index2 = Random.Range(0, verticesList.Length-1)) == index1) ;
		index2 = (index1 + 2) % verticesList.Length;
		index3 = (index1 + 1) % verticesList.Length;
		index4 = (index2 + 1) % verticesList.Length;
		
		//finding parametiric equation parameter t1
		float t1 = Random.Range (0.3f, .6f);
		//finding first random point
		Vector3 p1 = (1 - t1) * verticesList [index1] + t1 * verticesList [index3];
		//finding parametiric equation parameter t
		float t2 = Random.Range (0.3f, .6f);
		//finding second random point
		Vector3 p2 = (1 - t2) * verticesList [index2] + t2 * verticesList [index4];
		
		//draw road segment
		roadPoints [0] = p1;
		roadPoints [1] = p2;
		
		//add 4 line segments to the list
		if (counter < 3 && placeRoads) {
			//        /*
			listOfPoints.Add (new Pair (verticesList [index1], p1));
			listOfPoints.Add (new Pair (p1, verticesList [index3]));
			
			listOfPoints.Add (new Pair (verticesList [index2], p2));
			listOfPoints.Add (new Pair (p2, verticesList [index4]));
			//        */
		}
		if (counter == 1 && placeRoads) {
			listOfPoints.Add (new Pair (p1, p2));
			
		}
		if (placeRoads) {
			//drawRoadMesh (roadPoints);
			curvedSegments(roadPoints); // Uses curve segments method
		}
		
		Vector3 [] collection1;
		Vector3 [] collection2;
		int count1 = index2 - index1;
		if (count1 > 0) {
			collection1 = new Vector3 [count1 + 2];
			collection2 = new Vector3 [verticesList.Length - count1 + 2];
		} else {
			collection1 = new Vector3 [verticesList.Length + count1 + 2];
			collection2 = new Vector3 [-count1 + 2];
		}
		
		collection1 [0] = p1;
		collection2 [0] = p2;
		for (int i = 1; i < collection1.Length - 1; i++) {
			collection1 [i] = verticesList [(index3 + i - 1) % verticesList.Length];
		}
		collection1 [collection1.Length - 1] = p2;
		Partition (collection1, counter - 1, index1);
		for (int i = 1; i < collection2.Length - 1; i++) {
			collection2 [i] = verticesList [(index4 + i - 1) % verticesList.Length];
		}
		collection2 [collection2.Length - 1] = p1;
		Partition (collection2, counter - 1, index1);
		
	}
	
	public void GetObjectSlots (out Vector3[] slots, int segmentsNum, float padding, float offset)
	{
		/*******************Uncomment this code for street lamps positions******************
                                slots = new Vector3[(segmentsNum + 1) * listOfPoints.Count * 2];
                                int loopCount = (segmentsNum + 1) * 2;
                        
                                int index = 0;
                                for (int i = 0; i < listOfPoints.Count; i++) {
                                                Vector3 roadVector = listOfPoints [i].end - listOfPoints [i].start;
                                                float length = roadVector.magnitude;//proportion
                                                float p = padding / length;
                                                Vector3 start = (1 - p) * listOfPoints [i].start + p * listOfPoints [i].end;
                                                Vector3 end = listOfPoints [i].end * (1 - p) + listOfPoints [i].start * p;
                                                length -= 2 * padding;
                                                roadVector = RoadGenerator.GetUprightVector (roadVector) * offset;
                                                p = 0;
                                                float delta = 1.0f / segmentsNum;
                                                int c = index + loopCount;
                                                for (int j = index; j < c; j++) {
                                                                Vector3 v = start * (1 - p) + end * p;
                                                                slots [j++] = v + roadVector;
                                                                slots [j] = v - roadVector;
                                                                p += delta;
                                                }
                                                index += loopCount;
                                }
                                ***************************************************************************/
		
		/*******This code is for finding midlle point of building cells (also used for trees)*******/
		slots = new Vector3[listOfBuildingCells.Count];
		for (int i = 0; i <listOfBuildingCells.Count; i++) {
			//find midpoints of every side of cell
			float t = 0.5f;
			
			Vector3 midpoint1 = (1 - t) * listOfBuildingCells [i].v1_ + t * listOfBuildingCells [i].v2_;
			Vector3 midpoint2 = (1 - t) * listOfBuildingCells [i].v2_ + t * listOfBuildingCells [i].v3_;
			Vector3 midpoint3 = (1 - t) * listOfBuildingCells [i].v3_ + t * listOfBuildingCells [i].v4_;
			Vector3 midpoint4 = (1 - t) * listOfBuildingCells [i].v4_ + t * listOfBuildingCells [i].v1_;
			
			slots [i] = LineIntersectionPoint (midpoint1, midpoint3, midpoint2, midpoint4);
		}
		
	}
	
	Vector3 LineIntersectionPoint (Vector3 pS1, Vector3 pE1, Vector3 pS2, Vector3 pE2)
	{
		//find equation of two lines separately Y = MX + C, where M is slope, C is intercept
		
		//slope = (y2 - y1) / (x2 - x1)
		float m1 = (pE1.z - pS1.z) / (pE1.x - pS1.x);
		float m2 = (pE2.z - pS2.z) / (pE2.x - pS2.x);
		
		// intercept = y1 - (slope) * x1
		float c1 = pS1.z - m1 * pS1.x;
		float c2 = pS2.z - m2 * pS2.x;
		
		if ((m1 - m2) == 0) {//this should be never met, because we know that lines are not parallel
			throw new System.Exception ("Lines are parallel");
		} else {
			float intersectionX = (c2 - c1) / (m1 - m2);
			float intersectionZ = m1 * intersectionX + c1;
			return new Vector3 (intersectionX, 0, intersectionZ);
		}                        
		
	}

	// creates the segment points for each road and curve it
	public void curvedSegments(Vector3[]roadPoints) {
		// A temporary list that holds the segment points
		List<Vector3> tempSegPoints = new List<Vector3>();
		
		// curveResolution determines the 'smoothness' of the road. The greater the number, the more smoother it is.
		float curveResolution = 20;
		// for loop to create the curve for each road
		for (float j = 0; j <= curveResolution; j++) {
			float t = j/curveResolution;
			// Uses the CalculateBezierPoint function before storing it into tempSegPoints 
			tempSegPoints.Add(Curve.CalculateBezierPoint(t, roadPoints[0], roadPoints[1], roadPoints[2], roadPoints[3]));
		}
		drawRoadMesh(tempSegPoints.ToArray());
		/*
		// For each pass in the for loop, the curved segment points are stored in the 'listofSegments'
		// Each element of the list holds the start and end positions of each segment
		for (int k = 0; k < tempSegPoints.Count-1; k++){
			listofSegments.Add (new Segment(tempSegPoints[k], tempSegPoints[k+1]));
			Vector3[] segmentPoints = new Vector3[2];
			segmentPoints[0] = tempSegPoints[k];
			segmentPoints[1] = tempSegPoints[k+1];
			drawRoadMesh(segmentPoints);
		}
		//*/
		tempSegPoints.Clear();
		
		
	}

	public void drawRoadMesh (Vector3[]roadPoints)
	{
		
		Mesh mesh = RoadGenerator.GenerateRoadSegments (roadPoints, 10);
		//Mesh mesh = MeshCombiner.CombineMesh(meshs);
		GameObject road = new GameObject ("Road", typeof(MeshFilter), typeof(MeshRenderer));
		road.transform.position = new Vector3 (0, current_offset, 0);
		
		MeshFilter meshFilter = road.GetComponent<MeshFilter> ();
		meshFilter.mesh = mesh;
		MeshRenderer meshRender = road.GetComponent<MeshRenderer> ();
		//meshRender.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
		meshRender.material = Resources.Load ("RoadMat") as Material;
		meshRender.castShadows = false;                //Since the mesh is slightly above the ground, it may cast shadow so lets turn it off
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
