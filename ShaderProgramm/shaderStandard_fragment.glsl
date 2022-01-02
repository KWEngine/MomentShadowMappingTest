#version 450

layout(location = 0) out vec4 color;

in vec2 vTextureCoordinates;
in vec3 vPosition;
in vec3 vNormal;
in vec4 vFragmentPosLightSpace;

uniform sampler2D uTexture;
uniform sampler2D uTextureShadow;

uniform vec3 uLightPosition;
uniform vec3 uAmbientLight;

float shadowCalculation()
{
    // perform perspective divide
    vec3 projCoords = vFragmentPosLightSpace.xyz / vFragmentPosLightSpace.w;
    projCoords = projCoords * 0.5 + 0.5; 
	float closestDepth = texture(uTextureShadow, projCoords.xy).r;  
	float currentDepth = projCoords.z;  

	float shadow = currentDepth < closestDepth + 0.00001 ? 1.0 : 0.0;  
	//return shadow;
	float avAmbient = (uAmbientLight.r + uAmbientLight.g + uAmbientLight.b) / 3.0;
	return max(shadow, avAmbient);
}

void main()
{
	vec3 surfaceToLight = uLightPosition - vPosition;
	float surfaceToLightDistanceSquared = dot(surfaceToLight, surfaceToLight);
	vec3 surfaceToLightNormalized = normalize(surfaceToLight);

	float dotproduct = max(dot(surfaceToLightNormalized, vNormal), 0.0);

	float sumLightPower = dotproduct * (100.0 / surfaceToLightDistanceSquared);

	vec3 textureColor = texture(uTexture, vTextureCoordinates).xyz;
	vec3 lightIntensity = vec3(sumLightPower, sumLightPower, sumLightPower) * shadowCalculation();
	//color = vec4(textureColor * (lightIntensity + uAmbientLight), 1.0);
	color = vec4(textureColor * shadowCalculation(), 1.0);
}