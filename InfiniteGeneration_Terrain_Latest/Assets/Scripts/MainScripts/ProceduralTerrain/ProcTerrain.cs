/// <summary>
/// Do List:
/// 	> Create a SetHeight function for your Procedural Terrain
/// </summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// A procedurally generated terrain using a primitive mesh plane.
/// The “bumpiness” in this mesh is done by using the "Midpoint Displacement algorithm" to
/// generate random height to each vertex. 
/// 
/// http://en.wikipedia.org/wiki/Heightmap
/// http://en.wikipedia.org/wiki/Perlin_noise
/// http://en.wikipedia.org/wiki/Diamond-square_algorithm
/// </summary>
public class ProcTerrain : ProceduralBase
{
	//The width and length of each segment:
	public int meshWidth = 10;
	public int meshLength = 10;

	static public float sizeOfTerrain;

	//The maximum height of the mesh:
	public float meshHeight = 0.0f;

	//The number of segments in each dimension (the plane will be m_SegmentCount * m_SegmentCount in area):
	public int meshSegmentCount = 180;

	//Build the mesh:
	public override Mesh BuildMesh()
	{
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();
		sizeOfTerrain = meshWidth*meshSegmentCount;
		DiamondSquare.initializeDiamondSquare(meshWidth*meshSegmentCount);
		//Changing the height map
		//Loop through the rows:
		for (int i = 0; i <= meshSegmentCount; i++)
		{
			//incremented values for the Z position and V coordinate:
			float z = (float)meshLength * i;
			float v = (1.0f / meshSegmentCount) * i;

			//Loop through the collumns:
			for (int j = 0; j <= meshSegmentCount; j++)
			{
				//incremented values for the X position and U coordinate:
				float x = (float)meshWidth * j;
				float u = (1.0f / meshSegmentCount) * j;

				//The position offset for this quad, with the given height from the Diamond Square Algorithm
				Vector3 offset = new Vector3(x, DiamondSquare.SampleHeightMap(x/sizeOfTerrain, z/sizeOfTerrain), z);

				//build quads that share vertices:
				Vector2 uv = new Vector2(u, v);
				bool buildTriangles = i > 0 && j > 0;

				BuildQuadForGrid(meshBuilder, offset, uv, buildTriangles, meshSegmentCount + 1);
			}
		}


		//create the Unity mesh:
		Mesh mesh = meshBuilder.CreateMesh();

		//have the mesh calculate its own normals:
		mesh.RecalculateNormals();

		MeshFilter meshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		meshFilter.mesh = mesh;

		MeshRenderer meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		meshRenderer.material.mainTexture = Resources.Load("Textures/Grass&Rock") as Texture;
		meshRenderer.material.shader = Shader.Find ("Custom/TerrainShader");

		/*
		Texture2D colorMap = (Texture2D) Resources.Load("ColorMap/ColorMap_d-54");
		Texture2D normalMap = (Texture2D) Resources.Load("NormalMap/NormalMap_d-41");
		Texture2D lightMap = (Texture2D) Resources.Load("LightMap/LightMap_d-8");
		Texture2D splatMap = (Texture2D) Resources.Load("SplatMap/SplatMap_d-60");
		
		meshRenderer.material.mainTexture = colorMap;
		meshRenderer.material.SetTexture("_LightMap", lightMap);
		meshRenderer.material.SetTexture("_SplatMap", splatMap);
		meshRenderer.material.SetTexture("_NormalMap", normalMap);
		*/

		MeshCollider meshcollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		meshcollider.sharedMesh = mesh;


		SpacePartitioning sp = new SpacePartitioning();
		sp.initializeSpacePartitioning(meshWidth*meshSegmentCount, meshLength*meshSegmentCount);


		//return the new mesh:
		return mesh;
	}
}
