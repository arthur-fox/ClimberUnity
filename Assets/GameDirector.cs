using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Soomla;
using Soomla.Profile;

public class GameDirector : MonoBehaviour 
{
	public Provider twitterProvider = Provider.TWITTER;
	public string twitterMessage;

	public GameObject m_gameCameraEntity;
	public BackgroundManager m_backgroundManager;
	public Button m_endLevelButton;
	public Text m_scoreText;
	public Text m_highScoreText;
	public GameObject m_levelManagerPrefab;

	private int m_lastHighScore = 0;
		
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
			LevelManager levelManager = m_currLevelEntity.GetComponent<LevelManager>();
			m_lastHighScore = levelManager.GetHighScore();
			exitedLevel = true;

			Destroy (m_currLevelEntity);
			m_currLevelEntity = null;
		}
	}

	void OnGUI()
	{
		if (exitedLevel) //TODO: This succesfully tweets! However 1: It fails the first time as it needs needs a login 
		{												//		  2: I need to improve the UI for tweeting!
			if (SoomlaProfile.IsLoggedIn(twitterProvider))
			{
				twitterMessage = "Just scored " + m_lastHighScore + " on #ClimberGame";
				if (GUI.Button(new Rect(300, 150, 250, 50), "Post Score"))
				{
					SoomlaProfile.UpdateStatus(twitterProvider, twitterMessage, null, null);
					exitedLevel = false;
				}			
			}
			else if (GUI.Button(new Rect(285, 150, 215, 50), "Log in to Twitter to Post Scores"))
			{
				SoomlaProfile.Login(twitterProvider, null, null);
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
	}

	void Update () 
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
	
	void UpdateMainMenu()
	{
		if (Input.touchCount > 0 || Input.anyKeyDown) 
		{
			m_currLevelEntity = (GameObject) Instantiate(m_levelManagerPrefab);
			LevelManager levelManager = m_currLevelEntity.GetComponent<LevelManager>();
			levelManager.SetCamera(m_gameCameraEntity);
			levelManager.m_scoreText = m_scoreText;
			levelManager.m_highScoreText = m_highScoreText;
			levelManager.m_backgroundManager = m_backgroundManager;

			m_endLevelButton.gameObject.SetActive(true);

			m_gameState = GameState.kInGame;
		}
	}
	
	void UpdateInGame()
	{
		if (m_currLevelEntity == null) 
		{		
			m_endLevelButton.gameObject.SetActive(false);
			m_scoreText.text = "Climber";

			m_gameState = GameState.kMainMenu;
		}
	}
}
