#version 130
//passthrough_vertex.glsl
//Used to copy textures

out vec2 TC1;
void main(void){

    TC1 = gl_MultiTexCoord0.xy;
    gl_Position    = ftransform();

}