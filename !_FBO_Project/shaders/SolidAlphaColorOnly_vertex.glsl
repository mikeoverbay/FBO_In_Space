//SolidAlphaPColorOnly__vertex.glsl
//Used with gNormal and gPosition only

#version 330 compatibility

out vec4 color;
void main()
{
	color = gl_Color;
    gl_Position = ftransform();
}
