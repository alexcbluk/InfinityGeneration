using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class catmull : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Mesh mesh = new Mesh();

		List<Vector3> catPoints = new List<Vector3>();
		//List<Vector3> catPoints = new List<Vector3>();
		List<Vector3> catOut =  new List<Vector3>();

		//GameObject obj1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//obj1.transform.position = new Vector3(-10, 0.1f, 0);
		catPoints.Add(new Vector3(-10, 0.1f, 0));

		//GameObject obj2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//obj2.transform.position = new Vector3(-10, 0.1f, 100);
		catPoints.Add(new Vector3(-10, 0.1f, 100));

		//GameObject obj3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//obj3.transform.position = new Vector3(-10, 0.1f, 200);
		catPoints.Add(new Vector3(-10, 0.1f, 200));
		
		//GameObject obj4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//obj4.transform.position = new Vector3(-10, 0.1f, 300);
		catPoints.Add(new Vector3(-10, 0.1f, 300));

		CatmullRom(catPoints, catOut, 100);

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Takes an array of input coordinates used to define a Catmull-Rom spline, and then
	/// samples the resulting spline according to the specified sample count (per span),
	/// populating the output array with the newly sampled coordinates. The returned boolean
	/// indicates whether the operation was successful (true) or not (false).
	/// NOTE: The first and last points specified are used to describe curvature and will be dropped
	/// from the resulting spline. Duplicate them if you wish to include them in the curve.
	/// </summary>
	public static bool CatmullRom(List<Vector3> inCoordinates, List<Vector3> outCoordinates, int samples)
	{
		if (inCoordinates.Count < 4)
		{
			outCoordinates = null;
			return false;
		}
		
		for (int n = 1; n < inCoordinates.Count - 2; n++)
			for (int i = 0; i < samples; i++)
				outCoordinates.Add(PointOnCurve(inCoordinates[n - 1], inCoordinates[n], inCoordinates[n + 1], inCoordinates[n + 2], (1f / samples) * i ));
		
		outCoordinates.Add(inCoordinates[inCoordinates.Count - 2]);
		
		return true;
	}
	
	/// <summary>
	/// Return a point on the curve between P1 and P2 with P0 and P4 describing curvature, at
	/// the normalized distance t.
	/// </summary>
	
	public static Vector3 PointOnCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 result = new Vector3();
		
		float t0 = ((-t + 2f) * t - 1f) * t * 0.5f;
		float t1 = (((3f * t - 5f) * t) * t + 2f) * 0.5f;
		float t2 = ((-3f * t + 4f) * t + 1f) * t * 0.5f;
		float t3 = ((t - 1f) * t * t) * 0.5f;
		
		result.x = p0.x * t0 + p1.x * t1 + p2.x * t2 + p3.x * t3;
		result.y = p0.y * t0 + p1.y * t1 + p2.y * t2 + p3.y * t3;
		result.z = p0.z * t0 + p1.z * t1 + p2.z * t2 + p3.z * t3;
		
		return result;
	}
}
