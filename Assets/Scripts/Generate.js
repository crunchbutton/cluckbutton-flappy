#pragma strict

var rocks : GameObject;

function Start () {
	InvokeRepeating("CreateObstacle", Score.obstacleSpeedOffset, Score.obstacleSpeed);
}

function CreateObstacle () {
	Instantiate(rocks);
}