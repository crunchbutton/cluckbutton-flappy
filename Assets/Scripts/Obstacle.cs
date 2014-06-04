using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {
	public float range  = 6;
	
	void Start () {
		transform.position = new Vector3(Camera.main.transform.position.x + .5F, (Camera.main.transform.position.y / 2) - range * Random.value, transform.position.z);
	}
}
