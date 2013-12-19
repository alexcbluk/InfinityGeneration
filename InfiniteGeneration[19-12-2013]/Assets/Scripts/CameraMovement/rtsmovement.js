#pragma strict
private var mousePosX : int;
private var mousePosY : int;
var scrollDistance : int;
var scrollSpeed : float;

function Start () {
/*(mousePosX = Input.mousePosition.x;
mousePosY = Input.mousePosition.y;
scrollDistance = 300;
scrollSpeed = 1; */
}

function Update () {
	mousePosX = Input.mousePosition.x;
	mousePosY = Input.mousePosition.y;
	if (mousePosX < scrollDistance) { 
	transform.Translate(Vector3.right * -scrollSpeed * Time.deltaTime, Space.World); 
	}
	if (mousePosX >= Screen.width - scrollDistance) { 
	transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime, Space.World); 
	}
	
	if (mousePosY < scrollDistance) { 
	transform.Translate(Vector3.forward * -scrollSpeed * Time.deltaTime, Space.World); 
	} 
	if (mousePosY >= Screen.height - scrollDistance) {
	transform.Translate(Vector3.forward * scrollSpeed * Time.deltaTime, Space.World); 
	}
	
	if(Input.GetAxis("Mouse ScrollWheel")){
    transform.Translate(Vector3.up * scrollSpeed *100 * Time.deltaTime *-Input.GetAxis("Mouse ScrollWheel"), Space.World); 
    }
}