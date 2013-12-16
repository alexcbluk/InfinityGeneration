using UnityEngine;
using System.Collections;

public class OutputBezier : MonoBehaviour {
	BezierCurveFunction bezier;
	private float t = 0.0f;
	// Use this for initialization
	void Start () {
		bezier = new BezierCurveFunction(new Vector3(-5,0,0), (Random.insideUnitSphere * 5.0f), (Random.insideUnitSphere * 2.0f), new Vector3(5,0,0));

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 vec = bezier.GetPointAtTime(t);
	    transform.position = vec;
	
	    t += 0.001f;
	
	    if (t > 1.0f)
	        t = 0.0f;
		
	}
	
}
