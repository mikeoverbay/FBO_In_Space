#version 330 compatibility
layout (points) in;
layout (triangle_strip) out;
layout (max_vertices = 4) out;    
#extension GL_EXT_geometry_shader4 : enable
   
// GLSL Hacker automatic uniforms:

uniform float size; // Particle size
uniform float rotation;

in Vertex
{
  vec4 color;
} vertex[];


out vec2 Vertex_UV;
out vec4 Vertex_Color;
   
void main (void)
{
     mat3 Zr = mat3(cos(rotation),-sin(rotation),0.0,
                   sin(rotation), cos(rotation),0.0,
                   0.0,           0.0,          1.0);

 mat4 MV = gl_ModelViewMatrix;
  mat4 MVi = inverse(MV);
  vec4 P = MV * gl_PositionIn[0];
//p = inverse(gl_ModelViewMatrix) * p;

  mat4 VP = gl_ModelViewProjectionMatrix;
  
  vec4 vb = MVi * vec4(P.xyz + Zr * vec3(-0.5, 0.5, 0.0) * size,P.w);
  gl_Position = VP * (vb );
  Vertex_UV = vec2(1.0, 0.0);
  Vertex_Color = vertex[0].color;
  EmitVertex();  
 
  vec4 va = MVi * vec4(P.xyz + Zr * vec3(-0.5, -0.5, 0.0) * size,P.w);
  gl_Position = VP * ( va );
  Vertex_UV = vec2(1.0, 1.0);
  Vertex_Color = vertex[0].color;
  EmitVertex();  

  vec4 vc = MVi * vec4(P.xyz + Zr * vec3(0.5, 0.5, 0.0) * size,P.w);
  gl_Position = VP * (vc );
  Vertex_UV = vec2(0.0, 0.0);
  Vertex_Color = vertex[0].color;
  EmitVertex();  

   vec4 vd = MVi * vec4(P.xyz + Zr * vec3(0.5, -0.5, 0.0) * size,P.w);
  gl_Position = VP * ( vd );
  Vertex_UV = vec2(0.0, 1.0);
  Vertex_Color = vertex[0].color;
  EmitVertex();  
  
  EndPrimitive();  
}