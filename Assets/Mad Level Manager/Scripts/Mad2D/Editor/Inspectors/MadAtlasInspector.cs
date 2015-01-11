/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

[CustomEditor(typeof(MadAtlas))]
public class MadAtlasInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    static Texture textureDragDrop;
    
    MadAtlas atlas;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        atlas = target as MadAtlas;
    
        textureDragDrop = Resources.Load("Mad2D/Textures/icon_drag_drop") as Texture;
    }

    public override void OnInspectorGUI() {
        SpriteList();

        EditorGUILayout.Space();

        if (GUILayout.Button("Atlas Browser")) {
            MadAtlasBrowser.Show(atlas, null);
        }

        EditorGUILayout.Space();

        DropAreaGUI();
        
    }
    
    void DropAreaGUI() {
        Event evt = Event.current;
        
        var dropArea = EditorGUILayout.BeginHorizontal();
        MadGUI.Message("Drag & drop textures HERE to add them to this atlas.\n\n\n", MessageType.None);
        EditorGUILayout.EndHorizontal();
        
        GUI.color = new Color(1f, 1f, 1f, 0.5f);
        int iconW = textureDragDrop.width;
        int iconH = textureDragDrop.height;
        GUI.DrawTexture(new Rect(dropArea.center.x - iconW / 2, dropArea.yMax - iconH - 2, iconW, iconH), textureDragDrop);
        GUI.color = Color.white;

        List<Texture2D> texturesToAdd = new List<Texture2D>();
        
        switch (evt.type) {
            case EventType.MouseDrag:
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition)) {
                    break;
                }
                    
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                
                if (evt.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag ();
                    
                    foreach (Object draggedObject in DragAndDrop.objectReferences) {
                        if (draggedObject is Texture2D) {
                            texturesToAdd.Add(draggedObject as Texture2D);
                        }
                    }
                }
                break;
        }

        if (texturesToAdd.Count > 0) {
            AddTextures(texturesToAdd.ToArray());
        }
        
    }
    
    void AddTextures(Texture2D[] textures) {
        foreach (var tex in textures) {
            if (tex == atlas.atlasTexture) {
                EditorUtility.DisplayDialog("Wrong texture", "Cannot add atlas texture to the same atlas!", "OK");
                return;
            }
        }

        MadAtlasBuilder.AddToAtlas(atlas, textures);
    }
    
    void SpriteList() {
        GUILayout.Label("Sprites", "HeaderLabel");
        
        bool hasMissingTextures = false;
        
        EditorGUI.BeginChangeCheck();
        
        MadGUI.Indent(() => {
            for(int i = 0; i < atlas.ListItems().Count; ++i) {
                MadAtlas.Item item = atlas.ListItems()[i];
            
                EditorGUILayout.BeginHorizontal();
                var texture = MadAtlasUtil.GetItemOrigin(item);
                bool valid = MadGUI.Validate(() => texture != null, () => {
                    item.name = EditorGUILayout.TextField(item.name);
                });

                GUI.enabled = valid;                
                GUI.color = Color.yellow;
                if (GUILayout.Button("Ping", GUILayout.Width(40))) {
                    EditorGUIUtility.PingObject(texture);
                }
                GUI.enabled = true;
                
                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    MadAtlasBuilder.RemoveFromAtlas(atlas, item);
                }
                GUI.color = Color.white;
                
                EditorGUILayout.EndHorizontal();
                
                if (!valid) {
                    hasMissingTextures = true;
                }
            }
            
            if (hasMissingTextures) {
                MadGUI.Error("There are sprites with missing origin texture. This will for them to dissapear on edit.");
            }
        });
        
        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(atlas);
        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}