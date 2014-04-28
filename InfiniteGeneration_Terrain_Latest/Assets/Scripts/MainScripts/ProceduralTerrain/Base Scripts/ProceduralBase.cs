using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for procedural meshes. Contains generic initialisation code and shared methods such as BuildQuad() and BuildQuadForGrid()
/// </summary>
public abstract class ProceduralBase : MonoBehaviour
{

	/// <summary>
	/// Method for building a mesh. Called in Start()
	/// </summary>
	/// <returns>The completed mesh</returns>
	public abstract Mesh BuildMesh();
	/// <summary>
	/// Initialisation. Build the mesh and assigns it to the object's MeshFilter
	/// </summary>
	private void Start()
	{
		//Build the mesh:
		Mesh mesh = BuildMesh();
	}

	#region "BuildQuad() methods"

	/// <summary>
	/// Builds a single quad in the XZ plane, facing up the Y axis.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">A position offset for the quad.</param>
	/// <param name="width">The width of the quad.</param>
	/// <param name="length">The length of the quad.</param>
	protected void BuildQuad(MeshBuilder meshBuilder, Vector3 offset, float width, float length)
	{
		meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, 0.0f) + offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, length) + offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(width, 0.0f, length) + offset);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(Vector3.up);

		meshBuilder.Vertices.Add(new Vector3(width, 0.0f, 0.0f) + offset);
		meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
		meshBuilder.Normals.Add(Vector3.up);

		//we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
		int baseIndex = meshBuilder.Vertices.Count - 4;

		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
		meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
	}

	/// <summary>
	/// Builds a single quad based on a position offset and width and length vectors.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="offset">A position offset for the quad.</param>
	/// <param name="widthDir">The width vector of the quad.</param>
	/// <param name="lengthDir">The length vector of the quad.</param>
	protected void BuildQuad(MeshBuilder meshBuilder, Vector3 offset, Vector3 widthDir, Vector3 lengthDir)
	{
		Vector3 normal = Vector3.Cross(lengthDir, widthDir).normalized;

		meshBuilder.Vertices.Add(offset);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(offset + lengthDir);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(offset + lengthDir + widthDir);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(normal);

		meshBuilder.Vertices.Add(offset + widthDir);
		meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
		meshBuilder.Normals.Add(normal);

		//we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
		int baseIndex = meshBuilder.Vertices.Count - 4;

		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
		meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
	}

	protected void BuildQuad(MeshBuilder meshBuilder, Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
	{
		Vector3 normal = Vector3.Cross(P2 - P1, P4 - P1).normalized;
		
		meshBuilder.Vertices.Add(P1);
		meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
		meshBuilder.Normals.Add(normal);
		
		meshBuilder.Vertices.Add(P2);
		meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
		meshBuilder.Normals.Add(normal);
		
		meshBuilder.Vertices.Add(P3);
		meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
		meshBuilder.Normals.Add(normal);
		
		meshBuilder.Vertices.Add(P4);
		meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
		meshBuilder.Normals.Add(normal);
		
		//we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
		int baseIndex = meshBuilder.Vertices.Count - 4;
		
		meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
		meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
	}

	#endregion

	#region "BuildQuadForGrid() methods"

	/// <summary>
	/// Builds a single quad as part of a mesh grid.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="position">A position offset for the quad. Specifically the position of the corner vertex of the quad.</param>
	/// <param name="uv">The UV coordinates of the quad's corner vertex.</param>
	/// <param name="buildTriangles">Should triangles be built for this quad? This value should be false if this is the first quad in any row or collumn.</param>
	/// <param name="vertsPerRow">The number of vertices per row in this grid.</param>
	protected void BuildQuadForGrid(MeshBuilder meshBuilder, Vector3 position, Vector2 uv, bool buildTriangles, int vertsPerRow)
	{
		meshBuilder.Vertices.Add(position);
		meshBuilder.UVs.Add(uv);

		if (buildTriangles)
		{
			int baseIndex = meshBuilder.Vertices.Count - 1;

			int index0 = baseIndex;
			int index1 = baseIndex - 1;
			int index2 = baseIndex - vertsPerRow;
			int index3 = baseIndex - vertsPerRow - 1;

			meshBuilder.AddTriangle(index0, index2, index1);
			meshBuilder.AddTriangle(index2, index3, index1);
		}
	}

	/// <summary>
	/// Builds a single quad as part of a mesh grid.
	/// </summary>
	/// <param name="meshBuilder">The mesh builder currently being added to.</param>
	/// <param name="position">A position offset for the quad. Specifically the position of the corner vertex of the quad.</param>
	/// <param name="uv">The UV coordinates of the quad's corner vertex.</param>
	/// <param name="buildTriangles">Should triangles be built for this quad? This value should be false if this is the first quad in any row or collumn.</param>
	/// <param name="vertsPerRow">The number of vertices per row in this grid.</param>
	/// <param name="normal">The normal of the quad's corner vertex.</param>
	protected void BuildQuadForGrid(MeshBuilder meshBuilder, Vector3 position, Vector2 uv, bool buildTriangles, int vertsPerRow, Vector3 normal)
	{
		meshBuilder.Vertices.Add(position);
		meshBuilder.UVs.Add(uv);
		meshBuilder.Normals.Add(normal);

		if (buildTriangles)
		{
			int baseIndex = meshBuilder.Vertices.Count - 1;

			int index0 = baseIndex;
			int index1 = baseIndex - 1;
			int index2 = baseIndex - vertsPerRow;
			int index3 = baseIndex - vertsPerRow - 1;

			meshBuilder.AddTriangle(index0, index2, index1);
			meshBuilder.AddTriangle(index2, index3, index1);
		}
	}

	#endregion


}
