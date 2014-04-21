using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshCombiner : MonoBehaviour {

	public static Mesh CombineMesh(Mesh [] meshs)
	{
		List<Mesh> meshList = new List<Mesh>();
		for(int i = 0; i < meshs.Length; i++)
		{
			if(meshs[i] != null)
				meshList.Add(meshs[i]);
		}
		int verticeCount = 0, triangleCount = 0, normalCount = 0, uvCount = 0;
		for(int i = 0; i < meshList.Count; i++)
		{
			verticeCount += meshList[i].vertices.Length;
			triangleCount += meshList[i].triangles.Length;
			uvCount += meshList[i].uv.Length;
			normalCount += meshList[i].normals.Length;
		}
		Vector3[] vertices = new Vector3[verticeCount];
		Vector3[] normals = new Vector3[normalCount];
		Vector2[] uv = new Vector2[uvCount];
		int[] triangles = new int[triangleCount];
		int verticeIndex = 0, normalIndex = 0, uvIndex = 0, triangleIndex = 0;
		for(int i = 0; i < meshList.Count; i++)
		{
			for(int j = 0; j < meshList[i].vertices.Length; j++)
			{
				vertices[verticeIndex + j] = meshList[i].vertices[j];
			}
			for(int j = 0; j < meshList[i].normals.Length; j++)
			{
				normals[normalIndex + j] = meshList[i].normals[j];
			}
			normalIndex += meshList[i].normals.Length;
			for(int j = 0; j < meshList[i].uv.Length; j++)
			{
				uv[uvIndex + j] = meshList[i].uv[j];
			}
			uvIndex += meshList[i].uv.Length;
			for(int j = 0; j < meshList[i].triangles.Length; j++)
			{
				triangles[triangleIndex + j] = meshList[i].triangles[j] + verticeIndex;
			}
			verticeIndex += meshList[i].vertices.Length;
			triangleIndex += meshList[i].triangles.Length;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.normals = normals;
		return mesh;
	}
}
