@v------------------

#version 330

in vec2 vertexPosition;
in vec2 vertexUV;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec2 uv;

void main() {
	uv = vertexUV;
	gl_Position = projection * view * model * vec4(vertexPosition, 0.0, 1.0);
}
	
@------------------

@f------------------

#version 330

uniform sampler2D texture;
uniform vec4 color;
uniform bool fillColor;

uniform vec2 textureSize;
uniform vec2 tileSize;
uniform vec2 tilePosition;

in vec2 uv;

out vec4 fragment;

void main() {
	vec2 tile = tileSize / textureSize;
	vec2 tilePos = tile * tilePosition;

	vec2 texPos = vec2(tilePos.x + tile.x * uv.x, tilePos.y + tile.y * uv.y);

	vec4 finalColor = texture2D(texture, texPos) * color;

	if (fillColor) 
	{
		finalColor.rgb = color.rgb;
	}

    if (finalColor.a > 0.0) fragment = finalColor;
    else discard;
}

@-------------------