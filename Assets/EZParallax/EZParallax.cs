using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//************************************************************************************************************************************************
//**EZ Parallax script version 1.1 by TimeFloat. Thanks for purchasing! Feel free to scroll to the bottom for user facing funtions.             **
//************************************************************************************************************************************************

[System.Serializable]
public class EZParallaxObjectElement
{
	public string name;
	[SerializeField,]
	private Transform m_parallaxObject;
	public Transform parallaxObject
	{
		set
		{ 	
			m_parallaxObject = value;
			needsNewScreenspaceExtents = true;
		}
		
		get
		{
			return m_parallaxObject;	
		}
	}
	public float privateParallaxSpeedScalarX = 1.0f;
	public float privateParallaxSpeedScalarY = 1.0f;
	public bool isMotorized = false;
	public float motorSpeed { get { return initialMotorSpeed * scale; } }
	
	public bool spawnsDuplicateOnX = false;
	public bool randomSpawnX = false;
	public int spawnGroupIndex = 0;
	public float spawnDistanceX { get { return initialSpawnDistanceX * scale; } }
	public float spawnDistanceMinX { get { return initialSpawnDistanceMinX * scale; } }
	public float spawnDistanceMaxX { get { return initialSpawnDistanceMaxX * scale; } }
	public float spawnDistanceScreenX { get { return initialSpawnDistanceScreenX * scale; } }
	public float spawnDistanceScreenMinX { get { return initialSpawnDistanceScreenMinX * scale; } }
	public float spawnDistanceScreenMaxX { get { return initialSpawnDistanceScreenMaxX * scale; } }
	public float rightSpawnDistanceX { get { return initialRightSpawnDistanceX * scale; } }
	public float leftSpawnDistanceX { get { return initialLeftSpawnDistanceX * scale; } }
	public float rightSpawnDistanceScreenSpaceX { get { return initialRightSpawnDistanceScreenSpaceX * scale; } }
	public float leftSpawnDistanceScreenSpaceX { get { return initialLeftSpawnDistanceScreenSpaceX * scale; } }
	public int numElementsInSeriesX = 0;
	
	public float meshExtentX { get { return initialMeshExtentX * scale; } }
	public float meshExtentY { get { return initialMeshExtentY * scale; } }
	public float meshWidth { get { return meshExtentX * 2.0f; } }
	public float meshHeight { get { return meshExtentY * 2.0f; } }
	
	public bool hasCustomName;
	public bool needsNewScreenspaceExtents = false;
	public float elementScreenSpaceExtentX { get { return initialElementScreenSpaceExtentX * scale; } }
	public float elementScreenSpaceExtentY { get { return initialElementScreenSpaceExtentY * scale; } }
	public EZParallaxObjectElement dupeElementRight;
	public EZParallaxObjectElement dupeElementLeft;
	
	private Vector3 initialScale;
	public float initialMotorSpeed = 0;
	public float initialSpawnDistanceX = 0;
	public float initialSpawnDistanceMinX = 0;
	public float initialSpawnDistanceMaxX = 0;
	public float initialSpawnDistanceScreenX = 0;
	public float initialSpawnDistanceScreenMinX = 0;
	public float initialSpawnDistanceScreenMaxX = 0;
	public float initialRightSpawnDistanceX = 0;
	public float initialLeftSpawnDistanceX = 0;
	public float initialRightSpawnDistanceScreenSpaceX = 0;
	public float initialLeftSpawnDistanceScreenSpaceX = 0;
	private float initialMeshExtentX;
	private float initialMeshExtentY;
	public float initialElementScreenSpaceExtentX;
	public float initialElementScreenSpaceExtentY;
	
	public bool isDupe = false;
	
	public float scale = 1.0f;
	
	public DupeChainHandle dupeChainObject = null;
	
	public EZParallaxObjectElement(Transform targetTransform)
	{
		if(targetTransform)
		{
			name = targetTransform.name;
			parallaxObject = targetTransform;
			
			initialSpawnDistanceX = Mathf.Abs(initialSpawnDistanceX);
		}
	}
	
	public void Initialize()
	{
		scale = 1.0f;
		
		if(parallaxObject)
		{
			initialScale = parallaxObject.localScale;
			
			Vector4 myFullMeshBounds = RetrieveBounds(m_parallaxObject);
			initialMeshExtentX = Mathf.Abs( (myFullMeshBounds.x - myFullMeshBounds.z) / 2); //z in this case is the x min
			initialMeshExtentY = Mathf.Abs( (myFullMeshBounds.y - myFullMeshBounds.w) / 2); //w in this case is the y min
		}
	}
	
	public void UpdateScale(float newScaleRatio, GameObject m_mainCamera)
	{
		float scalar;
		Transform targetObject;
		Vector3 baseScale;
		
		if(dupeChainObject != null && dupeChainObject.scaleUpdated == false)
		{
			scalar = newScaleRatio / dupeChainObject.macroScale;
			targetObject = dupeChainObject.targetGameObject.transform;
			baseScale = new Vector3(1, 1, 1);
			dupeChainObject.scaleUpdated = true;
			dupeChainObject.macroScale = newScaleRatio;
		}
		else if(dupeChainObject != null)
		{
			return;
		}
		else
		{
			scalar = newScaleRatio / scale;
			targetObject = parallaxObject;
			baseScale = initialScale;
			scale = newScaleRatio;
		}
		
		targetObject.localScale = new Vector3(baseScale.x * newScaleRatio, baseScale.y * newScaleRatio, baseScale.z);
		
		Vector3 offset = targetObject.position - m_mainCamera.transform.position;
		float z = offset.z;
		offset.z = 0.0f;
		targetObject.position = m_mainCamera.transform.position + offset * scalar + new Vector3(0.0f, 0.0f, z);
	}
	
	private Vector4 RetrieveBounds(Transform targetTransform)
	{
		float? maxBoundX = null;
		float? maxBoundY = null;
		
		float? minBoundX = null;
		float? minBoundY = null;
		
		if(targetTransform.renderer != null)
		{
			Bounds myMeshBounds = targetTransform.renderer.bounds;
			maxBoundX = (float?)myMeshBounds.max.x;
			maxBoundY = (float?)myMeshBounds.max.y;
			minBoundX = (float?)myMeshBounds.min.x;
			minBoundY = (float?)myMeshBounds.min.x;
		}
		
		if(targetTransform.childCount > 0)
		{
			foreach(Transform newTargetTransform in targetTransform)
			{
				Vector4 fullChildBounds = RetrieveBounds(newTargetTransform);
				if(maxBoundX == null)
				{
					maxBoundX = fullChildBounds.x;
				}
				else
				{
					maxBoundX = Mathf.Max(fullChildBounds.x, (float)maxBoundX);
				}
				
				if(maxBoundY == null)
				{
					maxBoundY = fullChildBounds.y;
				}
				else
				{
					maxBoundY = Mathf.Max(fullChildBounds.y, (float)maxBoundY);
				}
				
				if(minBoundX == null)
				{
					minBoundX = fullChildBounds.z;
				}
				else
				{
					minBoundX = Mathf.Min(fullChildBounds.z, (float)minBoundX);
				}
				
				if(minBoundY == null)
				{
					minBoundY = fullChildBounds.w;
				}
				else
				{
					minBoundY = Mathf.Min(fullChildBounds.w, (float)minBoundY);
				}
			}
		}
		
		return new Vector4((float)maxBoundX, (float)maxBoundY, (float)minBoundX, (float)minBoundY);
	}
	
	public void Update(float dt)
	{
		if(isMotorized && m_parallaxObject)
		{
			Vector3 shiftVector = new Vector3(motorSpeed * dt, 0, 0);
			if(dupeChainObject != null && !dupeChainObject.hasMotorShifted)
			{
				dupeChainObject.targetGameObject.transform.position += shiftVector;
				dupeChainObject.hasMotorShifted = true;
			}
			else if(dupeChainObject == null)
			{
				m_parallaxObject.position += shiftVector;
			}
		}
	}
}

public class DupeChainHandle
{
	public bool hasBeenMoved           = false;
	public bool hasMotorShifted        = false;
	public bool scaleUpdated           = false;
	public float macroScale            = 1;
	public GameObject targetGameObject = null;
}

public class EZParallax : MonoBehaviour
{
	public string                    m_parallaxingTagName;
	public string                    m_wrapXParallaxingTagName;
	public GameObject                m_mainCamera;
	public GameObject                m_playerObj;
	public float                     m_parallaxSpeedScalarX = 1;
	public float                     m_parallaxSpeedScalarY = 1;
	public bool                      m_autoInitialize = true;
	public bool                      m_enableDollyZoom = true;
	public EZParallaxObjectElement[] m_parallaxElements;
	private DupeChainHandle[]        m_dupeChainHandles; 
	
	private float                    m_maxDist;
	private float                    m_maxDistDiv;
	private Vector3                  m_camStartVect;
	private float                    m_camStartOrthoSize;
	private float                    m_prevOrthoSize;
	private float                    m_currentOrthoSize;
	private bool                     m_initialized = false;
	
	private Vector3                  m_currentCameraPosition;
	private Vector3                  m_previousCameraPosition;
	
	private int[]                    m_rndDistArrayIndex;
	private int[]                    m_rndElementGroupSize;
	private int                      m_rndDistStartIndex;
	public int                       m_randomOffsetHistorySize = 300;
	private float?[,]                m_rndDistArray;
	private int                      m_randomSpawnCtr = 0;
	
	void Start()
	{
		if(m_autoInitialize){
			InitializeParallax();
		}
	}
	
	//Can be called manually when autoInitialization is disabled.
	public void InitializeParallax()
	{
		if(m_playerObj == null)
		{
			Debug.Log("EZParallax initialized, but a player has not been assigned. Aborting.");
			return;
		}

		if(m_mainCamera == null)
		{
			Debug.Log("EZParallax initialized, but a camera has not been assigned. Aborting.");
			return;
		}

		PurgeDupes();
		m_randomSpawnCtr = 0;
		if(!m_initialized)
		{
			AddTaggedElements();
		}
		
		if(m_parallaxElements.Length == 0)
		{
			Debug.Log("EZParallax initialized, but no objects have been assigned! No parallaxing effects will be present.");
			return;
		}
		
		SqueezeElementsArray();
		int randomSpawningCount = 0;
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			m_parallaxElements[i].Initialize();
			if(m_parallaxElements[i].randomSpawnX)
			{
				randomSpawningCount++;
			}
		}
		m_rndDistStartIndex = Mathf.CeilToInt(m_randomOffsetHistorySize / 2);
		m_rndDistArrayIndex = new int[randomSpawningCount];
		m_rndElementGroupSize = new int[randomSpawningCount];
		m_rndDistArray = new float?[randomSpawningCount, m_randomOffsetHistorySize];
		EstablishMaxDistance();
		m_camStartVect = m_mainCamera.transform.position;
		m_previousCameraPosition = m_camStartVect;
		m_camStartOrthoSize = m_mainCamera.camera.orthographicSize;
		m_currentOrthoSize = m_camStartOrthoSize;
		m_prevOrthoSize = m_camStartOrthoSize;
		SetElementScreenSpaceExtents(m_parallaxElements);
		SpawnDupes();
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			UpdateDupeObjects(m_parallaxElements[i], true);
		}
		m_initialized = true;
	}
	
	void SpawnDupes()
	{
		Camera actualCamera = m_mainCamera.camera;
		float screenWidth = actualCamera.pixelWidth;
		List<EZParallaxObjectElement> elementsToDupe = new List<EZParallaxObjectElement>();
		
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			if(m_parallaxElements[i].spawnsDuplicateOnX)
			{
				elementsToDupe.Add(m_parallaxElements[i]);
			}
		}
		
		
		for(int i = 0; i < elementsToDupe.Count; i++)
		{
			SpawnSingleElementDupes(elementsToDupe[i], actualCamera, screenWidth);
		}
	}
	
	void SpawnSingleElementDupes(EZParallaxObjectElement targetElement, Camera actualCamera, float screenWidth)
	{
		DupeChainHandle dupeChainParent;
		
		if(targetElement.dupeChainObject == null)
		{
			dupeChainParent = new DupeChainHandle();
			dupeChainParent.targetGameObject = new GameObject();
			dupeChainParent.targetGameObject.name = "EZP " + targetElement.name + " Dupe Handle";
			Transform origParent = targetElement.parallaxObject.parent;
			targetElement.parallaxObject.parent = dupeChainParent.targetGameObject.transform;
			dupeChainParent.targetGameObject.transform.parent = origParent;
			targetElement.dupeChainObject = dupeChainParent;
			
			if(m_dupeChainHandles != null)
			{
				DupeChainHandle[] tempDCArray = new DupeChainHandle[m_dupeChainHandles.Length + 1];
				for(int i = 0; i < m_dupeChainHandles.Length; i++)
				{
					tempDCArray[i] = m_dupeChainHandles[i];
				}
				
				tempDCArray[m_dupeChainHandles.Length] = dupeChainParent;
				m_dupeChainHandles = tempDCArray;
			}
			else
			{
				m_dupeChainHandles = new DupeChainHandle[1];
				m_dupeChainHandles[0] = dupeChainParent;
			}
		}
		else
		{
			dupeChainParent = targetElement.dupeChainObject;
		}
		
		float targetElementScreenWidth = targetElement.elementScreenSpaceExtentX * 2;
		Vector2 targetCenterPosScreenPt = actualCamera.WorldToScreenPoint(targetElement.parallaxObject.position);
		//Create an array of randomized offsets and use that array to figure out how wide the initial positioning needs to be, then draw from that array when spawning the individual elements.
		List<float> randomOffsetsList = new List<float>();
		int maxDupes = 0;
		if (targetElement.randomSpawnX)
		{
			//Goof check
			if(targetElement.initialSpawnDistanceMinX > targetElement.initialSpawnDistanceMaxX)
			{
				Debug.Log ("WARNING -- For your " + targetElement.name + " element, your minimum random spawn distance is greater than your maximum random spawn distance. Swapping your minimum for your maximum.");
				float swapVar = targetElement.initialSpawnDistanceMinX;
				targetElement.initialSpawnDistanceMinX = targetElement.initialSpawnDistanceMaxX;
				targetElement.initialSpawnDistanceMaxX = swapVar;
			}
			
			float minScreenSpcOffset = targetCenterPosScreenPt.x - actualCamera.WorldToScreenPoint(targetElement.parallaxObject.position - new Vector3( targetElement.spawnDistanceMinX, 0, targetElement.parallaxObject.position.z)).x;
			int greatestMaxDupesX = Mathf.CeilToInt( ( screenWidth * 3 )/ (targetElementScreenWidth + minScreenSpcOffset)) + 2; // * 3 screenwidth and +2 for buffer objects
			
			for(int k = 0; k < greatestMaxDupesX; k++)
			{
				float randomOffset = Random.Range(targetElement.spawnDistanceMinX, targetElement.spawnDistanceMaxX);
				randomOffsetsList.Add(randomOffset);
				m_rndDistArray[m_randomSpawnCtr, m_rndDistStartIndex + k] = randomOffset;
			}
			
			maxDupes = greatestMaxDupesX;
			m_rndElementGroupSize[m_randomSpawnCtr] = maxDupes; //Does not include the original object that was spawned from
			m_rndDistArrayIndex[m_randomSpawnCtr] = m_rndDistStartIndex + maxDupes - 1; //Subtract 1 to make room for the initial index shift when the character starts moving, pos or neg
			targetElement.spawnGroupIndex = m_randomSpawnCtr;
			m_randomSpawnCtr++;
			
			if(m_rndElementGroupSize[m_randomSpawnCtr - 1] >= m_randomOffsetHistorySize)
			{
				Debug.Log ("An EZParallax element object named " + targetElement.name + " needs to spawn more objects than there are slots in the random offset history! Raise your history size to greater than " + maxDupes + " to resolve this problem. Aborting the creation of duplicate objects for " + targetElement.name + ".");
				return;
			}
		}
		else
		{
			targetElement.initialSpawnDistanceScreenX = targetElement.spawnDistanceScreenX;
			targetElement.initialLeftSpawnDistanceScreenSpaceX = targetElement.spawnDistanceScreenX;
			targetElement.initialRightSpawnDistanceScreenSpaceX = targetElement.spawnDistanceScreenX;
			maxDupes = Mathf.CeilToInt( ( screenWidth * 3 ) / ( targetElementScreenWidth + targetElement.spawnDistanceScreenX ) ) + 2; // +2 buffer objects
			targetElement.spawnGroupIndex = 0;
		}
		
		
		
		int numRightDupes = Mathf.Min( Mathf.CeilToInt( ( (screenWidth * 1.5f) - targetCenterPosScreenPt.x) / targetElementScreenWidth), maxDupes);
		if(numRightDupes < 0)
		{
			numRightDupes = 0;
		}
		int numLeftDupes = maxDupes - numRightDupes;
		if(numLeftDupes < 0)
		{
			numLeftDupes = 0;
		}
		
		//Spawn all objects on the left side
		EZParallaxObjectElement dupeTargetElement = targetElement;
		EZParallaxObjectElement previousElement = targetElement;
		
		for ( int j = 0; j < numLeftDupes; ++j )
		{
			if(dupeTargetElement.dupeElementLeft != null)
			{
				dupeTargetElement = dupeTargetElement.dupeElementLeft;
				continue;
			}
			
			Vector3 objectOffsetVector;
			if(dupeTargetElement.randomSpawnX)
			{
				int offsetIndex = numLeftDupes - 1 - j;
				objectOffsetVector = new Vector3(randomOffsetsList[offsetIndex] + dupeTargetElement.meshWidth, 0, 0);
				dupeTargetElement.initialLeftSpawnDistanceX = randomOffsetsList[offsetIndex];
				dupeTargetElement.initialLeftSpawnDistanceScreenSpaceX = targetCenterPosScreenPt.x - actualCamera.WorldToScreenPoint(dupeTargetElement.parallaxObject.position - (new Vector3(randomOffsetsList[offsetIndex], 0, dupeTargetElement.parallaxObject.position.z))).x;
			}
			else
			{
				objectOffsetVector = new Vector3(targetElement.spawnDistanceX + dupeTargetElement.meshWidth, 0, 0);
				dupeTargetElement.initialLeftSpawnDistanceX = dupeTargetElement.initialSpawnDistanceX;
				dupeTargetElement.initialLeftSpawnDistanceScreenSpaceX = dupeTargetElement.initialSpawnDistanceScreenX;
			}
			Transform leftDupeObject = (Transform)(Instantiate(dupeTargetElement.parallaxObject, dupeTargetElement.parallaxObject.position - objectOffsetVector, dupeTargetElement.parallaxObject.rotation) );
			leftDupeObject.transform.parent =  dupeTargetElement.parallaxObject.parent;
			dupeTargetElement.dupeElementLeft = AddNewParallaxingElement(leftDupeObject);
			dupeTargetElement.dupeElementLeft.Initialize();
			dupeTargetElement.dupeElementLeft.initialRightSpawnDistanceX = dupeTargetElement.leftSpawnDistanceX;
			dupeTargetElement.dupeElementLeft.initialRightSpawnDistanceScreenSpaceX = dupeTargetElement.leftSpawnDistanceScreenSpaceX;
			dupeTargetElement = dupeTargetElement.dupeElementLeft;
			dupeTargetElement.isMotorized = targetElement.isMotorized;
			dupeTargetElement.initialMotorSpeed = targetElement.initialMotorSpeed;
			dupeTargetElement.randomSpawnX = targetElement.randomSpawnX;
			dupeTargetElement.spawnGroupIndex = targetElement.spawnGroupIndex;
			dupeTargetElement.privateParallaxSpeedScalarX = targetElement.privateParallaxSpeedScalarX;
			dupeTargetElement.privateParallaxSpeedScalarY = targetElement.privateParallaxSpeedScalarY;
			dupeTargetElement.isDupe = true;
			dupeTargetElement.dupeChainObject = dupeChainParent;
			dupeTargetElement.dupeElementRight = previousElement;
			previousElement = dupeTargetElement;
		}
		
		if(randomOffsetsList.Count > 0)
		{
			randomOffsetsList.RemoveRange(0, numLeftDupes);
		}
		
		//Spawn all objects on the right side
		dupeTargetElement = targetElement;
		previousElement = targetElement;
		for ( int k = 0; k < numRightDupes; ++k )
		{
			if(dupeTargetElement.dupeElementRight != null)
			{
				dupeTargetElement = dupeTargetElement.dupeElementRight;
				continue;
			}
			
			Vector3 objectOffsetVector;
			if(dupeTargetElement.randomSpawnX)
			{
				objectOffsetVector = new Vector3(randomOffsetsList[k] + dupeTargetElement.meshWidth, 0, 0);
				dupeTargetElement.initialRightSpawnDistanceX = randomOffsetsList[k];
				dupeTargetElement.initialRightSpawnDistanceScreenSpaceX = targetCenterPosScreenPt.x - actualCamera.camera.WorldToScreenPoint(dupeTargetElement.parallaxObject.position - (new Vector3(randomOffsetsList[k], 0, dupeTargetElement.parallaxObject.position.z))).x;
			}
			else
			{
				objectOffsetVector = new Vector3(targetElement.spawnDistanceX + dupeTargetElement.meshWidth, 0, 0);
				dupeTargetElement.initialRightSpawnDistanceX = dupeTargetElement.spawnDistanceX;
				dupeTargetElement.initialRightSpawnDistanceScreenSpaceX = dupeTargetElement.spawnDistanceScreenX;
			}
			
			Transform rightDupeObject = (Transform)(Instantiate(dupeTargetElement.parallaxObject, dupeTargetElement.parallaxObject.position + objectOffsetVector, dupeTargetElement.parallaxObject.rotation) );
			rightDupeObject.transform.parent =  dupeTargetElement.parallaxObject.parent;
			dupeTargetElement.dupeElementRight = AddNewParallaxingElement(rightDupeObject);
			dupeTargetElement.dupeElementRight.Initialize();
			dupeTargetElement.dupeElementRight.initialLeftSpawnDistanceX = dupeTargetElement.rightSpawnDistanceX;
			dupeTargetElement.dupeElementRight.initialLeftSpawnDistanceScreenSpaceX = dupeTargetElement.rightSpawnDistanceScreenSpaceX;
			dupeTargetElement = dupeTargetElement.dupeElementRight;
			dupeTargetElement.isMotorized = targetElement.isMotorized;
			dupeTargetElement.initialMotorSpeed = targetElement.initialMotorSpeed;
			dupeTargetElement.randomSpawnX = targetElement.randomSpawnX;
			dupeTargetElement.spawnGroupIndex = targetElement.spawnGroupIndex;
			dupeTargetElement.privateParallaxSpeedScalarX = targetElement.privateParallaxSpeedScalarX;
			dupeTargetElement.privateParallaxSpeedScalarY = targetElement.privateParallaxSpeedScalarY;
			dupeTargetElement.dupeChainObject = dupeChainParent;
			dupeTargetElement.isDupe = true;
			dupeTargetElement.dupeElementLeft = previousElement;
			previousElement = dupeTargetElement;
		}
	}
	
	void AddTaggedElements()
	{
		GameObject[] taggedElements = GameObject.FindGameObjectsWithTag(m_parallaxingTagName);
		GameObject[] taggedWrapXElements = null;
		
		int totalElementSum = taggedElements.Length;
		if(m_wrapXParallaxingTagName != "" && m_wrapXParallaxingTagName != null)
		{
			taggedWrapXElements = GameObject.FindGameObjectsWithTag(m_wrapXParallaxingTagName);
			totalElementSum += taggedWrapXElements.Length;
		}		
		
		if(totalElementSum == 0)
		{
			return;	
		}
		
		List<EZParallaxObjectElement> tempElementArray = new List<EZParallaxObjectElement>(m_parallaxElements);
		
		for(int i = 0; i < taggedElements.Length; i++)
		{
			if(!tempElementArray.Exists( ( EZParallaxObjectElement elem ) => { return elem.parallaxObject == taggedElements[i].transform; } ))
			{
				tempElementArray.Add(new EZParallaxObjectElement(taggedElements[i].transform));
			}
		}
		
		
		EZParallaxObjectElement newElement;
		
		if(taggedWrapXElements != null)
		{
			for(int i = 0; i < taggedWrapXElements.Length; i++)
			{
				if(!tempElementArray.Exists( ( EZParallaxObjectElement elem ) => { return elem.parallaxObject == taggedWrapXElements[i].transform; } ))
				{
					newElement = new EZParallaxObjectElement(taggedWrapXElements[i].transform);
					tempElementArray.Add(newElement);
					SetElementWrapSettings(newElement, true, 0);
				}
			}
		}
		
		m_parallaxElements = tempElementArray.ToArray();		
	}
	
	private void SetElementScreenSpaceExtents(EZParallaxObjectElement[] elementArray)
	{
		for(int i = 0; i < elementArray.Length; i++)
		{
			SetElementScreenSpaceExtents(elementArray[i]);
		}
	}
	
	private void SetElementScreenSpaceExtents(EZParallaxObjectElement targetElement)
	{
		Camera actualCamera = m_mainCamera.camera;
		if(targetElement.needsNewScreenspaceExtents == true)
		{
			Vector2 elementCenterScreenPt = actualCamera.WorldToScreenPoint(targetElement.parallaxObject.position);
			Vector2 elementXEdgeScreenPt = actualCamera.WorldToScreenPoint(targetElement.parallaxObject.position + new Vector3(targetElement.meshExtentX, 0.0f, 0.0f));
			float newExtentX = Mathf.Abs(elementXEdgeScreenPt.x - elementCenterScreenPt.x);
			targetElement.initialElementScreenSpaceExtentX = newExtentX;
		}
		
		if(targetElement.spawnsDuplicateOnX && targetElement.spawnDistanceX != 0)
		{
			float offset = actualCamera.WorldToScreenPoint(targetElement.parallaxObject.position).x -  actualCamera.WorldToScreenPoint(targetElement.parallaxObject.position - new Vector3(targetElement.spawnDistanceX, 0, targetElement.parallaxObject.position.z)).x;
			targetElement.initialRightSpawnDistanceScreenSpaceX = offset;
			targetElement.initialLeftSpawnDistanceScreenSpaceX = offset;
		}		
	}
	
	private EZParallaxObjectElement FindEdgeSpawningElement( EZParallaxObjectElement targetElement, bool direction)
	{
		if(direction) //If moving to the right
		{
			if(targetElement.dupeElementLeft == null || targetElement.dupeElementLeft == targetElement)
			{
				if(targetElement.dupeElementLeft == targetElement)
				{
					Debug.Log("WARNING -- You are trying to manipulate an element, " + targetElement.name + ", that was set to spawn duplicates, but in fact has none. You may need to initialize, or may have purged your duplicates without respawning new ones.");
				}
				return targetElement;	
			}
			return FindEdgeSpawningElement(targetElement.dupeElementLeft, direction);
		}
		else //Moving to the left
		{
			if(targetElement.dupeElementRight == null || targetElement.dupeElementRight == targetElement)
			{
				return targetElement;
			}
			
			if(targetElement.dupeElementRight == targetElement)
			{
				Debug.Log("WARNING -- You are trying to manipulate an element, " + targetElement.name + ", that was set to spawn duplicates, but in fact has none. You may need to initialize, or may have purged your duplicates without respawning new ones.");
			}
			return FindEdgeSpawningElement(targetElement.dupeElementRight, direction);
		}
	}
	
	void LateUpdate()
	{
		if ( m_mainCamera == null )
			return;
		
		m_currentCameraPosition = m_mainCamera.transform.position;
		Vector3 camFrameDelta = m_currentCameraPosition - m_previousCameraPosition;
		m_currentOrthoSize = m_mainCamera.camera.orthographicSize;
		if(m_initialized && camFrameDelta !=  Vector3.zero )
		{
			for(int i = 0; i < m_parallaxElements.Length; i++)
			{
				bool direction = m_mainCamera.camera.velocity.x >= 0;
				EZParallaxObjectElement targetPE = m_parallaxElements[i];
				if(targetPE.parallaxObject == null)
				{
					Debug.Log ("WARNING -- There is a parallax object, named " + targetPE.name + ", in your element list that doesn't have an actual transform attached to it. This is impossible to achieve from the EZP inspector, so your code must be causing this issue at runtime.");
					continue;
				}
				targetPE.Update(Time.deltaTime);
				float movementScalar = GetElementMovementScalar(targetPE);
				
				if(targetPE.spawnsDuplicateOnX)
				{
					if(targetPE.isMotorized)
					{
						float camVelocityX = m_mainCamera.camera.velocity.x;
						float movementWithScalar = camVelocityX - (camVelocityX * movementScalar);
						direction = (targetPE.motorSpeed + movementWithScalar - camVelocityX ) < 0;
					}
					
					UpdateDupeObjects(targetPE,direction);
				}
				
				UpdateElementWorldPosition(targetPE, camFrameDelta, movementScalar);
			}
			
			m_previousCameraPosition = m_currentCameraPosition;
		}
		else
		{
			for (int i = 0; i < m_parallaxElements.Length; i++)
			{
				EZParallaxObjectElement targetPE = m_parallaxElements[i];
				float movementScalar = GetElementMovementScalar(targetPE);
				if(targetPE.parallaxObject)
				{
					targetPE.Update(Time.deltaTime);
					if(targetPE.spawnsDuplicateOnX && targetPE.isMotorized)
					{
						bool direction = targetPE.motorSpeed < 0;
						UpdateDupeObjects(targetPE, direction);
					}
					
					UpdateElementWorldPosition(targetPE, camFrameDelta, movementScalar);
				}
			}
		}
		
		if(m_dupeChainHandles != null)
		{
			for(int i = 0; i < m_dupeChainHandles.Length; i++)
			{
				m_dupeChainHandles[i].hasBeenMoved = false;
				m_dupeChainHandles[i].hasMotorShifted = false;
				m_dupeChainHandles[i].scaleUpdated = false;
			}
		}
		m_prevOrthoSize = m_currentOrthoSize;
	}
	
	float GetElementMovementScalar(EZParallaxObjectElement targetPE)
	{
		Transform targetElem = targetPE.parallaxObject;
		float elemToPlayerZ = Mathf.Abs(targetElem.position.z - m_playerObj.transform.position.z);
		float modifiedSpeedScalar;
		if(m_playerObj.transform.position.z > targetElem.position.z)
		{
			//If between the player and the camera
			modifiedSpeedScalar = ( (m_maxDist + elemToPlayerZ) / m_maxDist );
		}
		else
		{
			//if between the player and the farthest bg element, including the furthest bg element
			modifiedSpeedScalar = Mathf.Abs( (m_maxDist - elemToPlayerZ) / m_maxDist);
		}
		
		return modifiedSpeedScalar;
	}
	
	void UpdateTargetScale(EZParallaxObjectElement targetElement)
	{
		Transform targetElemTransform = targetElement.parallaxObject;
		float currentOrthoSizeRatio = 1;
		if(m_currentOrthoSize != m_prevOrthoSize)
		{
			currentOrthoSizeRatio = 1 + ( (m_mainCamera.camera.orthographicSize / m_camStartOrthoSize) - 1) * (Mathf.Abs(targetElemTransform.position.z - m_playerObj.transform.position.z) * m_maxDistDiv);
			if(m_enableDollyZoom)
			{
				targetElement.UpdateScale(currentOrthoSizeRatio, m_mainCamera);
			}
		}
	}
	
	void UpdateElementWorldPosition(EZParallaxObjectElement targetPE, Vector3 camFrameDelta, float elemMovementScalar)
	{
		
		Transform targetElem = targetPE.parallaxObject;
		if(targetPE.dupeChainObject != null && targetPE.dupeChainObject.hasBeenMoved != true)
		{
			targetElem = targetPE.dupeChainObject.targetGameObject.transform;
			targetPE.dupeChainObject.hasBeenMoved = true;
		}
		else if(targetPE.dupeChainObject != null)
		{
			UpdateTargetScale(targetPE);
			return;
		}
		
		float modifiedSpeedScalar = elemMovementScalar;
		Vector3 newPosition;
		UpdateTargetScale(targetPE);
		float camXRounded = camFrameDelta.x; //Mathf.Round(camFrameDelta.x * 100000.0f) / 100000.0f;
		float newXPos = (camXRounded - ( camXRounded * modifiedSpeedScalar)) * m_parallaxSpeedScalarX * targetPE.privateParallaxSpeedScalarX;
		newPosition = new Vector3(newXPos, ( camFrameDelta.y - (camFrameDelta.y * modifiedSpeedScalar)) * m_parallaxSpeedScalarY * targetPE.privateParallaxSpeedScalarY, 0);
		targetElem.position += newPosition;
	}
	
	void UpdateDupeObjects(EZParallaxObjectElement targetPE, bool direction)
	{
		EZParallaxObjectElement edgeElement = FindEdgeSpawningElement(targetPE, direction);
		EZParallaxObjectElement prevEdgeElement = FindEdgeSpawningElement(targetPE, !direction);
		Camera actualCamera = m_mainCamera.camera;
		float screenWidth = actualCamera.pixelWidth;
		float newSpawnOffset = 0;
		float newSpawnOffsetScreenSpace = 0;
		float objectPosX = 0;
		if(edgeElement.parallaxObject != null)
		{
			objectPosX = actualCamera.WorldToScreenPoint(edgeElement.parallaxObject.position).x;
		}
		else
		{
			Debug.Log("WARNING -- UpdateDupeObjects is trying to access a parallax object, for the EZP object element " + edgeElement.name + ", that doesn't exist! objectPosX is remaining 0.");
		}
		
		float edgeCheckBuffer = screenWidth;
		if(direction) //Element is moving to the left side of the screen. Camera most likely is moving to the right.
		{
			while( (objectPosX + targetPE.elementScreenSpaceExtentX + edgeCheckBuffer) < 0.0f )
			{
				if(targetPE.randomSpawnX)
				{
					int spawnGroupIdx = targetPE.spawnGroupIndex;
					
					if(m_rndDistArrayIndex[spawnGroupIdx] == m_randomOffsetHistorySize)
					{
						m_rndDistArrayIndex[spawnGroupIdx] = 0;
					}
					else
					{
						m_rndDistArrayIndex[spawnGroupIdx]++;
					}
					
					if(!m_rndDistArray[spawnGroupIdx, m_rndDistArrayIndex[spawnGroupIdx]].HasValue)
					{
						//Random offset not stored for this index. Make a new one and store it at this index.
						newSpawnOffset = Random.Range(targetPE.spawnDistanceMinX, targetPE.spawnDistanceMaxX);
						m_rndDistArray[spawnGroupIdx, m_rndDistArrayIndex[spawnGroupIdx]] = newSpawnOffset;
					}
					else
					{
						newSpawnOffset = (float)m_rndDistArray[spawnGroupIdx, m_rndDistArrayIndex[spawnGroupIdx]];
					}
					
					Vector3 objectOffsetVector = new Vector3(newSpawnOffset, 0, 0);
					newSpawnOffsetScreenSpace = actualCamera.WorldToScreenPoint(targetPE.parallaxObject.position).x - actualCamera.WorldToScreenPoint(targetPE.parallaxObject.position - objectOffsetVector).x;
				}
				else
				{
					newSpawnOffset = targetPE.spawnDistanceX;
					newSpawnOffsetScreenSpace = targetPE.spawnDistanceScreenX;
				}
				
				edgeElement.parallaxObject.position = prevEdgeElement.parallaxObject.position + new Vector3(edgeElement.meshWidth + newSpawnOffset, 0, 0);
				edgeElement.initialSpawnDistanceX = newSpawnOffset;
				edgeElement.initialLeftSpawnDistanceScreenSpaceX = newSpawnOffsetScreenSpace;
				
				if(edgeElement.dupeElementRight != null)
				{
					edgeElement.dupeElementRight.dupeElementLeft = null;
				}
				edgeElement.dupeElementRight = null;
				if(prevEdgeElement.dupeElementLeft == edgeElement)
				{
					prevEdgeElement.dupeElementLeft = null;
				}
				prevEdgeElement.dupeElementRight = edgeElement;
				
				edgeElement.dupeElementLeft = prevEdgeElement;
				
				edgeElement = FindEdgeSpawningElement(targetPE, direction);
				prevEdgeElement = FindEdgeSpawningElement(targetPE, !direction);
				
				objectPosX = actualCamera.WorldToScreenPoint(edgeElement.parallaxObject.position).x;
			}
		}
		else //Element is moving to the right side of the screen. Camera most likely is moving to the left.
		{
			while( (objectPosX - ( targetPE.elementScreenSpaceExtentX + edgeCheckBuffer )) > screenWidth )
			{
				if(targetPE.randomSpawnX)
				{
					int spawnGroupIdx = targetPE.spawnGroupIndex;
					
					int indexLeftShift = m_rndDistArrayIndex[spawnGroupIdx] - m_rndElementGroupSize[spawnGroupIdx];
					if(indexLeftShift < 0)
					{
						indexLeftShift = m_randomOffsetHistorySize - Mathf.Abs(indexLeftShift);
					}
					
					if(!m_rndDistArray[spawnGroupIdx, indexLeftShift].HasValue)
					{
						newSpawnOffset = Random.Range(targetPE.spawnDistanceMinX, targetPE.spawnDistanceMaxX);
						m_rndDistArray[spawnGroupIdx, indexLeftShift] = newSpawnOffset;
					}
					else
					{
						newSpawnOffset = (float)m_rndDistArray[spawnGroupIdx, indexLeftShift];
					}
					
					Vector3 objectOffsetVector = new Vector3(newSpawnOffset, 0, 0);
					newSpawnOffsetScreenSpace = actualCamera.WorldToScreenPoint(targetPE.parallaxObject.position).x - actualCamera.WorldToScreenPoint(targetPE.parallaxObject.position - objectOffsetVector).x;
					
					if(m_rndDistArrayIndex[spawnGroupIdx] == 0)
					{
						m_rndDistArrayIndex[spawnGroupIdx] = m_randomOffsetHistorySize;
					}
					else
					{
						m_rndDistArrayIndex[spawnGroupIdx]--;
					}
					
				}
				else
				{
					newSpawnOffset = targetPE.spawnDistanceX;
					newSpawnOffsetScreenSpace = targetPE.spawnDistanceScreenX;
				}
				
				edgeElement.parallaxObject.position = prevEdgeElement.parallaxObject.position - new Vector3(edgeElement.meshWidth + newSpawnOffset, 0, 0);
				edgeElement.initialRightSpawnDistanceX = newSpawnOffset;
				edgeElement.initialRightSpawnDistanceScreenSpaceX = newSpawnOffsetScreenSpace;
				if(edgeElement.dupeElementLeft != null)
				{
					edgeElement.dupeElementLeft.dupeElementRight = null;	
				}
				edgeElement.dupeElementLeft = null;
				if(prevEdgeElement.dupeElementRight == edgeElement)
				{
					prevEdgeElement.dupeElementRight = null;
				}
				prevEdgeElement.dupeElementLeft = edgeElement;
				
				edgeElement.dupeElementRight = prevEdgeElement;
				
				edgeElement = FindEdgeSpawningElement(targetPE, direction);
				prevEdgeElement = FindEdgeSpawningElement(targetPE, !direction);
				
				objectPosX = actualCamera.WorldToScreenPoint(edgeElement.parallaxObject.position).x;
			}
		}
	}
	
	void EstablishMaxDistance()
	{
		float[] zDepths = new float[m_parallaxElements.Length];
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			if(m_parallaxElements[i].parallaxObject)
			{
				zDepths[i] = m_parallaxElements[i].parallaxObject.position.z - m_playerObj.transform.position.z;
			}
		}
		
		System.Array.Sort(zDepths);
		System.Array.Reverse (zDepths);
		m_maxDist = zDepths[0];
		m_maxDistDiv = 1 / m_maxDist;
	}
	
	private void SqueezeElementsArray()
	{
		List<EZParallaxObjectElement> tempElementArray = new List<EZParallaxObjectElement>(m_parallaxElements);
		IList<EZParallaxObjectElement> elemsToRemove = new List<EZParallaxObjectElement>();
		
		for(int i = 0; i < tempElementArray.Count; i++)
		{
			if(tempElementArray[i].parallaxObject == null)
			{
				elemsToRemove.Add(tempElementArray[i]);
			}
		}
		
		for(int i = 0; i < elemsToRemove.Count; i++)
		{
			tempElementArray.Remove (elemsToRemove[i]);
		}
		
		m_parallaxElements = tempElementArray.ToArray();
		
	}
	
	private void SetNullChainItems(EZParallaxObjectElement targetElement)
	{
		if(targetElement.dupeElementLeft != null)
		{
			targetElement.dupeElementLeft.dupeElementRight = null;
			targetElement.dupeElementLeft = null;
		}
		
		if(targetElement.dupeElementRight != null)
		{
			targetElement.dupeElementRight.dupeElementLeft = null;
			targetElement.dupeElementRight = null;
		}
	}
	
	public void PurgeDupes()
	{
		List<EZParallaxObjectElement> tempElementArray = new List<EZParallaxObjectElement>(m_parallaxElements);
		IList<EZParallaxObjectElement> elemsToRemove = new List<EZParallaxObjectElement>();
		
		for(int i = 0; i < tempElementArray.Count; i++)
		{
			if(tempElementArray[i].isDupe)
			{
				elemsToRemove.Add(tempElementArray[i]);
				SetNullChainItems(tempElementArray[i]);
			}
		}
		
		for(int i = 0; i < elemsToRemove.Count; i++)
		{
			tempElementArray.Remove (elemsToRemove[i]);
			GameObject.Destroy(elemsToRemove[i].parallaxObject.gameObject);
		}
		
		m_parallaxElements = tempElementArray.ToArray();
		SqueezeElementsArray();
	}
	
	//----------------------------------------------------------------------------------------------
	//  The following functions can be used by scripts at runtime to change the parallaxing elements
	//  or set the player/camera object. Calls into them would look something like this:
	//
	//       AssignPlayer(GameObject.Find("Player"), true); <--Camera function is just like this as well.
	//       AddNewParallaxingElement(GameObject.Find("BG6").transform);
	//	     RemoveParallaxingElement(GameObject.Find("BG2").transform);
	//----------------------------------------------------------------------------------------------
	
	//The second parameter here tells EZParallax if it should recalculate all the positions again, which should usually be "true" unless you are manually delaying parallax initialization.
	public void AssignPlayer(GameObject targetPlayerObj, bool doInit)
	{
		m_playerObj = targetPlayerObj;
		if(doInit)
		{
			InitializeParallax();
		}
	}
	
	//Like the AssignPlayer function, the second parameter here tells EZParallax if it should recalculate all the positions again. You definitely want to have this set to "true" unless you are manually delaying the init.
	public void AssignCamera(GameObject targetCameraObj, bool doInit)
	{
		m_mainCamera = targetCameraObj;
		if(doInit)
		{
			InitializeParallax();
		}
	}
	
	//Use this function to add elements to the parallax list at runtime. Best used for elements that spawn on the fly. For other cases, using the editor inspector interface is encouraged.
	//To specify private speed scalars for your new object, add the x and y scalars as 2nd and 3rd arguments to the function call in your code.
	public EZParallaxObjectElement AddNewParallaxingElement(Transform targetElement)
	{
		return AddNewParallaxingElement(targetElement, 1, 1);
	}
	
	public EZParallaxObjectElement AddNewParallaxingElement(Transform targetElement, float privateSpeedScalarX, float privateSpeedScalarY)
	{
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			if(m_parallaxElements[i].parallaxObject == targetElement)
			{
				Debug.Log("WARNING -- AddNewParallaxingElement attempted to add an element that was arleady in the parallax list. Aborting.");
				return null;
			}
		}
		List<EZParallaxObjectElement> tempElementArray = new List<EZParallaxObjectElement>(m_parallaxElements);
		EZParallaxObjectElement newParallaxElement = new EZParallaxObjectElement(targetElement);
		newParallaxElement.privateParallaxSpeedScalarX = privateSpeedScalarX;
		newParallaxElement.privateParallaxSpeedScalarY = privateSpeedScalarY;
		tempElementArray.Add(newParallaxElement);
		m_parallaxElements = tempElementArray.ToArray();
		
		if( (targetElement.position.z - m_playerObj.transform.position.z) > m_maxDist)
		{
			m_maxDist = targetElement.position.z - m_playerObj.transform.position.z;
			m_maxDistDiv = 1 / m_maxDist;
		}
		
		if(m_initialized)
		{
			newParallaxElement.Initialize();
			SetElementScreenSpaceExtents(newParallaxElement);
		}
		
		return newParallaxElement;
	}
	
	//If you remove an object from the parallaxing elements, make sure it's not visible in your camera view or it will break the parallaxing illusion for your player! :)
	//This function will only remove a single object. If you'd like to remove an object and all of its duplicates, use PurgeSingleDupeChain() found below.
	public void RemoveParallaxingElement(Transform targetElement)
	{
		int sourceElementCounter = 0;
		int newElementCounter = 0;
		EZParallaxObjectElement[] tempElementArray = new EZParallaxObjectElement[m_parallaxElements.Length - 1];
		foreach (EZParallaxObjectElement arrayElem in m_parallaxElements)
		{
			if(arrayElem.parallaxObject != targetElement)
			{
				tempElementArray[newElementCounter] = m_parallaxElements[sourceElementCounter];
				newElementCounter++;
			}
			sourceElementCounter++;
		}
		m_parallaxElements = tempElementArray;
	}
	
	//Use this function to set the infinite wrap settings on an object that you have dynamically added to the parallax list at runtime.
	public void SetElementWrapSettings(EZParallaxObjectElement targetElement, bool xWrapOn,  float spawnDistanceX)
	{
		targetElement.spawnsDuplicateOnX = xWrapOn;	
		targetElement.initialSpawnDistanceX = spawnDistanceX;
		
		if(m_initialized)
		{
			SetElementScreenSpaceExtents(targetElement);
			SpawnSingleElementDupes(targetElement, m_mainCamera.camera, m_mainCamera.camera.pixelWidth);
		}
	}
	
	//Use this function to set an object to be motorized after dynamically spawning it. If you wish to make the object wrap, be sure to set it to wrap FIRST, before applying the motorspeed.
	public void SetElementMotorized(EZParallaxObjectElement targetElement, float speed)
	{
		targetElement.isMotorized = true;
		targetElement.initialMotorSpeed = speed;
	}
	
	//Use this function to set an element to infinitely wrap with random distances between its duplicates. It autmatically turns on x wrapping on the target object, so there is no need to manually set wrapping on the target before hand.
	public void SetElementRandomSpawn(EZParallaxObjectElement targetElement, float minRange, float maxRange)
	{
		targetElement.spawnsDuplicateOnX = true;
		targetElement.randomSpawnX = true;
		targetElement.initialSpawnDistanceMinX = minRange;
		targetElement.initialSpawnDistanceMaxX = maxRange;
		
		if(m_initialized)
		{
			SetElementScreenSpaceExtents(targetElement);
			
			float?[,] tempArray = new float?[m_randomSpawnCtr + 1, m_randomOffsetHistorySize];
			System.Array.Copy(m_rndDistArray, tempArray, m_rndDistArray.Length);
			m_rndDistArray = new float?[m_randomSpawnCtr + 1, m_randomOffsetHistorySize];
			m_rndDistArray = tempArray;
			
			int[] tempGrpArray = new int[m_randomSpawnCtr + 1];
			System.Array.Copy(m_rndElementGroupSize, tempGrpArray, m_rndElementGroupSize.Length);
			m_rndElementGroupSize = new int[m_randomSpawnCtr + 1];
			m_rndElementGroupSize = tempGrpArray;
			
			int[] tempIdxArray = new int[m_randomSpawnCtr + 1];
			System.Array.Copy(m_rndDistArrayIndex, tempIdxArray, m_rndDistArrayIndex.Length);
			m_rndDistArrayIndex = new int[m_randomSpawnCtr + 1];
			m_rndDistArrayIndex = tempIdxArray;
			
			SpawnSingleElementDupes(targetElement, m_mainCamera.camera, m_mainCamera.camera.pixelWidth);
		}
	}
	
	//If the player changes its Z position, call this function to update the depth values for the parallaxing elements. If left alone after changing the position of the player on the Z axis, you may...
	//..experience strange parallaxing behavior, so be sure to use this.
	//This function doesn't need to be called if a call to InitializeParallax is pending, as it will update the depth values as well.
	public void updatePlayerZ()
	{
		EstablishMaxDistance();		
	}
	
	//Use this function to remove a SINGLE chain of elements spawned from a single EZP element. I.e., maybe you have wrapping clouds, and you want to remove all of them from the scene. Pass in any...
	//...of your cloud element's transforms, and all the sibling clouds generated by EZP will be removed from the scene, along with the original transform/elemnent being passed.
	public void PurgeSingleDupeChain(Transform targetTransform)
	{
		EZParallaxObjectElement targetElement = null;
		
		for(int i = 0; i < m_parallaxElements.Length; i++)
		{
			if(m_parallaxElements[i].parallaxObject == targetTransform)
			{
				targetElement = m_parallaxElements[i];
			}
		}
		
		if(targetElement == null)
		{
			Debug.Log("The call to PurgeSingleDupeChain could not find the target transform. Aborting purge.");
			return;
		}
		
		if(targetElement.randomSpawnX)
		{
			int spawnGroupIdx = targetElement.spawnGroupIndex;
			
			//Make all storage and history items related to this randomized object chain 0, so that people viewing the data will be able to identify cleared slots in the array, while still keeping array structure intact.
			for(int i = 0; i < m_rndDistArray.GetLength(0); i++)
			{
				m_rndDistArray[spawnGroupIdx, i] = 0.0f;
			}
			
			m_rndDistArrayIndex[spawnGroupIdx] = 0;
			m_rndElementGroupSize[spawnGroupIdx] = 0;
		}
		
		List<EZParallaxObjectElement> tempElementArray = new List<EZParallaxObjectElement>(m_parallaxElements);
		IList<EZParallaxObjectElement> elemsToRemove = new List<EZParallaxObjectElement>();
		
		elemsToRemove.Add (targetElement);
		EZParallaxObjectElement newTargetElement;
		
		//Are there elements to the left in the chain?
		if(targetElement.dupeElementLeft != null)
		{
			newTargetElement = targetElement.dupeElementLeft;
			elemsToRemove.Add (newTargetElement);
			
			//Get all elements to the left of our target element in the chain.
			while(newTargetElement.dupeElementLeft != null)
			{
				newTargetElement = newTargetElement.dupeElementLeft;
				elemsToRemove.Add (newTargetElement);
			}
		}
		
		//Are there elements to the right?
		if(targetElement.dupeElementRight != null)
		{
			newTargetElement = targetElement.dupeElementRight;
			elemsToRemove.Add (newTargetElement);
			
			//Get all elements to the left of our target element in the chain.
			while(newTargetElement.dupeElementRight != null)
			{
				newTargetElement = newTargetElement.dupeElementRight;
				elemsToRemove.Add (newTargetElement);
			}
		}
		
		//Purge.
		for(int i = 0; i < elemsToRemove.Count; i++)
		{
			tempElementArray.Remove (elemsToRemove[i]);
			SetNullChainItems(elemsToRemove[i]);
			GameObject.Destroy(elemsToRemove[i].parallaxObject.gameObject);
		}
		
		DupeChainHandle[] tempHandleArray = new DupeChainHandle[m_dupeChainHandles.Length - 1];
		int tempIdxCtr = 0;
		bool foundObj = false;
		for(int i = 0; i < m_dupeChainHandles.Length; i++)
		{
			if(m_dupeChainHandles[i].targetGameObject.transform.FindChild(targetTransform.gameObject.name) != null)
			{
				GameObject.Destroy(m_dupeChainHandles[i].targetGameObject);
				m_dupeChainHandles[i] = null;
				foundObj = true;
			}
			else
			{
				tempHandleArray[tempIdxCtr] = m_dupeChainHandles[i];
				tempIdxCtr++;
			}
		}
		
		if(foundObj)
		{
			m_dupeChainHandles = tempHandleArray;
		}
		
		m_parallaxElements = tempElementArray.ToArray();
		SqueezeElementsArray();
	}
	
	//Use this function to manually toggle dolly vs traditional zoom during runtime. Can be used effectively to take advantage of both effects!
	public void ToggleDollyZoom(bool newToggle)
	{
		m_enableDollyZoom = newToggle;
	}
}
