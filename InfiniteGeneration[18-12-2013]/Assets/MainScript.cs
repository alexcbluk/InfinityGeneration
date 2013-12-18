using UnityEngine;
using System.Collections;

public class MainScript : MonoBehaviour {

	LSystem lsystem;
	drawBezierCurve bezier;

	// Use this for initialization
	void Start () {
		lsystem = gameObject.GetComponent<LSystem> ();
		bezier = gameObject.GetComponent<drawBezierCurve> ();

		lsystem.iterate();
		lsystem.drawPoints ();

		bezier.drawRoads ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
