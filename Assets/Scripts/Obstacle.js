#pragma strict

//var velocity : Vector2 = new Vector2(-4, 0);
var range : float = 6;

function Start () {
	//rigidbody2D.velocity = velocity;
	transform.position = new Vector3(Camera.main.transform.position.x + .5F, (Camera.main.transform.position.y / 2) - range * Random.value, transform.position.z);
}