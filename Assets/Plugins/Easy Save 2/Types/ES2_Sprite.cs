using UnityEngine;
using System.Collections;

#if !UNITY_3_4 && !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class ES2_Sprite : ES2Type
{
	public ES2_Sprite()
	{
		key = (byte)31;
		type = typeof(Sprite);
	}
	
	public override void Write(object data, ES2Writer writer)
	{
		Sprite sprite = (Sprite)data;
		writer.Write(sprite.texture);
		writer.Write(sprite.rect);

		// Calculate and write pivot point
		float pivotX = - sprite.bounds.center.x / sprite.bounds.extents.x / 2 + 0.5f;
		float pivotY = - sprite.bounds.center.y / sprite.bounds.extents.y / 2 + 0.5f;
		writer.Write(new Vector2(pivotX, pivotY));

		// Calculate pixelsToUnits
		writer.Write(sprite.textureRect.width / sprite.bounds.size.x);
	}
	
	public override object Read(ES2Reader reader)
	{
		return Sprite.Create(reader.Read<Texture2D>(), reader.Read<Rect>(), reader.Read<Vector2>(), reader.Read<float>());
	}
}
#endif