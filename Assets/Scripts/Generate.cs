
/*
using UnityEngine;
using System.Collections;

public class Generate : MonoBehaviour {
	
	public GameObject[] objects;
	
	void Start () {
		InvokeRepeating("CreateObstacle", Score.obstacleSpeedOffset, Score.obstacleSpeed);
	}

	void CreateObstacle () {
		float i = Random.value * objects.Length;
		GameObject item = objects[(int)Mathf.Floor(i)];
		Instantiate(item);
	}
}
*/