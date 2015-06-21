using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	const float kGravity = 9.8f;

	public Camera m_gameCamera;
	public LevelManager m_levelManager;
	public float m_maxYVel = 10.0f;
	public float m_maxXVel = 5.0f;
	public float m_jumpAmount = 8.0f;
	public float m_strafeSensitivity = 50.0f;
	public float m_strafeResistance = 10.0f;
	public float m_gravityIncDampener = 0.5f;  // NOTE: This is just a magic number
	public float m_jumpIncDampener = 0.1f;  // NOTE: This is just a magic number

	private Platform m_onPlatform = null;
	private float m_levelWidth = 0.0f;
	private float m_levelHeight = 0.0f;
	private float m_xPos = 0.0f;
	private float m_yPos = 0.0f;
	private float m_xVel = 0.0f;
	private float m_yVel = 0.0f;
	private float m_currSpeed = 1.0f;

	public void SetSpeed(float speed)
	{
		m_currSpeed = speed;
	}

	void Start ()
	{
		m_xPos = transform.position.x;
		m_yPos = transform.position.y;

		Vector3 leftCornerPos = m_gameCamera.ViewportToWorldPoint( new Vector3( 1.0f, 1.0f, m_gameCamera.nearClipPlane) );
		m_levelWidth = leftCornerPos.x * 2.0f;
		m_levelHeight = leftCornerPos.y * 2.0f; 
	}

	void FixedUpdate () 
	{
		HandleJump ();
		HandleStrafe (); 
		UpdateState ();
		UpdatePos ();
	}

	void HandleJump()
	{
		m_yVel -= (kGravity * (1.0f + (m_currSpeed*m_gravityIncDampener))) * Time.fixedDeltaTime;
		if (IsOnGround ()) 
		{
			m_yVel = -m_onPlatform.m_speed;
			m_yVel += m_jumpAmount * (1.0f + (m_currSpeed*m_jumpIncDampener) );
		}
	}

	void HandleStrafe()
	{
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

		if (Input.GetKey (KeyCode.LeftArrow) || touchLHS)
		{
			m_xVel -= m_strafeSensitivity * Time.fixedDeltaTime;
		}
		
		if (Input.GetKey (KeyCode.RightArrow) || touchRHS) 
		{
			m_xVel += m_strafeSensitivity * Time.fixedDeltaTime;
		}

		if (m_xVel > 0) 
		{
			m_xVel = Mathf.Clamp (m_xVel - (m_strafeResistance * Time.fixedDeltaTime), 0, m_maxXVel);
		} 
		else if (m_xVel < 0)
		{
			m_xVel = Mathf.Clamp (m_xVel + (m_strafeResistance * Time.fixedDeltaTime), -m_maxXVel, 0);
		}
	}

	void UpdateState()
	{
		m_xPos += m_xVel * Time.fixedDeltaTime;
		m_yPos += m_yVel * Time.fixedDeltaTime;

		const float kWallWidth = 0.6f;
		m_xPos = Mathf.Clamp (m_xPos, (-m_levelWidth/2.0f) + kWallWidth, (m_levelWidth/2.0f) - kWallWidth);
		m_yPos = Mathf.Clamp (m_yPos, -m_levelHeight/2.0f, m_levelHeight/2.0f);

		if (m_yPos == m_levelHeight / 2.0f) 
		{
			m_yVel = 0;
		}

		if (m_yPos == -m_levelHeight / 2.0f) 
		{
			m_levelManager.GameOver();
		}
	}

	void UpdatePos()
	{
		transform.position = new Vector2 (m_xPos, m_yPos);
	}

	bool IsOnGround()
	{
		return m_onPlatform != null;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Platform") 
		{
			float platformHeight = other.bounds.max.y;
			if (m_yVel < 0.0f && m_yPos > platformHeight) 
			{
				SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
				Platform platform = other.GetComponent<Platform> ();
				m_onPlatform = platform;
				m_yPos = platformHeight + (sprite.bounds.extents.y - 0.1f);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		m_onPlatform = null;
	}
}
