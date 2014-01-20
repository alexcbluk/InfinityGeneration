using UnityEngine;
using System.Collections;

public class RoadGenerator : MonoBehaviour {
	
	public static Vector3 GetUprightVector(Vector3 v)
	{
		float length = Vector3.Magnitude(v);
		Vector3 v1 = new Vector3(v.z / length, 0, -v.x / length);
		if(v.x * v1.z - v.z * v1.x < 0)
			return v1;
		return -v1;
	}
	
	public static Mesh GenerateRoadSegments(Vector3 [] vertices, float scale)
	{
		if(vertices.Length < 2)
			return null;
		Vector3[] vertices2 = new Vector3[vertices.Length];
		int i = 0, length = 1;
		vertices2[0] = vertices[0];
		//merge all similar(distance < .00001) neighbour points
		for(; i < vertices.Length - 1; i++)
		{
			if(Vector3.Distance(vertices[i], vertices[i + 1]) > .00001f)
			{
				vertices2[length++] = vertices[i + 1];
			}
		}
		if(length < 2)
			return null;
		
		float halfMeshWidth = .5f * scale;
		float meshWidth = 2 * halfMeshWidth;
		Vector3 v1, v2 = vertices2[1] - vertices2[0];
		float meshLength1 = v2.magnitude;
		float meshLength = 0;
		Vector3 upright;
		upright = GetUprightVector(v2);
		upright *= halfMeshWidth;
		Vector3[] vertices1 = new Vector3[(length - 1) * 4];
		Vector2[] uv = new Vector2[(length - 1) * 4];
		int[] triangles = new int[6 * (length - 1)];
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		triangles[3] = 2;
		triangles[4] = 3;
		triangles[5] = 0;
		Vector3[] normals = new Vector3[(length - 1) * 4];
		for(i = 0; i < vertices1.Length; i++)
		{
			normals[i] = Vector3.up;
		};
		vertices1[0] = vertices2[0] + upright;
		vertices1[1] = vertices2[0] + -upright;
		uv[0] = new Vector2(0, 0);
		uv[1] =        new Vector2(0, 1);
		int index = 2;
		float f = 0;
		float currentU = 0;
		for(i = 0; i < length - 2; i++)
		{
			v1 = -v2;
			v1.Normalize();
			meshLength = meshLength1;
			v2 = vertices2[i + 2] - vertices2[i + 1];
			meshLength1 = v2.magnitude;
			v2.Normalize();
			Vector3 v3 = v1 + v2;
			//if(Mathf.Abs(Vector3.Dot(Vector3.Normalize(v1), Vector3.Normalize(v2))) > .999f)
			bool turnUpsideDown = false;
			//handle the case that two road segments are collinear
			if(v3.magnitude < .000001f)
			{
				v3 = upright;
			}
			//handle the case generating the road segment in the direction of 180 degree
			//this case causes the previous road segment to be overlapped. it doesn't make sense. don't do this.
			else if(Vector3.Dot(Vector3.Normalize(v1), Vector3.Normalize(v2)) > .999f)
			{
				v3 = upright;
				turnUpsideDown = true;
			}
			float u1, u2, u3, u4;
			upright.Normalize();
			if(Vector3.Dot(v3, upright) < 0)
			{
				v3 = -v3;
				v3 *= halfMeshWidth / (Vector3.Dot(v3, upright));
				float f3 = v3.sqrMagnitude - halfMeshWidth * halfMeshWidth;
				if(Mathf.Abs(f3) < .000001)//There's a bug in Sqrt: if the argument is too close to zero, Sqrt will return a non-numerical value
					f3 = 0;
				f = Mathf.Sqrt(f3) * 1 / scale;
				currentU += meshLength + 2 * f;
				u1 = currentU - 3 * f;
				u2 = currentU - f;
				u3 = currentU - f;
				u4 = currentU + f;
			}
			else
			{
				v3 *= halfMeshWidth / (Vector3.Dot(v3, upright));
				float f3 = v3.sqrMagnitude - halfMeshWidth * halfMeshWidth;
				if(Mathf.Abs(f3) < .000001)//There's a bug in Sqrt: if the argument is too close to zero, Sqrt will return a non-numerical value
					f3 = 0;
				f = Mathf.Sqrt(f3) * 1 / scale;
				currentU += meshLength + 2 * f;
				u1 = currentU - f;
				u2 = currentU - 3 * f;
				u3 = currentU + f;
				u4 = currentU - f;
			}
			
			vertices1[index] = vertices2[i + 1] + -v3;
			uv[index++] = new Vector2(u1, 1);
			vertices1[index] = vertices2[i + 1] + v3;
			uv[index++] = new Vector2(u2, 0);
			
			triangles[i * 6 + 6] = i * 4 + 4;
			triangles[i * 6 + 7] = i * 4 + 5;
			triangles[i * 6 + 8] = i * 4 + 6;
			triangles[i * 6 + 9] = i * 4 + 6;
			triangles[i * 6 + 10] = i * 4 + 7;
			triangles[i * 6 + 11] = i * 4 + 4;
			if(turnUpsideDown)
			{
				vertices1[index] = vertices2[i + 1] + -v3;
				uv[index++] = new Vector2(u4, 0);
				vertices1[index] = vertices2[i + 1] + v3;
				uv[index++] = new Vector2(u3, 1);
			}
			else
			{
				vertices1[index] = vertices2[i + 1] + v3;
				uv[index++] = new Vector2(u3, 0);
				vertices1[index] = vertices2[i + 1] + -v3;
				uv[index++] = new Vector2(u4, 1);
			}
			upright = GetUprightVector(v2);
			upright *= halfMeshWidth;
		}
		vertices1[index] = vertices2[i + 1] - upright;
		uv[index++] = new Vector2(currentU + meshLength1 * 1 / scale, 1);
		vertices1[index] = vertices2[i + 1] + upright;
		uv[index] = new Vector2(currentU + meshLength1 * 1 / scale, 0);
		
		Mesh mesh = new Mesh();
		mesh.vertices = vertices1;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.normals = normals;
		return mesh;
	}
}