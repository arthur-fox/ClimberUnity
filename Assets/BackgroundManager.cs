using UnityEngine;
using System.Collections;

public class BackgroundManager : MonoBehaviour 
{
	public ScrollingBackground m_scrollingBackground;

	public void SetSpeed(float speed) 
	{
		m_scrollingBackground.SetSpeed(speed);
	}
}