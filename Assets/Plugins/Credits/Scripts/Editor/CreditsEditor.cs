using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class CreditsEditor : EditorWindow
{
	// Current file and its content
	static private string currentFile = "";
	private List<CreditLine> lines = new List<CreditLine>();

	// GUI Stuff
	private static GUIStyle stylecenter = null;
	private static GUIStyle styleright = null;
	private Vector2 scrollPos;

	// Menu
	[MenuItem("Window/Credits Editor")]
	public static void ShowWindow()
	{
		CreditsEditor myself = EditorWindow.GetWindow(typeof(CreditsEditor)) as CreditsEditor;
		if(CreditsEditor.currentFile != "")
			myself.openFile(CreditsEditor.currentFile);
	}

	// The GUI
	void OnGUI()
	{
		// Set style if they do not exists
		if(stylecenter == null)
		{
			stylecenter = new GUIStyle(EditorStyles.textField);
			stylecenter.alignment = TextAnchor.MiddleCenter;
		}
		if(styleright == null)
		{
			styleright = new GUIStyle(EditorStyles.textField);
			styleright.alignment = TextAnchor.MiddleRight;
		}

		// Super silly: Create an empty textfield to give focus when we will move lines.
		GUI.SetNextControlName("sillydummytextfield");
		EditorGUI.TextField(new Rect(-10, -10, 0, 0), "");

		// Top
		EditorGUILayout.BeginHorizontal(GUIStyle.none, GUILayout.MinWidth(180));
		if(GUILayout.Button("New file..."))
		{
			EditorGUI.FocusTextInControl("sillydummytextfield");
			newFile();
		}
		if(GUILayout.Button("Open file..."))
		{
			EditorGUI.FocusTextInControl("sillydummytextfield");
			openFile();
		}
		if(currentFile != "")
		{
			if(GUILayout.Button("Save"))
			{
				EditorGUI.FocusTextInControl("sillydummytextfield");
				saveFile();
			}
			if(GUILayout.Button("Add a line"))
			{
				EditorGUI.FocusTextInControl("sillydummytextfield");
				addLine();
			}
		}
		EditorGUILayout.EndHorizontal();

		// Central
		if(currentFile != "")
		{
			// Display current line
			EditorGUILayout.LabelField("Current file:", currentFile);

			// Lines
			scrollPos = GUI.BeginScrollView(new Rect(0, 45, position.width, position.height-45), scrollPos, new Rect(0, 0, position.width-15, 10+(25*lines.Count)));
			for(int i = 0; i < lines.Count; i++)
			{
				lines[i].type = (CreditType)EditorGUI.EnumPopup(new Rect(10, 10+(i*25), 110, 17), "", lines[i].type);

				if(lines[i].type == CreditType.TwoColumns)
				{
					lines[i].data = EditorGUI.TextField(new Rect(130, 10+(i*25), (position.width-380)/2, 17), lines[i].data, styleright);
					lines[i].data2 = EditorGUI.TextField(new Rect(130+((position.width-380)/2)+10, 10+(i*25), (position.width-380)/2, 17), lines[i].data2);
				}
				else if(lines[i].type == CreditType.EmptySpace)
				{
					int v = 1;
					try{v = int.Parse(lines[i].data);}catch{}
					lines[i].data = ""+EditorGUI.IntField(new Rect(130, 10+(i*25), position.width-370, 17), "Empty lines:", v);
				}
				else
				{
					lines[i].data = EditorGUI.TextField(new Rect(130, 10+(i*25), position.width-370, 17), lines[i].data, stylecenter);
				}

				if(i > 0 && GUI.Button(new Rect(position.width-230, 10+(i*25), 40, 17), "Up"))
				{
					EditorGUI.FocusTextInControl("sillydummytextfield");
					CreditLine line = lines[i];
					lines.RemoveAt(i);
					lines.Insert(i-1, line);
					return;
				}
				if(i < lines.Count-1)
				{
					if(GUI.Button(new Rect(position.width-180, 10+(i*25), 50, 17), "Down"))
					{
						EditorGUI.FocusTextInControl("sillydummytextfield");
						CreditLine line = lines[i];
						lines.RemoveAt(i);
						lines.Insert(i+1, line);
						return;
					}
				}
				if(GUI.Button(new Rect(position.width-120, 10+(i*25), 40, 17), "Ins"))
				{
					EditorGUI.FocusTextInControl("sillydummytextfield");
					lines.Insert(i+1, new CreditLine(CreditType.TwoColumns, "", ""));
					return;
				}
				if(GUI.Button(new Rect(position.width-70, 10+(i*25), 40, 17), "Del"))
				{
					EditorGUI.FocusTextInControl("sillydummytextfield");
					lines.RemoveAt(i); return;
				}
			}
			GUI.EndScrollView();
		}
	}

	void newFile()
	{
		string file = EditorUtility.SaveFilePanel("Save Credits file", "", "credits.xml", "xml");
		XmlDocument doc = new XmlDocument();
		XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
		doc.AppendChild(docNode);
		XmlNode creditsNode = doc.CreateElement("credits");
		doc.AppendChild(creditsNode);
		doc.Save(file);
		openFile(file);
	}

	void openFile(string filename = "")
	{
		// Lazy try-catching :(
		try
		{
			if(filename == "")
				filename =  EditorUtility.OpenFilePanel("Open Credits file", "", "xml");
			if(filename != "")
			{
				// Clear precedent info
				lines.Clear();
				currentFile = filename;
				
				// Load XML
				string content = File.ReadAllText(filename);
				XmlDocument root = new XmlDocument();
				root.LoadXml(content);
				
				// Read credits
				foreach(XmlNode node in root.SelectNodes("credits/credit"))
				{
					CreditType type = CreditLine.textToType(node.Attributes.GetNamedItem("type").Value);
					string data = node.Attributes.GetNamedItem("data").Value;
					string data2 = node.Attributes.GetNamedItem("data2").Value;
					lines.Add(new CreditLine(type, data, data2));
				}
			}
		}
		catch
		{
			lines.Clear();
			currentFile = "";
			this.ShowNotification(new GUIContent("Incorrect file."));
		}
	}

	void saveFile()
	{
		// Lazy try-catching :(
		try
		{
			XmlDocument doc = new XmlDocument();
			XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			doc.AppendChild(docNode);
			XmlNode creditsNode = doc.CreateElement("credits");
			doc.AppendChild(creditsNode);

			XmlNode creditNode;
			XmlAttribute attrType;
			XmlAttribute attrData;
			XmlAttribute attrData2;
			for(int i = 0; i < lines.Count; i++)
			{
				creditNode = doc.CreateElement("credit");
				attrType = doc.CreateAttribute("type");
				attrType.Value = CreditLine.typeToText(lines[i].type);
				attrData = doc.CreateAttribute("data");
				attrData.Value = lines[i].data;
				attrData2 = doc.CreateAttribute("data2");
				attrData2.Value = lines[i].data2;
				creditNode.Attributes.Append(attrType);
				creditNode.Attributes.Append(attrData);
				creditNode.Attributes.Append(attrData2);
				creditsNode.AppendChild(creditNode);
			}

			doc.Save(currentFile);
			AssetDatabase.Refresh();
			this.ShowNotification(new GUIContent("Credits saved."));
		}
		catch
		{
			this.ShowNotification(new GUIContent("Error while saving."));
		}

	}

	void addLine()
	{
		lines.Add(new CreditLine(CreditType.TwoColumns, "", ""));
	}
}
