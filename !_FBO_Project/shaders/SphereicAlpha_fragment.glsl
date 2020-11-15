// Sphereic alpha textured shader
#version 330 compatibility

uniform sampler2D colorMap;
uniform vec3 ViewPosition;
uniform mat4 ModelViewMatrix;

in vec2 TexCoord;
in vec3 v_Normal;

out vec4 gColor;

void main(void){
    vec3 norm = normalize(v_Normal);
    vec3  LColor[3];
    vec3  LSpecular[3];
    float c = 0.6;
    LColor[0] = vec3(c, c, c);
    LColor[1] = vec3(c, c, c);
    LColor[2] = vec3(c, c, c);

    LSpecular[0] = vec3(1.0,0.0,0.0);
    LSpecular[1] = vec3(0.0,1.0,0.0);
    LSpecular[2] = vec3(0.0,0.0,1.0);

	vec3 FragPos = vec3(0.0);

    vec4 Albedo = texture2D(colorMap, TexCoord);
    float spec_level = 1.0;
    vec3 sum;
        vec3 camPosition = (mat3(ModelViewMatrix)*ViewPosition);//Move cam to ModelView Space
    /*======================================================================================*/
    // Do  lights
    for (int i = 0; i<3; i++){
       vec3 LightPos = gl_LightSource[i].position.xyz;
       vec3 lightDir = normalize(LightPos - FragPos);
       vec3 diffuse = max(dot(norm, lightDir), 0.0) * Albedo.rgb;
       diffuse *= LColor[i];
       vec3 lighting = Albedo.rgb * 0.23;

       vec3 viewDir = normalize(camPosition - FragPos);
       vec3 halfwayDir = normalize(lightDir + viewDir);
       float spec_power = 120.0;

       float spec = pow(max(dot(norm, halfwayDir), 0.0), spec_power) * spec_level * 0.5315;
       vec3 specular =  vec3(spec) ;
        sum += specular;
       float d = length(camPosition - FragPos);
       if (d < 4000.0){
          d = 1.0 - d / 4000.0;
       }else{
          d=0.0;
       }
       Albedo.a+= clamp(spec * d,0.0,1.0);
       sum.rgb += (lighting + diffuse + specular) * d;
       //sum.rgb +=texture2D(gPosition, TexCoords).rgb;
    }

        gl_FragColor.rgb = sum.rgb+vec3(0.0,0.3,0.0);
        gl_FragColor.a = Albedo.a*2.0;

}
