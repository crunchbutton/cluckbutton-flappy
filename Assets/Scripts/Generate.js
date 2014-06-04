#pragma strict

public var objects : GameObject[];

function Start () {
	InvokeRepeating("CreateObstacle", Score.obstacleSpeedOffset, Score.obstacleSpeed);
}

function CreateObstacle () {
	var item = objects[Mathf.Floor(Random.value * objects.length)];
	Instantiate(item);
}