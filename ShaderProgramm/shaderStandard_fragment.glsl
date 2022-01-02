#version 450

layout(location = 0) out vec4 color;

in vec2 vTextureCoordinates;
in vec3 vPosition;
in vec3 vNormal;

uniform sampler2D uTexture;

uniform vec3 uLightPosition;
uniform vec3 uAmbientLight;

void main()
{
	

	vec3 surfaceToLight = uLightPosition - vPosition;
	float surfaceToLightDistanceSquared = dot(surfaceToLight, surfaceToLight);
	vec3 surfaceToLightNormalized = normalize(surfaceToLight);

	float dotproduct = max(dot(surfaceToLightNormalized, vNormal), 0.0);

	float sumLightPower = dotproduct * (10000.0 / surfaceToLightDistanceSquared);

	vec3 textureColor = texture(uTexture, vTextureCoordinates).xyz;
	vec3 lightIntensity = vec3(sumLightPower, sumLightPower, sumLightPower);
	color = vec4(textureColor * (lightIntensity + uAmbientLight), 1.0);
}