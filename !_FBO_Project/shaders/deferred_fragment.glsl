// Deferred lighitng shader
#version 330 compatibility

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gColor;
uniform sampler2D AlphaColor;
//light uniforms
const int max_space_lights = 256;
const int max_instrument_lights = 32;
const int max_shiphull_lights = 32;
uniform vec4 spaceL_color[max_space_lights];
// color xyz and w as specular level
uniform vec4 spaceL_position[max_space_lights];
//  pos rgb and w as range
uniform int spaceL_count;
// count

uniform mat4 ModelViewMatrix;
in mat3 mvpm;
//uniform vec4 instL_color[max_instrument_lights];
//uniform vec4 instL_Position[max_instrument_lights];
//uniform int instL_Count;
//uniform int render_instL;

//uniform vec4 shipL_color[max_shiphull_lights];
//uniform vec4 shipL_Position[max_shiphull_lights];
//uniform int shipL_count;
//uniform int render_shipL;


in vec2 TexCoords;
vec4 correct(in vec4 hdrColor, in float exposure,in float gamma){
    // Exposure tone mapping
    vec3 mapped = vec3(1.0) - exp(-hdrColor.rgb * exposure);
    // Gamma correction 
    mapped.rgb = pow(mapped.rgb, vec3(1.0 / gamma));
    return vec4 (mapped, 1.0);
}


void main(void)
{
    vec4 sum;
    float spec_power = 50.0;

    vec3 Albedo      = texture2D(gColor, TexCoords).rgb;
    vec4 alphaMap    = texture2D(AlphaColor, TexCoords).rgba;
    int mask         = int(texture2D(gColor, TexCoords).a * 255);
    vec3 Normal      = normalize(texture2D(gNormal, TexCoords).rgb * 2.0-1.0);
    float spec_level = texture2D(gNormal, TexCoords).a;
    vec3 FragPos     = texture2D(gPosition, TexCoords).rgb;
     /*======================================================================================*/
     //Move cam to screen Space

    vec3 vd = normalize( - FragPos);
    vec3 lighting = Albedo.rgb * 0.23/float(spaceL_count);
    sum.rgb =lighting;
    if (length(FragPos) > 0.0){

    for (int i = 0; i < spaceL_count; i++){
        vec3 LightPos = vec3(ModelViewMatrix * vec4(spaceL_position[i].xyz,1.0));
        vec3 lightDir = normalize(LightPos - FragPos);
        //=========================================================
        // diffuse calculation
        vec3 diffuse = pow(max(dot(Normal, lightDir), 0.0), 1.0) * Albedo.rgb * spaceL_color[i].xyz;

        float dist = length(LightPos - FragPos);
        float cutoff = spaceL_position[i].w;
        //only light whats in range
        if (dist < cutoff) {
           //=========================================================
           // specular calculations
           vec3 ld = normalize(spaceL_position[i].xyz - FragPos);

           vec3 halfwayDir = normalize(lightDir + vd);

           float spec = pow(max(dot(Normal, halfwayDir), 0.0), spec_power) * spec_level * spaceL_color[i].w;
           vec3 specular = vec3(spec) * spaceL_color[i].xyz;
           //=========================================================
           sum.rgb = mix((diffuse + specular),lighting,  dist/cutoff);
           //sum.rgb +=  vec3(0.1, 0.0, 0.0);
           //sum.rgb +=texture2D(gPosition, TexCoords).rgb;
        }else{// cutoff
        sum.rgb =lighting;
        }// cutoff
        if (mask == 1){
                gl_FragColor.rgb += sum.rgb;
                gl_FragColor.a = 1.0;
            }else{
                gl_FragColor.rgb =  lighting;
                gl_FragColor.a = 1.0;
            }
    }//light loop
    }else{ // FragPos = 0
            // there is no posiion so this was rendered only to the gColor buffer
            gl_FragColor.rgb = Albedo;
            gl_FragColor.a = 1.0;
    } // FragPos > 00
    //mix in the alpha blend rendered objects created in the alpha_FBO
    gl_FragColor.rgb = mix(gl_FragColor.rgb ,alphaMap.rgb, alphaMap.a);
	// Color correction
    // color, expsure, gamma
    gl_FragColor = correct(gl_FragColor, 2.0, 0.65);
} // main
