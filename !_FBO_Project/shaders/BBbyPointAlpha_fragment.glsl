#version 330 compatibility

uniform sampler2D colorMap;
in vec2 Vertex_UV;
in vec4 Vertex_Color;
out vec4 FragColor;
void main (void)
{
  vec2 uv = Vertex_UV.xy;
  vec4 t = texture(colorMap,uv) * Vertex_Color;
  FragColor = vec4(t.rgb *t.a, t.a);
}