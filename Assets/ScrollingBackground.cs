using UnityEngine;
using System.Collections;

public class ScrollingBackground : MonoBehaviour {

	const float kHeight = 13.66f; // TODO: Don't just have this as a Magic number based on height of sprites

	public float m_speed = 1.0f;

	private float m_y = 0;

	public void SetSpeed(float speed)
	{
		m_speed = speed;
	}

	void Start ()
	{
		m_y = transform.position.y;
	}

	void FixedUpdate () 
	{
		m_y -= m_speed * Time.fixedDeltaTime;

		if (m_y <= -kHeight) 
		{
			m_y = kHeight;
		}

		transform.position = new Vector2(transform.position.x, m_y);
	}
}
