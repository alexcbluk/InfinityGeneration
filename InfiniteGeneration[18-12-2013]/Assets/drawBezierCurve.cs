using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class drawBezierCurve : MonoBehaviour 
{
    public LineRenderer line = new LineRenderer();
    public int a;
    public float curVarValue;
    public int lineRes;
	public Texture texture;


    void Start()
    {
		a = 0;
		lineRes = 200;
		line = gameObject.AddComponent<LineRenderer>();
		line.material = new Material(Shader.Find("Particles/Additive"));
		line.SetWidth(15.0F, 15.0F);
		line.material.mainTexture = texture; 
		//line.SetColors(Color.green, Color.green);
		line.SetVertexCount(lineRes);

		/*
		Vector3[] allPoints = new Vector3[]{};
		allPoints = getControlPoints().ToArray();
		SplineBuilder crs = new SplineBuilder(allPoints);
		
		for (int p = 0; p < lineRes; p++)
		{
			Vector3 temp = crs.Interp((float)p / lineRes);// points
			line.SetPosition(p, temp); //points
		}
		*/
		drawRoads ();
		Debug.Log ("Beizer Curve!");
    }

    List<Vector3> getControlPoints()
    {
        List<Vector3> templist = new List<Vector3>();

        foreach (Transform child in transform)
        {
            templist.Add(new Vector3(child.transform.position.x, child.transform.position.y, child.transform.position.z));
        }
        return templist;
    }

    void Update()
    {

    }

	public void drawRoads(){
		Vector3[] allPoints = new Vector3[]{};
		allPoints = getControlPoints().ToArray();
		SplineBuilder crs = new SplineBuilder(allPoints);
		
		for (int p = 0; p < lineRes; p++)
		{
			Vector3 temp = crs.Interp((float)p / lineRes);// points
			line.SetPosition(p, temp); //points
		}
	}
}


public class SplineBuilder
{
    public Vector3[] pts;

    public SplineBuilder(params Vector3[] pts)
    {
        this.pts = new Vector3[pts.Length];
        System.Array.Copy(pts, this.pts, pts.Length);
    }


    public Vector3 Interp(float t)
    {
        int numSections = pts.Length - 3;
        int curPoint = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)curPoint;

        Vector3 a = pts[curPoint];
        Vector3 b = pts[curPoint + 1];
        Vector3 c = pts[curPoint + 2];
        Vector3 d = pts[curPoint + 3];

        return .5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
    }
}