#version 130
//phong_vertex.glsl
//Light Only Shader

uniform mat4 shadowProjection;

out vec3 v_Position;
out vec3 v_Normal;
out vec4 color;
void main(void){
    
    color = gl_Color;
    v_Normal = gl_NormalMatrix * gl_Normal;
    v_Position = vec3(gl_ModelViewMatrix * gl_Vertex);
    //v_Position = vec3(gl_Vertex);
    gl_Position    = ftransform();

}