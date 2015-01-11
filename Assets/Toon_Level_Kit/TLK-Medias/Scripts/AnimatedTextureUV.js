var uvAnimationTileX = 8; //Here you can place the number of columns of your sheet. 
var uvAnimationTileY = 4; //Here you can place the number of rows of your sheet. 
var frameMax = 32;

var framesPerSecond = 10.0;
 
function Update () {
 
	// Calculate index
	var index : int = Time.time * framesPerSecond;
	// repeat when exhausting all frames
	index = index % (frameMax);
 
	// Size of every tile
	var size = Vector2 (1.0 / uvAnimationTileX, 1.0 / uvAnimationTileY);
 
	// split into horizontal and vertical index
	var uIndex = index % uvAnimationTileX;
	var vIndex = index / uvAnimationTileX;
 
	// build offset
	// v coordinate is the bottom of the image in opengl so we need to invert.
	var offset = Vector2 (uIndex * size.x, 1.0 - size.y - vIndex * size.y);
 
	renderer.material.SetTextureOffset ("_MainTex", offset);
	renderer.material.SetTextureScale ("_MainTex", size);
}

