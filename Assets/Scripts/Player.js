#pragma strict

var jumpForce : Vector2 = Vector2.zero;
var jump : AudioClip;
var gameOverSound : AudioClip;
var isDying : byte = 0;
var BGM: GameObject;

function Awake() {
	isDying = 0;
	BGM = GameObject.Find("BGM");
	
	if (Screen.dpi < 320) {
		var scale = transform.localScale.x * .8;
		transform.localScale = Vector3(scale, scale, scale);
	}
}

function Start() {
	BGM.audio.Play();
	//rigidbody2D.velocity = Vector3(Score.speed * 60,0,0);
}

function FadeOutSound() {
	BGM.audio.volume = 0;
	/*
    if (BGM) {
        // wait 4 seconds then fades for 4.5s
        for (var i = 9; i > 0; i--){
          BGM.audio.volume = i * .1;
          yield new WaitForSeconds (.1);
        }
        BGM.audio.volume = 0;
    }
    */
}

function Jump() {
	audio.PlayOneShot(jump, 1);
	//rigidbody2D.velocity = Vector3(Score.speed * 60,0,0);
	rigidbody2D.velocity = Vector3.zero;
	rigidbody2D.AddForce(jumpForce);
}

function Update () {
	if (isDying) {
		return;
	}

	// scroll
	transform.position = new Vector3(transform.position.x + Score.speed, transform.position.y, transform.position.z);
	/*
	var nextPosition : Vector3 = new Vector3(transform.position.x + Score.speed, transform.position.y, transform.position.z);
	
	
//	transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * 100);
	
	var smooth : Vector3 = Vector3.zero;
	var speed : float = Score.speed / 1000;
	transform.position = Vector3.SmoothDamp(transform.position, nextPosition, smooth, speed);
	
	*/
	// Jump
	if (Input.GetMouseButtonDown(0) || Input.GetKeyUp("space")) {
		Jump();
	} else if (Input.touchCount > 0) {
		for(var i : int = 0; i < Input.touchCount; i++) {
			if (Input.touches[i].phase == TouchPhase.Began) {
				Jump();
				break;
			}
		}
	}


	// Die by being off screen
	var screenPosition : Vector2 = Camera.main.WorldToScreenPoint(transform.position);
	
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
		Die();
	}
}

function OnCollisionEnter2D() {
	Die();
}

function Die() {
	if (isDying) {
		return;
	}
	isDying = 1;
	FadeOutSound();
	audio.PlayOneShot(gameOverSound);
	yield WaitForSeconds(1.5);
	Application.LoadLevel("GameOver");
}


//spiteRenderer.sprite.textureRect.width / spiteRenderer.bounds.size.x (or from height / y  ) at a scale = 1