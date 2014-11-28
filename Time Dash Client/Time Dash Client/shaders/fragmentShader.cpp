#version 130

uniform sampler2D texture;
uniform bool textureEnabled;
uniform vec4 u_color;

in vec2 uv;

out vec4 fragment;

void main(void)
{
	if (textureEnabled) {
		fragment = u_color * texture2D(texture, uv);
	}
	else {
		fragment = u_color;
	}
}