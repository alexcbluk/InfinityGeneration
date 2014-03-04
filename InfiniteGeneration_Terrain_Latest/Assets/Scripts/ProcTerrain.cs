/// <summary>
/// Do List:
/// 	> Create a SetHeight function for your Procedural Terrain
/// </summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// A "terrain" mesh. Or rather, a plane using a random height offset in each vertex.
/// 
/// Note: The “bumpiness” in this mesh is done by assigning a random height to each vertex. 
/// It’s done this way because it makes the code nice and simple, not because it makes a good 
/// looking terrain. If you’re serious about building a terrain mesh, you might try using a 
/// heightmap, or perlin noise, or looking into algorithms such as diamond-square.
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

	//The maximum height of the mesh:
	public float meshHeight = 0.0f;

	//The number of segments in each dimension (the plane will be m_SegmentCount * m_SegmentCount in area):
	public int meshSegmentCount = 180;
	 

	//Build the mesh:
	public override Mesh BuildMesh()
	{
		#region Not needed
		//Create a new mesh builder:
		MeshBuilder meshBuilder = new MeshBuilder();
		int sizeOfTerrain = meshWidth*meshSegmentCount;

		DiamondSquare ds = new DiamondSquare();
		ds.initializeDiamondSquare(meshWidth*meshSegmentCount);
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

				//The position offset for this quad, with a random height between zero and m_MaxHeight:
				Vector3 offset = new Vector3(x, ds.SampleHeightMap(x/sizeOfTerrain, z/sizeOfTerrain), z);

				//build quads that share vertices:
				Vector2 uv = new Vector2(u, v);
				bool buildTriangles = i > 0 && j > 0;

				BuildQuadForGrid(meshBuilder, offset, uv, buildTriangles, meshSegmentCount + 1);
			}
		}

		#endregion 

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


		SpacePartitioning sp = new SpacePartitioning();
		sp.initializeSpacePartitioning(meshWidth*meshSegmentCount, meshLength*meshSegmentCount);


		//return the new mesh:
		return mesh;
	}
}
