using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour {

	public Camera m_gameCamera;
	public GameObject[] m_platforms;
	public float m_platformGap = 2.0f;
	public float m_currSpeed = 2.0f;

	private List<GameObject> m_activePlatforms;
	//private float m_levelWidth = 0.0f;
	private float m_levelHeight = 0.0f;

	public void Reset(float speed)
	{
		for (int i = 0; i < m_activePlatforms.Count; ++i)
		{
			GameObject platform = m_activePlatforms[i];
			Destroy(platform);
		}
		
		m_activePlatforms.Clear();
		m_currSpeed = speed;
	}

	public void SetSpeed(float speed)
	{
		m_currSpeed = speed;
		for (int i = 0; i < m_activePlatforms.Count; ++i)
		{
			Platform platform = m_activePlatforms[i].GetComponent<Platform>(); 
			platform.m_speed = m_currSpeed;
		}
	}

	void Awake()
	{
		Random.seed = (int) Time.time;
		m_activePlatforms = new List<GameObject> ();

		Vector3 leftCornerPos = m_gameCamera.ViewportToWorldPoint( new Vector3( 1.0f, 1.0f, m_gameCamera.nearClipPlane) );
		//m_levelWidth = leftCornerPos.x * 2.0f;
		m_levelHeight = leftCornerPos.y * 2.0f; 
	}

	void FixedUpdate () 
	{
		if (m_activePlatforms.Count >= 3) 
		{
			Transform platTransform = m_activePlatforms [m_activePlatforms.Count-1].GetComponent<Transform> ();
			if (platTransform.position.y < (m_levelHeight/2.0f - m_platformGap) )
			{
				int prefabNum = Random.Range(0, m_platforms.Length);	
				float xOffset = CalcRandomXOffset();
				CreatePlatform(prefabNum, xOffset);
			}

			SpriteRenderer platSprite = m_activePlatforms [0].GetComponent<SpriteRenderer> ();
			if (!platSprite.isVisible)  // TODO: Do this based on OnBecameVisible()
			{
				GameObject inactivePlatform = m_activePlatforms [0];
				m_activePlatforms.RemoveAt (0);
				Destroy (inactivePlatform);
			}
		} 
		else if ( m_activePlatforms.Count == 0) 
		{
			int prefabNum = 0;
			float xOffset = 0.5f;
			CreatePlatform(prefabNum, xOffset);
		}
		else if ( m_activePlatforms.Count == 1) 
		{
			int prefabNum = 0;
			float xOffset = 0.25f;
			Transform platTransform = m_activePlatforms [m_activePlatforms.Count-1].GetComponent<Transform> ();
			if (platTransform.position.y < (m_levelHeight/2.0f - m_platformGap) )
			{			
				CreatePlatform(prefabNum, xOffset);
			}
		}
		else if ( m_activePlatforms.Count == 2) 
		{
			int prefabNum = 0;
			float xOffset = 0.75f;
			Transform platTransform = m_activePlatforms [m_activePlatforms.Count-1].GetComponent<Transform> ();
			if (platTransform.position.y < (m_levelHeight/2.0f - m_platformGap) )
			{
				CreatePlatform(prefabNum, xOffset);
			}
		}
	}

	float CalcRandomXOffset()
	{
		const float kMaxPlatformWidth = 160.0f; 
		const float kLevelWidth = 640.0f - 140.0f; //NOTE: 140.0f is to cater for red bricks on edge of image
		float widthOffset = (kMaxPlatformWidth/2.0f) / kLevelWidth;
		float offset = Random.Range(0.0f + widthOffset, 1.0f - widthOffset);
		return offset;
	}

	void CreatePlatform(int prefabNum, float xOffset)
	{	
		Vector3 position = m_gameCamera.ViewportToWorldPoint( new Vector3( xOffset, 1.0f, m_gameCamera.nearClipPlane) );
		GameObject platformObject = (GameObject) Instantiate(m_platforms[prefabNum], position, Quaternion.identity);

		Platform platform = platformObject.GetComponent<Platform>();
		platform.m_speed = m_currSpeed;

		m_activePlatforms.Add(platformObject);
	}
}