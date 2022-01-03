#version 450

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTextureCoordinates;
layout(location = 2) in vec3 aNormal;

out vec2 vTextureCoordinates;

void main()
{
	vTextureCoordinates = aTextureCoordinates;
	gl_Position = vec4(aPosition * 2.0, 1.0);
}