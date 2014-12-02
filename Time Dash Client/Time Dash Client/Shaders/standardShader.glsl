﻿@v------------------

#version 330

in vec2 vertexPosition;
in vec2 vertexUV;

uniform vec2 position;
uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec2 uv;

void main() {
	uv = vertexUV;
	gl_Position = projection * view * model * (vec4(vertexPosition, 0.0, 1.0) + vec4(position, 0.0, 1.0));
}
	
@------------------

@f------------------

#version 330

uniform sampler2D texture;
uniform vec4 color;

in vec2 uv;

out vec4 fragment;

void main() {
	fragment = texture2D(texture, uv) * color;
}

@-------------------