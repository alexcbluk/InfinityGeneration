using UnityEngine;
using System.Collections;

public class bezierPointBehaviour : MonoBehaviour
{

    void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }
}
