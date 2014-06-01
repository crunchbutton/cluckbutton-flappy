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
		var scale = .8f;
		this.transform.localScale = Vector3(scale, scale, scale);
	}
}

function Start() {
	BGM.audio.Play();
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
	rigidbody2D.velocity = Vector2.zero;
	rigidbody2D.AddForce(jumpForce);
}

function Update () {
	if (isDying) {
		return;
	}

	// Jump
	transform.position = new Vector3(transform.position.x + Score.speed, transform.position.y, transform.position.z);
	
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
	if (screenPosition.y > Screen.height || screenPosition.y < 0) {
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