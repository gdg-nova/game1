using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Class will render a set of spells of a specific type.
// Associate to a Panel object that also has the GridLayoutGroup script associated with it.
// The rendering location is based on the properties set in the GridLayoutGroup.
// Call the SetSpells to set the list of spells to add buttons for.
public class RenderSpells : MonoBehaviour {

	public float 		sizeOfButtonsRelativeToScreen = 0.18f;
	public int   		gapBetweenButtonsInPixels = 6;
	public SpellBase.eSpellType spellTypeToRender;
	SpellBase[] spells = new SpellBase[0];
	Vector2 lastScreenSize = new Vector2();

	GridLayoutGroup gridLayoutGroup = null;

	public ToggleGroup	toggleGroupToUse { get; set; }

	// Use this for initialization
	void Start () {
		gridLayoutGroup = this.GetComponent<GridLayoutGroup>();
		if (gridLayoutGroup == null)
		{
			Debug.LogError("Grid Layout Group not found to RenderSpells");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
		{
			RenderInternal();
		}
	}

	public bool SetSpells(SpellBase[] updatedSpells, bool setFirstItemAsActive)
	{
		if (updatedSpells != null)
		{
			spells = updatedSpells;
			RenderInternal();
			if (setFirstItemAsActive && transform.childCount > 0)
			{
				// Make the first child the active one
				Transform firstChild = transform.GetChild(0);
				Toggle toggle = firstChild.GetComponent<Toggle>();
				toggle.isOn = true;
			}
			else
			{
				return setFirstItemAsActive;
			}
			return false; // Child was set
		}
		else
		{
			Debug.LogError ("Invalid set of spells");
		}
		return setFirstItemAsActive; // To give the next panel the option to set the gui.
	}

	void RenderInternal()
	{
		if (gridLayoutGroup == null) return;

		lastScreenSize = new Vector2(Screen.width, Screen.height);

		// First get the expected size of each button relative to the screen's lowest scale resolution
		int lowest = (int)(lastScreenSize.x < lastScreenSize.y ? lastScreenSize.x : lastScreenSize.y);
		if (lowest <= 0)
		{
			Debug.LogError ("Unable to determine screen resolution");
			lowest = 600;
		}
		int buttonSize = (int)(lowest * sizeOfButtonsRelativeToScreen);
		int panelHeight = buttonSize + gapBetweenButtonsInPixels;

		Debug.Log ("Buttonsize=" + buttonSize + " PanelHeight=" + panelHeight);

		List<SpellBase> filteredSpells = FilterSpells();
		int panelWidth = panelHeight * filteredSpells.Count;

		gridLayoutGroup.cellSize = new Vector2(buttonSize, buttonSize);
		gridLayoutGroup.spacing = new Vector2(gapBetweenButtonsInPixels, gapBetweenButtonsInPixels);

		RectTransform rect = gridLayoutGroup.transform as RectTransform;
		// Now add buttons for each of the spells as children
		// Clear the current children first
		GameObject[] children = new GameObject[this.transform.childCount];
		for(int i = 0; i < this.transform.childCount; i++)
		{
			children[i] = transform.GetChild (i).gameObject;
		}
		transform.DetachChildren();
		foreach(GameObject obj in children)
		{
			Destroy(obj);
		}

		// Now add each spell by creating the prefab that it selected for itself.
		int count = 1;
		foreach(SpellBase spell in filteredSpells)
		{
			//Debug.Log ("Rendering: " + spell.name);
			Toggle toggle = Instantiate(spell.guiToggleButtonPrefab) as Toggle;
			toggle.transform.parent = this.transform;
			// Reset the localscale because for some reason it gets reset even though the parent is set to vector3.one
			toggle.transform.localScale = Vector3.one;
			toggle.name = "Btn " + spell.name;
			toggle.group = toggleGroupToUse; // Set the group that the toggle belongs to
			RectTransform grphRect = toggle.targetGraphic.transform as RectTransform;
			grphRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize);
			grphRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize);
			grphRect = toggle.graphic.transform as RectTransform;
			grphRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonSize-3);
			grphRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonSize-3);

			// Set the callback to the SpellBase handler so that any external listeners get the message
			toggle.onValueChanged.RemoveAllListeners();
			toggle.onValueChanged.AddListener(spell.SetToggleState);
		}
		// Now adjust the panel because it may have recalculated the size
		if (gridLayoutGroup.startCorner == GridLayoutGroup.Corner.LowerLeft)
		{
			rect.pivot = new Vector2(0.0f, 0.0f);
		}
		else if (gridLayoutGroup.startCorner == GridLayoutGroup.Corner.LowerRight)
		{
			rect.pivot = new Vector2(1.0f, 0.0f);
		}
		else if (gridLayoutGroup.startCorner == GridLayoutGroup.Corner.UpperLeft)
		{
			rect.pivot = new Vector2(0.0f, 1.0f);
		}
		else if (gridLayoutGroup.startCorner == GridLayoutGroup.Corner.UpperRight)
		{
			rect.pivot = new Vector2(1.0f, 1.0f);
		}
		rect.anchoredPosition = new Vector2(0.0f, 0.0f);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelHeight);
	}

	List<SpellBase> FilterSpells()
	{
		List<SpellBase> list = new List<SpellBase>();
		foreach(SpellBase spell in spells)
		{
			if (spell.type == this.spellTypeToRender)
			{
				list.Add(spell);
			}
		}
		return list;
	}
}
