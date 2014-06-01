#pragma strict

function Update () {
	transform.position = new Vector3(transform.position.x + Score.speed, transform.position.y, transform.position.z);
}
