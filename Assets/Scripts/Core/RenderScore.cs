using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RenderScore : MonoBehaviour {

	public float widthRelativeToSmallestResolution = 0.32f;

	public Text textToUpdateWithScore = null;

	public PlayerSettings playerSettings = null;

	private Vector2 lastScreenSize;

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
		float currentWidth = Mathf.Abs( rect.rect.xMax - rect.rect.xMin );
		float currentHeight = Mathf.Abs( rect.rect.yMax - rect.rect.yMin );
		float scaleBy = width / currentWidth;
		
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (int)(currentHeight*scaleBy));
		
		Text[] allText = rect.GetComponentsInChildren<Text>(true);
		foreach(Text text in allText)
		{
			text.fontSize = (int)(text.fontSize * scaleBy);
		}
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
