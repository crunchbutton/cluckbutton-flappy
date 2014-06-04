#pragma strict


function Update() {
	transform.position = new Vector3(transform.position.x + Score.speed, transform.position.y, transform.position.z);
}

/*
function Start() {
	//rigidbody2D.gravityScale = 0;
	//rigidbody2D.velocity = Vector3(Score.speed * 60,0,0) ;
	//InvokeRepeating("MoveCamera", 0f, 1f);
	
}


function MoveCamera () {
//	transform.position = new Vector3(transform.position.x + Score.speed, transform.position.y, transform.position.z);

	var nextPosition : Vector3 = new Vector3(transform.position.x + 1000, transform.position.y, transform.position.z);
	var smooth : Vector3 = Vector3.zero;
	var speed : float = 1f;
	transform.position = Vector3.SmoothDamp(transform.position, nextPosition, smooth, speed);
}
*/