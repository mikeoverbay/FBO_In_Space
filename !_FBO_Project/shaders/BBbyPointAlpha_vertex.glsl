#version 330 compatibility
// GLSL Hacker automatic uniforms:

out Vertex
{
    vec4 color;
}
 vertex;
void main()
{
    vertex.color = gl_Color;
    gl_Position = gl_Vertex;
}
