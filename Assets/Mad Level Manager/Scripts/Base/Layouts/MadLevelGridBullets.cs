/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

/// <summary>
/// Bullets component. Will display current page bullets on the screen.
/// Will rebuild itself on scene load and on scene play.
/// </summary>
[ExecuteInEditMode]
public class MadLevelGridBullets : MonoBehaviour {

    #region Public Fields

    // draggable object that will tell how many pages there are
    public MadDragStopDraggable draggable;

    // bullet textures
    public Texture2D bulletTextureOff;
    public Texture2D bulletTextureOn;

    // distance between bullets
    public Vector2 bulletDistance = new Vector2(64, 0);

    // gui depth for each texture
    public int guiDepth;

    // hidding managed objects
    public bool hideManagedObjects = true;

    #endregion

    #region Private Fields

    // keep the bullet sprites for the update
    private MadSprite[] bulletSprites;

    // current configuration hash code
    private int currentHash;

    #endregion

    #region Slots

    void Start() {
        if (CanRebuild()) {
            Rebuild();
        }
    }

    void Update() {
        if (RebuildNeeded() && CanRebuild()) {
            try {
                MadTransform.registerUndo = false;
                Rebuild();
            } finally {
                MadTransform.registerUndo = true;
            }
            
        } else {
            UpdateBullets();
        }
    }

    #endregion

    #region Private Methods

    void UpdateBullets() {
        if (bulletSprites == null) {
            return;
        }

        for (int i = 0; i < bulletSprites.Length; ++i) {
            bulletSprites[i].texture = BulletTexture(i);
        }
    }

    bool RebuildNeeded() {
        int hashCode = ConfigurationHash();
        if (hashCode != currentHash) {
            return true;
        } else {
            return false;
        }
    }

    int ConfigurationHash() {
        var hash = new MadHashCode();
        hash.Add(bulletTextureOff);
        hash.Add(bulletTextureOn);
        hash.Add(hideManagedObjects);
        hash.Add(bulletDistance);
        hash.Add(guiDepth);
        if (draggable != null) {
            hash.Add(draggable.dragStopCount);
        }
        return hash.GetHashCode();
    }

    bool CanRebuild() {
        return bulletTextureOff != null
            && bulletTextureOn != null
            && draggable != null;
    }

    void Rebuild() {
        currentHash = ConfigurationHash();

        RebuildClean();
        RebuildConstruct();
    }

    void RebuildClean() {
        var generated = MadTransform.FindChildren<MadSprite>(transform, (sprite) => sprite.name.StartsWith("generated_"));
        foreach (var obj in generated) {
            MadGameObject.SafeDestroy(obj.gameObject);
        }
    }

    void RebuildConstruct() {
        int count = draggable.dragStopCount;
        bulletSprites = new MadSprite[count];

        Vector3 startPosition = -(bulletDistance * (count - 1)) / 2;

        for (int i = 0; i < count; ++i) {
            var sprite = MadTransform.CreateChild<MadSprite>(transform, "generated_bullet_" + (i + 1));
            sprite.transform.localPosition = startPosition + ((Vector3) bulletDistance * i);

            sprite.texture = BulletTexture(i);
            sprite.guiDepth = guiDepth;

            bulletSprites[i] = sprite;

            if (hideManagedObjects) {
                sprite.gameObject.hideFlags = HideFlags.HideInHierarchy;
            } else {
                //sprite.gameObject.hideFlags = HideFlags.DontSave;
            }
        }
            
    }

    bool IsBulletOn(int index) {
        return draggable.dragStopCurrentIndex == index;
    }

    Texture2D BulletTexture(int index) {
        if (IsBulletOn(index)) {
            return bulletTextureOn;
        } else {
            return bulletTextureOff;
        }
    }

    #endregion

}

#if !UNITY_3_5
} // namespace
#endif