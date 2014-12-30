using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiPauseMenuController : MonoBehaviour {

	public Animator  menuAnimator;
	public Slider	 soundFxSlider;
	public Slider	 musicFxSlider;

	public int 			paddingBetweenItems = 6;
	public float		sizeRelativeToMinimumWidth = 0.16f;
	public GameObject[] elementsToPositionOnly;
	public GameObject[] elementsToPositionAndSlideIn;

	private float fOldTimeScale = 0.0f;
	private Vector2 lastScreenSize = new Vector2();

	public void Start()
	{
		musicFxSlider.value = globalEvents.CurrentMusicOnOffState?1.0f:0.0f;
		soundFxSlider.value = globalEvents.CurrentSoundOnOffState?1.0f:0.0f;

		RenderInternal();
	}

	public void Update()
	{
		if (lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
			if (Time.timeScale != 0.0f)
				RenderInternal();
	}

	private void RenderInternal()
	{
		lastScreenSize = new Vector2(Screen.width, Screen.height);

		float minimumDim = lastScreenSize.x < lastScreenSize.y ? lastScreenSize.x : lastScreenSize.y;
		int buttonSize = (int)(minimumDim * sizeRelativeToMinimumWidth);
		int halfPadding = (paddingBetweenItems >> 1);

		int yPosition = -halfPadding;
		int positionStepSize = -buttonSize - paddingBetweenItems;
		foreach(GameObject go in elementsToPositionOnly)
		{
			SetPositionAndSizeOfRectTransform(go.transform as RectTransform, -halfPadding, yPosition, buttonSize, buttonSize);

			yPosition += positionStepSize;
		}

		yPosition = -halfPadding;
		positionStepSize = -buttonSize - paddingBetweenItems;

		foreach(GameObject go in elementsToPositionAndSlideIn)
		{
			SetPositionAndSizeOfRectTransform(go.transform as RectTransform, -halfPadding, yPosition, buttonSize, buttonSize);

			yPosition += positionStepSize;
		}
	}

	private void SetPositionAndSizeOfRectTransform(RectTransform rect, float xpos, float ypos, float width, float height)
	{
		float currentHeight = Mathf.Abs( rect.rect.yMax - rect.rect.yMin );
		float ratio = height / currentHeight;

		rect.anchoredPosition = new Vector2(xpos, ypos);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

		Text[] allText = rect.GetComponentsInChildren<Text>(true);
		foreach(Text text in allText)
		{
			text.fontSize = (int)(text.fontSize * ratio);
		}
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

	public void GoToMap()
	{
		Application.LoadLevel("LevelChooseScene");
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
