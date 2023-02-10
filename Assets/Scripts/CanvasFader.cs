using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFader : MonoBehaviour
{
	public Image group;

	public float fadeInTime = 0f;
	public float fadeOutTime = 1f;
    private bool fading = false;
    
    public void FadeOut()
	{
		StartCoroutine(FadeRoutine(false));
	}

	public void FadeIn()
	{
		gameObject.SetActive(true);
		StartCoroutine(FadeRoutine(true));
	}

	private IEnumerator FadeRoutine(bool fadeIn)
	{
		float to = fadeIn ? 1 : 0;
		float t = 0;
		fading = true;
		
		while (t < 1)
		{
			t += Time.deltaTime * (fadeIn ? fadeInTime : fadeOutTime);
			Color color = group.color;
			color.a = Mathf.Lerp(color.a, to, t);
			group.color = color;
			yield return null;
		}
		
		
		fading = false;

		if (!fadeIn) gameObject.SetActive(false);
	}
}
