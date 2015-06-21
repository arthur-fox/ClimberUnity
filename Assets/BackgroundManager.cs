using UnityEngine;
using System.Collections;

public class BackgroundManager : MonoBehaviour 
{
	public ScrollingBackground[] m_scrollingBackgrounds;

	public void SetSpeed(float speed) 
	{
		for (int i = 0; i < m_scrollingBackgrounds.Length; ++i) 
		{
			m_scrollingBackgrounds[i].SetSpeed(speed);
		}
	}
}
