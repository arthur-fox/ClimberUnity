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

	private enum PlayerInput
	{
		kPlayerNotPressedLeft = 0,
		kPlayerNotPressedRight = 1,
		kPlayerPressedLeftAndRight = 2
	}
	private PlayerInput m_playerInput = PlayerInput.kPlayerNotPressedLeft;
	private GameObject m_clickParticle;
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

		if (m_clickParticle != null) 
		{
			Destroy(m_clickParticle);
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
	}

	void Start()
	{
		m_colourCorrection = m_gameCameraEntity.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ();
		m_overlay = m_gameCameraEntity.GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>();
	}

	void FixedUpdate () 
	{
		UpdateInput ();
		UpdateParticles ();
		UpdateSaturation ();
		UpdateOverlay ();
	}

	void UpdateInput() // TODO: Actually Delete the PoarticleSystems once they finished their Stop()
	{
		if (m_playerInput == PlayerInput.kPlayerPressedLeftAndRight) 
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

		if (m_playerInput == PlayerInput.kPlayerNotPressedLeft)
		{
			if (Input.GetKey (KeyCode.LeftArrow) || touchLHS)
			{
				m_playerInput = PlayerInput.kPlayerNotPressedRight;
				ParticleSystem[] particles = m_clickParticle.GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem particle in particles)
				{
					particle.Stop();
				}
				m_oldParticles.Add(m_clickParticle);
				m_clickParticle = null;
			}
		}
		else if (m_playerInput == PlayerInput.kPlayerNotPressedRight)
		{
			if (Input.GetKey (KeyCode.RightArrow) || touchRHS) 
			{
				m_playerInput = PlayerInput.kPlayerPressedLeftAndRight;
				ParticleSystem[] particles = m_clickParticle.GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem particle in particles)
				{
					particle.Stop();
				}
				m_oldParticles.Add(m_clickParticle);
				m_clickParticle = null;
			}
		}
	}

	void UpdateParticles()
	{
		if (m_clickParticle == null) 
		{
			if (m_playerInput == PlayerInput.kPlayerNotPressedLeft)
			{
				Vector3 position = new Vector3(-1.25f, 0.0f, -0.5f);
				m_clickParticle = (GameObject) Instantiate(m_clickMeParticlePrefab, position, Quaternion.identity);
			}
			else if (m_playerInput == PlayerInput.kPlayerNotPressedRight)
			{
				Vector3 position = new Vector3(1.25f, 0.0f, -0.5f);
				m_clickParticle = (GameObject) Instantiate (m_clickMeParticlePrefab, position, Quaternion.identity);
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
