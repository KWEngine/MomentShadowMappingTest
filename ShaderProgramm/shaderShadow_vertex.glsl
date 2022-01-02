#version 450

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTextureCoordinates;
layout(location = 2) in vec3 aNormal;

uniform mat4 uMatrix;

void main()
{
	gl_Position = uMatrix * vec4(aPosition, 1.0);
}