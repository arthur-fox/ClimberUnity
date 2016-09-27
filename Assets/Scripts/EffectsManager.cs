using UnityEngine;
using UnityEngine.UI;
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
	public Button m_leftTapButton;
	public Button m_rightTapButton;

	private enum PlayerInputState
	{
		kState1MustPressLeft = 0,
		kState2MustPressRight = 1,
		kState3PressedBoth = 2
	}
	private PlayerInputState m_playerInputState = PlayerInputState.kState1MustPressLeft;
	private UnityStandardAssets.ImageEffects.ColorCorrectionCurves m_colourCorrection = null;
	private UnityStandardAssets.ImageEffects.ScreenOverlay m_overlay = null;

	public void PlayOverlayEffect()
	{
		m_overlay.intensity = -2.0f;
	}

	public void OnLeftButton()
	{
		if (m_playerInputState == PlayerInputState.kState1MustPressLeft)
		{
			m_leftTapButton.gameObject.SetActive(false);
			m_rightTapButton.gameObject.SetActive(true);

			m_playerInputState = PlayerInputState.kState2MustPressRight;
		}
	}

	public void OnRightButton()
	{
		if (m_playerInputState == PlayerInputState.kState2MustPressRight)
		{
			m_leftTapButton.gameObject.SetActive(false);
			m_rightTapButton.gameObject.SetActive(false);

			m_playerInputState = PlayerInputState.kState3PressedBoth;
		}
	}

	void OnDestroy()
	{
		m_overlay.intensity = kDefaultOverlayIntensity;
		m_colourCorrection.saturation = kDefaultColourSaturation;

		m_leftTapButton.gameObject.SetActive(false);
		m_rightTapButton.gameObject.SetActive(false);
	}

	void Start()
	{
		m_playerInputState = PlayerInputState.kState1MustPressLeft;

		m_leftTapButton.gameObject.SetActive(true);
		m_rightTapButton.gameObject.SetActive(false);

		m_colourCorrection = m_gameCameraEntity.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves> ();
		m_overlay = m_gameCameraEntity.GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>();
	}

	void FixedUpdate () 
	{		
		UpdateSaturation();
		UpdateOverlay();
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