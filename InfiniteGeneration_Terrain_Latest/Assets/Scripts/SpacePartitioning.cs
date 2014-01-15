	//modified code for better partitioning (line 32) initial index not random, but always switches betwen 0 and 1, so there are no parallel lines like before
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;  // This is to use the List Collection

	public class Pair {
		public Pair(Vector3 s, Vector3 e)
		{
			start = s;
			end = e;
		}
		public Vector3 start;
		public Vector3 end;
	}

	public class Cell{
		public Cell(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
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

	public class SpacePartitioning : MonoBehaviour {
		List <Pair> listOfPoints = new List<Pair>();
		List <Cell> listOfCells = new List<Cell>();
		
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

			Partition (pointsList, 4,1);
		Vector3 [] slots;// = new Vector3[1];
		int num = 2;
		float padding  = 30;
		float offset = 20;
		GetObjectSlots(out slots, num, padding, offset);

		InstantiateGameObject ob = new InstantiateGameObject();
		ob.InstantiateAllGameObject(slots, "Tree");

		}
		
		void Partition(Vector3[] verticesList, int counter, int ind)
		{
			Vector3[] roadPoints = new Vector3[2];
			Vector3[] roadPointsDivided = new Vector3[2];
			int index1, index2, index3, index4;
			if(counter == 0)
			{
				listOfCells.Add(new Cell(verticesList[0], verticesList[1], verticesList[2], verticesList[3]));
				return;
			}
			//finding random indexes of points from vertices list
			//index1 = Random.Range(0, verticesList.Length-1);
			index1 =  Mathf.Abs(1 - ind);//Random.Range(0, verticesList.Length-1);
			//while((index2 = Random.Range(0, verticesList.Length-1)) == index1) ;
			index2 = (index1 + 2) % verticesList.Length;
			index3 = (index1+1)%verticesList.Length;
			index4 = (index2+1)%verticesList.Length;

			//finding parametiric equation parameter t1
			float t1 = Random.Range(0.3f, .6f);
			//finding first random point
			Vector3 p1 = (1-t1)*verticesList[index1] + t1*verticesList[index3];
			//finding parametiric equation parameter t
			float t2 = Random.Range(0.3f, .6f);
			//finding second random point
			Vector3 p2 = (1-t2)*verticesList[index2] + t2*verticesList[index4];

			//draw road segment
			roadPoints[0] = p1;
			roadPoints[1] = p2;

			//add 4 line segments to the list
			if(counter < 3)
			{
			//	/*
				listOfPoints.Add (new Pair(verticesList[index1], p1));
				listOfPoints.Add (new Pair(p1, verticesList[index3]));

				listOfPoints.Add (new Pair(verticesList[index2], p2));
				listOfPoints.Add (new Pair(p2, verticesList[index4]));
			//	*/
			}
			if(counter == 1)
			{
				listOfPoints.Add (new Pair(p1, p2));

			}
			drawRoadMesh(roadPoints);

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
			Partition (collection1, counter - 1, index1);
			for(int i = 1; i < collection2.Length - 1; i++)
			{
				collection2[i] = verticesList[(index4 + i - 1) % verticesList.Length];
			}
			collection2[collection2.Length - 1] = p1;
			Partition (collection2, counter - 1, index1);

		}

		public void GetObjectSlots(out Vector3 [] slots, int segmentsNum, float padding, float offset)
		{
			slots = new Vector3[(segmentsNum + 1) * listOfPoints.Count * 2];
		int loopCount = (segmentsNum + 1) * 2;
			
		int index = 0;
		///*
		for(int i = 0; i < listOfPoints.Count; i++)
		{
			Vector3 roadVector = listOfPoints[i].end - listOfPoints[i].start;
			float length = roadVector.magnitude;//proportion
			float p = padding / length;
			Vector3 start = (1 - p) * listOfPoints[i].start + p * listOfPoints[i].end;
			Vector3 end = listOfPoints[i].end * (1 - p) + listOfPoints[i].start * p;
			length -=  2 * padding;
			roadVector = RoadGenerator.GetUprightVector(roadVector) * offset;
			p = 0;
			float delta = 1.0f / segmentsNum;
			int c = index + loopCount;
			for(int j = index; j < c; j++)
			{
				Vector3 v = start * (1 - p) + end * p;
				slots[j++] = v + roadVector;
				slots[j] = v - roadVector;
				p += delta;
			}
			index += loopCount;
		}
		//*/
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
