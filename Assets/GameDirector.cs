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
	public Text m_climberText;

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
		kPostGame = 1,
		kInGame = 2
	}
	private GameState m_gameState = GameState.kMainMenu;
	private GameObject m_currLevelEntity = null;

	public void OnQuitLevel()
	{
		if (m_currLevelEntity != null)
		{	
			LevelManager levelManager = m_currLevelEntity.GetComponent<LevelManager>();
			m_lastHighScore = levelManager.GetHighScore();

			Destroy (m_currLevelEntity);
			m_currLevelEntity = null;

			m_twitterMessageText.text = "Just Scored " + m_lastHighScore + " on #ClimberGame";

			SetMainMenuUI(false);
			SetPostGameUI(true);
			SetInGameUI(false);
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

		SetMainMenuUI(false);
		SetPostGameUI(false);
		SetInGameUI(true);
		m_gameState = GameState.kInGame;
	}

	public void OnTweetScore()
	{
		SoomlaProfile.UpdateStatus(m_twitterProvider, m_twitterMessageText.text, null, null);

		OnReturnToMainMenu();
	}

	public void OnReturnToMainMenu()
	{
		SetMainMenuUI(true);
		SetPostGameUI(false);
		SetInGameUI(false);
		m_gameState = GameState.kMainMenu;
	}

	void Awake()
	{
		Application.targetFrameRate = 60;
	}

	void Start()
	{
		SoomlaProfile.Initialize ();

		m_backgroundManager.SetSpeed (0.0f);

		SetMainMenuUI(true);
		SetPostGameUI(false);
		SetInGameUI(false);
		m_gameState = GameState.kMainMenu;
	}

	void Update () 
	{
		if (m_gameState == GameState.kMainMenu) 
		{
			//UpdateMainMenu();
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
		
	void SetMainMenuUI(bool enable)
	{
		m_playLevelButton.gameObject.SetActive(enable);
		m_climberText.gameObject.SetActive(enable);
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