#version 130
//GenericBump_vertex.glsl
//Used with color and bump only

uniform mat4 shadowProjection;

out vec3 v_Position;
out vec3 v_Normal;
out vec2 TC1;
void main(void){

    TC1 = gl_MultiTexCoord0.xy;
    v_Normal = gl_NormalMatrix * gl_Normal;
    v_Position = vec3(gl_ModelViewMatrix * gl_Vertex);
    //v_Position = vec3(gl_Vertex);
    gl_Position    = ftransform();

}