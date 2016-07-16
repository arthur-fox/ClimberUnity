using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectsManager : MonoBehaviour 
{
	const float kDefaultOverlayIntensity = 0.2f; //TODO: Do not hardcode these variables!
	const float kDefaultColourSaturation = 1.0f;
	const float kMaxHeight = 5; 
	const float kMinHeight = -5;
	const float kMaxSaturation = 1.25f;
	const float kMinSaturation = 0.0f;

	public Player m_player;
	public GameObject m_gameCameraEntity;
	public GameObject m_clickMeParticlePrefab;
	public GameObject m_tapButtonPrefab;

	private enum PlayerInputState
	{
		kState1MustPressLeft = 0,
		kState2MustPressRight = 1,
		kState3PressedBoth = 2
	}
	private PlayerInputState m_playerInputState = PlayerInputState.kState1MustPressLeft;
	private GameObject m_currClickMeParticle;
	private GameObject m_leftTapButton;
	private GameObject m_rightTapButton;
	private List<GameObject> m_oldParticles;
	private UnityStandardAssets.ImageEffects.ColorCorrectionCurves m_colourCorrection = null;
	private UnityStandardAssets.ImageEffects.ScreenOverlay m_overlay = null;

	public void PlayOverlayEffect()
	{
		m_overlay.intensity = -2.0f;
	}

	void OnDestroy()
	{
		m_overlay.intensity = kDefaultOverlayIntensity;
		m_colourCorrection.saturation = kDefaultColourSaturation;

		if (m_currClickMeParticle != null) 
		{
			Destroy(m_currClickMeParticle);
		}

		for (int i = 0; i < m_oldParticles.Count; i++) 
		{
			Destroy(m_oldParticles[i]);
		}
		m_oldParticles = null;
	}

	void Awake()
	{ 
		m_oldParticles = new List<GameObject>();

		Vector3 leftPosition = new Vector3(-1.45f, -0.5f, 0.0f);
		m_leftTapButton = (GameObject) Instantiate(m_tapButtonPrefab, leftPosition, Quaternion.identity);
		m_leftTapButton.SetActive(false);

		Vector3 rightPosition = new Vector3(1.45f, -0.5f, 0.0f);
		m_rightTapButton = (GameObject) Instantiate(m_tapButtonPrefab, rightPosition, Quaternion.identity);
		m_rightTapButton.SetActive(false);
	}

	void Start()
	{
		m_colourCorrection = m_gameCameraEntity.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ();
		m_overlay = m_gameCameraEntity.GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>();
	}

	void FixedUpdate () 
	{
		UpdateTapButtons ();
		//UpdateParticles ();
		UpdateSaturation ();
		UpdateOverlay ();
	}

	void UpdateTapButtons()
	{
		if (m_playerInputState == PlayerInputState.kState3PressedBoth) 
		{
			return;
		}

		bool touchLHS = false;
		bool touchRHS = false;
		for (int i = 0; i < Input.touchCount; ++i) 
		{
			float touchX = Input.GetTouch (i).position.x;
			if (touchX < (Screen.width/2.0f) )
			{
				touchLHS = true;
			}
			else
			{
				touchRHS = true;
			}
		}

		if (m_playerInputState == PlayerInputState.kState1MustPressLeft)
		{
			m_leftTapButton.SetActive(true);
			m_rightTapButton.SetActive(false);
			
			if (Input.GetKey (KeyCode.LeftArrow) || touchLHS)
			{
				m_playerInputState = PlayerInputState.kState2MustPressRight;

//				ParticleSystem[] particles = m_currClickParticle.GetComponentsInChildren<ParticleSystem>();
//				foreach(ParticleSystem particle in particles)
//				{
//					particle.Stop();
//				}
//				m_oldParticles.Add(m_clickParticle);
//				m_currClickParticle = null;
			}
		}

		if (m_playerInputState == PlayerInputState.kState2MustPressRight)
		{
			m_leftTapButton.SetActive(false);
			m_rightTapButton.SetActive(true);

			if (Input.GetKey (KeyCode.RightArrow) || touchRHS) 
			{
				m_playerInputState = PlayerInputState.kState3PressedBoth;
//				ParticleSystem[] particles = m_currClickParticle.GetComponentsInChildren<ParticleSystem>();
//				foreach(ParticleSystem particle in particles)
//				{
//					particle.Stop();
//				}
//				m_oldParticles.Add(m_clickParticle);
//				m_currClickParticle = null;
			}
		}

		if (m_playerInputState == PlayerInputState.kState3PressedBoth)
		{
			m_leftTapButton.SetActive(false);
			m_rightTapButton.SetActive(false);
		}
	}

	void UpdateParticles()
	{
		if (m_currClickMeParticle == null) 
		{
			if (m_playerInputState == PlayerInputState.kState1MustPressLeft)
			{
				Vector3 position = new Vector3(-1.25f, 0.0f, -0.5f);
				m_currClickMeParticle = (GameObject) Instantiate(m_clickMeParticlePrefab, position, Quaternion.identity);
			}
			else if (m_playerInputState == PlayerInputState.kState2MustPressRight)
			{
				Vector3 position = new Vector3(1.25f, 0.0f, -0.5f);
				m_currClickMeParticle = (GameObject) Instantiate (m_clickMeParticlePrefab, position, Quaternion.identity);
			}
		}
	}

	void UpdateSaturation()
	{
		float playerHeight = 5.0f; // NOTE: This only works because i've hardcoded min and max Saturation
		if (m_player != null) 
		{
			playerHeight = m_player.transform.position.y;
		}	
		
		float heightRange = kMaxHeight - kMinHeight;
		float normalisedHeight = (playerHeight - kMinHeight) / heightRange;
		
		float saturationRange = kMaxSaturation - kMinSaturation;
		float desiredSaturation = (normalisedHeight * saturationRange) + kMinSaturation;

		const float kMagicLerpAmount = 0.8f;
		m_colourCorrection.saturation = Mathf.Lerp (m_colourCorrection.saturation, desiredSaturation, kMagicLerpAmount);
	}

	void UpdateOverlay()
	{
		m_overlay.intensity = Mathf.Lerp (m_overlay.intensity, kDefaultOverlayIntensity, Time.fixedDeltaTime);
	}
}