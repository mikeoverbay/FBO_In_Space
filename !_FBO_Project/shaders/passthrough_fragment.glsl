#version 130
//passthrough_fragment.glsl
//Used to copy textures
uniform sampler2D colorMap;
in vec2 TC1;
void main(void){

    vec4 color = texture2D(colorMap, TC1);
	gl_FragColor = color;

}