using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour 
{	
	public GameObject m_gameCameraEntity; //TODO: Make this tidier, shouldn't have GameObject and Camera...
	public Camera m_gameCamera;
	public BackgroundManager m_backgroundManager;
	public Text m_highScoreText;
	public Text m_scoreText;
	public GameObject m_playerPrefab;	
	public GameObject m_platformManagerPrefab;
	public GameObject m_effectsManagerPrefab;
	public float m_playerSpawnTime = 3.0f;
	public float m_speedIncGap = 20.0f;
	public float m_speedIncAmount = 0.5f;
	public float m_maxSpeed = 3.0f;
	public float m_initSpeed = 1.0f;

	private GameObject m_playerSpawnedEntity = null;
	private GameObject m_platformManagerEntity = null;
	private GameObject m_effectsManagerEntity = null;
	private float m_nextSpeedIncThresh = 10.0f;
	private float m_timer = 0.0f;
	private float m_currSpeed = 0;
	private int m_currHighScore = 0;

	public void GameOver()
	{
		if ((int)m_timer > m_currHighScore) 
		{
			m_currHighScore = (int)m_timer;
			m_highScoreText.text = "" + m_currHighScore;
		}

		m_timer = 0;
		m_currSpeed = m_initSpeed;

		Destroy(m_playerSpawnedEntity);
		m_playerSpawnedEntity = null;	

		PlatformManager platformManager = m_platformManagerEntity.GetComponent<PlatformManager> ();
		platformManager.Reset (m_currSpeed);
		UpdateSpeed ();

		m_nextSpeedIncThresh = m_speedIncGap;
	}

	public void SetCamera(GameObject gameCameraEntity)
	{
		m_gameCameraEntity = gameCameraEntity;
		m_gameCamera = gameCameraEntity.GetComponent<Camera>();
	}

	public int GetHighScore()
	{
		return m_currHighScore;
	}

	void OnDestroy()
	{
		m_highScoreText.text = "";
		m_scoreText.text = "";
		m_backgroundManager.SetSpeed(0.0f);
		
		Destroy (m_playerSpawnedEntity);
		Destroy (m_platformManagerEntity);
		Destroy (m_effectsManagerEntity);
	}

	void Start()
	{
		m_platformManagerEntity = (GameObject) Instantiate(m_platformManagerPrefab);
		PlatformManager platformManager = m_platformManagerEntity.GetComponent<PlatformManager>();
		platformManager.m_gameCamera = m_gameCamera;

		m_effectsManagerEntity = (GameObject)Instantiate (m_effectsManagerPrefab);
		EffectsManager effectsManager = m_effectsManagerEntity.GetComponent<EffectsManager>();
		effectsManager.m_gameCameraEntity = m_gameCameraEntity;

		m_nextSpeedIncThresh = m_speedIncGap;
		m_currSpeed = m_initSpeed;
		UpdateSpeed();
	}

	void FixedUpdate () 
	{
		m_timer += Time.fixedDeltaTime;
		m_scoreText.text = "" + (int)m_timer;
		if (!m_playerSpawnedEntity && m_timer > m_playerSpawnTime) 
		{
			Vector3 centreTopPos = m_gameCamera.ViewportToWorldPoint( new Vector3( 0.5f, 1.0f, m_gameCamera.nearClipPlane) );
			GameObject playerObject = (GameObject) Instantiate(m_playerPrefab, centreTopPos, Quaternion.identity);

			Player player = playerObject.GetComponent<Player>();
			player.m_gameCamera = m_gameCamera;
			player.m_levelManager = this;
			player.SetSpeed(m_currSpeed);
			m_playerSpawnedEntity = playerObject;

			EffectsManager effectsManager = m_effectsManagerEntity.GetComponent<EffectsManager>();
			effectsManager.m_player = player;
		}

		if (m_timer >= m_nextSpeedIncThresh) // && m_currSpeed < m_maxSpeed) 
		{
			EffectsManager effectsManager = m_effectsManagerEntity.GetComponent<EffectsManager>();
			effectsManager.PlayOverlayEffect();

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
		m_backgroundManager.SetSpeed (m_currSpeed);

		PlatformManager platformManager = m_platformManagerEntity.GetComponent<PlatformManager>();
		platformManager.SetSpeed(m_currSpeed);

		if (m_playerSpawnedEntity != null) 
		{
			Player player = m_playerSpawnedEntity.GetComponent<Player>();
			player.SetSpeed (m_currSpeed);
		}	
	}
}