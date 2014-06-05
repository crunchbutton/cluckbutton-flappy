using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Vector2 jumpForce = Vector2.zero;
	public AudioClip gameOverSound;
	public AudioClip jump;

	public bool isDying = false;

	public Main mainObject;

	
	void Awake() {
		isDying = false;

		if (Screen.dpi < 320) {
			float scale = (float)(transform.localScale.x * .8);
			transform.localScale = new Vector3(scale, scale, scale);
		}
	}
	
	void Start() {

		//rigidbody2D.velocity = Vector3(Main.speed * 60,0,0);
	}
	
	public void Jump() {
		if (Main.isPlaying) {
			audio.PlayOneShot(jump, 1);
		}
		//rigidbody2D.velocity = Vector3(Main.speed * 60,0,0);
		rigidbody2D.velocity = Vector3.zero;
		rigidbody2D.AddForce(jumpForce);
	}
	
	void Update () {
		if (isDying || !Main.isPlaying) {
			return;
		}
		
		// scroll
		//transform.position = new Vector3((float)(transform.position.x + Main.speed), transform.position.y, transform.position.z);
		/*
	var nextPosition : Vector3 = new Vector3(transform.position.x + Main.speed, transform.position.y, transform.position.z);
	
	
//	transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * 100);
	
	var smooth : Vector3 = Vector3.zero;
	var speed : float = Main.speed / 1000;
	transform.position = Vector3.SmoothDamp(transform.position, nextPosition, smooth, speed);
	
	*/
		// Jump
		if (Input.GetMouseButtonDown(0) || Input.GetKeyUp("space")) {
			Jump();
		} else if (Input.touchCount > 0) {
			for (int i = 0; i < Input.touchCount; i++) {
				if (Input.touches[i].phase == TouchPhase.Began) {
					Jump();
					break;
				}
			}
		}
		
		
		// Die by being off screen
		Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
		
		if (screenPosition.y > Screen.height) {
			//transform.position.y = 8;
			/*
		
		Camera.main.WorldToScreenPoint (transform.position);
		
		var wrld : Vector3 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.height, 0.0f, 0.0f));
		var half_sz : float = gameObject.renderer.bounds.size.y/2;
		var max : float = (wrld.y - half_sz);
		if (transform.position.y > 7) {
			transform.position.y = 7;
			Debug.Log("Moving");
			Debug.Log(transform.position.y);
			Debug.Log(half_sz);
			Debug.Log(wrld.y);
		}
		*/
			
			
			
			
		}
		
		if (screenPosition.y < 0 || screenPosition.y > Screen.height) {
			if (!isDying) {
				StartCoroutine (Die ());
			}
		}
	}
	
	void OnCollisionEnter2D() {
		if (!isDying) {
			StartCoroutine (Die ());
		}
	}
	
	IEnumerator Die() {
		//if (!isDying) {
			isDying = true;
			Main.BGM.audio.Stop();
			audio.PlayOneShot(gameOverSound);
		Debug.Log("RESET 1");
			yield return new WaitForSeconds(1.5f);
			Debug.Log("RESET 2");
			mainObject.reset();
//			return;
			//Application.LoadLevel("GameOver");
		//}
	}
	
	
	//spiteRenderer.sprite.textureRect.width / spiteRenderer.bounds.size.x (or from height / y  ) at a scale = 1
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
