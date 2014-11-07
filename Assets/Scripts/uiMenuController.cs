using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class uiMenuController : MonoBehaviour {

	public Animator  menuAnimator;
	public Slider	 soundFxSlider;
	public Slider	 musicFxSlider;

	private float fOldTimeScale = 0.0f;

	public void Start()
	{
		musicFxSlider.value = globalEvents.CurrentMusicOnOffState?1.0f:0.0f;
		soundFxSlider.value = globalEvents.CurrentSoundOnOffState?1.0f:0.0f;
	}

	public void OnShowMenu()
	{
		if (menuAnimator.GetInteger("showState") == 1)
		{
			//Debug.Log ("Currently visible to hide animation + reset timescale");
			menuAnimator.SetInteger("showState",2);
			PauseGame();
		}
		else
		{
			//Debug.Log ("Currently not visible so start animation");
			menuAnimator.SetInteger("showState",1);
		}
	}

	public void PauseGame()
	{
		if (menuAnimator.GetInteger("showState") == 1)
		{
			//Debug.Log ("Current timeScale = " + Time.timeScale);
			if (Time.timeScale != 0.0f)
			{
				fOldTimeScale = Time.timeScale;
				Time.timeScale = 0.0f;
			}
		}
		else
		{
			//Debug.Log ("Old time scale = " + fOldTimeScale);
			if (fOldTimeScale > 0.0f)
				Time.timeScale = fOldTimeScale;
		}

	}

	public void OnMusicVolumeChange(float vol)
	{
		bool onOrOff = (vol > 0.5f);
		globalEvents.OnMusicOnOff(this, onOrOff);
	}

	public void OnSoundVolumeChange(float vol)
	{
		bool onOrOff = (vol > 0.5f);
		globalEvents.OnSoundOnOff(this, onOrOff);
	}

}
