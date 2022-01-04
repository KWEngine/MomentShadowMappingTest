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

const float s3 = sqrt(3.0) / 2.0;
const float s12 = sqrt(12.0) / -9.0;
const vec4 addition = vec4(0.5, 0.0, 0.5, 0.0);
const mat4 quantizationMatrix2 = mat4(
										 1.5,  0.0,   s3,  0.0, 
										 0.0,  4.0,  0.0,  0.5,
										-2.0,  0.0,  s12,  0.0,
										 0.0, -4.0,  0.0,  0.5
										);
const mat4 quantizationMatrix2Inv = mat4(
										-0.333333, 0.0,   -0.75,      0.0,
										 0.0,      0.125,  0.0,      -0.125,
										 1.732051, 0.0,    1.299038,  0.0,
										 0.0,      1.0,    0.0,       1.0
										);

const float alpha = 0.00005;

float shadowCalculationMSM(vec4 bQuantized, float fragmentDepth)
{

	// undo quantization of stored texture values:
	vec4 b = quantizationMatrix2Inv * (bQuantized - addition);
	// lerp outside texels back inside:
	b = (1.0 - alpha) * b + alpha * vec4(0.0, 0.63, 0, 0.63);

	float L32D22 = -b.x * b.y + b.z;				
	float D22 = -b.x * b.x + b.y;					
	float SquaredDepthVariance = -b.y * b.y + b.w;	
	float D33D22 = dot(vec2(SquaredDepthVariance,-L32D22), vec2(D22, L32D22));
	float InvD22 = 1.0 / D22;
	float L32 = L32D22 * InvD22;
	vec3 z;
	z.x = fragmentDepth;
	vec3 c = vec3(1.0, z.x, z.x*z.x);
	c.y -= b.x;
	c.z -= b.y + L32 * c.y;
	c.y *= InvD22;
	c.z *= D22 / D33D22;
	c.y -= L32*c.z;
	c.x -= dot(c.yz,b.xy);
	float InvC2 = 1.0 / c.z;
	float p = c.y * InvC2;
	float q = c.x * InvC2;
	float r = sqrt((p * p * 0.25) - q);
	z.y =- p * 0.5f - r;
	z.z =- p * 0.5f + r;
	vec4 Switch= (z.z < z.x) ? vec4(z.y, z.x, 1.0, 1.0) : ((z.y < z.x) ? vec4(z.x, z.y, 0.0, 1.0) : vec4(0.0, 0.0, 0.0, 0.0));
	float Quotient = (Switch.x * z.z - b.x * (Switch.x + z.z) + b.y) / ((z.z - Switch.y) * (z.x - z.y));
	float shadowValue = Switch.z + Switch.w * Quotient;
	return 1.0 - clamp(shadowValue / 0.02, 0.0, 1.0);
}

void main()
{
	vec3 surfaceToLight = uLightPosition - vPosition;
	float surfaceToLightDistanceSquared = dot(surfaceToLight, surfaceToLight);
	vec3 surfaceToLightNormalized = normalize(surfaceToLight);

	float dotproduct = max(dot(surfaceToLightNormalized, vNormal), 0.0);

	float sumLightPower = dotproduct * (100.0 / surfaceToLightDistanceSquared);

	vec3 textureColor = texture(uTexture, vTextureCoordinates).xyz;
	vec3 lightIntensity = vec3(sumLightPower, sumLightPower, sumLightPower);
	//color = vec4(textureColor * (lightIntensity + uAmbientLight), 1.0);

	// moment shadow mapping:
	vec3 projCoords = vFragmentPosLightSpace.xyz / vFragmentPosLightSpace.w;
	float fragmentDepth = projCoords.z;
    projCoords = projCoords * 0.5 + 0.5;
	vec4 b = texture(uTextureShadow, projCoords.xy);


	float uAmbientLightAvg = (uAmbientLight.x + uAmbientLight.y + uAmbientLight.z) / 3.0;
	color = vec4(textureColor * max(shadowCalculationMSM(b, fragmentDepth), uAmbientLightAvg), 1.0);
}