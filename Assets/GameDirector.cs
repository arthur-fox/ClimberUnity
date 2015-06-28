using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameDirector : MonoBehaviour 
{
	public GameObject m_gameCameraEntity;
	public BackgroundManager m_backgroundManager;
	public Button m_endLevelButton;
	public Text m_scoreText;
	public Text m_highScoreText;
	public GameObject m_levelManagerPrefab;
		
	private enum GameState
	{
		kMainMenu = 0,
		kInGame = 1
	}
	private GameState m_gameState = GameState.kMainMenu;
	private GameObject m_currLevelEntity = null;

	public void QuitLevel()
	{
		if (m_currLevelEntity != null)
		{
			Destroy (m_currLevelEntity);
			m_currLevelEntity = null;
		}
	}

	void Awake()
	{
		Application.targetFrameRate = 60;
	}

	void Start()
	{
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
