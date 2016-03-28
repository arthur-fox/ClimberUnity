using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla;
using Soomla.Profile;

public class GameDirector : MonoBehaviour 
{
	public GameObject m_gameCameraEntity;
	public BackgroundManager m_backgroundManager;
	public Button m_playLevelButton;
	public Button m_endLevelButton;
	public Text m_climberText;
	public Text m_scoreText;
	public Text m_highScoreText;
	public GameObject m_levelManagerPrefab;
	public string m_twitterMessage;

	private Camera m_gameCamera = null;
	private int m_lastHighScore = 0;
	private Provider m_twitterProvider = Provider.TWITTER;
		
	private enum GameState
	{
		kMainMenu = 0,
		kInGame = 1
	}
	private GameState m_gameState = GameState.kMainMenu;
	private GameObject m_currLevelEntity = null;

	bool exitedLevel = false;

	public void OnQuitLevel()
	{
		if (m_currLevelEntity != null)
		{	
			if (!SoomlaProfile.IsLoggedIn(m_twitterProvider))
			{
				SoomlaProfile.Login(m_twitterProvider, null, null);
			}

			LevelManager levelManager = m_currLevelEntity.GetComponent<LevelManager>();
			m_lastHighScore = levelManager.GetHighScore();
			exitedLevel = true;

			m_endLevelButton.gameObject.SetActive(false);

			Destroy (m_currLevelEntity);
			m_currLevelEntity = null;
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

		m_climberText.text = "";

		m_playLevelButton.gameObject.SetActive(false);
		m_endLevelButton.gameObject.SetActive(true);

		m_gameState = GameState.kInGame;
	}

	void OnGUI()
	{
		if (exitedLevel) //TODO: This succesfully tweets! However I need to improve the UX for tweeting!
		{
			// float cameraHeight = m_gameCamera.pixelHeight; // (2.0f * m_gameCamera.orthographicSize) * 
			// float cameraWidth = m_gameCamera.pixelWidth; // (cameraHeight * m_gameCamera.aspect) *

			// TODO: GET THIS THE RIGHT SIZE - REPRESENTATIVE IN EDITOR AND IN-GAME
			if (SoomlaProfile.IsLoggedIn(m_twitterProvider))
			{
				m_twitterMessage = "Just Scored " + m_lastHighScore + " on #ClimberGame";
				GUI.Box(new Rect(Screen.width/4.0f, Screen.height/5.0f, Screen.width/2.0f, Screen.height/5.0f), "Tweet Score?"); 
				if (GUI.Button(new Rect(Screen.width/3.0f, Screen.height*(2.5f/10.0f), Screen.width/3.0f, Screen.height/15.0f), "Yeah!")) 
				{
					SoomlaProfile.UpdateStatus(m_twitterProvider, m_twitterMessage, null, null);
					exitedLevel = false;
				}	
				if (GUI.Button(new Rect(Screen.width/3.0f, Screen.height*(3.25f/10.0f), Screen.width/3.0f, Screen.height/15.0f), "Not Now")) 
				{
					exitedLevel = false;
				}
			}
			else if (GUI.Button(Rect.MinMaxRect(Screen.width/4.0f, Screen.height/5.0f, Screen.width/1.5f, Screen.height/10.0f), "Log in to Twitter to Post Scores"))
			{
				exitedLevel = false;
			}
		}
	}

	void Awake()
	{
		Application.targetFrameRate = 60;
	}

	void Start()
	{
		SoomlaProfile.Initialize ();

		m_backgroundManager.SetSpeed (0.0f);
		m_endLevelButton.gameObject.SetActive(false);
		m_gameCamera = m_gameCameraEntity.GetComponent<Camera>();
	}

	void Update () 
	{
		if (!exitedLevel)
		{
			if (m_gameState == GameState.kMainMenu) 
			{
				UpdateMainMenu();
			} 
			else if (m_gameState == GameState.kInGame) 
			{
				UpdateInGame();
			}
		}
	}
	
	void UpdateMainMenu()
	{
		/*
		if (Input.touchCount > 0 || Input.anyKeyDown) 
		{			
			m_currLevelEntity = (GameObject) Instantiate(m_levelManagerPrefab);
			LevelManager levelManager = m_currLevelEntity.GetComponent<LevelManager>();
			levelManager.SetCamera(m_gameCameraEntity);
			levelManager.m_scoreText = m_scoreText;
			levelManager.m_highScoreText = m_highScoreText;
			levelManager.m_backgroundManager = m_backgroundManager;

			m_climberText.text = "";

			m_endLevelButton.gameObject.SetActive(true);

			m_gameState = GameState.kInGame;
		}
		*/
	}
	
	void UpdateInGame()
	{		
		if (m_currLevelEntity == null) 
		{
			m_playLevelButton.gameObject.SetActive(true);
			m_scoreText.text = "";
			m_climberText.text = "Climber";

			m_gameState = GameState.kMainMenu;
		}
	}
}
