/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace MadLevelManager {

public class MadLevelGridBulletsTool : MonoBehaviour {

    #region Constants

    private const string BulletOff = "Assets/Mad Level Manager/Examples/Textures/bullet-off.png";
    private const string BulletOn = "Assets/Mad Level Manager/Examples/Textures/bullet-on.png";

    #endregion

    #region Static Methods

    public static MadLevelGridBullets Create(MadPanel panel) {
        var anchor = MadTransform.CreateChild<MadAnchor>(panel.transform, "Bullets Anchor");

        anchor.mode = MadAnchor.Mode.ScreenAnchor;
        anchor.position = MadAnchor.Position.Bottom;

        var bullets = MadTransform.CreateChild<MadLevelGridBullets>(anchor.transform, "Bullets");
        bullets.transform.localPosition = new Vector3(0, 50, 0);

        var draggable = MadTransform.FindChild<MadDragStopDraggable>(panel.transform, int.MaxValue);
        bullets.draggable = draggable;

        bullets.bulletTextureOff = (Texture2D) AssetDatabase.LoadAssetAtPath(BulletOff, typeof(Texture2D));
        bullets.bulletTextureOn = (Texture2D) AssetDatabase.LoadAssetAtPath(BulletOn, typeof(Texture2D));

        return bullets;
    }

    #endregion

}

} // namespace