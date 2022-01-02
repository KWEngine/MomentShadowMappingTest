#version 450

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTextureCoordinates;
layout(location = 2) in vec3 aNormal;

uniform mat4 uMatrix;
uniform mat4 uMatrixShadow;
uniform mat4 uModelMatrix;
uniform mat4 uNormalMatrix;

out vec2 vTextureCoordinates;
out vec3 vPosition;
out vec3 vNormal;
out vec4 vFragmentPosLightSpace;
out mat3 vTBN;

void main()
{
	vPosition = (uModelMatrix * vec4(aPosition, 1.0)).xyz;
	vNormal = normalize((uNormalMatrix * vec4(aNormal, 0.0)).xyz);
	vTextureCoordinates = aTextureCoordinates;

	vFragmentPosLightSpace = uMatrixShadow * vec4(vPosition, 1.0);

	vec4 positionNew = uMatrix * vec4(aPosition, 1.0);
	gl_Position = positionNew;
}