using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour 
{	
	public GameObject m_playerPrefab;	
	public Camera m_gameCamera;
	public PlatformManager m_platformManager;
	public ScrollingBackground[] m_scrollingBackgrounds;
	public EffectsManager m_effectsManager;
	public Text m_scoreText;
	public float m_playerSpawnTime = 3.0f;
	public float m_speedIncGap = 20.0f;
	public float m_speedIncAmount = 0.5f;
	public float m_maxSpeed = 3.0f;
	public float m_initSpeed = 1.0f;

	private GameObject m_playerSpawned = null;
	private float m_nextSpeedIncThresh = 10.0f;
	private float m_timer = 0.0f;
	private float m_currSpeed = 0;

	public void GameOver()
	{
		Destroy(m_playerSpawned);
		m_playerSpawned = null;
		m_timer = 0;

		m_currSpeed = m_initSpeed;
		m_platformManager.Reset (m_currSpeed);
		UpdateSpeed ();

		m_nextSpeedIncThresh = m_speedIncGap;
	}

	void Awake()
	{
		Application.targetFrameRate = 60; //TODO: place this in a more relevant class
	}

	void Start ()
	{
		m_nextSpeedIncThresh = m_speedIncGap;
		m_currSpeed = m_initSpeed;
		UpdateSpeed();
	}

	void FixedUpdate () 
	{
		m_timer += Time.fixedDeltaTime;
		m_scoreText.text = "" + (int)m_timer;
		if (!m_playerSpawned && m_timer > m_playerSpawnTime) 
		{
			Vector3 centreTopPos = m_gameCamera.ViewportToWorldPoint( new Vector3( 0.5f, 1.0f, m_gameCamera.nearClipPlane) );
			GameObject playerObject = (GameObject) Instantiate(m_playerPrefab, centreTopPos, Quaternion.identity);
			Player player = playerObject.GetComponent<Player>();
			player.m_gameCamera = m_gameCamera;
			player.m_levelManager = this;
			player.SetSpeed(m_currSpeed);
			m_effectsManager.m_player = player;
			m_playerSpawned = playerObject;
		}

		if (m_timer >= m_nextSpeedIncThresh) // && m_currSpeed < m_maxSpeed) 
		{
			m_effectsManager.PlayOverlayEffect();

			m_currSpeed += m_speedIncAmount;
			if ( m_currSpeed > m_maxSpeed)
			{
				m_currSpeed = m_maxSpeed;
			}
			UpdateSpeed();

			m_nextSpeedIncThresh += m_speedIncGap;
		}
	}

	void UpdateSpeed()
	{
		m_platformManager.SetSpeed(m_currSpeed);

		if (m_playerSpawned != null) 
		{
			Player player = m_playerSpawned.GetComponent<Player>();
			player.SetSpeed (m_currSpeed);
		}

		for (int i = 0; i < m_scrollingBackgrounds.Length; ++i) 
		{
			m_scrollingBackgrounds[i].SetSpeed(m_currSpeed);
		}
	}
}