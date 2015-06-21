using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameDirector : MonoBehaviour 
{
	public GameObject m_gameCameraEntity;
	public BackgroundManager m_backgroundManager;
	public Text m_scoreText;
	public GameObject m_levelManagerPrefab;
		
	private enum GameState
	{
		kMainMenu = 0,
		kInGame = 1
	}
	private GameState m_gameState = GameState.kMainMenu;
	private GameObject m_currLevelEntity = null;

	void Awake()
	{
		Application.targetFrameRate = 60;
	}

	void Start()
	{
		m_backgroundManager.SetSpeed (0.0f);
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
			levelManager.m_backgroundManager = m_backgroundManager;

			m_gameState = GameState.kInGame;
		}
	}
	
	void UpdateInGame()
	{
		if (m_currLevelEntity == null) 
		{		
			m_gameState = GameState.kMainMenu;
		}
	}
}
