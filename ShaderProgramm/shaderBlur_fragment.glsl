#version 450

layout(location = 0) out vec4 color;

in vec2 vTextureCoordinates;

uniform sampler2D uTextureInput;
uniform int uAxis;



vec4 blur13(vec2 resolution, vec2 direction) {
  vec4 color = vec4(0.0);
  vec2 off1 = vec2(1.411764705882353) * direction;
  vec2 off2 = vec2(3.2941176470588234) * direction;
  vec2 off3 = vec2(5.176470588235294) * direction;
  color += texture2D(uTextureInput, vTextureCoordinates) * 0.1964825501511404;
  color += texture2D(uTextureInput, vTextureCoordinates + (off1 / resolution)) * 0.2969069646728344;
  color += texture2D(uTextureInput, vTextureCoordinates - (off1 / resolution)) * 0.2969069646728344;
  color += texture2D(uTextureInput, vTextureCoordinates + (off2 / resolution)) * 0.09447039785044732;
  color += texture2D(uTextureInput, vTextureCoordinates - (off2 / resolution)) * 0.09447039785044732;
  color += texture2D(uTextureInput, vTextureCoordinates + (off3 / resolution)) * 0.010381362401148057;
  color += texture2D(uTextureInput, vTextureCoordinates - (off3 / resolution)) * 0.010381362401148057;
  return color;
}

void main()
{
    ivec2 txDims = textureSize(uTextureInput, 0);
    vec2 txDimsF = vec2(txDims.x, txDims.y);
    color = blur13(txDimsF, uAxis == 0 ? vec2(1.0, 0.0) : vec2(0.0, 1.0));
}