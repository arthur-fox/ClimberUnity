using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla;
using Soomla.Profile;

public class GameDirector : MonoBehaviour 
{
	public GameObject m_gameCameraEntity;
	public BackgroundManager m_backgroundManager;
	public GameObject m_levelManagerPrefab;

	// MainMenu UI
	public Button m_playLevelButton;
	public Button m_heartButton;
	public Text m_climberText;
	public GameObject m_climberCrown; // Added this because I had to call the game Climb King! =P

	// HeartPage UI - Re-using Yes + No buttons from PostGameUI
	public Text m_heartTwitterMessageText;
	public Text m_heartTweetSupportText;
	public Text m_heartPromptUserLogInText;

	// PostGame UI
	public Button m_twitterYesButton;
	public Button m_twitterNoButton;
	public Text m_twitterMessageText;
	public Text m_tweetScoreText;
	public Text m_promptUserLogInText;

	// InGame UI
	public Button m_endLevelButton;
	public Text m_scoreText;
	public Text m_highScoreText;

	private int m_lastHighScore = 0;
	private Provider m_twitterProvider = Provider.TWITTER;
		
	private enum GameState
	{
		kMainMenu = 0,
		kHeartPage = 1,
		kPostGame = 2,
		kInGame = 3
	}
	private GameState m_gameState = GameState.kMainMenu;
	private GameObject m_currLevelEntity = null;

	// Button press functions
	public void OnQuitLevel()
	{
		if (m_currLevelEntity != null)
		{	
			LevelManager levelManager = m_currLevelEntity.GetComponent<LevelManager>();
			m_lastHighScore = levelManager.GetHighScore();

			Destroy (m_currLevelEntity);
			m_currLevelEntity = null;

			m_twitterMessageText.text = "Just Scored " + m_lastHighScore + " on #ClimbKingGame!";

			SetAllUI(false);
			SetPostGameUI(true);
			m_gameState = GameState.kPostGame;		
		}
	}

	public void OnPlayLevel()
	{
		m_currLevelEntity = (GameObject) Instantiate(m_levelManagerPrefab);
		LevelManager levelManager = m_currLevelEntity.GetComponent<LevelManager>();
		levelManager.SetCamera(m_gameCameraEntity);
		levelManager.m_scoreText = m_scoreText;
		levelManager.m_highScoreText = m_highScoreText;
		levelManager.m_backgroundManager = m_backgroundManager;
		levelManager.transform.SetParent(transform);

		SetAllUI(false);
		SetInGameUI(true);
		m_gameState = GameState.kInGame;
	}

	public void OnHeartPage()
	{
		SetAllUI(false);
		SetHeartPageUI(true);
		m_gameState = GameState.kHeartPage;		
	}

	public void OnTweetScore()
	{
		if (m_gameState == GameState.kHeartPage)
		{
			SoomlaProfile.UpdateStatus(m_twitterProvider, m_heartTwitterMessageText.text, null, null);
		}
		if (m_gameState == GameState.kPostGame)
		{
			SoomlaProfile.UpdateStatus(m_twitterProvider, m_twitterMessageText.text, null, null);
		}

		OnReturnToMainMenu();
	}

	public void OnReturnToMainMenu()
	{
		SetAllUI(false);
		SetMainMenuUI(true);
		m_gameState = GameState.kMainMenu;
	}

	// State functions
	void Awake()
	{
		Application.targetFrameRate = 60;
	}

	void Start()
	{
		SoomlaProfile.Initialize ();

		m_backgroundManager.SetSpeed (0.0f);

		SetAllUI(false);
		SetMainMenuUI(true);
		m_gameState = GameState.kMainMenu;
	}

	// Update functions
	void Update () 
	{
		if (m_gameState == GameState.kMainMenu) 
		{
			//UpdateMainMenu();
		} 
		else if (m_gameState == GameState.kHeartPage) 
		{
			UpdateHeartPage();
		}
		else if (m_gameState == GameState.kPostGame) 
		{
			UpdatePostGame();
		}
		else if (m_gameState == GameState.kInGame) 
		{
			//UpdateInGame();
		}		
	}
		
	void UpdateMainMenu()
	{		
	}

	void UpdateHeartPage()
	{
		if (!SoomlaProfile.IsLoggedIn(m_twitterProvider))
		{
			SoomlaProfile.Login(m_twitterProvider, null, null);
		}

		m_heartTwitterMessageText.gameObject.SetActive(true);

		if (SoomlaProfile.IsLoggedIn(m_twitterProvider))
		{
			m_twitterYesButton.gameObject.SetActive(true);
			m_twitterNoButton.gameObject.SetActive(true);
			m_heartTweetSupportText.gameObject.SetActive(true);
			m_heartPromptUserLogInText.gameObject.SetActive(false);
		}
		else
		{
			m_twitterYesButton.gameObject.SetActive(false);
			m_twitterNoButton.gameObject.SetActive(true);
			m_heartTweetSupportText.gameObject.SetActive(false);
			m_heartPromptUserLogInText.gameObject.SetActive(true);
		}
	}

	void UpdatePostGame()
	{
		if (!SoomlaProfile.IsLoggedIn(m_twitterProvider))
		{
			SoomlaProfile.Login(m_twitterProvider, null, null);
		}
			
		m_twitterMessageText.gameObject.SetActive(true);
		
		if (SoomlaProfile.IsLoggedIn(m_twitterProvider))
		{
			m_twitterYesButton.gameObject.SetActive(true);
			m_twitterNoButton.gameObject.SetActive(true);
			m_tweetScoreText.gameObject.SetActive(true);
			m_promptUserLogInText.gameObject.SetActive(false);
		}
		else
		{
			m_twitterYesButton.gameObject.SetActive(false);
			m_twitterNoButton.gameObject.SetActive(true);
			m_tweetScoreText.gameObject.SetActive(false);
			m_promptUserLogInText.gameObject.SetActive(true);
		}
	}
	
	void UpdateInGame()
	{		
	}

	// UI functions
	void SetAllUI(bool enable)
	{
		m_playLevelButton.gameObject.SetActive(enable);
		m_heartButton.gameObject.SetActive(enable);
		m_climberText.gameObject.SetActive(enable);
		m_climberCrown.SetActive(enable);

		m_heartTwitterMessageText.gameObject.SetActive(enable);
		m_heartTweetSupportText.gameObject.SetActive(enable);
		m_heartPromptUserLogInText.gameObject.SetActive(enable);

		m_twitterYesButton.gameObject.SetActive(enable);
		m_twitterNoButton.gameObject.SetActive(enable);
		m_twitterMessageText.gameObject.SetActive(enable);
		m_tweetScoreText.gameObject.SetActive(enable);
		m_promptUserLogInText.gameObject.SetActive(enable);

		m_endLevelButton.gameObject.SetActive(enable);
		m_scoreText.gameObject.SetActive(enable);
		m_highScoreText.gameObject.SetActive(enable);
	}

	void SetMainMenuUI(bool enable)
	{
		m_playLevelButton.gameObject.SetActive(enable);
		m_heartButton.gameObject.SetActive(enable);
		m_climberText.gameObject.SetActive(enable);
		m_climberCrown.SetActive(enable);
	}

	void SetHeartPageUI(bool enable)
	{
		m_twitterYesButton.gameObject.SetActive(enable);
		m_twitterNoButton.gameObject.SetActive(enable);
		m_heartTwitterMessageText.gameObject.SetActive(enable);
		m_heartTweetSupportText.gameObject.SetActive(enable);
		m_heartPromptUserLogInText.gameObject.SetActive(enable);
	}

	void SetPostGameUI(bool enable)
	{
		m_twitterYesButton.gameObject.SetActive(enable);
		m_twitterNoButton.gameObject.SetActive(enable);
		m_twitterMessageText.gameObject.SetActive(enable);
		m_tweetScoreText.gameObject.SetActive(enable);
		m_promptUserLogInText.gameObject.SetActive(enable);
	}

	void SetInGameUI(bool enable)
	{
		m_endLevelButton.gameObject.SetActive(enable);
		m_scoreText.gameObject.SetActive(enable);
		m_highScoreText.gameObject.SetActive(enable);
	}
}