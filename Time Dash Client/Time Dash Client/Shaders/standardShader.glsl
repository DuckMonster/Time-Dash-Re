@v------------------

#version 330

in vec3 vertexPosition;
in vec2 vertexUV;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec2 uv;

void main() {
	uv = vertexUV;
	gl_Position = projection * view * model * vec4(vertexPosition, 1.0);
}
	
@------------------

@f------------------

#version 330

uniform sampler2D texture;
uniform bool usingTexture;
uniform vec4 color;
uniform bool fillColor;
uniform bool usingBlur;

uniform int blurRadius;
uniform float blurIntensity;

uniform int textureWidth;
uniform int textureHeight;

in vec2 uv;

out vec4 fragment;

vec4 blur();

void main() {
	vec4 finalColor;

	if (usingTexture) {
		if (usingBlur)
			finalColor = mix(texture2D(texture, uv), blur(), blurIntensity);
		else 
			finalColor = texture2D(texture, uv) * color;
	} else {
		finalColor = color;
	}

	if (fillColor) 
	{
		finalColor.rgb = color.rgb;
	}

	if (finalColor.a <= 0) discard;
	else fragment = finalColor;
}

vec4 blur() {
	vec4 sum = vec4(0,0,0,0);

	float blurSizeH = 1.0 / textureWidth;
	float blurSizeV = 1.0 / textureHeight;

	for(int x=-blurRadius; x<=blurRadius; x++)
		for(int y=-blurRadius; y<=blurRadius; y++) {
			sum += texture2D(
				texture,
				vec2(uv.x + x * blurSizeH, uv.y + y * blurSizeV)
			) / ((blurRadius*2+1) * (blurRadius*2+1));
		}

	return sum;
}

@-------------------