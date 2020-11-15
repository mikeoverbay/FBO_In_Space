//BillBoardBasic
#version 330 compatibility

uniform mat4 matrix;

varying vec2 texCoord;
out vec3 position;
out vec3 vNormal;
void main(void)
{

    vec3 n = vec3 (0.0,0.0,-1.0);
    vNormal= gl_NormalMatrix * n;
    texCoord    = gl_MultiTexCoord0.xy;

    vec4 p =  gl_ModelViewMatrix * gl_Vertex;
    p.xyz -= gl_MultiTexCoord1.xyz;
    position = p.xyz;
    p = inverse(gl_ModelViewMatrix) * p;
    gl_Position = gl_ModelViewProjectionMatrix * p;
    

}
