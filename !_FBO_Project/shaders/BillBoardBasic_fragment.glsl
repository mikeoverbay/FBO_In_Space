//BillBoardBasic
#version 330 compatibility

uniform sampler2D colorMap;
uniform float alpharef;
uniform int mask;
uniform vec2 TextureSize;
varying vec2 texCoord;

in vec3 position;
in vec3 vNormal;

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec3 gPosition;


void main(void){
    vec2 offset = vec2(0.5/TextureSize.x, -0.5/ TextureSize.y);
    vec4 color = texture2D(colorMap, texCoord - offset);
    if (color.a < alpharef) discard;
    gColor.rgb = color.rgb;
    gColor.a = float(mask)/255.0;

    gPosition = position;
    gNormal.rgb = normalize(vNormal.rgb)* 0.5 + 0.5;
    gNormal.a = 1.0;
}
