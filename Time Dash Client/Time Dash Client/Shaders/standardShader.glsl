@v------------------

#version 330
in vec3 vertexPosition;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main() {
	gl_Position = projection * view * model * vec4(vertexPosition, 1.0);
}
	
@------------------

@f------------------

#version 330
out vec4 fragment;

void main() {
	fragment = vec4(1.0, 1.0, 1.0, 1.0);
}

@-------------------