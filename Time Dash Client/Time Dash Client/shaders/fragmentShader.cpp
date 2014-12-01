#version 130

uniform sampler2D texture;
uniform bool textureEnabled;
uniform vec4 u_color;

in vec2 uv;
out vec4 fragment;

void main(void)
{
	if (textureEnabled) {
		fragment = texture2D(texture, uv) * u_color;
	}
	else {
		fragment = u_color;
	}

	fragment = vec4(1, 1, 1, 1);
}