//BillBoardBasic
#version 330 compatibility

uniform float rotation;
out vec2 texCoord;

void main(void)
{

    mat3 Zr = mat3(cos(rotation),-sin(rotation),0.0,
                   sin(rotation), cos(rotation),0.0,
                   0.0,           0.0,          1.0);


    texCoord    = gl_MultiTexCoord0.xy;


    vec4 p =  gl_ModelViewMatrix * gl_Vertex;
    p.xyz -= Zr * gl_MultiTexCoord1.xyz;
    
    p = inverse(gl_ModelViewMatrix) * p;

    gl_Position = gl_ModelViewProjectionMatrix * p;
    

}
