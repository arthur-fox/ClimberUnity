using UnityEngine;
using System.Collections;

public class EffectsManager : MonoBehaviour {

	public GameObject m_gameCamera;
	public Player m_player;
	public GameObject m_clickMeParticlePrefab;

	private enum PlayerInput
	{
		kPlayerNotPressedLeft = 0,
		kPlayerNotPressedRight = 1,
		kPlayerPressedLeftAndRight = 2
	}
	private PlayerInput m_playerInput = PlayerInput.kPlayerNotPressedLeft;
	private GameObject m_clickParticle;
	private UnityStandardAssets.ImageEffects.ColorCorrectionCurves m_colourCorrection = null;
	private UnityStandardAssets.ImageEffects.ScreenOverlay m_overlay = null;

	public void PlayOverlayEffect()
	{
		m_overlay.intensity = -2.0f;
	}

	void Start()
	{
		m_colourCorrection = m_gameCamera.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ();
		m_overlay = m_gameCamera.GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>();
	}

	void FixedUpdate () 
	{
		HandleInput ();
		HandleParticles ();
		HandleSaturation ();
		HandleOverlay ();
	}

	void HandleInput() // TODO: Actually Delete the PoarticleSystems once they finished their Stop()
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
				ParticleSystem particleSystem = m_clickParticle.GetComponent<ParticleSystem>();
				particleSystem.Stop();
				//Destroy(m_clickParticle);
				m_clickParticle = null;
			}
		}
		else if (m_playerInput == PlayerInput.kPlayerNotPressedRight)
		{
			if (Input.GetKey (KeyCode.RightArrow) || touchRHS) 
			{
				m_playerInput = PlayerInput.kPlayerPressedLeftAndRight;
				ParticleSystem particleSystem = m_clickParticle.GetComponent<ParticleSystem>();
				particleSystem.Stop();
				//Destroy(m_clickParticle);
				m_clickParticle = null;
			}
		}
	}

	void HandleParticles()
	{
		if (m_clickParticle == null) 
		{
			if (m_playerInput == PlayerInput.kPlayerNotPressedLeft)
			{
				Vector3 position = new Vector3(-1.3f, 0.0f, 0.0f);
				m_clickParticle = (GameObject) Instantiate(m_clickMeParticlePrefab, position, Quaternion.identity);
			}
			else if (m_playerInput == PlayerInput.kPlayerNotPressedRight)
			{
				Vector3 position = new Vector3(1.3f, 0.0f, 0.0f);
				m_clickParticle = (GameObject) Instantiate (m_clickMeParticlePrefab, position, Quaternion.identity);
			}
		}
	}

	void HandleSaturation()
	{
		float playerHeight = 5.0f; // NOTE: This only works because i've hardcoded min and max Saturation
		if (m_player != null) 
		{
			playerHeight = m_player.transform.position.y;
		}
		
		const float kMaxHeight = 5; //TODO: Do not hardcode this stuff
		const float kMinHeight = -5;
		const float kMaxSaturation = 1.25f;
		const float kMinSaturation = 0.0f;
		
		float heightRange = kMaxHeight - kMinHeight;
		float normalisedHeight = (playerHeight - kMinHeight) / heightRange;
		
		float saturationRange = kMaxSaturation - kMinSaturation;
		float desiredSaturation = (normalisedHeight * saturationRange) + kMinSaturation;
		
		m_colourCorrection.saturation = Mathf.Lerp (m_colourCorrection.saturation, desiredSaturation, 0.8f);
	}

	void HandleOverlay()
	{
		const float kNormalIntensity = 0.2f;

		m_overlay.intensity = Mathf.Lerp (m_overlay.intensity, kNormalIntensity, Time.fixedDeltaTime);
	}
}
