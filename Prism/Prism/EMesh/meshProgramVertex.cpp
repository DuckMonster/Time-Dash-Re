#version 330

in vec3 vertexPosition;
in vec2 vertexUV;
in vec3 vertexHSL;

uniform mat4 view;
uniform mat4 projection;

out vec2 _uv;
out vec3 _hsl;

void main() {
	_uv = vertexUV;
	_hsl = vertexHSL;
	gl_Position = projection * view * vec4(vertexPosition, 1.0);
}