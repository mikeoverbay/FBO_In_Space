// solid alpha textured only shader

#version 330 compatibility
out vec2 TexCoord;

out vec3 v_Normal;
void main()
{
    v_Normal = gl_NormalMatrix * gl_Normal;
    gl_Position = ftransform();
    TexCoord.xy = gl_MultiTexCoord0.xy;
}
