#version 330 compatibility
//GenericBump_fragment.glsl
//Used with color and bump only

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec3 gPosition;

uniform int mask;

in vec3 v_Position;
in vec3 v_Normal;
in vec4 color;


void main(void){
    gPosition.rgb = v_Position.xyz;
    gColor.rgb = color.rgb;
	gColor.a = float(mask)/255.0;

    gNormal.rgb = normalize(v_Normal.xyz) *0.5+0.5;
    gNormal.a = 1.0; //specular at some point
    }