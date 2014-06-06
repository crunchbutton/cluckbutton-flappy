using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;


public class Main : MonoBehaviour {

	public GUIStyle hiScoreStyle;
	public GUIStyle scoreStyle;
	public GUIStyle logoStyle;
	public GUIStyle promoTitleStyle;
	public GUIStyle promoDescriptionStyle;
	public GameObject[] obstacles;

	public Player playerObject;

	public GUIStyle startButton;
	public GUIStyle promoButton;
	public GUIStyle connectButton;
	public GUIStyle promoBackButton;
	
	private int score = 0;
	private int scoreToSkip = 2;
	private double sourceSpeed = .16;
	private float obstacleSourceSpeed = 2f;
	private int incrimentAt = 20;
	private bool hasPlayed = false;
	private float uiScale;
	private int bestScore = 0;
	private bool promoPage = false;

	private double dpi;

	public static GameObject BGM;

	public static float obstacleSpeed;
	public static float obstacleSpeedOffset = 1f;

	public float lastObstacleSpeed = 0f;
	public static double speed;

	private string GUID;

	private string labelTitle;
	private string labelPlay;

	public static bool isPlaying = false;

	private bool FBInitSuccess = false;
	private string FBuid = "";

	private bool isCBUser = false;

	private string api = "http://cluckbutton.com/api/";



	private Dictionary<string, object> promoData = new Dictionary<string, object>();


	void Update() {
		/*
		if (!isPlaying && !hasPlayed) {

			if (Input.GetMouseButtonDown(0) || Input.GetKeyUp("space")) {
				play();
			} else if (Input.touchCount > 0) {
				for (int i = 0; i < Input.touchCount; i++) {
					if (Input.touches[i].phase == TouchPhase.Began) {
						play();
						break;
					}
				}
			}

		}
		*/

		// move the camera
		double newSpeed = Camera.main.transform.position.x + speed;
		Camera.main.transform.position = new Vector3((float)newSpeed, Camera.main.transform.position.y, Camera.main.transform.position.z);
		//Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3((float)newSpeed, Camera.main.transform.position.y, Camera.main.transform.position.z), 5f * Time.deltaTime);

		// move the player
		if (!playerObject.isDying) {
			playerObject.transform.position = new Vector3((float)(playerObject.transform.position.x + speed), playerObject.transform.position.y, playerObject.transform.position.z);
			//playerObject.transform.position = Vector3.Lerp(playerObject.transform.position, new Vector3((float)(playerObject.transform.position.x + speed), playerObject.transform.position.y, playerObject.transform.position.z), 5f * Time.deltaTime);
		}
	}
	
	void Awake() {

		GUID = PlayerPrefs.GetString ("GUID");
		bestScore = PlayerPrefs.GetInt ("BestScore");

		dpi = Screen.dpi < 1 ? 100 : (double)Screen.dpi;

		if (GUID == "") {
			System.Guid newGUID = System.Guid.NewGuid();
			GUID = newGUID.ToString();
			PlayerPrefs.SetString("GUID", GUID);
		}

		if (Application.platform == RuntimePlatform.Android) {
			sourceSpeed = speed = .19;
		}

			speed = sourceSpeed = sourceSpeed * (dpi / 320);
			
			if (speed < .08) {
				speed = sourceSpeed = .08;
			}




		if (dpi >= 320) {
			//speed = sourceSpeed = sourceSpeed * 2;
			//obstacleSpeed = obstacleSourceSpeed = 1.8f;
			obstacleSpeed = obstacleSourceSpeed;
		} else {
			obstacleSpeed = obstacleSourceSpeed;
		}

		if (dpi < 101) {
			//uiScale = 320 / Screen.dpi;
			uiScale = .8f;
		} else {
			uiScale = 1f;
		}

		logoStyle.fontSize = (int)(logoStyle.fontSize * uiScale);
		scoreStyle.fontSize = (int)(scoreStyle.fontSize * uiScale);
		promoTitleStyle.fontSize = (int)(promoTitleStyle.fontSize * uiScale);
		promoDescriptionStyle.fontSize = (int)(promoDescriptionStyle.fontSize * uiScale);

		hiScoreStyle.fontSize = (int)(hiScoreStyle.fontSize * uiScale);
		hiScoreStyle.fixedWidth = Screen.width;

		startButton.fontSize = (int)(startButton.fontSize * uiScale);
		startButton.fixedWidth = (int)(startButton.fixedWidth * uiScale);

		promoButton.fontSize = (int)(promoButton.fontSize * uiScale);
		promoButton.fixedWidth = (int)(promoButton.fixedWidth * uiScale);

		connectButton.fontSize = (int)(connectButton.fontSize * uiScale);
		connectButton.fixedWidth = (int)(connectButton.fixedWidth * uiScale);

		promoBackButton.fontSize = (int)(promoBackButton.fontSize * uiScale);
		promoBackButton.fixedWidth = (int)(promoBackButton.fixedWidth * uiScale);




		BGM = GameObject.Find("BGM");
		try {
			StartCoroutine (getConfig ());
		} catch(System.Exception e) {
			Debug.Log("big bad fail");
		}
		FB.Init(FBInitComplete);

		reset (true);
	}
	
	void OnGUI () {



		if (promoPage && promoEnabled()) {

			if (FBuid == "") {

				GUI.Label (new Rect (Screen.width/2-50, Screen.height/2-(200 * uiScale), 100, 100 * uiScale), getPromoValue("title"), promoTitleStyle);
				GUI.Label (new Rect (Screen.width/2-250, Screen.height/2-(50 * uiScale), 100, 100 * uiScale), getPromoValue("description"), promoDescriptionStyle);

				if (GUI.Button (new Rect ((Screen.width/2)-(connectButton.fixedWidth/2), Screen.height/2+(100*uiScale), connectButton.fixedWidth, 74 * uiScale), "Connect", connectButton)) {
					FB.Login("", FBLoginComplete);
				}
			} else {

				if (!isCBUser) {
					GUI.Label (new Rect (Screen.width/2-250, Screen.height/2-(80 * uiScale), 100, 100 * uiScale), "Please download the Crunchbutton app and log in with Facebook. A credit will be applied automagically.", promoDescriptionStyle);
					if (GUI.Button (new Rect ((Screen.width/2)-(promoButton.fixedWidth/2), Screen.height/2+(100*uiScale), promoButton.fixedWidth, 74 * uiScale), "Download", promoButton)) {

						if (Application.platform == RuntimePlatform.IPhonePlayer) {
							Application.OpenURL("itms://itunes.apple.com/app/id721780390");
						} else {
							Application.OpenURL("http://crunchbutton.com/app");
						}


					
					}
				} else {
					GUI.Label (new Rect (Screen.width/2-250, Screen.height/2-(50 * uiScale), 100, 100 * uiScale), getPromoValue("success"), promoDescriptionStyle);
				}
			}

			//if (GUI.Button (new Rect (15, Screen.height-(85 * uiScale), 40, 70 * uiScale), "back", promoBackButton)) {
			//	promoPage = false;
			//}

			if (GUI.Button (new Rect (15, Screen.height-(85*uiScale), 150, 70 * uiScale), "back", promoBackButton)) {
				promoPage = false;
			}
			
			
		} else {
			if (hasPlayed) {
				GUILayout.Label("Score: " + getScore().ToString(), scoreStyle);
				hiScoreStyle.contentOffset = new Vector2(0f, -(hiScoreStyle.fontSize * uiScale + (hiScoreStyle.padding.top * 2)));
			}

			if (bestScore > 0) {
				GUILayout.Label("Best: " + bestScore.ToString(), hiScoreStyle);
			}

			if (!isPlaying) {
				if (hasPlayed) {
					labelTitle = "Game Over";
					labelPlay = "Try Again";
				} else {
					labelTitle = "Cluck Button";
					labelPlay = "Start Clucking";
				}

				GUI.Label (new Rect (Screen.width/2-50, Screen.height/2-(150 * uiScale), 100, 100 * uiScale), labelTitle, logoStyle);

				if (GUI.Button (new Rect ((Screen.width/2)-(startButton.fixedWidth/2), Screen.height/2+(30*uiScale), startButton.fixedWidth, 74 * uiScale), labelPlay, startButton)) {
					play();
				}

				if (hasPlayed && promoEnabled()) {
					if (GUI.Button (new Rect ((Screen.width/2)-(promoButton.fixedWidth/2), Screen.height/2+(120*uiScale), promoButton.fixedWidth, 74 * uiScale), getPromoValue("button"), promoButton)) {
						promoPage = true;
					}
				}

			}
		}
	}

	void UpdateScore() {
		score++;

		// add obstacle
		float i = Random.value * obstacles.Length;
		GameObject item = obstacles[(int)Mathf.Floor(i)];
		Instantiate(item);

		// update the scroll speed
		if (speed < sourceSpeed * 2) {
			speed = sourceSpeed + (getScore() / incrimentAt) * .01;
		}

		// update the obstacle speed
		if (obstacleSpeed > 1) {
			float newSpeed;

			newSpeed = (float)(obstacleSourceSpeed - (getScore() / incrimentAt) * .1);

			if (newSpeed < obstacleSpeed) {
				obstacleSpeed = newSpeed;
				CancelInvoke ("UpdateScore");
				InvokeRepeating("UpdateScore", obstacleSpeed, obstacleSpeed);

			}
		}

	}
	
	int getScore() {
		return score > scoreToSkip ? score - scoreToSkip : 0;
	}

	// call to play
	void play() {
		if (isPlaying) {
			return;
		}
		Debug.Log ("Starting scene...");
		hasPlayed = true;
		isPlaying = true;
		InvokeRepeating("UpdateScore", obstacleSpeedOffset, obstacleSpeed);
		CancelInvoke ("FakeFly");
		BGM.audio.Play();
		score = 0;
		
		playerObject.rigidbody2D.gravityScale = 1;

	}

	// call after dying
	public void reset(bool force = false) {
		if (!isPlaying && !force) {
			return;
		}
		Debug.Log ("Resetting scene...");
		playerObject.rigidbody2D.gravityScale = .02f;

		InvokeRepeating("FakeFly", 2f, 4.07f);

		BGM.audio.Stop ();
		isPlaying = false;
		if (getScore() > bestScore) {
			PlayerPrefs.SetInt("BestScore", getScore());
			bestScore = getScore();
		}
		PlayerPrefs.SetInt("LastScore", getScore());

		CancelInvoke ("UpdateScore");
		speed = sourceSpeed;
		obstacleSpeed = obstacleSourceSpeed;
		playerObject.isDying = false;
		playerObject.rigidbody2D.velocity = Vector3.zero;
		playerObject.rigidbody2D.angularVelocity = 0f;

		playerObject.transform.position = new Vector3(Camera.main.transform.position.x-7f,5.4f,playerObject.transform.position.z);
		playerObject.transform.rotation = new Quaternion (0f,0f,0f,0f);

		foreach(GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle")) {
			Destroy (obstacle);
		}

		if (!force) {

			//try {

				StartCoroutine (reportScore());
			//} catch (System.Exception e) {
			//}
		}

	}


	public static void stopBGM() {

	}

	private IEnumerator getConfig() {
		Debug.Log ("Getting config...");

		string promo = PlayerPrefs.GetString("Promo");

		if (promo == "") {
			// use the default promo
			//promo = '{"id":1,"exp":"2015-01-01","value":"$2","button":"Get Free Delivery Food","title":"Free delivery from Crunchbutton","description":"Connect your facebook account to Cluckbutton and Crunchbutton and get your next delivery fee is on us","img":null}';
		}

		if (promo != "") {
			promoData = Json.Deserialize(promo) as Dictionary<string, object>;
		}

		WWW www = new WWW(api + "/config");
		yield return www;
		
		if (!string.IsNullOrEmpty (www.text)) {
			promo = www.text;
			promoData = Json.Deserialize(promo) as Dictionary<string, object>;
		}
		
		if (promoData["exp"].ToString() != "") {
			System.DateTime promoDate;
			System.DateTime currentTime = System.DateTime.Now;
			bool validDate = System.DateTime.TryParse(promoData["exp"].ToString(), out promoDate);

			int result = 0;
			
			if (validDate) {
				result = System.DateTime.Compare(currentTime, promoDate);
			}

			if (result > 0 || !validDate) {
				promoData = new Dictionary<string, object>();
				promo = "";
			}
		}

		//if (!string.IsNullOrEmpty (promoData)) {
			PlayerPrefs.SetString("Promo", promo);
		//}
	}

	private void FakeFly() {
		//playerObject.GetComponent ("Script").Jump ();
		var jumpForce = new Vector2 (0,20);
		playerObject.rigidbody2D.velocity = Vector3.zero;
		playerObject.rigidbody2D.AddForce(jumpForce);
		//Player.Jump ();
	}

	IEnumerator reportScore() {

		Debug.Log ("Reporting Score: " + score.ToString ());
		Debug.Log (getScore ());

		WWWForm form = new WWWForm();
		form.AddField("fb", FB.UserId, System.Text.Encoding.UTF8);
		form.AddField("score", getScore().ToString(), System.Text.Encoding.UTF8);
		form.AddField("guid", GUID, System.Text.Encoding.UTF8);
		form.AddField("screen-height", Screen.height.ToString(), System.Text.Encoding.UTF8);
		form.AddField("screen-width", Screen.width.ToString(), System.Text.Encoding.UTF8);
		form.AddField("screen-dpi", Screen.dpi.ToString(), System.Text.Encoding.UTF8);
		form.AddField("device", SystemInfo.deviceModel, System.Text.Encoding.UTF8);
		WWW www = new WWW(api + "score", form);
		
		yield return www;
		
		if (!string.IsNullOrEmpty(www.text)) {
			Debug.Log(www.text);
		}
	}
	/*
	public static string GetUniqueID(){
		string key = "ID";
		
		var random = new System.Random();              
		DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
		
		string uniqueID = Application.systemLanguage                 //Language
			+"-"+GetPlatform()                           //Device   
				+"-"+String.Format("{0:X}", Convert.ToInt32(timestamp))          //Time
				+"-"+String.Format("{0:X}", Convert.ToInt32(Time.time*1000000))     //Time in game
				+"-"+String.Format("{0:X}", random.Next(1000000000));          //random number
		
		Debug.Log("Generated Unique ID: "+uniqueID);
		
		if(PlayerPrefs.HasKey(key)){
			uniqueID = PlayerPrefs.GetString(key);      
		} else {       
			PlayerPrefs.SetString(key, uniqueID);
			PlayerPrefs.Save();  
		}
		
		return uniqueID;
	}
	*/

	void FBInitComplete() {
		FBInitSuccess = true;
		if (!string.IsNullOrEmpty(FB.UserId)) {
			StartCoroutine(checkUser());
		}
	}

	void FBLoginComplete(FBResult result) {
		Debug.Log ("User");
		if (result.Error != null) {
			Debug.Log(result.Error);
		} else if (!FB.IsLoggedIn) {
			Debug.Log("canceled");
		} else {
			Debug.Log(result);

			StartCoroutine(checkUser());
		}
	}

	IEnumerator checkUser() {
		Debug.Log ("checking user");
		WWWForm form = new WWWForm();
		form.AddField("fb", FB.UserId, System.Text.Encoding.UTF8);
		form.AddField("guid", GUID, System.Text.Encoding.UTF8);
		form.AddField("screen-height", Screen.height.ToString(), System.Text.Encoding.UTF8);
		form.AddField("screen-width", Screen.width.ToString(), System.Text.Encoding.UTF8);
		form.AddField("screen-dpi", Screen.dpi.ToString(), System.Text.Encoding.UTF8);
		form.AddField("device", SystemInfo.deviceModel, System.Text.Encoding.UTF8);
		WWW www = new WWW(api + "check-user", form);
		
		yield return www;
		
		if (!string.IsNullOrEmpty(www.text)) {
			Dictionary<string, object> userCheck = Json.Deserialize(www.text) as Dictionary<string, object>;
			Debug.Log(userCheck["status"].ToString());
			if (userCheck["status"].ToString() == "True") {
				isCBUser = true;
			} else {
				isCBUser = false;
			}
			FBuid = FB.UserId;
		}
	}

	bool promoEnabled() {
		if (FBInitSuccess && getPromoValue("id") != "") {
			return true;
		}
		return false;
	}

	string getPromoValue(string key) {

		try {
			if (promoData.ContainsKey(key)) {
				return promoData[key].ToString();
			}
		
		} catch (UnityException e) {

		} catch (System.Exception e) {

		}
		return "";
	}


}