@v------------------

#version 330
in vec3 vertexPosition;
in vec4 vertexTemp;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main() {
	vec3 pos = vertexPosition + vec3(vertexTemp.xyz);
	gl_Position = vec4(pos, 1.0);
}
	
@------------------

@f------------------

#version 330
out vec4 fragment;

void main() {
	fragment = vec4(1.0, 1.0, 1.0, 1.0);
}

@-------------------