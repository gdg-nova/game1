using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RenderScore : MonoBehaviour {

	public float widthRelativeToSmallestResolution = 0.32f;

	public Text textToUpdateWithScore = null;

	public PlayerSettings playerSettings = null;

	private Vector2 lastScreenSize;
	private Vector2 originalPanelSize;
	private int originalFontSize;

	// Use this for initialization
	void Start () {

		if (textToUpdateWithScore == null)
		{
			Debug.LogError("TextToUpadteWithScore not set, this script won't work properly!");
		}

		if (playerSettings == null)
		{
			// Try to find it within the scene as a tag
			GameObject go = GameObject.FindGameObjectWithTag("PlayerSettings");
			if (go != null)
			{
				playerSettings = go.GetComponent<PlayerSettings>();
			}
			if (playerSettings == null)
			{
				Debug.LogError("PlayerSettings object not found or not set, this script won't work properly!");
			}
		}

		RectTransform rect = this.transform as RectTransform;
		originalPanelSize.x = Mathf.Abs( rect.rect.xMax - rect.rect.xMin );
		originalPanelSize.y = Mathf.Abs( rect.rect.yMax - rect.rect.yMin );
		originalFontSize = textToUpdateWithScore.fontSize;
		RenderInternal();
	}

	private void RenderInternal()
	{
		lastScreenSize = new Vector2(Screen.width, Screen.height);

		// Hard coded to just adjust the width and nothing else, UI designer can move it around

		float minimumDim = lastScreenSize.x < lastScreenSize.y ? lastScreenSize.x : lastScreenSize.y;
		int wantedWidth = (int)(minimumDim * widthRelativeToSmallestResolution);

		RectTransform rect = this.transform as RectTransform;
		ScaleSizeGivenExpectedWidth(rect, wantedWidth);

	}

	private void ScaleSizeGivenExpectedWidth(RectTransform rect, int width)
	{
		float originalWidth = originalPanelSize.x;
		float originalHeight = originalPanelSize.y;
		float scaleBy = width / originalWidth;
		
		Text[] allText = rect.GetComponentsInChildren<Text>(true);
		foreach(Text text in allText)
		{
			//Debug.Log ("Original font size: " + text.fontSize + " Font size: " + (int)(originalFontSize * scaleBy));
			text.fontSize = (int)(originalFontSize * scaleBy);
		}

		//Debug.Log ("Original width: " + originalWidth + " width: " + width);
		//Debug.Log ("Original height: " + originalHeight + " height: " + (int)(originalHeight*scaleBy));
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (int)(originalHeight*scaleBy)+2);
	}
	
	// Update is called once per frame
	float lastScore = -1f;
	void Update () {
		if (lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
			RenderInternal();

		if (playerSettings.PlayerScore != lastScore)
		{
			lastScore = playerSettings.PlayerScore;
			textToUpdateWithScore.text = "Score: " + lastScore.ToString ();
		}
	}
}
