#version 450

layout(location = 0) out vec4 color;

const float s3 = sqrt(3.0) / 2.0;
const float s12 = sqrt(12.0) / -9.0;
const mat4 quantizationMatrix = mat4(
										1.5, 0.0, -2.0,  0.0, 
										0.0, 4.0,  0.0, -4.0,
										s3,  0.0,  s12,  0.0,
										0.0, 0.5,  0.0,  0.5
										);

const mat4 quantizationMatrix2 = mat4(
										 1.5,  0.0,   s3,  0.0, 
										 0.0,  4.0,  0.0,  0.5,
										-2.0,  0.0,  s12,  0.0,
										 0.0, -4.0,  0.0,  0.5
										);

const vec4 addition = vec4(0.5, 0.0, 0.5, 0.0);

in vec2 vTextureCoordinates;

uniform sampler2D uTextureDepthMS;

void main()
{
	float z = texture(uTextureDepthMS, vTextureCoordinates).r * 2.0 - 1.0;
	vec4 b_unbiased = vec4(z, pow(z, 2), pow(z, 3), pow(z, 4));

	color = quantizationMatrix2 * b_unbiased + addition;
	//color = b_unbiased;
}