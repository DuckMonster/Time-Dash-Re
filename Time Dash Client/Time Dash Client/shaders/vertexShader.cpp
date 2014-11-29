#version 130

in vec2 vertexPosition;
in vec2 vertexUV;

out vec2 uv;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void) {
	uv = vertexUV;
	gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 0, 1);
}