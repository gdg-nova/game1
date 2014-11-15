using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

// Type
public enum CreditType
{
	Title,
	SingleColumn,
	TwoColumns,
	Image,
	EmptySpace
}

// A single line of credits
public class CreditLine
{
	public CreditType type;
	public string data;
	public string data2;
	public CreditLine(CreditType type, string data, string data2)
	{
		this.type = type; this.data = data; this.data2 = data2;
	}

	// Utils functions for CreditType
	public static string typeToText(CreditType t)
	{
		switch(t)
		{
		case CreditType.Title: return "titleHLD";
		case CreditType.SingleColumn: return "singlecolumn";
		case CreditType.Image: return "texture";
		case CreditType.EmptySpace: return "space";
		default: return "twocolumns";
		}
	}
	public static CreditType textToType(string t)
	{
		if(t == "title") return CreditType.Title;
		if(t == "singlecolumn") return CreditType.SingleColumn;
		if(t == "texture") return CreditType.Image;
		if(t == "space") return CreditType.EmptySpace;
		return CreditType.TwoColumns;
	}
}

public class Credits : MonoBehaviour
{
	private enum CreditTextType
	{
		Image,
		SingleColumn,
		Title,
		TwoColumnHeader,
		TwoColumnText
	}

	private class CreditText
	{
		public CreditTextType type = CreditTextType.SingleColumn;
		public string text = "";
		public Rect position = new Rect(0, 0, 0, 0);
		public Texture2D texture = null;
	}

	// Settings
	public TextAsset creditsFile;
	public GUISkin baseSkin;
	public bool playOnAwake = true;
	public int lineSpacing = 10;
	public int speed = 100;
	public int marginPercent = 5;
	public bool fadeInOut = true;
	public int fadePercent = 5;
	public Color titleColor = Color.white;
	public bool titleBold = true;
	public int titleSize = 30;
	public Color headerColor = Color.white;
	public bool headerBold = true;
	public bool shadowText = false;
	public Vector2 shadowOffset = new Vector2(1, 1);
	public Color shadowColor = Color.black;

	// Private
	private bool started = false;
	private float startTime = 0f;
	private float lastUpdate = 0f;
	private List<CreditText> lines = new List<CreditText>();
	private int marginTop = 0;
	private int marginBot = 0;
	private int fadeTop = 0;
	private int fadeBot = 0;
	private int minPosition = -100;

	// Signals
	public event CreditsEndListener endListeners;
	public delegate void CreditsEndListener(Credits c);

	void Start()
	{
		if(playOnAwake)
		{
			beginCredits();
		}
	}

	public void beginCredits()
	{
		// Sanitize input
		if(speed <= 0) speed = 50;

		// Calculate margin & fade %
		if(marginPercent < 0) marginPercent = 0;
		if(marginPercent > 45) marginPercent = 45;
		if(fadePercent < 0) fadePercent = 0;
		if(fadePercent > 50-marginPercent) fadePercent = 50-marginPercent;
		marginTop = (int)(Screen.height*marginPercent*0.01);
		fadeTop = (int)(marginTop+Screen.height*fadePercent*0.01);
		marginBot = Screen.height-marginTop;
		fadeBot = Screen.height-fadeTop;

		// Get font line height
		int lineHeight = (int)baseSkin.label.CalcHeight(new GUIContent("lp"), Screen.width)+1;
		int originalSize = baseSkin.label.fontSize;
		baseSkin.label.fontSize = titleSize;
		int lineTitleHeight = (int)baseSkin.label.CalcHeight(new GUIContent("lp"), Screen.width)+1;
		baseSkin.label.fontSize = originalSize;

		// Starting Y
		int y = Screen.height+lineSpacing;

		// Min position
		minPosition = Mathf.Max(lineHeight, lineTitleHeight);

		// Helpers
		int screenWidth = Screen.width;
		int middle = (int)(screenWidth*0.5f);

		// Load from XML and calculate position
		XmlDocument root = new XmlDocument();
		root.LoadXml(creditsFile.text);
		foreach(XmlNode node in root.SelectNodes("credits/credit"))
		{
			CreditType type = CreditLine.textToType(node.Attributes.GetNamedItem("type").Value);
			string data = node.Attributes.GetNamedItem("data").Value;
			string data2 = node.Attributes.GetNamedItem("data2").Value;

			if(type == CreditType.EmptySpace)
			{
				int v = 1;
				try{v = int.Parse(data);}catch{}
				y += v*(lineHeight+lineSpacing);
			}
			else if(type == CreditType.Image)
			{
				CreditText line = new CreditText();
				line.type = CreditTextType.Image;
				line.texture = Resources.Load(data, typeof(Texture2D)) as Texture2D;
				if(line.texture != null)
				{
					line.position = new Rect(middle-(line.texture.width/2), y, line.texture.width, line.texture.height);
					lines.Add(line);
					y += line.texture.height + lineSpacing;
				}

				// Min/Max
				if(minPosition < line.texture.height)
					minPosition = line.texture.height;
			}
			else if(type == CreditType.SingleColumn)
			{
				CreditText line = new CreditText();
				line.type = CreditTextType.SingleColumn;
				line.text = data;
				line.position = new Rect(0, y, screenWidth, lineHeight);
				lines.Add(line);
				y += lineHeight + lineSpacing;
			}
			else if(type == CreditType.Title)
			{
				CreditText line = new CreditText();
				line.type = CreditTextType.Title;
				line.text = data;
				line.position = new Rect(0, y, screenWidth, lineTitleHeight);
				lines.Add(line);
				y += lineTitleHeight + lineSpacing;
			}
			else if(type == CreditType.TwoColumns)
			{
				CreditText line = new CreditText();
				line.type = CreditTextType.TwoColumnHeader;
				line.text = data;
				line.position = new Rect(0, y, middle-20, lineHeight);
				CreditText line2 = new CreditText();
				line2.type = CreditTextType.TwoColumnText;
				line2.text = data2;
				line2.position = new Rect(middle+20, y, middle-20, lineHeight);
				lines.Add(line);
				lines.Add(line2);
				y += lineHeight + lineSpacing;
			}
		}

		// Start
		started = true;
		startTime = Time.time;
		lastUpdate = startTime;
	}

	void Update()
	{
		if(started)
		{
			float delta = Time.time - lastUpdate;
			bool active = false;

			for(int i = 0; i < lines.Count; i++)
			{
				lines[i].position.y -= speed*delta;
				if(lines[i].position.y > -minPosition)
					active = true;
			}

			if(!active)
			{
				if(endListeners != null)
					endListeners(this);
				started = false;
			}

			lastUpdate = Time.time;
		}
	}

	void OnGUI()
	{
		if(started)
		{
			// Get skin settings
			GUISkin originalSkin = GUI.skin;
			GUI.skin = baseSkin;
			Color originalColor = GUI.color;
			int originalSize = GUI.skin.label.fontSize;
			TextAnchor originalAlignment = GUI.skin.label.alignment;
			FontStyle originalStyle = GUI.skin.label.fontStyle;

			// Foreach lines...
			for(int i = 0; i < lines.Count; i++)
			{
				// Check if visible
				if(lines[i].position.y < marginBot && lines[i].position.y > -minPosition)
				{
					// Alpha
					float alpha = 1f;
					if(fadeInOut)
					{
						if(lines[i].position.y > fadeBot)
							alpha = (marginBot-lines[i].position.y)/(marginBot-fadeBot);
						if(lines[i].position.y < fadeTop)
							alpha = 1f-((fadeTop-lines[i].position.y)/(fadeTop-marginTop));
					}
					Color temp;

					// Draw
					if(lines[i].type == CreditTextType.Image)
					{
						temp = Color.white; temp.a = alpha; GUI.color = temp;
						GUI.DrawTexture(lines[i].position, lines[i].texture);
					}
					else if(lines[i].type == CreditTextType.SingleColumn)
					{
						GUI.skin.label.fontSize = originalSize;
						GUI.skin.label.fontStyle = originalStyle;
						GUI.skin.label.alignment = TextAnchor.UpperCenter;
						if(shadowText)
						{
							temp = shadowColor; temp.a = alpha; GUI.color = temp;
							Rect r = lines[i].position; r.x += shadowOffset.x; r.y += shadowOffset.y;
							GUI.Label(r, lines[i].text);
						}
						temp = originalColor; temp.a = alpha; GUI.color = temp;
						GUI.Label(lines[i].position, lines[i].text);
					}
					else if(lines[i].type == CreditTextType.Title)
					{
						GUI.skin.label.fontSize = titleSize;
						GUI.skin.label.fontStyle = (titleBold)?FontStyle.Bold:FontStyle.Normal;
						GUI.skin.label.alignment = TextAnchor.UpperCenter;
						if(shadowText)
						{
							temp = shadowColor; temp.a = alpha; GUI.color = temp;
							Rect r = lines[i].position; r.x += shadowOffset.x; r.y += shadowOffset.y;
							GUI.Label(r, lines[i].text);
						}
						temp = titleColor; temp.a = alpha; GUI.color = temp;
						GUI.Label(lines[i].position, lines[i].text);
					}
					else if(lines[i].type == CreditTextType.TwoColumnHeader)
					{
						GUI.skin.label.fontSize = originalSize;
						GUI.skin.label.fontStyle = (headerBold)?FontStyle.Bold:FontStyle.Normal;
						GUI.skin.label.alignment = TextAnchor.UpperRight;
						if(shadowText)
						{
							temp = shadowColor; temp.a = alpha; GUI.color = temp;
							Rect r = lines[i].position; r.x += shadowOffset.x; r.y += shadowOffset.y;
							GUI.Label(r, lines[i].text);
						}
						temp = headerColor; temp.a = alpha; GUI.color = temp;
						GUI.Label(lines[i].position, lines[i].text);
					}
					else if(lines[i].type == CreditTextType.TwoColumnText)
					{
						GUI.skin.label.fontSize = originalSize;
						GUI.skin.label.fontStyle = originalStyle;
						GUI.skin.label.alignment = TextAnchor.UpperLeft;
						if(shadowText)
						{
							temp = shadowColor; temp.a = alpha; GUI.color = temp;
							Rect r = lines[i].position; r.x += shadowOffset.x; r.y += shadowOffset.y;
							GUI.Label(r, lines[i].text);
						}
						temp = originalColor; temp.a = alpha; GUI.color = temp;
						GUI.Label(lines[i].position, lines[i].text);
					}
				}
			}

			// Restore GUI settings
			GUI.color = originalColor;
			GUI.skin.label.fontSize = originalSize;
			GUI.skin.label.fontStyle = originalStyle;
			GUI.skin.label.alignment = originalAlignment;
			GUI.skin = originalSkin;
		}
	}
}
