using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour 
{
	public float m_speed = 1;

	void FixedUpdate () 
	{
		float newY = transform.position.y - (m_speed * Time.fixedDeltaTime);
		transform.position = new Vector2 (transform.position.x, newY);
	}
}
