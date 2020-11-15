//BillBoardBasic
#version 330 compatibility

uniform sampler2D colorMap;
uniform vec2 TextureSize;
in vec2 texCoord;

out vec4 gColor;

void main(void){
    vec2 offset = vec2(0.5/TextureSize.x, -0.5/ TextureSize.y);
    vec4 color = texture2D(colorMap, texCoord - offset);
    gColor = color;

}
