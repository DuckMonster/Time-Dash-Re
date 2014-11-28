#version 130

in vec3 vertexPosition;
in vec2 vertexUV;

out vec2 uv;

uniform vec2 position_offset;
uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void) 
{
	uv = vertexUV;
	vec3 pos = vertexPosition + vec3(position_offset, 1);
	gl_Position = projection_matrix * view_matrix * model_matrix * vec4(pos, 1);
}