/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MadLevelManager;

// not finished, do not use
// most probably it shouldn't be done like that
public class MadAtlasConverter {

    public static void Convert() {
        MadUndo.LegacyRegisterSceneUndo("Convert to Atlas");

        var allSprites = GameObject.FindObjectsOfType(typeof(MadSprite)) as MadSprite[];

        var texturedSprites =
            from sprite in allSprites
            where sprite.inputType == MadSprite.InputType.SingleTexture && sprite.texture != null
            select sprite;

        var textures =
            from sprite in texturedSprites
            select sprite.texture;

        textures = textures.Distinct();

        var atlas = MadAtlasBuilder.CreateAtlas(textures.ToArray());
        if (atlas == null) {
            EditorUtility.DisplayDialog("Convert", "Conversion cancelled!", "OK");
            return;
        }

        foreach (var sprite in texturedSprites) {
            sprite.inputType = MadSprite.InputType.TextureAtlas;
            sprite.textureAtlas = atlas;
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sprite.texture));
            sprite.textureAtlasSpriteGUID = guid;

            EditorUtility.SetDirty(sprite);
        }

        EditorUtility.DisplayDialog("Convert", "Conversion done! Please save your scene!", "OK");
    }

}