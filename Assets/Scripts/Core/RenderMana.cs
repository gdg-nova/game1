using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RenderMana : MonoBehaviour {

	public float widthRelativeToSmallestResolution = 0.16f;
	public float ylocationRelativeToSmallestResolution = 0.19f;
	public int paddingFromScreenEdge = 3;

	public Text textToUpdateWithMana = null;

	public PlayerSettings playerSettings = null;

	private Vector2 lastScreenSize;

	// Use this for initialization
	void Start () {

		if (textToUpdateWithMana == null)
		{
			Debug.LogError("TextToUpadteWithMana not set, this script won't work properly!");
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

		// Hard coded to be above the create monsters spell panel, size and location change dynamically but relative position 
		// on the screen is fix.
		// Really should be revamped to be something more generic if you want to allow a UI designer to
		// adjust it on the fly.

		float minimumDim = lastScreenSize.x < lastScreenSize.y ? lastScreenSize.x : lastScreenSize.y;
		int wantedWidth = (int)(minimumDim * widthRelativeToSmallestResolution);

		RectTransform rect = this.transform as RectTransform;
		ScaleSizeGivenExpectedWidth(rect, wantedWidth);

		int yLocation = (int)(minimumDim * ylocationRelativeToSmallestResolution);

		rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, yLocation);
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
	float lastMana = -1f;
	void Update () {
		if (lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
			RenderInternal();

		if (playerSettings.PlayerMana != lastMana)
		{
			lastMana = playerSettings.PlayerMana;
			textToUpdateWithMana.text = lastMana.ToString ();
		}
	}
}
