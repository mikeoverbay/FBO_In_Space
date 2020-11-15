#version 330 compatibility
//GenericBump_fragment.glsl
//Used with color and bump only

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec3 gPosition;

uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform int mask;

in vec3 v_Position;
in vec3 v_Normal;

in vec2 TC1;


vec3 getNormal()
{
    // Retrieve the tangent space matrix
    vec3 pos_dx = dFdx(v_Position);
    vec3 pos_dy = dFdy(v_Position);
    vec3 tex_dx = dFdx(vec3(TC1, 0.0));
    vec3 tex_dy = dFdy(vec3(TC1, 0.0));
    vec3 t = (tex_dy.t * pos_dx - tex_dx.t * pos_dy) / (tex_dx.s * tex_dy.t - tex_dy.s * tex_dx.t);
    vec3 ng = normalize(v_Normal);

    t = normalize(t - ng * dot(ng, t));
    vec3 b = normalize(cross(ng, t));
    mat3 tbn = mat3(t, b, ng);
    vec3 n = ng;
    n = texture2D(normalMap, TC1).rgb*2.0-1.0;
    n.x*=-1.0;
    n = normalize(tbn * n);
    return n;
}

void main(void){
    vec3 PrepNormal = getNormal(); // normal at surface point
    gPosition.rgb = v_Position.xyz;
    gColor.rgb = texture2D(colorMap,TC1).rgb;
	gColor.a = float(mask)/255.0;
    gNormal.rgb = normalize(PrepNormal.xyz) *0.5+0.5;
    gNormal.a = 1.0; //specular at some point
	}