Shader "Hedgehog Team/Glass" {
Properties {
	_Color ("Main Color (RGB)", Color) = (1,1,1,1)
	_ReflectColor ("Reflection Color (RGB)", Color) = (1,1,1,0.5)
	_Cube ("Reflection Cubemap (CUBE)", Cube) = "_Skybox" { TexGen CubeReflect }
	_FresnelPower ("Fresnel Power", Range(0.05,5.0)) = 0.75
}
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	Cull Front	
	
	Alphatest Greater 0 ZWrite Off ColorMask RGB
	
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }
		Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
// Vertex combos: 4
//   opengl - ALU: 25 to 71
//   d3d9 - ALU: 25 to 71
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 15 [unity_SHAr]
Vector 16 [unity_SHAg]
Vector 17 [unity_SHAb]
Vector 18 [unity_SHBr]
Vector 19 [unity_SHBg]
Vector 20 [unity_SHBb]
Vector 21 [unity_SHC]
"!!ARBvp1.0
# 42 ALU
PARAM c[22] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..21] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
MUL R1.xyz, vertex.normal, c[13].w;
DP3 R3.w, R1, c[6];
DP3 R2.w, R1, c[7];
DP3 R0.w, R1, c[5];
MOV R0.x, R3.w;
MOV R0.y, R2.w;
MOV R0.z, c[0].x;
MUL R1, R0.wxyy, R0.xyyw;
DP4 R2.z, R0.wxyz, c[17];
DP4 R2.y, R0.wxyz, c[16];
DP4 R2.x, R0.wxyz, c[15];
DP4 R0.z, R1, c[20];
DP4 R0.x, R1, c[18];
DP4 R0.y, R1, c[19];
ADD R2.xyz, R2, R0;
MOV R1.w, c[0].x;
MOV R1.xyz, c[14];
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R1.xyz, R0, c[13].w, -vertex.position;
MUL R0.y, R3.w, R3.w;
MAD R1.w, R0, R0, -R0.y;
DP3 R0.x, vertex.normal, -R1;
MUL R0.xyz, vertex.normal, R0.x;
MAD R0.xyz, -R0, c[0].y, -R1;
MUL R3.xyz, R1.w, c[21];
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
ADD result.texcoord[3].xyz, R2, R3;
ADD result.texcoord[1].xyz, -R0, c[14];
MOV result.texcoord[2].z, R2.w;
MOV result.texcoord[2].y, R3.w;
MOV result.texcoord[2].x, R0.w;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 42 instructions, 4 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_SHAr]
Vector 15 [unity_SHAg]
Vector 16 [unity_SHAb]
Vector 17 [unity_SHBr]
Vector 18 [unity_SHBg]
Vector 19 [unity_SHBb]
Vector 20 [unity_SHC]
"vs_2_0
; 42 ALU
def c21, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mul r1.xyz, v1, c12.w
dp3 r3.w, r1, c5
dp3 r2.w, r1, c6
dp3 r0.w, r1, c4
mov r0.x, r3.w
mov r0.y, r2.w
mov r0.z, c21.x
mul r1, r0.wxyy, r0.xyyw
dp4 r2.z, r0.wxyz, c16
dp4 r2.y, r0.wxyz, c15
dp4 r2.x, r0.wxyz, c14
dp4 r0.z, r1, c19
dp4 r0.x, r1, c17
dp4 r0.y, r1, c18
add r2.xyz, r2, r0
mov r1.w, c21.x
mov r1.xyz, c13
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r1.xyz, r0, c12.w, -v0
mul r0.y, r3.w, r3.w
mad r1.w, r0, r0, -r0.y
dp3 r0.x, v1, -r1
mul r0.xyz, v1, r0.x
mad r0.xyz, -r0, c21.y, -r1
mul r3.xyz, r1.w, c20
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
add oT3.xyz, r2, r3
add oT1.xyz, -r0, c13
mov oT2.z, r2.w
mov oT2.y, r3.w
mov oT2.x, r0.w
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  shlight = tmpvar_11;
  tmpvar_4 = shlight;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_3 * xlv_TEXCOORD3));
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  shlight = tmpvar_11;
  tmpvar_4 = shlight;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_3 * xlv_TEXCOORD3));
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_SHAr]
Vector 15 [unity_SHAg]
Vector 16 [unity_SHAb]
Vector 17 [unity_SHBr]
Vector 18 [unity_SHBg]
Vector 19 [unity_SHBb]
Vector 20 [unity_SHC]
"agal_vs
c21 1.0 2.0 0.0 0.0
[bc]
adaaaaaaabaaahacabaaaaoeaaaaaaaaamaaaappabaaaaaa mul r1.xyz, a1, c12.w
bcaaaaaaadaaaiacabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 r3.w, r1.xyzz, c5
bcaaaaaaacaaaiacabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 r2.w, r1.xyzz, c6
bcaaaaaaaaaaaiacabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 r0.w, r1.xyzz, c4
aaaaaaaaaaaaabacadaaaappacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r3.w
aaaaaaaaaaaaacacacaaaappacaaaaaaaaaaaaaaaaaaaaaa mov r0.y, r2.w
aaaaaaaaaaaaaeacbfaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r0.z, c21.x
adaaaaaaabaaapacaaaaaafdacaaaaaaaaaaaaneacaaaaaa mul r1, r0.wxyy, r0.xyyw
bdaaaaaaacaaaeacaaaaaajdacaaaaaabaaaaaoeabaaaaaa dp4 r2.z, r0.wxyz, c16
bdaaaaaaacaaacacaaaaaajdacaaaaaaapaaaaoeabaaaaaa dp4 r2.y, r0.wxyz, c15
bdaaaaaaacaaabacaaaaaajdacaaaaaaaoaaaaoeabaaaaaa dp4 r2.x, r0.wxyz, c14
bdaaaaaaaaaaaeacabaaaaoeacaaaaaabdaaaaoeabaaaaaa dp4 r0.z, r1, c19
bdaaaaaaaaaaabacabaaaaoeacaaaaaabbaaaaoeabaaaaaa dp4 r0.x, r1, c17
bdaaaaaaaaaaacacabaaaaoeacaaaaaabcaaaaoeabaaaaaa dp4 r0.y, r1, c18
abaaaaaaacaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa add r2.xyz, r2.xyzz, r0.xyzz
aaaaaaaaabaaaiacbfaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c21.x
aaaaaaaaabaaahacanaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c13
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaaeaaahacaaaaaakeacaaaaaaamaaaappabaaaaaa mul r4.xyz, r0.xyzz, c12.w
acaaaaaaabaaahacaeaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r1.xyz, r4.xyzz, a0
adaaaaaaaaaaacacadaaaappacaaaaaaadaaaappacaaaaaa mul r0.y, r3.w, r3.w
adaaaaaaaeaaaiacaaaaaappacaaaaaaaaaaaappacaaaaaa mul r4.w, r0.w, r0.w
acaaaaaaabaaaiacaeaaaappacaaaaaaaaaaaaffacaaaaaa sub r1.w, r4.w, r0.y
bfaaaaaaaeaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, r1.xyzz
bcaaaaaaaaaaabacabaaaaoeaaaaaaaaaeaaaakeacaaaaaa dp3 r0.x, a1, r4.xyzz
adaaaaaaaaaaahacabaaaaoeaaaaaaaaaaaaaaaaacaaaaaa mul r0.xyz, a1, r0.x
bfaaaaaaaeaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, r0.xyzz
adaaaaaaaeaaahacaeaaaakeacaaaaaabfaaaaffabaaaaaa mul r4.xyz, r4.xyzz, c21.y
acaaaaaaaaaaahacaeaaaakeacaaaaaaabaaaakeacaaaaaa sub r0.xyz, r4.xyzz, r1.xyzz
adaaaaaaadaaahacabaaaappacaaaaaabeaaaaoeabaaaaaa mul r3.xyz, r1.w, c20
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
abaaaaaaadaaahaeacaaaakeacaaaaaaadaaaakeacaaaaaa add v3.xyz, r2.xyzz, r3.xyzz
bfaaaaaaaeaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, r0.xyzz
abaaaaaaabaaahaeaeaaaakeacaaaaaaanaaaaoeabaaaaaa add v1.xyz, r4.xyzz, c13
aaaaaaaaacaaaeaeacaaaappacaaaaaaaaaaaaaaaaaaaaaa mov v2.z, r2.w
aaaaaaaaacaaacaeadaaaappacaaaaaaaaaaaaaaaaaaaaaa mov v2.y, r3.w
aaaaaaaaacaaabaeaaaaaappacaaaaaaaaaaaaaaaaaaaaaa mov v2.x, r0.w
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 15 [unity_LightmapST]
"!!ARBvp1.0
# 25 ALU
PARAM c[16] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..15] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[14];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[13].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[13].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
ADD result.texcoord[1].xyz, -R0, c[14];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MAD result.texcoord[3].xy, vertex.texcoord[1], c[15], c[15].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 25 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_LightmapST]
"vs_2_0
; 25 ALU
def c15, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
mov r1.xyz, c13
mov r1.w, c15.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c12.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c15.y, -r0
mul r1.xyz, v1, c12.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
add oT1.xyz, -r0, c13
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mad oT3.xy, v2, c14, c14.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord1;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_4).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_6;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_8;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _ReflectColor;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_3 * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD3).xyz));
  c.w = refl2Refr;
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord1;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_4).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_6;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_8;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _ReflectColor;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (unity_Lightmap, xlv_TEXCOORD3);
  c.xyz = (tmpvar_3 * ((8.0 * tmpvar_9.w) * tmpvar_9.xyz));
  c.w = refl2Refr;
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_LightmapST]
"agal_vs
c15 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacanaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c13
aaaaaaaaabaaaiacapaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c15.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaaamaaaappabaaaaaa mul r2.xyz, r0.xyzz, c12.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaaapaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c15.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaamaaaappabaaaaaa mul r1.xyz, a1, c12.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaaanaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c13
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
adaaaaaaacaaadacaeaaaaoeaaaaaaaaaoaaaaoeabaaaaaa mul r2.xy, a4, c14
abaaaaaaadaaadaeacaaaafeacaaaaaaaoaaaaooabaaaaaa add v3.xy, r2.xyyy, c14.zwzw
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.zw, c0
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 15 [unity_LightmapST]
"!!ARBvp1.0
# 25 ALU
PARAM c[16] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..15] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[14];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[13].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[13].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
ADD result.texcoord[1].xyz, -R0, c[14];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MAD result.texcoord[3].xy, vertex.texcoord[1], c[15], c[15].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 25 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_LightmapST]
"vs_2_0
; 25 ALU
def c15, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
mov r1.xyz, c13
mov r1.w, c15.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c12.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c15.y, -r0
mul r1.xyz, v1, c12.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
add oT1.xyz, -r0, c13
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mad oT3.xy, v2, c14, c14.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord1;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_4).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_6;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_8;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _ReflectColor;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  mediump vec3 lm_i0;
  lowp vec3 tmpvar_9;
  tmpvar_9 = (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD3).xyz);
  lm_i0 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_3 * lm_i0);
  c.xyz = tmpvar_10;
  c.w = refl2Refr;
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord1;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_4).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_6;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_8;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _ReflectColor;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (unity_Lightmap, xlv_TEXCOORD3);
  mediump vec3 lm_i0;
  lowp vec3 tmpvar_10;
  tmpvar_10 = ((8.0 * tmpvar_9.w) * tmpvar_9.xyz);
  lm_i0 = tmpvar_10;
  mediump vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_3 * lm_i0);
  c.xyz = tmpvar_11;
  c.w = refl2Refr;
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_LightmapST]
"agal_vs
c15 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacanaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c13
aaaaaaaaabaaaiacapaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c15.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaaamaaaappabaaaaaa mul r2.xyz, r0.xyzz, c12.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaaapaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c15.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaamaaaappabaaaaaa mul r1.xyz, a1, c12.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaaanaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c13
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
adaaaaaaacaaadacaeaaaaoeaaaaaaaaaoaaaaoeabaaaaaa mul r2.xy, a4, c14
abaaaaaaadaaadaeacaaaafeacaaaaaaaoaaaaooabaaaaaa add v3.xy, r2.xyyy, c14.zwzw
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.zw, c0
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 15 [unity_4LightPosX0]
Vector 16 [unity_4LightPosY0]
Vector 17 [unity_4LightPosZ0]
Vector 18 [unity_4LightAtten0]
Vector 19 [unity_LightColor0]
Vector 20 [unity_LightColor1]
Vector 21 [unity_LightColor2]
Vector 22 [unity_LightColor3]
Vector 23 [unity_SHAr]
Vector 24 [unity_SHAg]
Vector 25 [unity_SHAb]
Vector 26 [unity_SHBr]
Vector 27 [unity_SHBg]
Vector 28 [unity_SHBb]
Vector 29 [unity_SHC]
"!!ARBvp1.0
# 71 ALU
PARAM c[30] = { { 1, 2, 0 },
		state.matrix.mvp,
		program.local[5..29] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
MUL R3.xyz, vertex.normal, c[13].w;
DP3 R5.x, R3, c[5];
DP4 R4.zw, vertex.position, c[6];
ADD R2, -R4.z, c[16];
DP3 R4.z, R3, c[6];
DP4 R3.w, vertex.position, c[5];
MUL R0, R4.z, R2;
ADD R1, -R3.w, c[15];
MUL R2, R2, R2;
MOV R5.y, R4.z;
MOV R5.w, c[0].x;
DP4 R4.xy, vertex.position, c[7];
MAD R0, R5.x, R1, R0;
MAD R2, R1, R1, R2;
ADD R1, -R4.x, c[17];
DP3 R4.x, R3, c[7];
MAD R2, R1, R1, R2;
MAD R0, R4.x, R1, R0;
MUL R1, R2, c[18];
ADD R1, R1, c[0].x;
MOV R5.z, R4.x;
RSQ R2.x, R2.x;
RSQ R2.y, R2.y;
RSQ R2.z, R2.z;
RSQ R2.w, R2.w;
MUL R0, R0, R2;
DP4 R2.z, R5, c[25];
DP4 R2.y, R5, c[24];
DP4 R2.x, R5, c[23];
RCP R1.x, R1.x;
RCP R1.y, R1.y;
RCP R1.w, R1.w;
RCP R1.z, R1.z;
MAX R0, R0, c[0].z;
MUL R0, R0, R1;
MUL R1.xyz, R0.y, c[20];
MAD R1.xyz, R0.x, c[19], R1;
MAD R0.xyz, R0.z, c[21], R1;
MUL R1, R5.xyzz, R5.yzzx;
MAD R0.xyz, R0.w, c[22], R0;
DP4 R3.z, R1, c[28];
DP4 R3.x, R1, c[26];
DP4 R3.y, R1, c[27];
ADD R3.xyz, R2, R3;
MOV R1.w, c[0].x;
MOV R1.xyz, c[14];
DP4 R2.z, R1, c[11];
DP4 R2.y, R1, c[10];
DP4 R2.x, R1, c[9];
MUL R0.w, R4.z, R4.z;
MAD R1.w, R5.x, R5.x, -R0;
MAD R1.xyz, R2, c[13].w, -vertex.position;
DP3 R0.w, vertex.normal, -R1;
MUL R5.yzw, R1.w, c[29].xxyz;
ADD R3.xyz, R3, R5.yzww;
ADD result.texcoord[3].xyz, R3, R0;
MUL R2.xyz, vertex.normal, R0.w;
MAD R1.xyz, -R2, c[0].y, -R1;
MOV R3.x, R4.w;
MOV R3.y, R4;
DP3 result.texcoord[0].z, R1, c[7];
DP3 result.texcoord[0].y, R1, c[6];
DP3 result.texcoord[0].x, R1, c[5];
ADD result.texcoord[1].xyz, -R3.wxyw, c[14];
MOV result.texcoord[2].z, R4.x;
MOV result.texcoord[2].y, R4.z;
MOV result.texcoord[2].x, R5;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 71 instructions, 6 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_4LightPosX0]
Vector 15 [unity_4LightPosY0]
Vector 16 [unity_4LightPosZ0]
Vector 17 [unity_4LightAtten0]
Vector 18 [unity_LightColor0]
Vector 19 [unity_LightColor1]
Vector 20 [unity_LightColor2]
Vector 21 [unity_LightColor3]
Vector 22 [unity_SHAr]
Vector 23 [unity_SHAg]
Vector 24 [unity_SHAb]
Vector 25 [unity_SHBr]
Vector 26 [unity_SHBg]
Vector 27 [unity_SHBb]
Vector 28 [unity_SHC]
"vs_2_0
; 71 ALU
def c29, 1.00000000, 2.00000000, 0.00000000, 0
dcl_position0 v0
dcl_normal0 v1
mul r3.xyz, v1, c12.w
dp3 r5.x, r3, c4
dp4 r4.zw, v0, c5
add r2, -r4.z, c15
dp3 r4.z, r3, c5
dp4 r3.w, v0, c4
mul r0, r4.z, r2
add r1, -r3.w, c14
mul r2, r2, r2
mov r5.y, r4.z
mov r5.w, c29.x
dp4 r4.xy, v0, c6
mad r0, r5.x, r1, r0
mad r2, r1, r1, r2
add r1, -r4.x, c16
dp3 r4.x, r3, c6
mad r2, r1, r1, r2
mad r0, r4.x, r1, r0
mul r1, r2, c17
add r1, r1, c29.x
mov r5.z, r4.x
rsq r2.x, r2.x
rsq r2.y, r2.y
rsq r2.z, r2.z
rsq r2.w, r2.w
mul r0, r0, r2
dp4 r2.z, r5, c24
dp4 r2.y, r5, c23
dp4 r2.x, r5, c22
rcp r1.x, r1.x
rcp r1.y, r1.y
rcp r1.w, r1.w
rcp r1.z, r1.z
max r0, r0, c29.z
mul r0, r0, r1
mul r1.xyz, r0.y, c19
mad r1.xyz, r0.x, c18, r1
mad r0.xyz, r0.z, c20, r1
mul r1, r5.xyzz, r5.yzzx
mad r0.xyz, r0.w, c21, r0
dp4 r3.z, r1, c27
dp4 r3.x, r1, c25
dp4 r3.y, r1, c26
add r3.xyz, r2, r3
mov r1.w, c29.x
mov r1.xyz, c13
dp4 r2.z, r1, c10
dp4 r2.y, r1, c9
dp4 r2.x, r1, c8
mul r0.w, r4.z, r4.z
mad r1.w, r5.x, r5.x, -r0
mad r1.xyz, r2, c12.w, -v0
dp3 r0.w, v1, -r1
mul r5.yzw, r1.w, c28.xxyz
add r3.xyz, r3, r5.yzww
add oT3.xyz, r3, r0
mul r2.xyz, v1, r0.w
mad r1.xyz, -r2, c29.y, -r1
mov r3.x, r4.w
mov r3.y, r4
dp3 oT0.z, r1, c6
dp3 oT0.y, r1, c5
dp3 oT0.x, r1, c4
add oT1.xyz, -r3.wxyw, c13
mov oT2.z, r4.x
mov oT2.y, r4.z
mov oT2.x, r5
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_LightColor[4];
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightAtten0;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  shlight = tmpvar_11;
  tmpvar_4 = shlight;
  highp vec3 tmpvar_21;
  tmpvar_21 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_22;
  tmpvar_22 = (unity_4LightPosX0 - tmpvar_21.x);
  highp vec4 tmpvar_23;
  tmpvar_23 = (unity_4LightPosY0 - tmpvar_21.y);
  highp vec4 tmpvar_24;
  tmpvar_24 = (unity_4LightPosZ0 - tmpvar_21.z);
  highp vec4 tmpvar_25;
  tmpvar_25 = (((tmpvar_22 * tmpvar_22) + (tmpvar_23 * tmpvar_23)) + (tmpvar_24 * tmpvar_24));
  highp vec4 tmpvar_26;
  tmpvar_26 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_22 * tmpvar_9.x) + (tmpvar_23 * tmpvar_9.y)) + (tmpvar_24 * tmpvar_9.z)) * inversesqrt (tmpvar_25))) * (1.0/((1.0 + (tmpvar_25 * unity_4LightAtten0)))));
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_4 + ((((unity_LightColor[0].xyz * tmpvar_26.x) + (unity_LightColor[1].xyz * tmpvar_26.y)) + (unity_LightColor[2].xyz * tmpvar_26.z)) + (unity_LightColor[3].xyz * tmpvar_26.w)));
  tmpvar_4 = tmpvar_27;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_3 * xlv_TEXCOORD3));
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_LightColor[4];
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightAtten0;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  shlight = tmpvar_11;
  tmpvar_4 = shlight;
  highp vec3 tmpvar_21;
  tmpvar_21 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_22;
  tmpvar_22 = (unity_4LightPosX0 - tmpvar_21.x);
  highp vec4 tmpvar_23;
  tmpvar_23 = (unity_4LightPosY0 - tmpvar_21.y);
  highp vec4 tmpvar_24;
  tmpvar_24 = (unity_4LightPosZ0 - tmpvar_21.z);
  highp vec4 tmpvar_25;
  tmpvar_25 = (((tmpvar_22 * tmpvar_22) + (tmpvar_23 * tmpvar_23)) + (tmpvar_24 * tmpvar_24));
  highp vec4 tmpvar_26;
  tmpvar_26 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_22 * tmpvar_9.x) + (tmpvar_23 * tmpvar_9.y)) + (tmpvar_24 * tmpvar_9.z)) * inversesqrt (tmpvar_25))) * (1.0/((1.0 + (tmpvar_25 * unity_4LightAtten0)))));
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_4 + ((((unity_LightColor[0].xyz * tmpvar_26.x) + (unity_LightColor[1].xyz * tmpvar_26.y)) + (unity_LightColor[2].xyz * tmpvar_26.z)) + (unity_LightColor[3].xyz * tmpvar_26.w)));
  tmpvar_4 = tmpvar_27;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_3 * xlv_TEXCOORD3));
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_4LightPosX0]
Vector 15 [unity_4LightPosY0]
Vector 16 [unity_4LightPosZ0]
Vector 17 [unity_4LightAtten0]
Vector 18 [unity_LightColor0]
Vector 19 [unity_LightColor1]
Vector 20 [unity_LightColor2]
Vector 21 [unity_LightColor3]
Vector 22 [unity_SHAr]
Vector 23 [unity_SHAg]
Vector 24 [unity_SHAb]
Vector 25 [unity_SHBr]
Vector 26 [unity_SHBg]
Vector 27 [unity_SHBb]
Vector 28 [unity_SHC]
"agal_vs
c29 1.0 2.0 0.0 0.0
[bc]
adaaaaaaadaaahacabaaaaoeaaaaaaaaamaaaappabaaaaaa mul r3.xyz, a1, c12.w
bcaaaaaaafaaabacadaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 r5.x, r3.xyzz, c4
bdaaaaaaaeaaamacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r4.zw, a0, c5
bfaaaaaaacaaaeacaeaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r2.z, r4.z
abaaaaaaacaaapacacaaaakkacaaaaaaapaaaaoeabaaaaaa add r2, r2.z, c15
bcaaaaaaaeaaaeacadaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 r4.z, r3.xyzz, c5
bdaaaaaaadaaaiacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r3.w, a0, c4
adaaaaaaaaaaapacaeaaaakkacaaaaaaacaaaaoeacaaaaaa mul r0, r4.z, r2
bfaaaaaaabaaaiacadaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r1.w, r3.w
abaaaaaaabaaapacabaaaappacaaaaaaaoaaaaoeabaaaaaa add r1, r1.w, c14
adaaaaaaacaaapacacaaaaoeacaaaaaaacaaaaoeacaaaaaa mul r2, r2, r2
aaaaaaaaafaaacacaeaaaakkacaaaaaaaaaaaaaaaaaaaaaa mov r5.y, r4.z
aaaaaaaaafaaaiacbnaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r5.w, c29.x
bdaaaaaaaeaaadacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r4.xy, a0, c6
adaaaaaaagaaapacafaaaaaaacaaaaaaabaaaaoeacaaaaaa mul r6, r5.x, r1
abaaaaaaaaaaapacagaaaaoeacaaaaaaaaaaaaoeacaaaaaa add r0, r6, r0
adaaaaaaagaaapacabaaaaoeacaaaaaaabaaaaoeacaaaaaa mul r6, r1, r1
abaaaaaaacaaapacagaaaaoeacaaaaaaacaaaaoeacaaaaaa add r2, r6, r2
bfaaaaaaabaaabacaeaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r1.x, r4.x
abaaaaaaabaaapacabaaaaaaacaaaaaabaaaaaoeabaaaaaa add r1, r1.x, c16
bcaaaaaaaeaaabacadaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 r4.x, r3.xyzz, c6
adaaaaaaagaaapacabaaaaoeacaaaaaaabaaaaoeacaaaaaa mul r6, r1, r1
abaaaaaaacaaapacagaaaaoeacaaaaaaacaaaaoeacaaaaaa add r2, r6, r2
adaaaaaaagaaapacaeaaaaaaacaaaaaaabaaaaoeacaaaaaa mul r6, r4.x, r1
abaaaaaaaaaaapacagaaaaoeacaaaaaaaaaaaaoeacaaaaaa add r0, r6, r0
adaaaaaaabaaapacacaaaaoeacaaaaaabbaaaaoeabaaaaaa mul r1, r2, c17
abaaaaaaabaaapacabaaaaoeacaaaaaabnaaaaaaabaaaaaa add r1, r1, c29.x
aaaaaaaaafaaaeacaeaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r5.z, r4.x
akaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r2.x
akaaaaaaacaaacacacaaaaffacaaaaaaaaaaaaaaaaaaaaaa rsq r2.y, r2.y
akaaaaaaacaaaeacacaaaakkacaaaaaaaaaaaaaaaaaaaaaa rsq r2.z, r2.z
akaaaaaaacaaaiacacaaaappacaaaaaaaaaaaaaaaaaaaaaa rsq r2.w, r2.w
adaaaaaaaaaaapacaaaaaaoeacaaaaaaacaaaaoeacaaaaaa mul r0, r0, r2
bdaaaaaaacaaaeacafaaaaoeacaaaaaabiaaaaoeabaaaaaa dp4 r2.z, r5, c24
bdaaaaaaacaaacacafaaaaoeacaaaaaabhaaaaoeabaaaaaa dp4 r2.y, r5, c23
bdaaaaaaacaaabacafaaaaoeacaaaaaabgaaaaoeabaaaaaa dp4 r2.x, r5, c22
afaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rcp r1.x, r1.x
afaaaaaaabaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa rcp r1.y, r1.y
afaaaaaaabaaaiacabaaaappacaaaaaaaaaaaaaaaaaaaaaa rcp r1.w, r1.w
afaaaaaaabaaaeacabaaaakkacaaaaaaaaaaaaaaaaaaaaaa rcp r1.z, r1.z
ahaaaaaaaaaaapacaaaaaaoeacaaaaaabnaaaakkabaaaaaa max r0, r0, c29.z
adaaaaaaaaaaapacaaaaaaoeacaaaaaaabaaaaoeacaaaaaa mul r0, r0, r1
adaaaaaaabaaahacaaaaaaffacaaaaaabdaaaaoeabaaaaaa mul r1.xyz, r0.y, c19
adaaaaaaagaaahacaaaaaaaaacaaaaaabcaaaaoeabaaaaaa mul r6.xyz, r0.x, c18
abaaaaaaabaaahacagaaaakeacaaaaaaabaaaakeacaaaaaa add r1.xyz, r6.xyzz, r1.xyzz
adaaaaaaaaaaahacaaaaaakkacaaaaaabeaaaaoeabaaaaaa mul r0.xyz, r0.z, c20
abaaaaaaaaaaahacaaaaaakeacaaaaaaabaaaakeacaaaaaa add r0.xyz, r0.xyzz, r1.xyzz
adaaaaaaabaaapacafaaaakeacaaaaaaafaaaacjacaaaaaa mul r1, r5.xyzz, r5.yzzx
adaaaaaaagaaahacaaaaaappacaaaaaabfaaaaoeabaaaaaa mul r6.xyz, r0.w, c21
abaaaaaaaaaaahacagaaaakeacaaaaaaaaaaaakeacaaaaaa add r0.xyz, r6.xyzz, r0.xyzz
bdaaaaaaadaaaeacabaaaaoeacaaaaaablaaaaoeabaaaaaa dp4 r3.z, r1, c27
bdaaaaaaadaaabacabaaaaoeacaaaaaabjaaaaoeabaaaaaa dp4 r3.x, r1, c25
bdaaaaaaadaaacacabaaaaoeacaaaaaabkaaaaoeabaaaaaa dp4 r3.y, r1, c26
abaaaaaaadaaahacacaaaakeacaaaaaaadaaaakeacaaaaaa add r3.xyz, r2.xyzz, r3.xyzz
aaaaaaaaabaaaiacbnaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c29.x
aaaaaaaaabaaahacanaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c13
bdaaaaaaacaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r2.z, r1, c10
bdaaaaaaacaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r2.y, r1, c9
bdaaaaaaacaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r2.x, r1, c8
adaaaaaaaaaaaiacaeaaaakkacaaaaaaaeaaaakkacaaaaaa mul r0.w, r4.z, r4.z
adaaaaaaagaaaiacafaaaaaaacaaaaaaafaaaaaaacaaaaaa mul r6.w, r5.x, r5.x
acaaaaaaabaaaiacagaaaappacaaaaaaaaaaaappacaaaaaa sub r1.w, r6.w, r0.w
adaaaaaaagaaahacacaaaakeacaaaaaaamaaaappabaaaaaa mul r6.xyz, r2.xyzz, c12.w
acaaaaaaabaaahacagaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r1.xyz, r6.xyzz, a0
bfaaaaaaagaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r6.xyz, r1.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaagaaaakeacaaaaaa dp3 r0.w, a1, r6.xyzz
adaaaaaaafaaaoacabaaaappacaaaaaabmaaaajaabaaaaaa mul r5.yzw, r1.w, c28.xxyz
abaaaaaaadaaahacadaaaakeacaaaaaaafaaaapjacaaaaaa add r3.xyz, r3.xyzz, r5.yzww
abaaaaaaadaaahaeadaaaakeacaaaaaaaaaaaakeacaaaaaa add v3.xyz, r3.xyzz, r0.xyzz
adaaaaaaacaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r2.xyz, a1, r0.w
bfaaaaaaagaaahacacaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r6.xyz, r2.xyzz
adaaaaaaagaaahacagaaaakeacaaaaaabnaaaaffabaaaaaa mul r6.xyz, r6.xyzz, c29.y
acaaaaaaabaaahacagaaaakeacaaaaaaabaaaakeacaaaaaa sub r1.xyz, r6.xyzz, r1.xyzz
aaaaaaaaadaaabacaeaaaappacaaaaaaaaaaaaaaaaaaaaaa mov r3.x, r4.w
aaaaaaaaadaaacacaeaaaaffacaaaaaaaaaaaaaaaaaaaaaa mov r3.y, r4.y
bcaaaaaaaaaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r1.xyzz, c6
bcaaaaaaaaaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r1.xyzz, c5
bcaaaaaaaaaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r1.xyzz, c4
bfaaaaaaagaaalacadaaaapdacaaaaaaaaaaaaaaaaaaaaaa neg r6.xyw, r3.wxww
abaaaaaaabaaahaeagaaaafdacaaaaaaanaaaaoeabaaaaaa add v1.xyz, r6.wxyy, c13
aaaaaaaaacaaaeaeaeaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov v2.z, r4.x
aaaaaaaaacaaacaeaeaaaakkacaaaaaaaaaaaaaaaaaaaaaa mov v2.y, r4.z
aaaaaaaaacaaabaeafaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov v2.x, r5.x
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
"
}

}
Program "fp" {
// Fragment combos: 3
//   opengl - ALU: 20 to 22, TEX: 1 to 2
//   d3d9 - ALU: 23 to 26, TEX: 1 to 2
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Vector 3 [_ReflectColor]
Float 4 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 22 ALU, 1 TEX
PARAM c[7] = { program.local[0..4],
		{ 0, 1, 0.79638672, 0.20373535 },
		{ 0.25, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, -fragment.texcoord[0], texture[0], CUBE;
DP3 R1.x, -fragment.texcoord[2], -fragment.texcoord[2];
RSQ R1.x, R1.x;
DP3 R0.w, fragment.texcoord[1], fragment.texcoord[1];
DP3 R1.w, fragment.texcoord[2], c[0];
MUL R2.xyz, R1.x, -fragment.texcoord[2];
RSQ R0.w, R0.w;
MUL R1.xyz, R0.w, fragment.texcoord[1];
DP3 R0.w, R1, R2;
MAX R0.w, R0, c[5].x;
MOV R1.xyz, c[2];
MAD R1.xyz, R0, c[3], R1;
ADD_SAT R0.w, -R0, c[5].y;
POW R0.w, R0.w, c[4].x;
MAD R0.w, R0, c[5].z, c[5];
MUL R2.xyz, R1, fragment.texcoord[3];
MUL R0.xyz, R1, c[1];
MAX R1.w, R1, c[5].x;
MUL R0.xyz, R1.w, R0;
MAD R0.xyz, R0, c[6].y, R2;
MAD result.color.xyz, R1, c[6].x, R0;
MAX result.color.w, R0, c[5].x;
END
# 22 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Vector 3 [_ReflectColor]
Float 4 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"ps_2_0
; 26 ALU, 1 TEX
dcl_cube s0
def c5, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c6, 2.00000000, 0.25000000, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
mov r0.xyz, -t0
dp3_pp r1.x, -t2, -t2
rsq_pp r1.x, r1.x
mul_pp r1.xyz, r1.x, -t2
texld r2, r0, s0
dp3_pp r0.x, t1, t1
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
dp3_pp r0.x, r0, r1
max_pp r0.x, r0, c5
add_pp_sat r0.x, -r0, c5.y
pow_pp r1.w, r0.x, c4.x
mov_pp r0.xyz, c2
mad_pp r2.xyz, r2, c3, r0
mov_pp r0.x, r1.w
dp3_pp r1.x, t2, c0
mad_pp r0.x, r0, c5.z, c5.w
mul_pp r4.xyz, r2, t3
mul_pp r3.xyz, r2, c1
max_pp r1.x, r1, c5
mul_pp r1.xyz, r1.x, r3
mad_pp r1.xyz, r1, c6.x, r4
mad_pp r1.xyz, r2, c6.y, r1
max_pp r1.w, r0.x, c5.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Vector 3 [_ReflectColor]
Float 4 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"agal_ps
c5 0.0 1.0 0.796387 0.203735
c6 2.0 0.25 0.0 0.0
[bc]
bfaaaaaaaaaaahacaaaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r0.xyz, v0
bfaaaaaaabaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r1.xyz, v2
bfaaaaaaacaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, v2
bcaaaaaaabaaabacabaaaakeacaaaaaaacaaaakeacaaaaaa dp3 r1.x, r1.xyzz, r2.xyzz
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
bfaaaaaaadaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r3.xyz, v2
adaaaaaaabaaahacabaaaaaaacaaaaaaadaaaakeacaaaaaa mul r1.xyz, r1.x, r3.xyzz
ciaaaaaaacaaapacaaaaaageacaaaaaaaaaaaaaaafbababb tex r2, r0.xyzy, s0 <cube wrap linear point>
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
bcaaaaaaaaaaabacaaaaaakeacaaaaaaabaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r1.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaafaaaaoeabaaaaaa max r0.x, r0.x, c5
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaafaaaaffabaaaaaa add r0.x, r0.x, c5.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaabaaapacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa pow r1, r0.x, c4.x
aaaaaaaaaaaaahacacaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.xyz, c2
adaaaaaaacaaahacacaaaakeacaaaaaaadaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c3
abaaaaaaacaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa add r2.xyz, r2.xyzz, r0.xyzz
aaaaaaaaaaaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r1.x
bcaaaaaaabaaabacacaaaaoeaeaaaaaaaaaaaaoeabaaaaaa dp3 r1.x, v2, c0
adaaaaaaaaaaabacaaaaaaaaacaaaaaaafaaaakkabaaaaaa mul r0.x, r0.x, c5.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaafaaaappabaaaaaa add r0.x, r0.x, c5.w
adaaaaaaaeaaahacacaaaakeacaaaaaaadaaaaoeaeaaaaaa mul r4.xyz, r2.xyzz, v3
adaaaaaaadaaahacacaaaakeacaaaaaaabaaaaoeabaaaaaa mul r3.xyz, r2.xyzz, c1
ahaaaaaaabaaabacabaaaaaaacaaaaaaafaaaaoeabaaaaaa max r1.x, r1.x, c5
adaaaaaaabaaahacabaaaaaaacaaaaaaadaaaakeacaaaaaa mul r1.xyz, r1.x, r3.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaagaaaaaaabaaaaaa mul r1.xyz, r1.xyzz, c6.x
abaaaaaaabaaahacabaaaakeacaaaaaaaeaaaakeacaaaaaa add r1.xyz, r1.xyzz, r4.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaaagaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c6.y
abaaaaaaabaaahacacaaaakeacaaaaaaabaaaakeacaaaaaa add r1.xyz, r2.xyzz, r1.xyzz
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaafaaaaaaabaaaaaa max r1.w, r0.x, c5.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 20 ALU, 2 TEX
PARAM c[5] = { program.local[0..2],
		{ 0, 1, 0.79638672, 0.20373535 },
		{ 8, 0.25 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0, fragment.texcoord[3], texture[1], 2D;
TEX R1.xyz, -fragment.texcoord[0], texture[0], CUBE;
DP3 R2.x, -fragment.texcoord[2], -fragment.texcoord[2];
RSQ R2.x, R2.x;
DP3 R1.w, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.w, R1.w;
MUL R3.xyz, R2.x, -fragment.texcoord[2];
MUL R2.xyz, R1.w, fragment.texcoord[1];
DP3 R1.w, R2, R3;
MAX R1.w, R1, c[3].x;
MOV R2.xyz, c[0];
MAD R1.xyz, R1, c[1], R2;
MUL R0.xyz, R0.w, R0;
MUL R0.xyz, R0, R1;
ADD_SAT R1.w, -R1, c[3].y;
POW R0.w, R1.w, c[2].x;
MUL R1.xyz, R1, c[4].y;
MAD R0.w, R0, c[3].z, c[3];
MAD result.color.xyz, R0, c[4].x, R1;
MAX result.color.w, R0, c[3].x;
END
# 20 instructions, 4 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"ps_2_0
; 23 ALU, 2 TEX
dcl_cube s0
dcl_2d s1
def c3, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c4, 0.25000000, 8.00000000, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xy
texld r1, t3, s1
mov r0.xyz, -t0
dp3_pp r2.x, -t2, -t2
rsq_pp r2.x, r2.x
mul_pp r2.xyz, r2.x, -t2
texld r3, r0, s0
dp3_pp r0.x, t1, t1
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
dp3_pp r0.x, r0, r2
max_pp r0.x, r0, c3
add_pp_sat r0.x, -r0, c3.y
pow_pp r2.w, r0.x, c2.x
mov_pp r0.xyz, c0
mad_pp r3.xyz, r3, c1, r0
mul_pp r0.xyz, r1.w, r1
mul_pp r1.xyz, r0, r3
mov_pp r0.x, r2.w
mul_pp r2.xyz, r3, c4.x
mad_pp r0.x, r0, c3.z, c3.w
mad_pp r1.xyz, r1, c4.y, r2
max_pp r1.w, r0.x, c3.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"agal_ps
c3 0.0 1.0 0.796387 0.203735
c4 0.25 8.0 0.0 0.0
[bc]
ciaaaaaaabaaapacadaaaaoeaeaaaaaaabaaaaaaafaababb tex r1, v3, s1 <2d wrap linear point>
bfaaaaaaaaaaahacaaaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r0.xyz, v0
bfaaaaaaacaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, v2
bfaaaaaaadaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r3.xyz, v2
bcaaaaaaacaaabacacaaaakeacaaaaaaadaaaakeacaaaaaa dp3 r2.x, r2.xyzz, r3.xyzz
akaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r2.x
bfaaaaaaaeaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, v2
adaaaaaaacaaahacacaaaaaaacaaaaaaaeaaaakeacaaaaaa mul r2.xyz, r2.x, r4.xyzz
ciaaaaaaadaaapacaaaaaageacaaaaaaaaaaaaaaafbababb tex r3, r0.xyzy, s0 <cube wrap linear point>
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
bcaaaaaaaaaaabacaaaaaakeacaaaaaaacaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r2.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaaoeabaaaaaa max r0.x, r0.x, c3
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaaffabaaaaaa add r0.x, r0.x, c3.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaacaaapacaaaaaaaaacaaaaaaacaaaaaaabaaaaaa pow r2, r0.x, c2.x
aaaaaaaaaaaaahacaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.xyz, c0
adaaaaaaadaaahacadaaaakeacaaaaaaabaaaaoeabaaaaaa mul r3.xyz, r3.xyzz, c1
abaaaaaaadaaahacadaaaakeacaaaaaaaaaaaakeacaaaaaa add r3.xyz, r3.xyzz, r0.xyzz
adaaaaaaaaaaahacabaaaappacaaaaaaabaaaakeacaaaaaa mul r0.xyz, r1.w, r1.xyzz
adaaaaaaabaaahacaaaaaakeacaaaaaaadaaaakeacaaaaaa mul r1.xyz, r0.xyzz, r3.xyzz
aaaaaaaaaaaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r2.x
adaaaaaaacaaahacadaaaakeacaaaaaaaeaaaaaaabaaaaaa mul r2.xyz, r3.xyzz, c4.x
adaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaakkabaaaaaa mul r0.x, r0.x, c3.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaappabaaaaaa add r0.x, r0.x, c3.w
adaaaaaaabaaahacabaaaakeacaaaaaaaeaaaaffabaaaaaa mul r1.xyz, r1.xyzz, c4.y
abaaaaaaabaaahacabaaaakeacaaaaaaacaaaakeacaaaaaa add r1.xyz, r1.xyzz, r2.xyzz
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa max r1.w, r0.x, c3.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 20 ALU, 2 TEX
PARAM c[5] = { program.local[0..2],
		{ 0, 1, 0.79638672, 0.20373535 },
		{ 8, 0.25 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0, fragment.texcoord[3], texture[1], 2D;
TEX R1.xyz, -fragment.texcoord[0], texture[0], CUBE;
DP3 R2.x, -fragment.texcoord[2], -fragment.texcoord[2];
RSQ R2.x, R2.x;
DP3 R1.w, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.w, R1.w;
MUL R3.xyz, R2.x, -fragment.texcoord[2];
MUL R2.xyz, R1.w, fragment.texcoord[1];
DP3 R1.w, R2, R3;
MAX R1.w, R1, c[3].x;
MOV R2.xyz, c[0];
MAD R1.xyz, R1, c[1], R2;
MUL R0.xyz, R0.w, R0;
MUL R0.xyz, R0, R1;
ADD_SAT R1.w, -R1, c[3].y;
POW R0.w, R1.w, c[2].x;
MUL R1.xyz, R1, c[4].y;
MAD R0.w, R0, c[3].z, c[3];
MAD result.color.xyz, R0, c[4].x, R1;
MAX result.color.w, R0, c[3].x;
END
# 20 instructions, 4 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"ps_2_0
; 23 ALU, 2 TEX
dcl_cube s0
dcl_2d s1
def c3, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c4, 0.25000000, 8.00000000, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xy
texld r1, t3, s1
mov r0.xyz, -t0
dp3_pp r2.x, -t2, -t2
rsq_pp r2.x, r2.x
mul_pp r2.xyz, r2.x, -t2
texld r3, r0, s0
dp3_pp r0.x, t1, t1
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
dp3_pp r0.x, r0, r2
max_pp r0.x, r0, c3
add_pp_sat r0.x, -r0, c3.y
pow_pp r2.w, r0.x, c2.x
mov_pp r0.xyz, c0
mad_pp r3.xyz, r3, c1, r0
mul_pp r0.xyz, r1.w, r1
mul_pp r1.xyz, r0, r3
mov_pp r0.x, r2.w
mul_pp r2.xyz, r3, c4.x
mad_pp r0.x, r0, c3.z, c3.w
mad_pp r1.xyz, r1, c4.y, r2
max_pp r1.w, r0.x, c3.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"agal_ps
c3 0.0 1.0 0.796387 0.203735
c4 0.25 8.0 0.0 0.0
[bc]
ciaaaaaaabaaapacadaaaaoeaeaaaaaaabaaaaaaafaababb tex r1, v3, s1 <2d wrap linear point>
bfaaaaaaaaaaahacaaaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r0.xyz, v0
bfaaaaaaacaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, v2
bfaaaaaaadaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r3.xyz, v2
bcaaaaaaacaaabacacaaaakeacaaaaaaadaaaakeacaaaaaa dp3 r2.x, r2.xyzz, r3.xyzz
akaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r2.x
bfaaaaaaaeaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, v2
adaaaaaaacaaahacacaaaaaaacaaaaaaaeaaaakeacaaaaaa mul r2.xyz, r2.x, r4.xyzz
ciaaaaaaadaaapacaaaaaageacaaaaaaaaaaaaaaafbababb tex r3, r0.xyzy, s0 <cube wrap linear point>
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
bcaaaaaaaaaaabacaaaaaakeacaaaaaaacaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r2.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaaoeabaaaaaa max r0.x, r0.x, c3
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaaffabaaaaaa add r0.x, r0.x, c3.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaacaaapacaaaaaaaaacaaaaaaacaaaaaaabaaaaaa pow r2, r0.x, c2.x
aaaaaaaaaaaaahacaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.xyz, c0
adaaaaaaadaaahacadaaaakeacaaaaaaabaaaaoeabaaaaaa mul r3.xyz, r3.xyzz, c1
abaaaaaaadaaahacadaaaakeacaaaaaaaaaaaakeacaaaaaa add r3.xyz, r3.xyzz, r0.xyzz
adaaaaaaaaaaahacabaaaappacaaaaaaabaaaakeacaaaaaa mul r0.xyz, r1.w, r1.xyzz
adaaaaaaabaaahacaaaaaakeacaaaaaaadaaaakeacaaaaaa mul r1.xyz, r0.xyzz, r3.xyzz
aaaaaaaaaaaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r2.x
adaaaaaaacaaahacadaaaakeacaaaaaaaeaaaaaaabaaaaaa mul r2.xyz, r3.xyzz, c4.x
adaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaakkabaaaaaa mul r0.x, r0.x, c3.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaappabaaaaaa add r0.x, r0.x, c3.w
adaaaaaaabaaahacabaaaakeacaaaaaaaeaaaaffabaaaaaa mul r1.xyz, r1.xyzz, c4.y
abaaaaaaabaaahacabaaaakeacaaaaaaacaaaakeacaaaaaa add r1.xyz, r1.xyzz, r2.xyzz
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa max r1.w, r0.x, c3.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

}
	}
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardAdd" }
		ZWrite Off Blend One One Fog { Color (0,0,0,0) }
		Blend SrcAlpha One
Program "vp" {
// Vertex combos: 5
//   opengl - ALU: 25 to 30
//   d3d9 - ALU: 25 to 30
SubProgram "opengl " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
"!!ARBvp1.0
# 29 ALU
PARAM c[20] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[18];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[17].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[17].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[4].z, R0, c[15];
DP4 result.texcoord[4].y, R0, c[14];
DP4 result.texcoord[4].x, R0, c[13];
ADD result.texcoord[1].xyz, -R0, c[18];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
ADD result.texcoord[3].xyz, -R0, c[19];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 29 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"vs_2_0
; 29 ALU
def c19, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1.xyz, c17
mov r1.w, c19.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c16.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c19.y, -r0
mul r1.xyz, v1, c16.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 oT4.z, r0, c14
dp4 oT4.y, r0, c13
dp4 oT4.x, r0, c12
add oT1.xyz, -r0, c17
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
add oT3.xyz, -r0, c18
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "POINT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = vec2(dot (xlv_TEXCOORD4, xlv_TEXCOORD4));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * texture2D (_LightTexture0, tmpvar_10).w) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "POINT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = vec2(dot (xlv_TEXCOORD4, xlv_TEXCOORD4));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * texture2D (_LightTexture0, tmpvar_10).w) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"agal_vs
c19 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacbbaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c17
aaaaaaaaabaaaiacbdaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c19.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaabaaaaappabaaaaaa mul r2.xyz, r0.xyzz, c16.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaabdaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c19.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaabaaaaappabaaaaaa mul r1.xyz, a1, c16.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bdaaaaaaaaaaaiacaaaaaaoeaaaaaaaaahaaaaoeabaaaaaa dp4 r0.w, a0, c7
bdaaaaaaaeaaaeaeaaaaaaoeacaaaaaaaoaaaaoeabaaaaaa dp4 v4.z, r0, c14
bdaaaaaaaeaaacaeaaaaaaoeacaaaaaaanaaaaoeabaaaaaa dp4 v4.y, r0, c13
bdaaaaaaaeaaabaeaaaaaaoeacaaaaaaamaaaaoeabaaaaaa dp4 v4.x, r0, c12
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaabbaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c17
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaadaaahaeacaaaakeacaaaaaabcaaaaoeabaaaaaa add v3.xyz, r2.xyzz, c18
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
aaaaaaaaaeaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v4.w, c0
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Vector 15 [_WorldSpaceLightPos0]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
"!!ARBvp1.0
# 25 ALU
PARAM c[16] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..15] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[14];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[13].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[13].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
ADD result.texcoord[1].xyz, -R0, c[14];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MOV result.texcoord[3].xyz, c[15];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 25 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Vector 14 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
"vs_2_0
; 25 ALU
def c15, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1.xyz, c13
mov r1.w, c15.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c12.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c15.y, -r0
mul r1.xyz, v1, c12.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
add oT1.xyz, -r0, c13
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mov oT3.xyz, c14
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, lightDir)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, lightDir)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Vector 14 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
"agal_vs
c15 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacanaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c13
aaaaaaaaabaaaiacapaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c15.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaaamaaaappabaaaaaa mul r2.xyz, r0.xyzz, c12.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaaapaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c15.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaamaaaappabaaaaaa mul r1.xyz, a1, c12.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaaanaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c13
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
aaaaaaaaadaaahaeaoaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.xyz, c14
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
"
}

SubProgram "opengl " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
"!!ARBvp1.0
# 30 ALU
PARAM c[20] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[18];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[17].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[17].w;
DP4 R0.w, vertex.position, c[8];
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 result.texcoord[4].w, R0, c[16];
DP4 result.texcoord[4].z, R0, c[15];
DP4 result.texcoord[4].y, R0, c[14];
DP4 result.texcoord[4].x, R0, c[13];
ADD result.texcoord[1].xyz, -R0, c[18];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
ADD result.texcoord[3].xyz, -R0, c[19];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 30 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"vs_2_0
; 30 ALU
def c19, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1.xyz, c17
mov r1.w, c19.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c16.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c19.y, -r0
mul r1.xyz, v1, c16.w
dp4 r0.w, v0, c7
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 oT4.w, r0, c15
dp4 oT4.z, r0, c14
dp4 oT4.y, r0, c13
dp4 oT4.x, r0, c12
add oT1.xyz, -r0, c17
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
add oT3.xyz, -r0, c18
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_9;
  highp vec3 LightCoord_i0;
  LightCoord_i0 = xlv_TEXCOORD4.xyz;
  highp vec2 tmpvar_10;
  tmpvar_10 = vec2(dot (LightCoord_i0, LightCoord_i0));
  lowp float atten;
  atten = ((float((xlv_TEXCOORD4.z > 0.0)) * texture2D (_LightTexture0, ((xlv_TEXCOORD4.xy / xlv_TEXCOORD4.w) + 0.5)).w) * texture2D (_LightTextureB0, tmpvar_10).w);
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * atten) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "SPOT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_9;
  highp vec3 LightCoord_i0;
  LightCoord_i0 = xlv_TEXCOORD4.xyz;
  highp vec2 tmpvar_10;
  tmpvar_10 = vec2(dot (LightCoord_i0, LightCoord_i0));
  lowp float atten;
  atten = ((float((xlv_TEXCOORD4.z > 0.0)) * texture2D (_LightTexture0, ((xlv_TEXCOORD4.xy / xlv_TEXCOORD4.w) + 0.5)).w) * texture2D (_LightTextureB0, tmpvar_10).w);
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * atten) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"agal_vs
c19 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacbbaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c17
aaaaaaaaabaaaiacbdaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c19.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaabaaaaappabaaaaaa mul r2.xyz, r0.xyzz, c16.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaabdaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c19.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaabaaaaappabaaaaaa mul r1.xyz, a1, c16.w
bdaaaaaaaaaaaiacaaaaaaoeaaaaaaaaahaaaaoeabaaaaaa dp4 r0.w, a0, c7
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bdaaaaaaaeaaaiaeaaaaaaoeacaaaaaaapaaaaoeabaaaaaa dp4 v4.w, r0, c15
bdaaaaaaaeaaaeaeaaaaaaoeacaaaaaaaoaaaaoeabaaaaaa dp4 v4.z, r0, c14
bdaaaaaaaeaaacaeaaaaaaoeacaaaaaaanaaaaoeabaaaaaa dp4 v4.y, r0, c13
bdaaaaaaaeaaabaeaaaaaaoeacaaaaaaamaaaaoeabaaaaaa dp4 v4.x, r0, c12
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaabbaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c17
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaadaaahaeacaaaakeacaaaaaabcaaaaoeabaaaaaa add v3.xyz, r2.xyzz, c18
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
"
}

SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
"!!ARBvp1.0
# 29 ALU
PARAM c[20] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[18];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[17].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[17].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[4].z, R0, c[15];
DP4 result.texcoord[4].y, R0, c[14];
DP4 result.texcoord[4].x, R0, c[13];
ADD result.texcoord[1].xyz, -R0, c[18];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
ADD result.texcoord[3].xyz, -R0, c[19];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 29 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"vs_2_0
; 29 ALU
def c19, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1.xyz, c17
mov r1.w, c19.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c16.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c19.y, -r0
mul r1.xyz, v1, c16.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 oT4.z, r0, c14
dp4 oT4.y, r0, c13
dp4 oT4.x, r0, c12
add oT1.xyz, -r0, c17
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
add oT3.xyz, -r0, c18
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = vec2(dot (xlv_TEXCOORD4, xlv_TEXCOORD4));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * (texture2D (_LightTextureB0, tmpvar_10).w * textureCube (_LightTexture0, xlv_TEXCOORD4).w)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "POINT_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = vec2(dot (xlv_TEXCOORD4, xlv_TEXCOORD4));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * (texture2D (_LightTextureB0, tmpvar_10).w * textureCube (_LightTexture0, xlv_TEXCOORD4).w)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"agal_vs
c19 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacbbaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c17
aaaaaaaaabaaaiacbdaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c19.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaabaaaaappabaaaaaa mul r2.xyz, r0.xyzz, c16.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaabdaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c19.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaabaaaaappabaaaaaa mul r1.xyz, a1, c16.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bdaaaaaaaaaaaiacaaaaaaoeaaaaaaaaahaaaaoeabaaaaaa dp4 r0.w, a0, c7
bdaaaaaaaeaaaeaeaaaaaaoeacaaaaaaaoaaaaoeabaaaaaa dp4 v4.z, r0, c14
bdaaaaaaaeaaacaeaaaaaaoeacaaaaaaanaaaaoeabaaaaaa dp4 v4.y, r0, c13
bdaaaaaaaeaaabaeaaaaaaoeacaaaaaaamaaaaoeabaaaaaa dp4 v4.x, r0, c12
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaabbaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c17
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaadaaahaeacaaaakeacaaaaaabcaaaaoeabaaaaaa add v3.xyz, r2.xyzz, c18
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
aaaaaaaaaeaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v4.w, c0
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
"!!ARBvp1.0
# 28 ALU
PARAM c[20] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[18];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[17].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[17].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[4].y, R0, c[14];
DP4 result.texcoord[4].x, R0, c[13];
ADD result.texcoord[1].xyz, -R0, c[18];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MOV result.texcoord[3].xyz, c[19];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 28 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"vs_2_0
; 28 ALU
def c19, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1.xyz, c17
mov r1.w, c19.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c16.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c19.y, -r0
mul r1.xyz, v1, c16.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 oT4.y, r0, c13
dp4 oT4.x, r0, c12
add oT1.xyz, -r0, c17
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mov oT3.xyz, c18
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * texture2D (_LightTexture0, xlv_TEXCOORD4).w) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  lowp vec3 tmpvar_4;
  tmpvar_4 = -(tmpvar_1);
  worldReflVec = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_5;
  lowp float tmpvar_6;
  tmpvar_6 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (-(xlv_TEXCOORD2))), 0.0)), 0.0, 1.0);
  mediump float tmpvar_7;
  tmpvar_7 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_6, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_8;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * texture2D (_LightTexture0, xlv_TEXCOORD4).w) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"agal_vs
c19 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacbbaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c17
aaaaaaaaabaaaiacbdaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c19.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaabaaaaappabaaaaaa mul r2.xyz, r0.xyzz, c16.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaabdaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c19.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaabaaaaappabaaaaaa mul r1.xyz, a1, c16.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bdaaaaaaaaaaaiacaaaaaaoeaaaaaaaaahaaaaoeabaaaaaa dp4 r0.w, a0, c7
bdaaaaaaaeaaacaeaaaaaaoeacaaaaaaanaaaaoeabaaaaaa dp4 v4.y, r0, c13
bdaaaaaaaeaaabaeaaaaaaoeacaaaaaaamaaaaoeabaaaaaa dp4 v4.x, r0, c12
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaabbaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c17
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
aaaaaaaaadaaahaebcaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.xyz, c18
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
aaaaaaaaaeaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v4.zw, c0
"
}

}
Program "fp" {
// Fragment combos: 5
//   opengl - ALU: 21 to 32, TEX: 1 to 3
//   d3d9 - ALU: 25 to 35, TEX: 1 to 3
SubProgram "opengl " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 26 ALU, 2 TEX
PARAM c[6] = { program.local[0..3],
		{ 0.20373535, 0.79638672, 1, 0 },
		{ 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, -fragment.texcoord[0], texture[0], CUBE;
DP3 R0.w, fragment.texcoord[4], fragment.texcoord[4];
DP3 R1.y, -fragment.texcoord[2], -fragment.texcoord[2];
RSQ R1.y, R1.y;
DP3 R1.x, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R1.y, -fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3 R1.x, R1, R2;
DP3 R1.y, fragment.texcoord[3], fragment.texcoord[3];
RSQ R2.x, R1.y;
MAX R1.x, R1, c[4].w;
ADD_SAT R1.x, -R1, c[4].z;
POW R1.x, R1.x, c[3].x;
MAD R1.w, R1.x, c[4].y, c[4].x;
MOV R1.xyz, c[1];
MAD R0.xyz, R0, c[2], R1;
MUL R2.xyz, R2.x, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R2;
MUL R0.xyz, R0, c[0];
MAX R1.x, R1, c[4].w;
MAX result.color.w, R1, c[4];
TEX R0.w, R0.w, texture[1], 2D;
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[5].x;
END
# 26 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 30 ALU, 2 TEX
dcl_cube s0
dcl_2d s1
def c4, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c5, 2.00000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dcl t4.xyz
dp3 r0.x, t4, t4
mov r1.xy, r0.x
mov r0.xyz, -t0
mov_pp r4.xyz, c1
texld r3, r1, s1
texld r2, r0, s0
dp3_pp r1.x, -t2, -t2
dp3_pp r0.x, t1, t1
rsq_pp r1.x, r1.x
rsq_pp r0.x, r0.x
mad_pp r2.xyz, r2, c2, r4
mul_pp r1.xyz, r1.x, -t2
mul_pp r0.xyz, r0.x, t1
dp3_pp r0.x, r0, r1
max_pp r0.x, r0, c4
add_pp_sat r0.x, -r0, c4.y
pow_pp r1.w, r0.x, c3.x
mov_pp r0.x, r1.w
dp3_pp r1.x, t3, t3
rsq_pp r1.x, r1.x
mul_pp r1.xyz, r1.x, t3
dp3_pp r1.x, t2, r1
mad_pp r0.x, r0, c4.z, c4.w
mul_pp r2.xyz, r2, c0
max_pp r1.x, r1, c4
mul_pp r1.x, r1, r3
mul_pp r1.xyz, r1.x, r2
mul_pp r1.xyz, r1, c5.x
max_pp r1.w, r0.x, c4.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "POINT" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "POINT" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"agal_ps
c4 0.0 1.0 0.796387 0.203735
c5 2.0 0.0 0.0 0.0
[bc]
bfaaaaaaabaaahacaaaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r1.xyz, v0
bcaaaaaaaaaaabacaeaaaaoeaeaaaaaaaeaaaaoeaeaaaaaa dp3 r0.x, v4, v4
aaaaaaaaaaaaadacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.xy, r0.x
aaaaaaaaadaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r3.xyz, c1
ciaaaaaaacaaapacabaaaageacaaaaaaaaaaaaaaafbababb tex r2, r1.xyzy, s0 <cube wrap linear point>
ciaaaaaaaaaaapacaaaaaafeacaaaaaaabaaaaaaafaababb tex r0, r0.xyyy, s1 <2d wrap linear point>
bfaaaaaaafaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r5.xyz, v2
bfaaaaaaaeaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, v2
bcaaaaaaabaaabacafaaaakeacaaaaaaaeaaaakeacaaaaaa dp3 r1.x, r5.xyzz, r4.xyzz
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaacaaahacacaaaakeacaaaaaaacaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c2
abaaaaaaacaaahacacaaaakeacaaaaaaadaaaakeacaaaaaa add r2.xyz, r2.xyzz, r3.xyzz
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
bfaaaaaaafaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r5.xyz, v2
adaaaaaaabaaahacabaaaaaaacaaaaaaafaaaakeacaaaaaa mul r1.xyz, r1.x, r5.xyzz
bcaaaaaaaaaaabacaaaaaakeacaaaaaaabaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r1.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaffabaaaaaa add r0.x, r0.x, c4.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaabaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r1, r0.x, c3.x
aaaaaaaaaaaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r1.x
bcaaaaaaabaaabacadaaaaoeaeaaaaaaadaaaaoeaeaaaaaa dp3 r1.x, v3, v3
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
adaaaaaaabaaahacabaaaaaaacaaaaaaadaaaaoeaeaaaaaa mul r1.xyz, r1.x, v3
bcaaaaaaabaaabacacaaaaoeaeaaaaaaabaaaakeacaaaaaa dp3 r1.x, v2, r1.xyzz
ahaaaaaaabaaabacabaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r1.x, r1.x, c4
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaakkabaaaaaa mul r0.x, r0.x, c4.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaappabaaaaaa add r0.x, r0.x, c4.w
adaaaaaaacaaahacacaaaakeacaaaaaaaaaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c0
adaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaappacaaaaaa mul r1.x, r1.x, r0.w
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaakeacaaaaaa mul r1.xyz, r1.x, r2.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaafaaaaaaabaaaaaa mul r1.xyz, r1.xyzz, c5.x
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa max r1.w, r0.x, c4.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 21 ALU, 1 TEX
PARAM c[6] = { program.local[0..3],
		{ 0.20373535, 0.79638672, 1, 0 },
		{ 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, -fragment.texcoord[0], texture[0], CUBE;
DP3 R1.x, -fragment.texcoord[2], -fragment.texcoord[2];
RSQ R1.x, R1.x;
DP3 R0.w, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R1.x, -fragment.texcoord[2];
RSQ R0.w, R0.w;
MUL R1.xyz, R0.w, fragment.texcoord[1];
DP3 R0.w, R1, R2;
MOV R1.xyz, c[1];
MAD R0.xyz, R0, c[2], R1;
MAX R0.w, R0, c[4];
ADD_SAT R0.w, -R0, c[4].z;
POW R0.w, R0.w, c[3].x;
MAD R0.w, R0, c[4].y, c[4].x;
MOV R2.xyz, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R2;
MUL R0.xyz, R0, c[0];
MAX R1.x, R1, c[4].w;
MUL R0.xyz, R1.x, R0;
MAX result.color.w, R0, c[4];
MUL result.color.xyz, R0, c[5].x;
END
# 21 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"ps_2_0
; 25 ALU, 1 TEX
dcl_cube s0
def c4, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c5, 2.00000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
mov r0.xyz, -t0
dp3_pp r2.x, -t2, -t2
rsq_pp r2.x, r2.x
mul_pp r2.xyz, r2.x, -t2
mov_pp r3.xyz, t3
texld r1, r0, s0
dp3_pp r0.x, t1, t1
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
dp3_pp r0.x, r0, r2
max_pp r0.x, r0, c4
add_pp_sat r0.x, -r0, c4.y
pow_pp r2.w, r0.x, c3.x
mov_pp r0.x, r2.w
mov_pp r2.xyz, c1
mad_pp r2.xyz, r1, c2, r2
dp3_pp r1.x, t2, r3
mad_pp r0.x, r0, c4.z, c4.w
mul_pp r2.xyz, r2, c0
max_pp r1.x, r1, c4
mul_pp r1.xyz, r1.x, r2
mul_pp r1.xyz, r1, c5.x
max_pp r1.w, r0.x, c4.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"agal_ps
c4 0.0 1.0 0.796387 0.203735
c5 2.0 0.0 0.0 0.0
[bc]
bfaaaaaaaaaaahacaaaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r0.xyz, v0
bfaaaaaaabaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r1.xyz, v2
bfaaaaaaacaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, v2
bcaaaaaaacaaabacabaaaakeacaaaaaaacaaaakeacaaaaaa dp3 r2.x, r1.xyzz, r2.xyzz
akaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r2.x
bfaaaaaaadaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r3.xyz, v2
adaaaaaaacaaahacacaaaaaaacaaaaaaadaaaakeacaaaaaa mul r2.xyz, r2.x, r3.xyzz
aaaaaaaaadaaahacadaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa mov r3.xyz, v3
ciaaaaaaabaaapacaaaaaageacaaaaaaaaaaaaaaafbababb tex r1, r0.xyzy, s0 <cube wrap linear point>
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
bcaaaaaaaaaaabacaaaaaakeacaaaaaaacaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r2.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaffabaaaaaa add r0.x, r0.x, c4.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaacaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r2, r0.x, c3.x
aaaaaaaaaaaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r2.x
aaaaaaaaacaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r2.xyz, c1
adaaaaaaaeaaahacabaaaakeacaaaaaaacaaaaoeabaaaaaa mul r4.xyz, r1.xyzz, c2
abaaaaaaacaaahacaeaaaakeacaaaaaaacaaaakeacaaaaaa add r2.xyz, r4.xyzz, r2.xyzz
bcaaaaaaabaaabacacaaaaoeaeaaaaaaadaaaakeacaaaaaa dp3 r1.x, v2, r3.xyzz
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaakkabaaaaaa mul r0.x, r0.x, c4.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaappabaaaaaa add r0.x, r0.x, c4.w
adaaaaaaacaaahacacaaaakeacaaaaaaaaaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c0
ahaaaaaaabaaabacabaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r1.x, r1.x, c4
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaakeacaaaaaa mul r1.xyz, r1.x, r2.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaafaaaaaaabaaaaaa mul r1.xyz, r1.xyzz, c5.x
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa max r1.w, r0.x, c4.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 32 ALU, 3 TEX
PARAM c[6] = { program.local[0..3],
		{ 0.20373535, 0.79638672, 1, 0 },
		{ 0.5, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
RCP R0.x, fragment.texcoord[4].w;
MAD R1.xy, fragment.texcoord[4], R0.x, c[5].x;
DP3 R1.z, fragment.texcoord[4], fragment.texcoord[4];
TEX R0.w, R1, texture[1], 2D;
TEX R1.w, R1.z, texture[2], 2D;
TEX R0.xyz, -fragment.texcoord[0], texture[0], CUBE;
DP3 R1.y, -fragment.texcoord[2], -fragment.texcoord[2];
RSQ R1.y, R1.y;
DP3 R1.x, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R1.y, -fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3 R1.x, R1, R2;
MAX R1.x, R1, c[4].w;
ADD_SAT R1.x, -R1, c[4].z;
POW R1.x, R1.x, c[3].x;
MAD R2.x, R1, c[4].y, c[4];
MOV R1.xyz, c[1];
MAD R0.xyz, R0, c[2], R1;
DP3 R2.y, fragment.texcoord[3], fragment.texcoord[3];
RSQ R2.y, R2.y;
MUL R1.xyz, R2.y, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R1;
SLT R1.y, c[4].w, fragment.texcoord[4].z;
MUL R0.w, R1.y, R0;
MUL R1.y, R0.w, R1.w;
MAX R0.w, R1.x, c[4];
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MAX result.color.w, R2.x, c[4];
MUL result.color.xyz, R0, c[5].y;
END
# 32 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"ps_2_0
; 35 ALU, 3 TEX
dcl_cube s0
dcl_2d s1
dcl_2d s2
def c4, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c5, 0.50000000, 2.00000000, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dcl t4
mov r1.xyz, -t0
dp3 r2.x, t4, t4
mov r2.xy, r2.x
rcp r0.x, t4.w
mad r0.xy, t4, r0.x, c5.x
texld r3, r2, s2
texld r0, r0, s1
texld r1, r1, s0
dp3_pp r2.x, -t2, -t2
dp3_pp r0.x, t1, t1
rsq_pp r2.x, r2.x
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
mul_pp r2.xyz, r2.x, -t2
dp3_pp r0.x, r0, r2
max_pp r0.x, r0, c4
add_pp_sat r0.x, -r0, c4.y
pow_pp r2.w, r0.x, c3.x
mov_pp r0.xyz, c1
mad_pp r1.xyz, r1, c2, r0
mov_pp r0.x, r2.w
mul_pp r4.xyz, r1, c0
dp3_pp r1.x, t3, t3
rsq_pp r2.x, r1.x
cmp r1.x, -t4.z, c4, c4.y
mul_pp r2.xyz, r2.x, t3
dp3_pp r2.x, t2, r2
mad_pp r0.x, r0, c4.z, c4.w
mul_pp r1.x, r1, r0.w
mul_pp r1.x, r1, r3
max_pp r2.x, r2, c4
mul_pp r1.x, r2, r1
mul_pp r1.xyz, r1.x, r4
mul_pp r1.xyz, r1, c5.y
max_pp r1.w, r0.x, c4.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "SPOT" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"agal_ps
c4 0.0 1.0 0.796387 0.203735
c5 0.5 2.0 0.0 0.0
[bc]
bcaaaaaaacaaabacaeaaaaoeaeaaaaaaaeaaaaoeaeaaaaaa dp3 r2.x, v4, v4
bfaaaaaaabaaahacaaaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r1.xyz, v0
aaaaaaaaadaaadacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r3.xy, r2.x
afaaaaaaaaaaabacaeaaaappaeaaaaaaaaaaaaaaaaaaaaaa rcp r0.x, v4.w
adaaaaaaacaaadacaeaaaaoeaeaaaaaaaaaaaaaaacaaaaaa mul r2.xy, v4, r0.x
abaaaaaaacaaadacacaaaafeacaaaaaaafaaaaaaabaaaaaa add r2.xy, r2.xyyy, c5.x
ciaaaaaaaaaaapacadaaaafeacaaaaaaacaaaaaaafaababb tex r0, r3.xyyy, s2 <2d wrap linear point>
ciaaaaaaadaaapacacaaaafeacaaaaaaabaaaaaaafaababb tex r3, r2.xyyy, s1 <2d wrap linear point>
ciaaaaaaabaaapacabaaaageacaaaaaaaaaaaaaaafbababb tex r1, r1.xyzy, s0 <cube wrap linear point>
bfaaaaaaaeaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, v2
bfaaaaaaafaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r5.xyz, v2
bcaaaaaaacaaabacaeaaaakeacaaaaaaafaaaakeacaaaaaa dp3 r2.x, r4.xyzz, r5.xyzz
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r2.x
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
bfaaaaaaaeaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, v2
adaaaaaaacaaahacacaaaaaaacaaaaaaaeaaaakeacaaaaaa mul r2.xyz, r2.x, r4.xyzz
bcaaaaaaaaaaabacaaaaaakeacaaaaaaacaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r2.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaffabaaaaaa add r0.x, r0.x, c4.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaacaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r2, r0.x, c3.x
aaaaaaaaaaaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.xyz, c1
adaaaaaaabaaahacabaaaakeacaaaaaaacaaaaoeabaaaaaa mul r1.xyz, r1.xyzz, c2
abaaaaaaabaaahacabaaaakeacaaaaaaaaaaaakeacaaaaaa add r1.xyz, r1.xyzz, r0.xyzz
adaaaaaaadaaahacabaaaakeacaaaaaaaaaaaaoeabaaaaaa mul r3.xyz, r1.xyzz, c0
aaaaaaaaaaaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r2.x
bcaaaaaaabaaabacadaaaaoeaeaaaaaaadaaaaoeaeaaaaaa dp3 r1.x, v3, v3
akaaaaaaacaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r1.x
adaaaaaaacaaahacacaaaaaaacaaaaaaadaaaaoeaeaaaaaa mul r2.xyz, r2.x, v3
bcaaaaaaacaaabacacaaaaoeaeaaaaaaacaaaakeacaaaaaa dp3 r2.x, v2, r2.xyzz
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaakkabaaaaaa mul r0.x, r0.x, c4.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaappabaaaaaa add r0.x, r0.x, c4.w
bfaaaaaaafaaaeacaeaaaakkaeaaaaaaaaaaaaaaaaaaaaaa neg r5.z, v4.z
ckaaaaaaabaaabacafaaaakkacaaaaaaafaaaakkabaaaaaa slt r1.x, r5.z, c5.z
adaaaaaaabaaabacabaaaaaaacaaaaaaadaaaappacaaaaaa mul r1.x, r1.x, r3.w
adaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaappacaaaaaa mul r1.x, r1.x, r0.w
ahaaaaaaacaaabacacaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r2.x, r2.x, c4
adaaaaaaabaaabacacaaaaaaacaaaaaaabaaaaaaacaaaaaa mul r1.x, r2.x, r1.x
adaaaaaaabaaahacabaaaaaaacaaaaaaadaaaakeacaaaaaa mul r1.xyz, r1.x, r3.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaafaaaaffabaaaaaa mul r1.xyz, r1.xyzz, c5.y
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa max r1.w, r0.x, c4.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 28 ALU, 3 TEX
PARAM c[6] = { program.local[0..3],
		{ 0.20373535, 0.79638672, 1, 0 },
		{ 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, -fragment.texcoord[0], texture[0], CUBE;
TEX R1.w, fragment.texcoord[4], texture[2], CUBE;
DP3 R0.w, fragment.texcoord[4], fragment.texcoord[4];
DP3 R1.y, -fragment.texcoord[2], -fragment.texcoord[2];
RSQ R1.y, R1.y;
DP3 R1.x, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R1.y, -fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3 R1.x, R1, R2;
MAX R1.x, R1, c[4].w;
ADD_SAT R1.x, -R1, c[4].z;
POW R1.x, R1.x, c[3].x;
MAD R2.x, R1, c[4].y, c[4];
MOV R1.xyz, c[1];
MAD R0.xyz, R0, c[2], R1;
DP3 R2.y, fragment.texcoord[3], fragment.texcoord[3];
RSQ R2.y, R2.y;
MUL R1.xyz, R2.y, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R1;
MUL R0.xyz, R0, c[0];
MAX result.color.w, R2.x, c[4];
TEX R0.w, R0.w, texture[1], 2D;
MUL R1.y, R0.w, R1.w;
MAX R0.w, R1.x, c[4];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[5].x;
END
# 28 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"ps_2_0
; 31 ALU, 3 TEX
dcl_cube s0
dcl_2d s1
dcl_cube s2
def c4, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c5, 2.00000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dcl t4.xyz
dp3 r0.x, t4, t4
mov r2.xy, r0.x
mov r1.xyz, -t0
texld r3, r2, s1
texld r0, t4, s2
texld r1, r1, s0
dp3_pp r2.x, -t2, -t2
dp3_pp r0.x, t1, t1
rsq_pp r2.x, r2.x
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
mul_pp r2.xyz, r2.x, -t2
dp3_pp r0.x, r0, r2
max_pp r0.x, r0, c4
add_pp_sat r0.x, -r0, c4.y
pow_pp r2.w, r0.x, c3.x
mov_pp r0.x, r2.w
mov_pp r2.xyz, c1
mad_pp r2.xyz, r1, c2, r2
dp3_pp r1.x, t3, t3
rsq_pp r1.x, r1.x
mul_pp r1.xyz, r1.x, t3
dp3_pp r1.x, t2, r1
mad_pp r0.x, r0, c4.z, c4.w
mul_pp r2.xyz, r2, c0
mul r3.x, r3, r0.w
max_pp r1.x, r1, c4
mul_pp r1.x, r1, r3
mul_pp r1.xyz, r1.x, r2
mul_pp r1.xyz, r1, c5.x
max_pp r1.w, r0.x, c4.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "POINT_COOKIE" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"agal_ps
c4 0.0 1.0 0.796387 0.203735
c5 2.0 0.0 0.0 0.0
[bc]
bcaaaaaaaaaaabacaeaaaaoeaeaaaaaaaeaaaaoeaeaaaaaa dp3 r0.x, v4, v4
aaaaaaaaacaaadacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r2.xy, r0.x
bfaaaaaaabaaahacaaaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r1.xyz, v0
ciaaaaaaadaaapacacaaaafeacaaaaaaabaaaaaaafaababb tex r3, r2.xyyy, s1 <2d wrap linear point>
ciaaaaaaaaaaapacaeaaaaoeaeaaaaaaacaaaaaaafbababb tex r0, v4, s2 <cube wrap linear point>
ciaaaaaaabaaapacabaaaageacaaaaaaaaaaaaaaafbababb tex r1, r1.xyzy, s0 <cube wrap linear point>
bfaaaaaaaeaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, v2
bfaaaaaaafaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r5.xyz, v2
bcaaaaaaacaaabacaeaaaakeacaaaaaaafaaaakeacaaaaaa dp3 r2.x, r4.xyzz, r5.xyzz
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r2.x
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
bfaaaaaaaeaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, v2
adaaaaaaacaaahacacaaaaaaacaaaaaaaeaaaakeacaaaaaa mul r2.xyz, r2.x, r4.xyzz
bcaaaaaaaaaaabacaaaaaakeacaaaaaaacaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r2.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaffabaaaaaa add r0.x, r0.x, c4.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaacaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r2, r0.x, c3.x
aaaaaaaaaaaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r2.x
aaaaaaaaacaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r2.xyz, c1
adaaaaaaafaaahacabaaaakeacaaaaaaacaaaaoeabaaaaaa mul r5.xyz, r1.xyzz, c2
abaaaaaaacaaahacafaaaakeacaaaaaaacaaaakeacaaaaaa add r2.xyz, r5.xyzz, r2.xyzz
bcaaaaaaabaaabacadaaaaoeaeaaaaaaadaaaaoeaeaaaaaa dp3 r1.x, v3, v3
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
adaaaaaaabaaahacabaaaaaaacaaaaaaadaaaaoeaeaaaaaa mul r1.xyz, r1.x, v3
bcaaaaaaabaaabacacaaaaoeaeaaaaaaabaaaakeacaaaaaa dp3 r1.x, v2, r1.xyzz
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaakkabaaaaaa mul r0.x, r0.x, c4.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaappabaaaaaa add r0.x, r0.x, c4.w
adaaaaaaacaaahacacaaaakeacaaaaaaaaaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c0
adaaaaaaadaaabacadaaaappacaaaaaaaaaaaappacaaaaaa mul r3.x, r3.w, r0.w
ahaaaaaaabaaabacabaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r1.x, r1.x, c4
adaaaaaaabaaabacabaaaaaaacaaaaaaadaaaaaaacaaaaaa mul r1.x, r1.x, r3.x
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaakeacaaaaaa mul r1.xyz, r1.x, r2.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaafaaaaaaabaaaaaa mul r1.xyz, r1.xyzz, c5.x
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa max r1.w, r0.x, c4.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 23 ALU, 2 TEX
PARAM c[6] = { program.local[0..3],
		{ 0.20373535, 0.79638672, 1, 0 },
		{ 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, -fragment.texcoord[0], texture[0], CUBE;
TEX R0.w, fragment.texcoord[4], texture[1], 2D;
DP3 R1.y, -fragment.texcoord[2], -fragment.texcoord[2];
RSQ R1.y, R1.y;
DP3 R1.x, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R1.y, -fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3 R1.x, R1, R2;
MAX R1.x, R1, c[4].w;
ADD_SAT R1.x, -R1, c[4].z;
POW R1.x, R1.x, c[3].x;
MAD R1.w, R1.x, c[4].y, c[4].x;
MOV R1.xyz, c[1];
MAD R0.xyz, R0, c[2], R1;
MOV R2.xyz, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R2;
MAX R1.x, R1, c[4].w;
MUL R0.xyz, R0, c[0];
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MAX result.color.w, R1, c[4];
MUL result.color.xyz, R0, c[5].x;
END
# 23 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 26 ALU, 2 TEX
dcl_cube s0
dcl_2d s1
def c4, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c5, 2.00000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dcl t4.xy
texld r0, t4, s1
mov r1.xyz, -t0
dp3_pp r0.x, t1, t1
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
texld r2, r1, s0
dp3_pp r1.x, -t2, -t2
rsq_pp r1.x, r1.x
mul_pp r1.xyz, r1.x, -t2
dp3_pp r0.x, r0, r1
max_pp r0.x, r0, c4
add_pp_sat r0.x, -r0, c4.y
pow_pp r1.w, r0.x, c3.x
mov_pp r0.x, r1.w
mad_pp r0.x, r0, c4.z, c4.w
mov_pp r1.xyz, c1
mad_pp r2.xyz, r2, c2, r1
mov_pp r1.xyz, t3
dp3_pp r1.x, t2, r1
max_pp r1.x, r1, c4
mul_pp r2.xyz, r2, c0
mul_pp r1.x, r1, r0.w
mul_pp r1.xyz, r1.x, r2
mul_pp r1.xyz, r1, c5.x
max_pp r1.w, r0.x, c4.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"agal_ps
c4 0.0 1.0 0.796387 0.203735
c5 2.0 0.0 0.0 0.0
[bc]
ciaaaaaaaaaaapacaeaaaaoeaeaaaaaaabaaaaaaafaababb tex r0, v4, s1 <2d wrap linear point>
bfaaaaaaabaaahacaaaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r1.xyz, v0
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
ciaaaaaaacaaapacabaaaageacaaaaaaaaaaaaaaafbababb tex r2, r1.xyzy, s0 <cube wrap linear point>
bfaaaaaaaeaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, v2
bfaaaaaaadaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r3.xyz, v2
bcaaaaaaabaaabacaeaaaakeacaaaaaaadaaaakeacaaaaaa dp3 r1.x, r4.xyzz, r3.xyzz
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
bfaaaaaaaeaaahacacaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, v2
adaaaaaaabaaahacabaaaaaaacaaaaaaaeaaaakeacaaaaaa mul r1.xyz, r1.x, r4.xyzz
bcaaaaaaaaaaabacaaaaaakeacaaaaaaabaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r1.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaffabaaaaaa add r0.x, r0.x, c4.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaabaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r1, r0.x, c3.x
aaaaaaaaaaaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r1.x
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaakkabaaaaaa mul r0.x, r0.x, c4.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaappabaaaaaa add r0.x, r0.x, c4.w
aaaaaaaaabaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c1
adaaaaaaacaaahacacaaaakeacaaaaaaacaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c2
abaaaaaaacaaahacacaaaakeacaaaaaaabaaaakeacaaaaaa add r2.xyz, r2.xyzz, r1.xyzz
aaaaaaaaabaaahacadaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, v3
bcaaaaaaabaaabacacaaaaoeaeaaaaaaabaaaakeacaaaaaa dp3 r1.x, v2, r1.xyzz
ahaaaaaaabaaabacabaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r1.x, r1.x, c4
adaaaaaaacaaahacacaaaakeacaaaaaaaaaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c0
adaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaappacaaaaaa mul r1.x, r1.x, r0.w
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaakeacaaaaaa mul r1.xyz, r1.x, r2.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaafaaaaaaabaaaaaa mul r1.xyz, r1.xyzz, c5.x
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa max r1.w, r0.x, c4.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

}
	}

#LINE 46


	Cull Back	
	
	Alphatest Greater 0 ZWrite Off ColorMask RGB
	
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }
		Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
// Vertex combos: 4
//   opengl - ALU: 25 to 71
//   d3d9 - ALU: 25 to 71
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 15 [unity_SHAr]
Vector 16 [unity_SHAg]
Vector 17 [unity_SHAb]
Vector 18 [unity_SHBr]
Vector 19 [unity_SHBg]
Vector 20 [unity_SHBb]
Vector 21 [unity_SHC]
"!!ARBvp1.0
# 42 ALU
PARAM c[22] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..21] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
MUL R1.xyz, vertex.normal, c[13].w;
DP3 R3.w, R1, c[6];
DP3 R2.w, R1, c[7];
DP3 R0.w, R1, c[5];
MOV R0.x, R3.w;
MOV R0.y, R2.w;
MOV R0.z, c[0].x;
MUL R1, R0.wxyy, R0.xyyw;
DP4 R2.z, R0.wxyz, c[17];
DP4 R2.y, R0.wxyz, c[16];
DP4 R2.x, R0.wxyz, c[15];
DP4 R0.z, R1, c[20];
DP4 R0.x, R1, c[18];
DP4 R0.y, R1, c[19];
ADD R2.xyz, R2, R0;
MOV R1.w, c[0].x;
MOV R1.xyz, c[14];
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R1.xyz, R0, c[13].w, -vertex.position;
MUL R0.y, R3.w, R3.w;
MAD R1.w, R0, R0, -R0.y;
DP3 R0.x, vertex.normal, -R1;
MUL R0.xyz, vertex.normal, R0.x;
MAD R0.xyz, -R0, c[0].y, -R1;
MUL R3.xyz, R1.w, c[21];
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
ADD result.texcoord[3].xyz, R2, R3;
ADD result.texcoord[1].xyz, -R0, c[14];
MOV result.texcoord[2].z, R2.w;
MOV result.texcoord[2].y, R3.w;
MOV result.texcoord[2].x, R0.w;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 42 instructions, 4 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_SHAr]
Vector 15 [unity_SHAg]
Vector 16 [unity_SHAb]
Vector 17 [unity_SHBr]
Vector 18 [unity_SHBg]
Vector 19 [unity_SHBb]
Vector 20 [unity_SHC]
"vs_2_0
; 42 ALU
def c21, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mul r1.xyz, v1, c12.w
dp3 r3.w, r1, c5
dp3 r2.w, r1, c6
dp3 r0.w, r1, c4
mov r0.x, r3.w
mov r0.y, r2.w
mov r0.z, c21.x
mul r1, r0.wxyy, r0.xyyw
dp4 r2.z, r0.wxyz, c16
dp4 r2.y, r0.wxyz, c15
dp4 r2.x, r0.wxyz, c14
dp4 r0.z, r1, c19
dp4 r0.x, r1, c17
dp4 r0.y, r1, c18
add r2.xyz, r2, r0
mov r1.w, c21.x
mov r1.xyz, c13
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r1.xyz, r0, c12.w, -v0
mul r0.y, r3.w, r3.w
mad r1.w, r0, r0, -r0.y
dp3 r0.x, v1, -r1
mul r0.xyz, v1, r0.x
mad r0.xyz, -r0, c21.y, -r1
mul r3.xyz, r1.w, c20
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
add oT3.xyz, r2, r3
add oT1.xyz, -r0, c13
mov oT2.z, r2.w
mov oT2.y, r3.w
mov oT2.x, r0.w
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  shlight = tmpvar_11;
  tmpvar_4 = shlight;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_3 * xlv_TEXCOORD3));
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  shlight = tmpvar_11;
  tmpvar_4 = shlight;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_3 * xlv_TEXCOORD3));
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_SHAr]
Vector 15 [unity_SHAg]
Vector 16 [unity_SHAb]
Vector 17 [unity_SHBr]
Vector 18 [unity_SHBg]
Vector 19 [unity_SHBb]
Vector 20 [unity_SHC]
"agal_vs
c21 1.0 2.0 0.0 0.0
[bc]
adaaaaaaabaaahacabaaaaoeaaaaaaaaamaaaappabaaaaaa mul r1.xyz, a1, c12.w
bcaaaaaaadaaaiacabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 r3.w, r1.xyzz, c5
bcaaaaaaacaaaiacabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 r2.w, r1.xyzz, c6
bcaaaaaaaaaaaiacabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 r0.w, r1.xyzz, c4
aaaaaaaaaaaaabacadaaaappacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r3.w
aaaaaaaaaaaaacacacaaaappacaaaaaaaaaaaaaaaaaaaaaa mov r0.y, r2.w
aaaaaaaaaaaaaeacbfaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r0.z, c21.x
adaaaaaaabaaapacaaaaaafdacaaaaaaaaaaaaneacaaaaaa mul r1, r0.wxyy, r0.xyyw
bdaaaaaaacaaaeacaaaaaajdacaaaaaabaaaaaoeabaaaaaa dp4 r2.z, r0.wxyz, c16
bdaaaaaaacaaacacaaaaaajdacaaaaaaapaaaaoeabaaaaaa dp4 r2.y, r0.wxyz, c15
bdaaaaaaacaaabacaaaaaajdacaaaaaaaoaaaaoeabaaaaaa dp4 r2.x, r0.wxyz, c14
bdaaaaaaaaaaaeacabaaaaoeacaaaaaabdaaaaoeabaaaaaa dp4 r0.z, r1, c19
bdaaaaaaaaaaabacabaaaaoeacaaaaaabbaaaaoeabaaaaaa dp4 r0.x, r1, c17
bdaaaaaaaaaaacacabaaaaoeacaaaaaabcaaaaoeabaaaaaa dp4 r0.y, r1, c18
abaaaaaaacaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa add r2.xyz, r2.xyzz, r0.xyzz
aaaaaaaaabaaaiacbfaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c21.x
aaaaaaaaabaaahacanaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c13
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaaeaaahacaaaaaakeacaaaaaaamaaaappabaaaaaa mul r4.xyz, r0.xyzz, c12.w
acaaaaaaabaaahacaeaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r1.xyz, r4.xyzz, a0
adaaaaaaaaaaacacadaaaappacaaaaaaadaaaappacaaaaaa mul r0.y, r3.w, r3.w
adaaaaaaaeaaaiacaaaaaappacaaaaaaaaaaaappacaaaaaa mul r4.w, r0.w, r0.w
acaaaaaaabaaaiacaeaaaappacaaaaaaaaaaaaffacaaaaaa sub r1.w, r4.w, r0.y
bfaaaaaaaeaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, r1.xyzz
bcaaaaaaaaaaabacabaaaaoeaaaaaaaaaeaaaakeacaaaaaa dp3 r0.x, a1, r4.xyzz
adaaaaaaaaaaahacabaaaaoeaaaaaaaaaaaaaaaaacaaaaaa mul r0.xyz, a1, r0.x
bfaaaaaaaeaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, r0.xyzz
adaaaaaaaeaaahacaeaaaakeacaaaaaabfaaaaffabaaaaaa mul r4.xyz, r4.xyzz, c21.y
acaaaaaaaaaaahacaeaaaakeacaaaaaaabaaaakeacaaaaaa sub r0.xyz, r4.xyzz, r1.xyzz
adaaaaaaadaaahacabaaaappacaaaaaabeaaaaoeabaaaaaa mul r3.xyz, r1.w, c20
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
abaaaaaaadaaahaeacaaaakeacaaaaaaadaaaakeacaaaaaa add v3.xyz, r2.xyzz, r3.xyzz
bfaaaaaaaeaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r4.xyz, r0.xyzz
abaaaaaaabaaahaeaeaaaakeacaaaaaaanaaaaoeabaaaaaa add v1.xyz, r4.xyzz, c13
aaaaaaaaacaaaeaeacaaaappacaaaaaaaaaaaaaaaaaaaaaa mov v2.z, r2.w
aaaaaaaaacaaacaeadaaaappacaaaaaaaaaaaaaaaaaaaaaa mov v2.y, r3.w
aaaaaaaaacaaabaeaaaaaappacaaaaaaaaaaaaaaaaaaaaaa mov v2.x, r0.w
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 15 [unity_LightmapST]
"!!ARBvp1.0
# 25 ALU
PARAM c[16] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..15] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[14];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[13].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[13].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
ADD result.texcoord[1].xyz, -R0, c[14];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MAD result.texcoord[3].xy, vertex.texcoord[1], c[15], c[15].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 25 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_LightmapST]
"vs_2_0
; 25 ALU
def c15, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
mov r1.xyz, c13
mov r1.w, c15.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c12.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c15.y, -r0
mul r1.xyz, v1, c12.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
add oT1.xyz, -r0, c13
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mad oT3.xy, v2, c14, c14.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord1;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_4).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_6;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_8;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _ReflectColor;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  c.xyz = (tmpvar_3 * (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD3).xyz));
  c.w = refl2Refr;
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord1;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_4).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_6;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_8;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _ReflectColor;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (unity_Lightmap, xlv_TEXCOORD3);
  c.xyz = (tmpvar_3 * ((8.0 * tmpvar_8.w) * tmpvar_8.xyz));
  c.w = refl2Refr;
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_LightmapST]
"agal_vs
c15 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacanaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c13
aaaaaaaaabaaaiacapaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c15.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaaamaaaappabaaaaaa mul r2.xyz, r0.xyzz, c12.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaaapaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c15.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaamaaaappabaaaaaa mul r1.xyz, a1, c12.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaaanaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c13
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
adaaaaaaacaaadacaeaaaaoeaaaaaaaaaoaaaaoeabaaaaaa mul r2.xy, a4, c14
abaaaaaaadaaadaeacaaaafeacaaaaaaaoaaaaooabaaaaaa add v3.xy, r2.xyyy, c14.zwzw
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.zw, c0
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 15 [unity_LightmapST]
"!!ARBvp1.0
# 25 ALU
PARAM c[16] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..15] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[14];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[13].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[13].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
ADD result.texcoord[1].xyz, -R0, c[14];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MAD result.texcoord[3].xy, vertex.texcoord[1], c[15], c[15].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 25 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_LightmapST]
"vs_2_0
; 25 ALU
def c15, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
mov r1.xyz, c13
mov r1.w, c15.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c12.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c15.y, -r0
mul r1.xyz, v1, c12.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
add oT1.xyz, -r0, c13
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mad oT3.xy, v2, c14, c14.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord1;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_4).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_6;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_8;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _ReflectColor;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  mediump vec3 lm_i0;
  lowp vec3 tmpvar_8;
  tmpvar_8 = (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD3).xyz);
  lm_i0 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_3 * lm_i0);
  c.xyz = tmpvar_9;
  c.w = refl2Refr;
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_LightmapST;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec4 _glesMultiTexCoord1;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_5;
  tmpvar_5[0] = _Object2World[0].xyz;
  tmpvar_5[1] = _Object2World[1].xyz;
  tmpvar_5[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_4).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_6;
  mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_8;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp vec4 _ReflectColor;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  c = vec4(0.0, 0.0, 0.0, 0.0);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (unity_Lightmap, xlv_TEXCOORD3);
  mediump vec3 lm_i0;
  lowp vec3 tmpvar_9;
  tmpvar_9 = ((8.0 * tmpvar_8.w) * tmpvar_8.xyz);
  lm_i0 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_3 * lm_i0);
  c.xyz = tmpvar_10;
  c.w = refl2Refr;
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_LightmapST]
"agal_vs
c15 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacanaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c13
aaaaaaaaabaaaiacapaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c15.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaaamaaaappabaaaaaa mul r2.xyz, r0.xyzz, c12.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaaapaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c15.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaamaaaappabaaaaaa mul r1.xyz, a1, c12.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaaanaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c13
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
adaaaaaaacaaadacaeaaaaoeaaaaaaaaaoaaaaoeabaaaaaa mul r2.xy, a4, c14
abaaaaaaadaaadaeacaaaafeacaaaaaaaoaaaaooabaaaaaa add v3.xy, r2.xyyy, c14.zwzw
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.zw, c0
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 15 [unity_4LightPosX0]
Vector 16 [unity_4LightPosY0]
Vector 17 [unity_4LightPosZ0]
Vector 18 [unity_4LightAtten0]
Vector 19 [unity_LightColor0]
Vector 20 [unity_LightColor1]
Vector 21 [unity_LightColor2]
Vector 22 [unity_LightColor3]
Vector 23 [unity_SHAr]
Vector 24 [unity_SHAg]
Vector 25 [unity_SHAb]
Vector 26 [unity_SHBr]
Vector 27 [unity_SHBg]
Vector 28 [unity_SHBb]
Vector 29 [unity_SHC]
"!!ARBvp1.0
# 71 ALU
PARAM c[30] = { { 1, 2, 0 },
		state.matrix.mvp,
		program.local[5..29] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
MUL R3.xyz, vertex.normal, c[13].w;
DP3 R5.x, R3, c[5];
DP4 R4.zw, vertex.position, c[6];
ADD R2, -R4.z, c[16];
DP3 R4.z, R3, c[6];
DP4 R3.w, vertex.position, c[5];
MUL R0, R4.z, R2;
ADD R1, -R3.w, c[15];
MUL R2, R2, R2;
MOV R5.y, R4.z;
MOV R5.w, c[0].x;
DP4 R4.xy, vertex.position, c[7];
MAD R0, R5.x, R1, R0;
MAD R2, R1, R1, R2;
ADD R1, -R4.x, c[17];
DP3 R4.x, R3, c[7];
MAD R2, R1, R1, R2;
MAD R0, R4.x, R1, R0;
MUL R1, R2, c[18];
ADD R1, R1, c[0].x;
MOV R5.z, R4.x;
RSQ R2.x, R2.x;
RSQ R2.y, R2.y;
RSQ R2.z, R2.z;
RSQ R2.w, R2.w;
MUL R0, R0, R2;
DP4 R2.z, R5, c[25];
DP4 R2.y, R5, c[24];
DP4 R2.x, R5, c[23];
RCP R1.x, R1.x;
RCP R1.y, R1.y;
RCP R1.w, R1.w;
RCP R1.z, R1.z;
MAX R0, R0, c[0].z;
MUL R0, R0, R1;
MUL R1.xyz, R0.y, c[20];
MAD R1.xyz, R0.x, c[19], R1;
MAD R0.xyz, R0.z, c[21], R1;
MUL R1, R5.xyzz, R5.yzzx;
MAD R0.xyz, R0.w, c[22], R0;
DP4 R3.z, R1, c[28];
DP4 R3.x, R1, c[26];
DP4 R3.y, R1, c[27];
ADD R3.xyz, R2, R3;
MOV R1.w, c[0].x;
MOV R1.xyz, c[14];
DP4 R2.z, R1, c[11];
DP4 R2.y, R1, c[10];
DP4 R2.x, R1, c[9];
MUL R0.w, R4.z, R4.z;
MAD R1.w, R5.x, R5.x, -R0;
MAD R1.xyz, R2, c[13].w, -vertex.position;
DP3 R0.w, vertex.normal, -R1;
MUL R5.yzw, R1.w, c[29].xxyz;
ADD R3.xyz, R3, R5.yzww;
ADD result.texcoord[3].xyz, R3, R0;
MUL R2.xyz, vertex.normal, R0.w;
MAD R1.xyz, -R2, c[0].y, -R1;
MOV R3.x, R4.w;
MOV R3.y, R4;
DP3 result.texcoord[0].z, R1, c[7];
DP3 result.texcoord[0].y, R1, c[6];
DP3 result.texcoord[0].x, R1, c[5];
ADD result.texcoord[1].xyz, -R3.wxyw, c[14];
MOV result.texcoord[2].z, R4.x;
MOV result.texcoord[2].y, R4.z;
MOV result.texcoord[2].x, R5;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 71 instructions, 6 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_4LightPosX0]
Vector 15 [unity_4LightPosY0]
Vector 16 [unity_4LightPosZ0]
Vector 17 [unity_4LightAtten0]
Vector 18 [unity_LightColor0]
Vector 19 [unity_LightColor1]
Vector 20 [unity_LightColor2]
Vector 21 [unity_LightColor3]
Vector 22 [unity_SHAr]
Vector 23 [unity_SHAg]
Vector 24 [unity_SHAb]
Vector 25 [unity_SHBr]
Vector 26 [unity_SHBg]
Vector 27 [unity_SHBb]
Vector 28 [unity_SHC]
"vs_2_0
; 71 ALU
def c29, 1.00000000, 2.00000000, 0.00000000, 0
dcl_position0 v0
dcl_normal0 v1
mul r3.xyz, v1, c12.w
dp3 r5.x, r3, c4
dp4 r4.zw, v0, c5
add r2, -r4.z, c15
dp3 r4.z, r3, c5
dp4 r3.w, v0, c4
mul r0, r4.z, r2
add r1, -r3.w, c14
mul r2, r2, r2
mov r5.y, r4.z
mov r5.w, c29.x
dp4 r4.xy, v0, c6
mad r0, r5.x, r1, r0
mad r2, r1, r1, r2
add r1, -r4.x, c16
dp3 r4.x, r3, c6
mad r2, r1, r1, r2
mad r0, r4.x, r1, r0
mul r1, r2, c17
add r1, r1, c29.x
mov r5.z, r4.x
rsq r2.x, r2.x
rsq r2.y, r2.y
rsq r2.z, r2.z
rsq r2.w, r2.w
mul r0, r0, r2
dp4 r2.z, r5, c24
dp4 r2.y, r5, c23
dp4 r2.x, r5, c22
rcp r1.x, r1.x
rcp r1.y, r1.y
rcp r1.w, r1.w
rcp r1.z, r1.z
max r0, r0, c29.z
mul r0, r0, r1
mul r1.xyz, r0.y, c19
mad r1.xyz, r0.x, c18, r1
mad r0.xyz, r0.z, c20, r1
mul r1, r5.xyzz, r5.yzzx
mad r0.xyz, r0.w, c21, r0
dp4 r3.z, r1, c27
dp4 r3.x, r1, c25
dp4 r3.y, r1, c26
add r3.xyz, r2, r3
mov r1.w, c29.x
mov r1.xyz, c13
dp4 r2.z, r1, c10
dp4 r2.y, r1, c9
dp4 r2.x, r1, c8
mul r0.w, r4.z, r4.z
mad r1.w, r5.x, r5.x, -r0
mad r1.xyz, r2, c12.w, -v0
dp3 r0.w, v1, -r1
mul r5.yzw, r1.w, c28.xxyz
add r3.xyz, r3, r5.yzww
add oT3.xyz, r3, r0
mul r2.xyz, v1, r0.w
mad r1.xyz, -r2, c29.y, -r1
mov r3.x, r4.w
mov r3.y, r4
dp3 oT0.z, r1, c6
dp3 oT0.y, r1, c5
dp3 oT0.x, r1, c4
add oT1.xyz, -r3.wxyw, c13
mov oT2.z, r4.x
mov oT2.y, r4.z
mov oT2.x, r5
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_LightColor[4];
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightAtten0;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  shlight = tmpvar_11;
  tmpvar_4 = shlight;
  highp vec3 tmpvar_21;
  tmpvar_21 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_22;
  tmpvar_22 = (unity_4LightPosX0 - tmpvar_21.x);
  highp vec4 tmpvar_23;
  tmpvar_23 = (unity_4LightPosY0 - tmpvar_21.y);
  highp vec4 tmpvar_24;
  tmpvar_24 = (unity_4LightPosZ0 - tmpvar_21.z);
  highp vec4 tmpvar_25;
  tmpvar_25 = (((tmpvar_22 * tmpvar_22) + (tmpvar_23 * tmpvar_23)) + (tmpvar_24 * tmpvar_24));
  highp vec4 tmpvar_26;
  tmpvar_26 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_22 * tmpvar_9.x) + (tmpvar_23 * tmpvar_9.y)) + (tmpvar_24 * tmpvar_9.z)) * inversesqrt (tmpvar_25))) * (1.0/((1.0 + (tmpvar_25 * unity_4LightAtten0)))));
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_4 + ((((unity_LightColor[0].xyz * tmpvar_26.x) + (unity_LightColor[1].xyz * tmpvar_26.y)) + (unity_LightColor[2].xyz * tmpvar_26.z)) + (unity_LightColor[3].xyz * tmpvar_26.w)));
  tmpvar_4 = tmpvar_27;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_3 * xlv_TEXCOORD3));
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;
uniform highp vec4 unity_SHC;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_LightColor[4];
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightAtten0;

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  highp vec3 shlight;
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal;
  normal = tmpvar_10;
  mediump vec3 x3;
  highp float vC;
  mediump vec3 x2;
  mediump vec3 x1;
  highp float tmpvar_12;
  tmpvar_12 = dot (unity_SHAr, normal);
  x1.x = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = dot (unity_SHAg, normal);
  x1.y = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = dot (unity_SHAb, normal);
  x1.z = tmpvar_14;
  mediump vec4 tmpvar_15;
  tmpvar_15 = (normal.xyzz * normal.yzzx);
  highp float tmpvar_16;
  tmpvar_16 = dot (unity_SHBr, tmpvar_15);
  x2.x = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (unity_SHBg, tmpvar_15);
  x2.y = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (unity_SHBb, tmpvar_15);
  x2.z = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = ((normal.x * normal.x) - (normal.y * normal.y));
  vC = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (unity_SHC.xyz * vC);
  x3 = tmpvar_20;
  tmpvar_11 = ((x1 + x2) + x3);
  shlight = tmpvar_11;
  tmpvar_4 = shlight;
  highp vec3 tmpvar_21;
  tmpvar_21 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_22;
  tmpvar_22 = (unity_4LightPosX0 - tmpvar_21.x);
  highp vec4 tmpvar_23;
  tmpvar_23 = (unity_4LightPosY0 - tmpvar_21.y);
  highp vec4 tmpvar_24;
  tmpvar_24 = (unity_4LightPosZ0 - tmpvar_21.z);
  highp vec4 tmpvar_25;
  tmpvar_25 = (((tmpvar_22 * tmpvar_22) + (tmpvar_23 * tmpvar_23)) + (tmpvar_24 * tmpvar_24));
  highp vec4 tmpvar_26;
  tmpvar_26 = (max (vec4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_22 * tmpvar_9.x) + (tmpvar_23 * tmpvar_9.y)) + (tmpvar_24 * tmpvar_9.z)) * inversesqrt (tmpvar_25))) * (1.0/((1.0 + (tmpvar_25 * unity_4LightAtten0)))));
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_4 + ((((unity_LightColor[0].xyz * tmpvar_26.x) + (unity_LightColor[1].xyz * tmpvar_26.y)) + (unity_LightColor[2].xyz * tmpvar_26.z)) + (unity_LightColor[3].xyz * tmpvar_26.w)));
  tmpvar_4 = tmpvar_27;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, _WorldSpaceLightPos0.xyz)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.xyz = (c_i0_i1.xyz + (tmpvar_3 * xlv_TEXCOORD3));
  c.xyz = (c.xyz + (tmpvar_3 * 0.25));
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 14 [unity_4LightPosX0]
Vector 15 [unity_4LightPosY0]
Vector 16 [unity_4LightPosZ0]
Vector 17 [unity_4LightAtten0]
Vector 18 [unity_LightColor0]
Vector 19 [unity_LightColor1]
Vector 20 [unity_LightColor2]
Vector 21 [unity_LightColor3]
Vector 22 [unity_SHAr]
Vector 23 [unity_SHAg]
Vector 24 [unity_SHAb]
Vector 25 [unity_SHBr]
Vector 26 [unity_SHBg]
Vector 27 [unity_SHBb]
Vector 28 [unity_SHC]
"agal_vs
c29 1.0 2.0 0.0 0.0
[bc]
adaaaaaaadaaahacabaaaaoeaaaaaaaaamaaaappabaaaaaa mul r3.xyz, a1, c12.w
bcaaaaaaafaaabacadaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 r5.x, r3.xyzz, c4
bdaaaaaaaeaaamacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r4.zw, a0, c5
bfaaaaaaacaaaeacaeaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r2.z, r4.z
abaaaaaaacaaapacacaaaakkacaaaaaaapaaaaoeabaaaaaa add r2, r2.z, c15
bcaaaaaaaeaaaeacadaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 r4.z, r3.xyzz, c5
bdaaaaaaadaaaiacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r3.w, a0, c4
adaaaaaaaaaaapacaeaaaakkacaaaaaaacaaaaoeacaaaaaa mul r0, r4.z, r2
bfaaaaaaabaaaiacadaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r1.w, r3.w
abaaaaaaabaaapacabaaaappacaaaaaaaoaaaaoeabaaaaaa add r1, r1.w, c14
adaaaaaaacaaapacacaaaaoeacaaaaaaacaaaaoeacaaaaaa mul r2, r2, r2
aaaaaaaaafaaacacaeaaaakkacaaaaaaaaaaaaaaaaaaaaaa mov r5.y, r4.z
aaaaaaaaafaaaiacbnaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r5.w, c29.x
bdaaaaaaaeaaadacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r4.xy, a0, c6
adaaaaaaagaaapacafaaaaaaacaaaaaaabaaaaoeacaaaaaa mul r6, r5.x, r1
abaaaaaaaaaaapacagaaaaoeacaaaaaaaaaaaaoeacaaaaaa add r0, r6, r0
adaaaaaaagaaapacabaaaaoeacaaaaaaabaaaaoeacaaaaaa mul r6, r1, r1
abaaaaaaacaaapacagaaaaoeacaaaaaaacaaaaoeacaaaaaa add r2, r6, r2
bfaaaaaaabaaabacaeaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r1.x, r4.x
abaaaaaaabaaapacabaaaaaaacaaaaaabaaaaaoeabaaaaaa add r1, r1.x, c16
bcaaaaaaaeaaabacadaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 r4.x, r3.xyzz, c6
adaaaaaaagaaapacabaaaaoeacaaaaaaabaaaaoeacaaaaaa mul r6, r1, r1
abaaaaaaacaaapacagaaaaoeacaaaaaaacaaaaoeacaaaaaa add r2, r6, r2
adaaaaaaagaaapacaeaaaaaaacaaaaaaabaaaaoeacaaaaaa mul r6, r4.x, r1
abaaaaaaaaaaapacagaaaaoeacaaaaaaaaaaaaoeacaaaaaa add r0, r6, r0
adaaaaaaabaaapacacaaaaoeacaaaaaabbaaaaoeabaaaaaa mul r1, r2, c17
abaaaaaaabaaapacabaaaaoeacaaaaaabnaaaaaaabaaaaaa add r1, r1, c29.x
aaaaaaaaafaaaeacaeaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r5.z, r4.x
akaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r2.x
akaaaaaaacaaacacacaaaaffacaaaaaaaaaaaaaaaaaaaaaa rsq r2.y, r2.y
akaaaaaaacaaaeacacaaaakkacaaaaaaaaaaaaaaaaaaaaaa rsq r2.z, r2.z
akaaaaaaacaaaiacacaaaappacaaaaaaaaaaaaaaaaaaaaaa rsq r2.w, r2.w
adaaaaaaaaaaapacaaaaaaoeacaaaaaaacaaaaoeacaaaaaa mul r0, r0, r2
bdaaaaaaacaaaeacafaaaaoeacaaaaaabiaaaaoeabaaaaaa dp4 r2.z, r5, c24
bdaaaaaaacaaacacafaaaaoeacaaaaaabhaaaaoeabaaaaaa dp4 r2.y, r5, c23
bdaaaaaaacaaabacafaaaaoeacaaaaaabgaaaaoeabaaaaaa dp4 r2.x, r5, c22
afaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rcp r1.x, r1.x
afaaaaaaabaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa rcp r1.y, r1.y
afaaaaaaabaaaiacabaaaappacaaaaaaaaaaaaaaaaaaaaaa rcp r1.w, r1.w
afaaaaaaabaaaeacabaaaakkacaaaaaaaaaaaaaaaaaaaaaa rcp r1.z, r1.z
ahaaaaaaaaaaapacaaaaaaoeacaaaaaabnaaaakkabaaaaaa max r0, r0, c29.z
adaaaaaaaaaaapacaaaaaaoeacaaaaaaabaaaaoeacaaaaaa mul r0, r0, r1
adaaaaaaabaaahacaaaaaaffacaaaaaabdaaaaoeabaaaaaa mul r1.xyz, r0.y, c19
adaaaaaaagaaahacaaaaaaaaacaaaaaabcaaaaoeabaaaaaa mul r6.xyz, r0.x, c18
abaaaaaaabaaahacagaaaakeacaaaaaaabaaaakeacaaaaaa add r1.xyz, r6.xyzz, r1.xyzz
adaaaaaaaaaaahacaaaaaakkacaaaaaabeaaaaoeabaaaaaa mul r0.xyz, r0.z, c20
abaaaaaaaaaaahacaaaaaakeacaaaaaaabaaaakeacaaaaaa add r0.xyz, r0.xyzz, r1.xyzz
adaaaaaaabaaapacafaaaakeacaaaaaaafaaaacjacaaaaaa mul r1, r5.xyzz, r5.yzzx
adaaaaaaagaaahacaaaaaappacaaaaaabfaaaaoeabaaaaaa mul r6.xyz, r0.w, c21
abaaaaaaaaaaahacagaaaakeacaaaaaaaaaaaakeacaaaaaa add r0.xyz, r6.xyzz, r0.xyzz
bdaaaaaaadaaaeacabaaaaoeacaaaaaablaaaaoeabaaaaaa dp4 r3.z, r1, c27
bdaaaaaaadaaabacabaaaaoeacaaaaaabjaaaaoeabaaaaaa dp4 r3.x, r1, c25
bdaaaaaaadaaacacabaaaaoeacaaaaaabkaaaaoeabaaaaaa dp4 r3.y, r1, c26
abaaaaaaadaaahacacaaaakeacaaaaaaadaaaakeacaaaaaa add r3.xyz, r2.xyzz, r3.xyzz
aaaaaaaaabaaaiacbnaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c29.x
aaaaaaaaabaaahacanaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c13
bdaaaaaaacaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r2.z, r1, c10
bdaaaaaaacaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r2.y, r1, c9
bdaaaaaaacaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r2.x, r1, c8
adaaaaaaaaaaaiacaeaaaakkacaaaaaaaeaaaakkacaaaaaa mul r0.w, r4.z, r4.z
adaaaaaaagaaaiacafaaaaaaacaaaaaaafaaaaaaacaaaaaa mul r6.w, r5.x, r5.x
acaaaaaaabaaaiacagaaaappacaaaaaaaaaaaappacaaaaaa sub r1.w, r6.w, r0.w
adaaaaaaagaaahacacaaaakeacaaaaaaamaaaappabaaaaaa mul r6.xyz, r2.xyzz, c12.w
acaaaaaaabaaahacagaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r1.xyz, r6.xyzz, a0
bfaaaaaaagaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r6.xyz, r1.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaagaaaakeacaaaaaa dp3 r0.w, a1, r6.xyzz
adaaaaaaafaaaoacabaaaappacaaaaaabmaaaajaabaaaaaa mul r5.yzw, r1.w, c28.xxyz
abaaaaaaadaaahacadaaaakeacaaaaaaafaaaapjacaaaaaa add r3.xyz, r3.xyzz, r5.yzww
abaaaaaaadaaahaeadaaaakeacaaaaaaaaaaaakeacaaaaaa add v3.xyz, r3.xyzz, r0.xyzz
adaaaaaaacaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r2.xyz, a1, r0.w
bfaaaaaaagaaahacacaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r6.xyz, r2.xyzz
adaaaaaaagaaahacagaaaakeacaaaaaabnaaaaffabaaaaaa mul r6.xyz, r6.xyzz, c29.y
acaaaaaaabaaahacagaaaakeacaaaaaaabaaaakeacaaaaaa sub r1.xyz, r6.xyzz, r1.xyzz
aaaaaaaaadaaabacaeaaaappacaaaaaaaaaaaaaaaaaaaaaa mov r3.x, r4.w
aaaaaaaaadaaacacaeaaaaffacaaaaaaaaaaaaaaaaaaaaaa mov r3.y, r4.y
bcaaaaaaaaaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r1.xyzz, c6
bcaaaaaaaaaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r1.xyzz, c5
bcaaaaaaaaaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r1.xyzz, c4
bfaaaaaaagaaalacadaaaapdacaaaaaaaaaaaaaaaaaaaaaa neg r6.xyw, r3.wxww
abaaaaaaabaaahaeagaaaafdacaaaaaaanaaaaoeabaaaaaa add v1.xyz, r6.wxyy, c13
aaaaaaaaacaaaeaeaeaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov v2.z, r4.x
aaaaaaaaacaaacaeaeaaaakkacaaaaaaaaaaaaaaaaaaaaaa mov v2.y, r4.z
aaaaaaaaacaaabaeafaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov v2.x, r5.x
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
"
}

}
Program "fp" {
// Fragment combos: 3
//   opengl - ALU: 20 to 22, TEX: 1 to 2
//   d3d9 - ALU: 22 to 25, TEX: 1 to 2
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Vector 3 [_ReflectColor]
Float 4 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 22 ALU, 1 TEX
PARAM c[7] = { program.local[0..4],
		{ 0, 1, 0.79638672, 0.20373535 },
		{ 0.25, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, fragment.texcoord[0], texture[0], CUBE;
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
DP3 R0.w, fragment.texcoord[1], fragment.texcoord[1];
DP3 R1.w, fragment.texcoord[2], c[0];
MUL R2.xyz, R1.x, fragment.texcoord[2];
RSQ R0.w, R0.w;
MUL R1.xyz, R0.w, fragment.texcoord[1];
DP3 R0.w, R1, R2;
MAX R0.w, R0, c[5].x;
MOV R1.xyz, c[2];
MAD R1.xyz, R0, c[3], R1;
ADD_SAT R0.w, -R0, c[5].y;
POW R0.w, R0.w, c[4].x;
MAD R0.w, R0, c[5].z, c[5];
MUL R2.xyz, R1, fragment.texcoord[3];
MUL R0.xyz, R1, c[1];
MAX R1.w, R1, c[5].x;
MUL R0.xyz, R1.w, R0;
MAD R0.xyz, R0, c[6].y, R2;
MAD result.color.xyz, R1, c[6].x, R0;
MAX result.color.w, R0, c[5].x;
END
# 22 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Vector 3 [_ReflectColor]
Float 4 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"ps_2_0
; 25 ALU, 1 TEX
dcl_cube s0
def c5, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c6, 2.00000000, 0.25000000, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
texld r2, t0, s0
dp3_pp r1.x, t2, t2
dp3_pp r0.x, t1, t1
rsq_pp r1.x, r1.x
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
mul_pp r1.xyz, r1.x, t2
dp3_pp r0.x, r0, r1
max_pp r0.x, r0, c5
add_pp_sat r0.x, -r0, c5.y
pow_pp r1.w, r0.x, c4.x
mov_pp r0.xyz, c2
mad_pp r2.xyz, r2, c3, r0
mov_pp r0.x, r1.w
dp3_pp r1.x, t2, c0
mad_pp r0.x, r0, c5.z, c5.w
mul_pp r4.xyz, r2, t3
mul_pp r3.xyz, r2, c1
max_pp r1.x, r1, c5
mul_pp r1.xyz, r1.x, r3
mad_pp r1.xyz, r1, c6.x, r4
mad_pp r1.xyz, r2, c6.y, r1
max_pp r1.w, r0.x, c5.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Vector 3 [_ReflectColor]
Float 4 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"agal_ps
c5 0.0 1.0 0.796387 0.203735
c6 2.0 0.25 0.0 0.0
[bc]
ciaaaaaaacaaapacaaaaaaoeaeaaaaaaaaaaaaaaafbababb tex r2, v0, s0 <cube wrap linear point>
bcaaaaaaabaaabacacaaaaoeaeaaaaaaacaaaaoeaeaaaaaa dp3 r1.x, v2, v2
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaaoeaeaaaaaa mul r1.xyz, r1.x, v2
bcaaaaaaaaaaabacaaaaaakeacaaaaaaabaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r1.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaafaaaaoeabaaaaaa max r0.x, r0.x, c5
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaafaaaaffabaaaaaa add r0.x, r0.x, c5.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaabaaapacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa pow r1, r0.x, c4.x
aaaaaaaaaaaaahacacaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.xyz, c2
adaaaaaaacaaahacacaaaakeacaaaaaaadaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c3
abaaaaaaacaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa add r2.xyz, r2.xyzz, r0.xyzz
aaaaaaaaaaaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r1.x
bcaaaaaaabaaabacacaaaaoeaeaaaaaaaaaaaaoeabaaaaaa dp3 r1.x, v2, c0
adaaaaaaaaaaabacaaaaaaaaacaaaaaaafaaaakkabaaaaaa mul r0.x, r0.x, c5.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaafaaaappabaaaaaa add r0.x, r0.x, c5.w
adaaaaaaaeaaahacacaaaakeacaaaaaaadaaaaoeaeaaaaaa mul r4.xyz, r2.xyzz, v3
adaaaaaaadaaahacacaaaakeacaaaaaaabaaaaoeabaaaaaa mul r3.xyz, r2.xyzz, c1
ahaaaaaaabaaabacabaaaaaaacaaaaaaafaaaaoeabaaaaaa max r1.x, r1.x, c5
adaaaaaaabaaahacabaaaaaaacaaaaaaadaaaakeacaaaaaa mul r1.xyz, r1.x, r3.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaagaaaaaaabaaaaaa mul r1.xyz, r1.xyzz, c6.x
abaaaaaaabaaahacabaaaakeacaaaaaaaeaaaakeacaaaaaa add r1.xyz, r1.xyzz, r4.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaaagaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c6.y
abaaaaaaabaaahacacaaaakeacaaaaaaabaaaakeacaaaaaa add r1.xyz, r2.xyzz, r1.xyzz
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaafaaaaaaabaaaaaa max r1.w, r0.x, c5.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 20 ALU, 2 TEX
PARAM c[5] = { program.local[0..2],
		{ 0, 1, 0.79638672, 0.20373535 },
		{ 8, 0.25 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0, fragment.texcoord[3], texture[1], 2D;
TEX R1.xyz, fragment.texcoord[0], texture[0], CUBE;
DP3 R2.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R2.x, R2.x;
DP3 R1.w, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.w, R1.w;
MUL R3.xyz, R2.x, fragment.texcoord[2];
MUL R2.xyz, R1.w, fragment.texcoord[1];
DP3 R1.w, R2, R3;
MAX R1.w, R1, c[3].x;
MOV R2.xyz, c[0];
MAD R1.xyz, R1, c[1], R2;
MUL R0.xyz, R0.w, R0;
MUL R0.xyz, R0, R1;
ADD_SAT R1.w, -R1, c[3].y;
POW R0.w, R1.w, c[2].x;
MUL R1.xyz, R1, c[4].y;
MAD R0.w, R0, c[3].z, c[3];
MAD result.color.xyz, R0, c[4].x, R1;
MAX result.color.w, R0, c[3].x;
END
# 20 instructions, 4 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"ps_2_0
; 22 ALU, 2 TEX
dcl_cube s0
dcl_2d s1
def c3, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c4, 0.25000000, 8.00000000, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xy
texld r3, t0, s0
texld r1, t3, s1
dp3_pp r2.x, t2, t2
dp3_pp r0.x, t1, t1
rsq_pp r2.x, r2.x
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
mul_pp r2.xyz, r2.x, t2
dp3_pp r0.x, r0, r2
max_pp r0.x, r0, c3
add_pp_sat r0.x, -r0, c3.y
pow_pp r2.w, r0.x, c2.x
mov_pp r0.xyz, c0
mad_pp r3.xyz, r3, c1, r0
mul_pp r0.xyz, r1.w, r1
mul_pp r1.xyz, r0, r3
mov_pp r0.x, r2.w
mul_pp r2.xyz, r3, c4.x
mad_pp r0.x, r0, c3.z, c3.w
mad_pp r1.xyz, r1, c4.y, r2
max_pp r1.w, r0.x, c3.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"agal_ps
c3 0.0 1.0 0.796387 0.203735
c4 0.25 8.0 0.0 0.0
[bc]
ciaaaaaaadaaapacaaaaaaoeaeaaaaaaaaaaaaaaafbababb tex r3, v0, s0 <cube wrap linear point>
ciaaaaaaabaaapacadaaaaoeaeaaaaaaabaaaaaaafaababb tex r1, v3, s1 <2d wrap linear point>
bcaaaaaaacaaabacacaaaaoeaeaaaaaaacaaaaoeaeaaaaaa dp3 r2.x, v2, v2
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r2.x
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
adaaaaaaacaaahacacaaaaaaacaaaaaaacaaaaoeaeaaaaaa mul r2.xyz, r2.x, v2
bcaaaaaaaaaaabacaaaaaakeacaaaaaaacaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r2.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaaoeabaaaaaa max r0.x, r0.x, c3
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaaffabaaaaaa add r0.x, r0.x, c3.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaacaaapacaaaaaaaaacaaaaaaacaaaaaaabaaaaaa pow r2, r0.x, c2.x
aaaaaaaaaaaaahacaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.xyz, c0
adaaaaaaadaaahacadaaaakeacaaaaaaabaaaaoeabaaaaaa mul r3.xyz, r3.xyzz, c1
abaaaaaaadaaahacadaaaakeacaaaaaaaaaaaakeacaaaaaa add r3.xyz, r3.xyzz, r0.xyzz
adaaaaaaaaaaahacabaaaappacaaaaaaabaaaakeacaaaaaa mul r0.xyz, r1.w, r1.xyzz
adaaaaaaabaaahacaaaaaakeacaaaaaaadaaaakeacaaaaaa mul r1.xyz, r0.xyzz, r3.xyzz
aaaaaaaaaaaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r2.x
adaaaaaaacaaahacadaaaakeacaaaaaaaeaaaaaaabaaaaaa mul r2.xyz, r3.xyzz, c4.x
adaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaakkabaaaaaa mul r0.x, r0.x, c3.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaappabaaaaaa add r0.x, r0.x, c3.w
adaaaaaaabaaahacabaaaakeacaaaaaaaeaaaaffabaaaaaa mul r1.xyz, r1.xyzz, c4.y
abaaaaaaabaaahacabaaaakeacaaaaaaacaaaakeacaaaaaa add r1.xyz, r1.xyzz, r2.xyzz
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa max r1.w, r0.x, c3.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 20 ALU, 2 TEX
PARAM c[5] = { program.local[0..2],
		{ 0, 1, 0.79638672, 0.20373535 },
		{ 8, 0.25 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0, fragment.texcoord[3], texture[1], 2D;
TEX R1.xyz, fragment.texcoord[0], texture[0], CUBE;
DP3 R2.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R2.x, R2.x;
DP3 R1.w, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.w, R1.w;
MUL R3.xyz, R2.x, fragment.texcoord[2];
MUL R2.xyz, R1.w, fragment.texcoord[1];
DP3 R1.w, R2, R3;
MAX R1.w, R1, c[3].x;
MOV R2.xyz, c[0];
MAD R1.xyz, R1, c[1], R2;
MUL R0.xyz, R0.w, R0;
MUL R0.xyz, R0, R1;
ADD_SAT R1.w, -R1, c[3].y;
POW R0.w, R1.w, c[2].x;
MUL R1.xyz, R1, c[4].y;
MAD R0.w, R0, c[3].z, c[3];
MAD result.color.xyz, R0, c[4].x, R1;
MAX result.color.w, R0, c[3].x;
END
# 20 instructions, 4 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"ps_2_0
; 22 ALU, 2 TEX
dcl_cube s0
dcl_2d s1
def c3, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c4, 0.25000000, 8.00000000, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xy
texld r3, t0, s0
texld r1, t3, s1
dp3_pp r2.x, t2, t2
dp3_pp r0.x, t1, t1
rsq_pp r2.x, r2.x
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
mul_pp r2.xyz, r2.x, t2
dp3_pp r0.x, r0, r2
max_pp r0.x, r0, c3
add_pp_sat r0.x, -r0, c3.y
pow_pp r2.w, r0.x, c2.x
mov_pp r0.xyz, c0
mad_pp r3.xyz, r3, c1, r0
mul_pp r0.xyz, r1.w, r1
mul_pp r1.xyz, r0, r3
mov_pp r0.x, r2.w
mul_pp r2.xyz, r3, c4.x
mad_pp r0.x, r0, c3.z, c3.w
mad_pp r1.xyz, r1, c4.y, r2
max_pp r1.w, r0.x, c3.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
Vector 0 [_Color]
Vector 1 [_ReflectColor]
Float 2 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [unity_Lightmap] 2D
"agal_ps
c3 0.0 1.0 0.796387 0.203735
c4 0.25 8.0 0.0 0.0
[bc]
ciaaaaaaadaaapacaaaaaaoeaeaaaaaaaaaaaaaaafbababb tex r3, v0, s0 <cube wrap linear point>
ciaaaaaaabaaapacadaaaaoeaeaaaaaaabaaaaaaafaababb tex r1, v3, s1 <2d wrap linear point>
bcaaaaaaacaaabacacaaaaoeaeaaaaaaacaaaaoeaeaaaaaa dp3 r2.x, v2, v2
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r2.x
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
adaaaaaaacaaahacacaaaaaaacaaaaaaacaaaaoeaeaaaaaa mul r2.xyz, r2.x, v2
bcaaaaaaaaaaabacaaaaaakeacaaaaaaacaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r2.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaaoeabaaaaaa max r0.x, r0.x, c3
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaaffabaaaaaa add r0.x, r0.x, c3.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaacaaapacaaaaaaaaacaaaaaaacaaaaaaabaaaaaa pow r2, r0.x, c2.x
aaaaaaaaaaaaahacaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.xyz, c0
adaaaaaaadaaahacadaaaakeacaaaaaaabaaaaoeabaaaaaa mul r3.xyz, r3.xyzz, c1
abaaaaaaadaaahacadaaaakeacaaaaaaaaaaaakeacaaaaaa add r3.xyz, r3.xyzz, r0.xyzz
adaaaaaaaaaaahacabaaaappacaaaaaaabaaaakeacaaaaaa mul r0.xyz, r1.w, r1.xyzz
adaaaaaaabaaahacaaaaaakeacaaaaaaadaaaakeacaaaaaa mul r1.xyz, r0.xyzz, r3.xyzz
aaaaaaaaaaaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r2.x
adaaaaaaacaaahacadaaaakeacaaaaaaaeaaaaaaabaaaaaa mul r2.xyz, r3.xyzz, c4.x
adaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaakkabaaaaaa mul r0.x, r0.x, c3.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaadaaaappabaaaaaa add r0.x, r0.x, c3.w
adaaaaaaabaaahacabaaaakeacaaaaaaaeaaaaffabaaaaaa mul r1.xyz, r1.xyzz, c4.y
abaaaaaaabaaahacabaaaakeacaaaaaaacaaaakeacaaaaaa add r1.xyz, r1.xyzz, r2.xyzz
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa max r1.w, r0.x, c3.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

}
	}
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardAdd" }
		ZWrite Off Blend One One Fog { Color (0,0,0,0) }
		Blend SrcAlpha One
Program "vp" {
// Vertex combos: 5
//   opengl - ALU: 25 to 30
//   d3d9 - ALU: 25 to 30
SubProgram "opengl " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
"!!ARBvp1.0
# 29 ALU
PARAM c[20] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[18];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[17].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[17].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[4].z, R0, c[15];
DP4 result.texcoord[4].y, R0, c[14];
DP4 result.texcoord[4].x, R0, c[13];
ADD result.texcoord[1].xyz, -R0, c[18];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
ADD result.texcoord[3].xyz, -R0, c[19];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 29 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"vs_2_0
; 29 ALU
def c19, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1.xyz, c17
mov r1.w, c19.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c16.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c19.y, -r0
mul r1.xyz, v1, c16.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 oT4.z, r0, c14
dp4 oT4.y, r0, c13
dp4 oT4.x, r0, c12
add oT1.xyz, -r0, c17
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
add oT3.xyz, -r0, c18
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "POINT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = vec2(dot (xlv_TEXCOORD4, xlv_TEXCOORD4));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * texture2D (_LightTexture0, tmpvar_9).w) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "POINT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = vec2(dot (xlv_TEXCOORD4, xlv_TEXCOORD4));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * texture2D (_LightTexture0, tmpvar_9).w) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"agal_vs
c19 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacbbaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c17
aaaaaaaaabaaaiacbdaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c19.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaabaaaaappabaaaaaa mul r2.xyz, r0.xyzz, c16.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaabdaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c19.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaabaaaaappabaaaaaa mul r1.xyz, a1, c16.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bdaaaaaaaaaaaiacaaaaaaoeaaaaaaaaahaaaaoeabaaaaaa dp4 r0.w, a0, c7
bdaaaaaaaeaaaeaeaaaaaaoeacaaaaaaaoaaaaoeabaaaaaa dp4 v4.z, r0, c14
bdaaaaaaaeaaacaeaaaaaaoeacaaaaaaanaaaaoeabaaaaaa dp4 v4.y, r0, c13
bdaaaaaaaeaaabaeaaaaaaoeacaaaaaaamaaaaoeabaaaaaa dp4 v4.x, r0, c12
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaabbaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c17
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaadaaahaeacaaaakeacaaaaaabcaaaaoeabaaaaaa add v3.xyz, r2.xyzz, c18
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
aaaaaaaaaeaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v4.w, c0
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Vector 15 [_WorldSpaceLightPos0]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
"!!ARBvp1.0
# 25 ALU
PARAM c[16] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..15] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[14];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[13].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[13].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
ADD result.texcoord[1].xyz, -R0, c[14];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MOV result.texcoord[3].xyz, c[15];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 25 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Vector 14 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
"vs_2_0
; 25 ALU
def c15, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1.xyz, c13
mov r1.w, c15.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c12.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c15.y, -r0
mul r1.xyz, v1, c12.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
add oT1.xyz, -r0, c13
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mov oT3.xyz, c14
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, lightDir)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * (max (0.0, dot (xlv_TEXCOORD2, lightDir)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceCameraPos]
Vector 14 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
"agal_vs
c15 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacanaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c13
aaaaaaaaabaaaiacapaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c15.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaaamaaaappabaaaaaa mul r2.xyz, r0.xyzz, c12.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaaapaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c15.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaamaaaappabaaaaaa mul r1.xyz, a1, c12.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaaanaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c13
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
aaaaaaaaadaaahaeaoaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.xyz, c14
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
"
}

SubProgram "opengl " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
"!!ARBvp1.0
# 30 ALU
PARAM c[20] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[18];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[17].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[17].w;
DP4 R0.w, vertex.position, c[8];
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 result.texcoord[4].w, R0, c[16];
DP4 result.texcoord[4].z, R0, c[15];
DP4 result.texcoord[4].y, R0, c[14];
DP4 result.texcoord[4].x, R0, c[13];
ADD result.texcoord[1].xyz, -R0, c[18];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
ADD result.texcoord[3].xyz, -R0, c[19];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 30 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"vs_2_0
; 30 ALU
def c19, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1.xyz, c17
mov r1.w, c19.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c16.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c19.y, -r0
mul r1.xyz, v1, c16.w
dp4 r0.w, v0, c7
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 oT4.w, r0, c15
dp4 oT4.z, r0, c14
dp4 oT4.y, r0, c13
dp4 oT4.x, r0, c12
add oT1.xyz, -r0, c17
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
add oT3.xyz, -r0, c18
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_8;
  highp vec3 LightCoord_i0;
  LightCoord_i0 = xlv_TEXCOORD4.xyz;
  highp vec2 tmpvar_9;
  tmpvar_9 = vec2(dot (LightCoord_i0, LightCoord_i0));
  lowp float atten;
  atten = ((float((xlv_TEXCOORD4.z > 0.0)) * texture2D (_LightTexture0, ((xlv_TEXCOORD4.xy / xlv_TEXCOORD4.w) + 0.5)).w) * texture2D (_LightTextureB0, tmpvar_9).w);
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * atten) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "SPOT" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec4 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_8;
  highp vec3 LightCoord_i0;
  LightCoord_i0 = xlv_TEXCOORD4.xyz;
  highp vec2 tmpvar_9;
  tmpvar_9 = vec2(dot (LightCoord_i0, LightCoord_i0));
  lowp float atten;
  atten = ((float((xlv_TEXCOORD4.z > 0.0)) * texture2D (_LightTexture0, ((xlv_TEXCOORD4.xy / xlv_TEXCOORD4.w) + 0.5)).w) * texture2D (_LightTextureB0, tmpvar_9).w);
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * atten) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"agal_vs
c19 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacbbaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c17
aaaaaaaaabaaaiacbdaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c19.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaabaaaaappabaaaaaa mul r2.xyz, r0.xyzz, c16.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaabdaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c19.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaabaaaaappabaaaaaa mul r1.xyz, a1, c16.w
bdaaaaaaaaaaaiacaaaaaaoeaaaaaaaaahaaaaoeabaaaaaa dp4 r0.w, a0, c7
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bdaaaaaaaeaaaiaeaaaaaaoeacaaaaaaapaaaaoeabaaaaaa dp4 v4.w, r0, c15
bdaaaaaaaeaaaeaeaaaaaaoeacaaaaaaaoaaaaoeabaaaaaa dp4 v4.z, r0, c14
bdaaaaaaaeaaacaeaaaaaaoeacaaaaaaanaaaaoeabaaaaaa dp4 v4.y, r0, c13
bdaaaaaaaeaaabaeaaaaaaoeacaaaaaaamaaaaoeabaaaaaa dp4 v4.x, r0, c12
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaabbaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c17
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaadaaahaeacaaaakeacaaaaaabcaaaaoeabaaaaaa add v3.xyz, r2.xyzz, c18
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
"
}

SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
"!!ARBvp1.0
# 29 ALU
PARAM c[20] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[18];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[17].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[17].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[4].z, R0, c[15];
DP4 result.texcoord[4].y, R0, c[14];
DP4 result.texcoord[4].x, R0, c[13];
ADD result.texcoord[1].xyz, -R0, c[18];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
ADD result.texcoord[3].xyz, -R0, c[19];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 29 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"vs_2_0
; 29 ALU
def c19, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1.xyz, c17
mov r1.w, c19.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c16.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c19.y, -r0
mul r1.xyz, v1, c16.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 oT4.z, r0, c14
dp4 oT4.y, r0, c13
dp4 oT4.x, r0, c12
add oT1.xyz, -r0, c17
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
add oT3.xyz, -r0, c18
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = vec2(dot (xlv_TEXCOORD4, xlv_TEXCOORD4));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * (texture2D (_LightTextureB0, tmpvar_9).w * textureCube (_LightTexture0, xlv_TEXCOORD4).w)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "POINT_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform highp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - (_Object2World * _glesVertex).xyz);
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTextureB0;
uniform samplerCube _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  mediump vec3 tmpvar_8;
  tmpvar_8 = normalize (xlv_TEXCOORD3);
  lightDir = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = vec2(dot (xlv_TEXCOORD4, xlv_TEXCOORD4));
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * (texture2D (_LightTextureB0, tmpvar_9).w * textureCube (_LightTexture0, xlv_TEXCOORD4).w)) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"agal_vs
c19 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacbbaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c17
aaaaaaaaabaaaiacbdaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c19.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaabaaaaappabaaaaaa mul r2.xyz, r0.xyzz, c16.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaabdaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c19.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaabaaaaappabaaaaaa mul r1.xyz, a1, c16.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bdaaaaaaaaaaaiacaaaaaaoeaaaaaaaaahaaaaoeabaaaaaa dp4 r0.w, a0, c7
bdaaaaaaaeaaaeaeaaaaaaoeacaaaaaaaoaaaaoeabaaaaaa dp4 v4.z, r0, c14
bdaaaaaaaeaaacaeaaaaaaoeacaaaaaaanaaaaoeabaaaaaa dp4 v4.y, r0, c13
bdaaaaaaaeaaabaeaaaaaaoeacaaaaaaamaaaaoeabaaaaaa dp4 v4.x, r0, c12
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaabbaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c17
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaadaaahaeacaaaakeacaaaaaabcaaaaoeabaaaaaa add v3.xyz, r2.xyzz, c18
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
aaaaaaaaaeaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v4.w, c0
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
"!!ARBvp1.0
# 28 ALU
PARAM c[20] = { { 1, 2 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
MOV R1.xyz, c[18];
MOV R1.w, c[0].x;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD R0.xyz, R0, c[17].w, -vertex.position;
DP3 R0.w, vertex.normal, -R0;
MUL R1.xyz, vertex.normal, R0.w;
MAD R0.xyz, -R1, c[0].y, -R0;
MUL R1.xyz, vertex.normal, c[17].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[4].y, R0, c[14];
DP4 result.texcoord[4].x, R0, c[13];
ADD result.texcoord[1].xyz, -R0, c[18];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MOV result.texcoord[3].xyz, c[19];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 28 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"vs_2_0
; 28 ALU
def c19, 1.00000000, 2.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1.xyz, c17
mov r1.w, c19.x
dp4 r0.z, r1, c10
dp4 r0.x, r1, c8
dp4 r0.y, r1, c9
mad r0.xyz, r0, c16.w, -v0
dp3 r0.w, v1, -r0
mul r1.xyz, v1, r0.w
mad r0.xyz, -r1, c19.y, -r0
mul r1.xyz, v1, c16.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 oT4.y, r0, c13
dp4 oT4.x, r0, c12
add oT1.xyz, -r0, c17
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mov oT3.xyz, c18
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * texture2D (_LightTexture0, xlv_TEXCOORD4).w) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform highp vec4 unity_Scale;

uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 _LightMatrix0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize (_glesNormal);
  mediump vec3 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _WorldSpaceCameraPos;
  mat3 tmpvar_6;
  tmpvar_6[0] = _Object2World[0].xyz;
  tmpvar_6[1] = _Object2World[1].xyz;
  tmpvar_6[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * reflect ((_glesVertex.xyz - ((_World2Object * tmpvar_5).xyz * unity_Scale.w)), tmpvar_1));
  tmpvar_2 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (tmpvar_1 * unity_Scale.w));
  tmpvar_3 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_4 = tmpvar_10;
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (_WorldSpaceCameraPos - (_Object2World * _glesVertex).xyz);
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD4;
varying mediump vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying highp vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD0;
uniform lowp vec4 _ReflectColor;
uniform sampler2D _LightTexture0;
uniform lowp vec4 _LightColor0;
uniform mediump float _FresnelPower;
uniform samplerCube _Cube;
uniform lowp vec4 _Color;
void main ()
{
  lowp vec4 c;
  lowp vec3 lightDir;
  lowp vec3 tmpvar_1;
  lowp vec3 tmpvar_2;
  tmpvar_1 = xlv_TEXCOORD0;
  tmpvar_2 = xlv_TEXCOORD1;
  lowp vec3 tmpvar_3;
  lowp float refl2Refr;
  mediump vec4 reflcol;
  mediump vec3 worldReflVec;
  mediump vec4 c_i0;
  c_i0 = _Color;
  worldReflVec = tmpvar_1;
  lowp vec4 tmpvar_4;
  tmpvar_4 = textureCube (_Cube, worldReflVec);
  reflcol = tmpvar_4;
  lowp float tmpvar_5;
  tmpvar_5 = clamp ((1.0 - max (dot (normalize (tmpvar_2), normalize (xlv_TEXCOORD2)), 0.0)), 0.0, 1.0);
  mediump float tmpvar_6;
  tmpvar_6 = max ((0.20373 + ((1.0 - 0.20373) * pow (tmpvar_5, _FresnelPower))), 0.0);
  refl2Refr = tmpvar_6;
  mediump vec3 tmpvar_7;
  tmpvar_7 = ((reflcol.xyz * _ReflectColor.xyz) + c_i0.xyz);
  tmpvar_3 = tmpvar_7;
  lightDir = xlv_TEXCOORD3;
  lowp vec4 c_i0_i1;
  c_i0_i1.xyz = ((tmpvar_3 * _LightColor0.xyz) * ((max (0.0, dot (xlv_TEXCOORD2, lightDir)) * texture2D (_LightTexture0, xlv_TEXCOORD4).w) * 2.0));
  c_i0_i1.w = refl2Refr;
  c = c_i0_i1;
  c.w = refl2Refr;
  gl_FragData[0] = c;
}



#endif"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
"agal_vs
c19 1.0 2.0 0.0 0.0
[bc]
aaaaaaaaabaaahacbbaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c17
aaaaaaaaabaaaiacbdaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r1.w, c19.x
bdaaaaaaaaaaaeacabaaaaoeacaaaaaaakaaaaoeabaaaaaa dp4 r0.z, r1, c10
bdaaaaaaaaaaabacabaaaaoeacaaaaaaaiaaaaoeabaaaaaa dp4 r0.x, r1, c8
bdaaaaaaaaaaacacabaaaaoeacaaaaaaajaaaaoeabaaaaaa dp4 r0.y, r1, c9
adaaaaaaacaaahacaaaaaakeacaaaaaabaaaaappabaaaaaa mul r2.xyz, r0.xyzz, c16.w
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaaoeaaaaaaaa sub r0.xyz, r2.xyzz, a0
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
bcaaaaaaaaaaaiacabaaaaoeaaaaaaaaacaaaakeacaaaaaa dp3 r0.w, a1, r2.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaaaaaaaappacaaaaaa mul r1.xyz, a1, r0.w
bfaaaaaaacaaahacabaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r1.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaabdaaaaffabaaaaaa mul r2.xyz, r2.xyzz, c19.y
acaaaaaaaaaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r2.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaaoeaaaaaaaabaaaaappabaaaaaa mul r1.xyz, a1, c16.w
bcaaaaaaaaaaaeaeaaaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v0.z, r0.xyzz, c6
bcaaaaaaaaaaacaeaaaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v0.y, r0.xyzz, c5
bcaaaaaaaaaaabaeaaaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v0.x, r0.xyzz, c4
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
bdaaaaaaaaaaaiacaaaaaaoeaaaaaaaaahaaaaoeabaaaaaa dp4 r0.w, a0, c7
bdaaaaaaaeaaacaeaaaaaaoeacaaaaaaanaaaaoeabaaaaaa dp4 v4.y, r0, c13
bdaaaaaaaeaaabaeaaaaaaoeacaaaaaaamaaaaoeabaaaaaa dp4 v4.x, r0, c12
bfaaaaaaacaaahacaaaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r2.xyz, r0.xyzz
abaaaaaaabaaahaeacaaaakeacaaaaaabbaaaaoeabaaaaaa add v1.xyz, r2.xyzz, c17
bcaaaaaaacaaaeaeabaaaakeacaaaaaaagaaaaoeabaaaaaa dp3 v2.z, r1.xyzz, c6
bcaaaaaaacaaacaeabaaaakeacaaaaaaafaaaaoeabaaaaaa dp3 v2.y, r1.xyzz, c5
bcaaaaaaacaaabaeabaaaakeacaaaaaaaeaaaaoeabaaaaaa dp3 v2.x, r1.xyzz, c4
aaaaaaaaadaaahaebcaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.xyz, c18
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.w, c0
aaaaaaaaabaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v1.w, c0
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
aaaaaaaaaeaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v4.zw, c0
"
}

}
Program "fp" {
// Fragment combos: 5
//   opengl - ALU: 21 to 32, TEX: 1 to 3
//   d3d9 - ALU: 24 to 34, TEX: 1 to 3
SubProgram "opengl " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 26 ALU, 2 TEX
PARAM c[6] = { program.local[0..3],
		{ 0.20373535, 0.79638672, 1, 0 },
		{ 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, fragment.texcoord[0], texture[0], CUBE;
DP3 R0.w, fragment.texcoord[4], fragment.texcoord[4];
DP3 R1.y, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.y, R1.y;
DP3 R1.x, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R1.y, fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3 R1.x, R1, R2;
DP3 R1.y, fragment.texcoord[3], fragment.texcoord[3];
RSQ R2.x, R1.y;
MAX R1.x, R1, c[4].w;
ADD_SAT R1.x, -R1, c[4].z;
POW R1.x, R1.x, c[3].x;
MAD R1.w, R1.x, c[4].y, c[4].x;
MOV R1.xyz, c[1];
MAD R0.xyz, R0, c[2], R1;
MUL R2.xyz, R2.x, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R2;
MUL R0.xyz, R0, c[0];
MAX R1.x, R1, c[4].w;
MAX result.color.w, R1, c[4];
TEX R0.w, R0.w, texture[1], 2D;
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[5].x;
END
# 26 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 29 ALU, 2 TEX
dcl_cube s0
dcl_2d s1
def c4, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c5, 2.00000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dcl t4.xyz
texld r2, t0, s0
dp3 r0.x, t4, t4
mov r0.xy, r0.x
dp3_pp r1.x, t2, t2
rsq_pp r1.x, r1.x
mul_pp r1.xyz, r1.x, t2
mov_pp r4.xyz, c1
mad_pp r2.xyz, r2, c2, r4
mul_pp r2.xyz, r2, c0
texld r3, r0, s1
dp3_pp r0.x, t1, t1
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
dp3_pp r0.x, r0, r1
max_pp r0.x, r0, c4
add_pp_sat r0.x, -r0, c4.y
pow_pp r1.w, r0.x, c3.x
mov_pp r0.x, r1.w
dp3_pp r1.x, t3, t3
rsq_pp r1.x, r1.x
mul_pp r1.xyz, r1.x, t3
dp3_pp r1.x, t2, r1
mad_pp r0.x, r0, c4.z, c4.w
max_pp r1.x, r1, c4
mul_pp r1.x, r1, r3
mul_pp r1.xyz, r1.x, r2
mul_pp r1.xyz, r1, c5.x
max_pp r1.w, r0.x, c4.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "POINT" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "POINT" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"agal_ps
c4 0.0 1.0 0.796387 0.203735
c5 2.0 0.0 0.0 0.0
[bc]
ciaaaaaaacaaapacaaaaaaoeaeaaaaaaaaaaaaaaafbababb tex r2, v0, s0 <cube wrap linear point>
bcaaaaaaaaaaabacaeaaaaoeaeaaaaaaaeaaaaoeaeaaaaaa dp3 r0.x, v4, v4
aaaaaaaaaaaaadacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.xy, r0.x
bcaaaaaaabaaabacacaaaaoeaeaaaaaaacaaaaoeaeaaaaaa dp3 r1.x, v2, v2
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaaoeaeaaaaaa mul r1.xyz, r1.x, v2
aaaaaaaaadaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r3.xyz, c1
adaaaaaaacaaahacacaaaakeacaaaaaaacaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c2
abaaaaaaacaaahacacaaaakeacaaaaaaadaaaakeacaaaaaa add r2.xyz, r2.xyzz, r3.xyzz
adaaaaaaacaaahacacaaaakeacaaaaaaaaaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c0
ciaaaaaaaaaaapacaaaaaafeacaaaaaaabaaaaaaafaababb tex r0, r0.xyyy, s1 <2d wrap linear point>
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
bcaaaaaaaaaaabacaaaaaakeacaaaaaaabaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r1.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaffabaaaaaa add r0.x, r0.x, c4.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaabaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r1, r0.x, c3.x
aaaaaaaaaaaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r1.x
bcaaaaaaabaaabacadaaaaoeaeaaaaaaadaaaaoeaeaaaaaa dp3 r1.x, v3, v3
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
adaaaaaaabaaahacabaaaaaaacaaaaaaadaaaaoeaeaaaaaa mul r1.xyz, r1.x, v3
bcaaaaaaabaaabacacaaaaoeaeaaaaaaabaaaakeacaaaaaa dp3 r1.x, v2, r1.xyzz
ahaaaaaaabaaabacabaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r1.x, r1.x, c4
adaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaappacaaaaaa mul r1.x, r1.x, r0.w
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaakeacaaaaaa mul r1.xyz, r1.x, r2.xyzz
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaakkabaaaaaa mul r0.x, r0.x, c4.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaappabaaaaaa add r0.x, r0.x, c4.w
adaaaaaaabaaahacabaaaakeacaaaaaaafaaaaaaabaaaaaa mul r1.xyz, r1.xyzz, c5.x
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa max r1.w, r0.x, c4.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 21 ALU, 1 TEX
PARAM c[6] = { program.local[0..3],
		{ 0.20373535, 0.79638672, 1, 0 },
		{ 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, fragment.texcoord[0], texture[0], CUBE;
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
DP3 R0.w, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R1.x, fragment.texcoord[2];
RSQ R0.w, R0.w;
MUL R1.xyz, R0.w, fragment.texcoord[1];
DP3 R0.w, R1, R2;
MOV R1.xyz, c[1];
MAD R0.xyz, R0, c[2], R1;
MAX R0.w, R0, c[4];
ADD_SAT R0.w, -R0, c[4].z;
POW R0.w, R0.w, c[3].x;
MAD R0.w, R0, c[4].y, c[4].x;
MOV R2.xyz, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R2;
MUL R0.xyz, R0, c[0];
MAX R1.x, R1, c[4].w;
MUL R0.xyz, R1.x, R0;
MAX result.color.w, R0, c[4];
MUL result.color.xyz, R0, c[5].x;
END
# 21 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"ps_2_0
; 24 ALU, 1 TEX
dcl_cube s0
def c4, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c5, 2.00000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
texld r1, t0, s0
dp3_pp r2.x, t2, t2
dp3_pp r0.x, t1, t1
rsq_pp r2.x, r2.x
rsq_pp r0.x, r0.x
mul_pp r2.xyz, r2.x, t2
mul_pp r0.xyz, r0.x, t1
dp3_pp r0.x, r0, r2
max_pp r0.x, r0, c4
add_pp_sat r0.x, -r0, c4.y
pow_pp r2.w, r0.x, c3.x
mov_pp r0.x, r2.w
mov_pp r2.xyz, c1
mad_pp r2.xyz, r1, c2, r2
mov_pp r3.xyz, t3
dp3_pp r1.x, t2, r3
mad_pp r0.x, r0, c4.z, c4.w
mul_pp r2.xyz, r2, c0
max_pp r1.x, r1, c4
mul_pp r1.xyz, r1.x, r2
mul_pp r1.xyz, r1, c5.x
max_pp r1.w, r0.x, c4.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
"agal_ps
c4 0.0 1.0 0.796387 0.203735
c5 2.0 0.0 0.0 0.0
[bc]
ciaaaaaaabaaapacaaaaaaoeaeaaaaaaaaaaaaaaafbababb tex r1, v0, s0 <cube wrap linear point>
bcaaaaaaacaaabacacaaaaoeaeaaaaaaacaaaaoeaeaaaaaa dp3 r2.x, v2, v2
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaacaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r2.x
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaacaaahacacaaaaaaacaaaaaaacaaaaoeaeaaaaaa mul r2.xyz, r2.x, v2
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
bcaaaaaaaaaaabacaaaaaakeacaaaaaaacaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r2.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaffabaaaaaa add r0.x, r0.x, c4.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaacaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r2, r0.x, c3.x
aaaaaaaaaaaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r2.x
aaaaaaaaacaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r2.xyz, c1
adaaaaaaadaaahacabaaaakeacaaaaaaacaaaaoeabaaaaaa mul r3.xyz, r1.xyzz, c2
abaaaaaaacaaahacadaaaakeacaaaaaaacaaaakeacaaaaaa add r2.xyz, r3.xyzz, r2.xyzz
aaaaaaaaadaaahacadaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa mov r3.xyz, v3
bcaaaaaaabaaabacacaaaaoeaeaaaaaaadaaaakeacaaaaaa dp3 r1.x, v2, r3.xyzz
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaakkabaaaaaa mul r0.x, r0.x, c4.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaappabaaaaaa add r0.x, r0.x, c4.w
adaaaaaaacaaahacacaaaakeacaaaaaaaaaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c0
ahaaaaaaabaaabacabaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r1.x, r1.x, c4
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaakeacaaaaaa mul r1.xyz, r1.x, r2.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaafaaaaaaabaaaaaa mul r1.xyz, r1.xyzz, c5.x
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa max r1.w, r0.x, c4.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 32 ALU, 3 TEX
PARAM c[6] = { program.local[0..3],
		{ 0.20373535, 0.79638672, 1, 0 },
		{ 0.5, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
RCP R0.x, fragment.texcoord[4].w;
MAD R1.xy, fragment.texcoord[4], R0.x, c[5].x;
DP3 R1.z, fragment.texcoord[4], fragment.texcoord[4];
TEX R0.w, R1, texture[1], 2D;
TEX R1.w, R1.z, texture[2], 2D;
TEX R0.xyz, fragment.texcoord[0], texture[0], CUBE;
DP3 R1.y, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.y, R1.y;
DP3 R1.x, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R1.y, fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3 R1.x, R1, R2;
MAX R1.x, R1, c[4].w;
ADD_SAT R1.x, -R1, c[4].z;
POW R1.x, R1.x, c[3].x;
MAD R2.x, R1, c[4].y, c[4];
MOV R1.xyz, c[1];
MAD R0.xyz, R0, c[2], R1;
DP3 R2.y, fragment.texcoord[3], fragment.texcoord[3];
RSQ R2.y, R2.y;
MUL R1.xyz, R2.y, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R1;
SLT R1.y, c[4].w, fragment.texcoord[4].z;
MUL R0.w, R1.y, R0;
MUL R1.y, R0.w, R1.w;
MAX R0.w, R1.x, c[4];
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MAX result.color.w, R2.x, c[4];
MUL result.color.xyz, R0, c[5].y;
END
# 32 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"ps_2_0
; 34 ALU, 3 TEX
dcl_cube s0
dcl_2d s1
dcl_2d s2
def c4, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c5, 0.50000000, 2.00000000, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dcl t4
texld r2, t0, s0
dp3 r1.x, t4, t4
mov r1.xy, r1.x
rcp r0.x, t4.w
mad r0.xy, t4, r0.x, c5.x
texld r3, r1, s2
texld r0, r0, s1
dp3_pp r1.x, t2, t2
dp3_pp r0.x, t1, t1
rsq_pp r1.x, r1.x
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
mul_pp r1.xyz, r1.x, t2
dp3_pp r0.x, r0, r1
max_pp r0.x, r0, c4
add_pp_sat r0.x, -r0, c4.y
pow_pp r1.w, r0.x, c3.x
mov_pp r0.xyz, c1
mad_pp r2.xyz, r2, c2, r0
mov_pp r0.x, r1.w
mad_pp r0.x, r0, c4.z, c4.w
mul_pp r4.xyz, r2, c0
dp3_pp r1.x, t3, t3
rsq_pp r2.x, r1.x
cmp r1.x, -t4.z, c4, c4.y
mul_pp r2.xyz, r2.x, t3
dp3_pp r2.x, t2, r2
mul_pp r1.x, r1, r0.w
mul_pp r1.x, r1, r3
max_pp r2.x, r2, c4
mul_pp r1.x, r2, r1
mul_pp r1.xyz, r1.x, r4
mul_pp r1.xyz, r1, c5.y
max_pp r1.w, r0.x, c4.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "SPOT" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"agal_ps
c4 0.0 1.0 0.796387 0.203735
c5 0.5 2.0 0.0 0.0
[bc]
bcaaaaaaabaaabacaeaaaaoeaeaaaaaaaeaaaaoeaeaaaaaa dp3 r1.x, v4, v4
aaaaaaaaacaaadacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r2.xy, r1.x
afaaaaaaaaaaabacaeaaaappaeaaaaaaaaaaaaaaaaaaaaaa rcp r0.x, v4.w
adaaaaaaabaaadacaeaaaaoeaeaaaaaaaaaaaaaaacaaaaaa mul r1.xy, v4, r0.x
abaaaaaaabaaadacabaaaafeacaaaaaaafaaaaaaabaaaaaa add r1.xy, r1.xyyy, c5.x
ciaaaaaaaaaaapacacaaaafeacaaaaaaacaaaaaaafaababb tex r0, r2.xyyy, s2 <2d wrap linear point>
ciaaaaaaadaaapacabaaaafeacaaaaaaabaaaaaaafaababb tex r3, r1.xyyy, s1 <2d wrap linear point>
ciaaaaaaacaaapacaaaaaaoeaeaaaaaaaaaaaaaaafbababb tex r2, v0, s0 <cube wrap linear point>
bcaaaaaaabaaabacacaaaaoeaeaaaaaaacaaaaoeaeaaaaaa dp3 r1.x, v2, v2
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaaoeaeaaaaaa mul r1.xyz, r1.x, v2
bcaaaaaaaaaaabacaaaaaakeacaaaaaaabaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r1.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaffabaaaaaa add r0.x, r0.x, c4.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaabaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r1, r0.x, c3.x
aaaaaaaaaaaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.xyz, c1
adaaaaaaacaaahacacaaaakeacaaaaaaacaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c2
abaaaaaaacaaahacacaaaakeacaaaaaaaaaaaakeacaaaaaa add r2.xyz, r2.xyzz, r0.xyzz
aaaaaaaaaaaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r1.x
adaaaaaaadaaahacacaaaakeacaaaaaaaaaaaaoeabaaaaaa mul r3.xyz, r2.xyzz, c0
bcaaaaaaabaaabacadaaaaoeaeaaaaaaadaaaaoeaeaaaaaa dp3 r1.x, v3, v3
akaaaaaaacaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r2.x, r1.x
adaaaaaaacaaahacacaaaaaaacaaaaaaadaaaaoeaeaaaaaa mul r2.xyz, r2.x, v3
bcaaaaaaacaaabacacaaaaoeaeaaaaaaacaaaakeacaaaaaa dp3 r2.x, v2, r2.xyzz
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaakkabaaaaaa mul r0.x, r0.x, c4.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaappabaaaaaa add r0.x, r0.x, c4.w
bfaaaaaaacaaaiacaeaaaakkaeaaaaaaaaaaaaaaaaaaaaaa neg r2.w, v4.z
ckaaaaaaabaaabacacaaaappacaaaaaaafaaaakkabaaaaaa slt r1.x, r2.w, c5.z
adaaaaaaabaaabacabaaaaaaacaaaaaaadaaaappacaaaaaa mul r1.x, r1.x, r3.w
adaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaappacaaaaaa mul r1.x, r1.x, r0.w
ahaaaaaaacaaabacacaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r2.x, r2.x, c4
adaaaaaaabaaabacacaaaaaaacaaaaaaabaaaaaaacaaaaaa mul r1.x, r2.x, r1.x
adaaaaaaabaaahacabaaaaaaacaaaaaaadaaaakeacaaaaaa mul r1.xyz, r1.x, r3.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaafaaaaffabaaaaaa mul r1.xyz, r1.xyzz, c5.y
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa max r1.w, r0.x, c4.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 28 ALU, 3 TEX
PARAM c[6] = { program.local[0..3],
		{ 0.20373535, 0.79638672, 1, 0 },
		{ 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, fragment.texcoord[0], texture[0], CUBE;
TEX R1.w, fragment.texcoord[4], texture[2], CUBE;
DP3 R0.w, fragment.texcoord[4], fragment.texcoord[4];
DP3 R1.y, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.y, R1.y;
DP3 R1.x, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R1.y, fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3 R1.x, R1, R2;
MAX R1.x, R1, c[4].w;
ADD_SAT R1.x, -R1, c[4].z;
POW R1.x, R1.x, c[3].x;
MAD R2.x, R1, c[4].y, c[4];
MOV R1.xyz, c[1];
MAD R0.xyz, R0, c[2], R1;
DP3 R2.y, fragment.texcoord[3], fragment.texcoord[3];
RSQ R2.y, R2.y;
MUL R1.xyz, R2.y, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R1;
MUL R0.xyz, R0, c[0];
MAX result.color.w, R2.x, c[4];
TEX R0.w, R0.w, texture[1], 2D;
MUL R1.y, R0.w, R1.w;
MAX R0.w, R1.x, c[4];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[5].x;
END
# 28 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"ps_2_0
; 30 ALU, 3 TEX
dcl_cube s0
dcl_2d s1
dcl_cube s2
def c4, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c5, 2.00000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dcl t4.xyz
texld r2, t0, s0
dp3 r0.x, t4, t4
mov r1.xy, r0.x
texld r3, r1, s1
texld r0, t4, s2
dp3_pp r1.x, t2, t2
dp3_pp r0.x, t1, t1
rsq_pp r1.x, r1.x
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
mul_pp r1.xyz, r1.x, t2
dp3_pp r0.x, r0, r1
max_pp r0.x, r0, c4
add_pp_sat r0.x, -r0, c4.y
pow_pp r1.w, r0.x, c3.x
mov_pp r0.x, r1.w
mov_pp r1.xyz, c1
mad_pp r2.xyz, r2, c2, r1
dp3_pp r1.x, t3, t3
rsq_pp r1.x, r1.x
mul_pp r1.xyz, r1.x, t3
dp3_pp r1.x, t2, r1
mad_pp r0.x, r0, c4.z, c4.w
mul_pp r2.xyz, r2, c0
mul r3.x, r3, r0.w
max_pp r1.x, r1, c4
mul_pp r1.x, r1, r3
mul_pp r1.xyz, r1.x, r2
mul_pp r1.xyz, r1, c5.x
max_pp r1.w, r0.x, c4.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "POINT_COOKIE" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"agal_ps
c4 0.0 1.0 0.796387 0.203735
c5 2.0 0.0 0.0 0.0
[bc]
ciaaaaaaacaaapacaaaaaaoeaeaaaaaaaaaaaaaaafbababb tex r2, v0, s0 <cube wrap linear point>
bcaaaaaaaaaaabacaeaaaaoeaeaaaaaaaeaaaaoeaeaaaaaa dp3 r0.x, v4, v4
aaaaaaaaabaaadacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r1.xy, r0.x
ciaaaaaaadaaapacabaaaafeacaaaaaaabaaaaaaafaababb tex r3, r1.xyyy, s1 <2d wrap linear point>
ciaaaaaaaaaaapacaeaaaaoeaeaaaaaaacaaaaaaafbababb tex r0, v4, s2 <cube wrap linear point>
bcaaaaaaabaaabacacaaaaoeaeaaaaaaacaaaaoeaeaaaaaa dp3 r1.x, v2, v2
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaaoeaeaaaaaa mul r1.xyz, r1.x, v2
bcaaaaaaaaaaabacaaaaaakeacaaaaaaabaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r1.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaffabaaaaaa add r0.x, r0.x, c4.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaabaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r1, r0.x, c3.x
aaaaaaaaaaaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r1.x
aaaaaaaaabaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c1
adaaaaaaacaaahacacaaaakeacaaaaaaacaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c2
abaaaaaaacaaahacacaaaakeacaaaaaaabaaaakeacaaaaaa add r2.xyz, r2.xyzz, r1.xyzz
bcaaaaaaabaaabacadaaaaoeaeaaaaaaadaaaaoeaeaaaaaa dp3 r1.x, v3, v3
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
adaaaaaaabaaahacabaaaaaaacaaaaaaadaaaaoeaeaaaaaa mul r1.xyz, r1.x, v3
bcaaaaaaabaaabacacaaaaoeaeaaaaaaabaaaakeacaaaaaa dp3 r1.x, v2, r1.xyzz
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaakkabaaaaaa mul r0.x, r0.x, c4.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaappabaaaaaa add r0.x, r0.x, c4.w
adaaaaaaacaaahacacaaaakeacaaaaaaaaaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c0
adaaaaaaadaaabacadaaaappacaaaaaaaaaaaappacaaaaaa mul r3.x, r3.w, r0.w
ahaaaaaaabaaabacabaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r1.x, r1.x, c4
adaaaaaaabaaabacabaaaaaaacaaaaaaadaaaaaaacaaaaaa mul r1.x, r1.x, r3.x
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaakeacaaaaaa mul r1.xyz, r1.x, r2.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaafaaaaaaabaaaaaa mul r1.xyz, r1.xyzz, c5.x
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa max r1.w, r0.x, c4.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 23 ALU, 2 TEX
PARAM c[6] = { program.local[0..3],
		{ 0.20373535, 0.79638672, 1, 0 },
		{ 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, fragment.texcoord[0], texture[0], CUBE;
TEX R0.w, fragment.texcoord[4], texture[1], 2D;
DP3 R1.y, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.y, R1.y;
DP3 R1.x, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R1.y, fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3 R1.x, R1, R2;
MAX R1.x, R1, c[4].w;
ADD_SAT R1.x, -R1, c[4].z;
POW R1.x, R1.x, c[3].x;
MAD R1.w, R1.x, c[4].y, c[4].x;
MOV R1.xyz, c[1];
MAD R0.xyz, R0, c[2], R1;
MOV R2.xyz, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R2;
MAX R1.x, R1, c[4].w;
MUL R0.xyz, R0, c[0];
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MAX result.color.w, R1, c[4];
MUL result.color.xyz, R0, c[5].x;
END
# 23 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 25 ALU, 2 TEX
dcl_cube s0
dcl_2d s1
def c4, 0.00000000, 1.00000000, 0.79638672, 0.20373535
def c5, 2.00000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dcl t4.xy
texld r0, t4, s1
texld r2, t0, s0
dp3_pp r1.x, t2, t2
dp3_pp r0.x, t1, t1
rsq_pp r1.x, r1.x
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
mul_pp r1.xyz, r1.x, t2
dp3_pp r0.x, r0, r1
max_pp r0.x, r0, c4
add_pp_sat r0.x, -r0, c4.y
pow_pp r1.w, r0.x, c3.x
mov_pp r0.x, r1.w
mad_pp r0.x, r0, c4.z, c4.w
mov_pp r1.xyz, c1
mad_pp r2.xyz, r2, c2, r1
mov_pp r1.xyz, t3
dp3_pp r1.x, t2, r1
max_pp r1.x, r1, c4
mul_pp r2.xyz, r2, c0
mul_pp r1.x, r1, r0.w
mul_pp r1.xyz, r1.x, r2
mul_pp r1.xyz, r1, c5.x
max_pp r1.w, r0.x, c4.x
mov_pp oC0, r1
"
}

SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES"
}

SubProgram "flash " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Vector 2 [_ReflectColor]
Float 3 [_FresnelPower]
SetTexture 0 [_Cube] CUBE
SetTexture 1 [_LightTexture0] 2D
"agal_ps
c4 0.0 1.0 0.796387 0.203735
c5 2.0 0.0 0.0 0.0
[bc]
ciaaaaaaaaaaapacaeaaaaoeaeaaaaaaabaaaaaaafaababb tex r0, v4, s1 <2d wrap linear point>
ciaaaaaaacaaapacaaaaaaoeaeaaaaaaaaaaaaaaafbababb tex r2, v0, s0 <cube wrap linear point>
bcaaaaaaabaaabacacaaaaoeaeaaaaaaacaaaaoeaeaaaaaa dp3 r1.x, v2, v2
bcaaaaaaaaaaabacabaaaaoeaeaaaaaaabaaaaoeaeaaaaaa dp3 r0.x, v1, v1
akaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r1.x, r1.x
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaabaaaaoeaeaaaaaa mul r0.xyz, r0.x, v1
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaaoeaeaaaaaa mul r1.xyz, r1.x, v2
bcaaaaaaaaaaabacaaaaaakeacaaaaaaabaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r1.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
bfaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r0.x, r0.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaffabaaaaaa add r0.x, r0.x, c4.y
bgaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa sat r0.x, r0.x
alaaaaaaabaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r1, r0.x, c3.x
aaaaaaaaaaaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.x, r1.x
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaakkabaaaaaa mul r0.x, r0.x, c4.z
abaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaappabaaaaaa add r0.x, r0.x, c4.w
aaaaaaaaabaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, c1
adaaaaaaacaaahacacaaaakeacaaaaaaacaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c2
abaaaaaaacaaahacacaaaakeacaaaaaaabaaaakeacaaaaaa add r2.xyz, r2.xyzz, r1.xyzz
aaaaaaaaabaaahacadaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa mov r1.xyz, v3
bcaaaaaaabaaabacacaaaaoeaeaaaaaaabaaaakeacaaaaaa dp3 r1.x, v2, r1.xyzz
ahaaaaaaabaaabacabaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r1.x, r1.x, c4
adaaaaaaacaaahacacaaaakeacaaaaaaaaaaaaoeabaaaaaa mul r2.xyz, r2.xyzz, c0
adaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaappacaaaaaa mul r1.x, r1.x, r0.w
adaaaaaaabaaahacabaaaaaaacaaaaaaacaaaakeacaaaaaa mul r1.xyz, r1.x, r2.xyzz
adaaaaaaabaaahacabaaaakeacaaaaaaafaaaaaaabaaaaaa mul r1.xyz, r1.xyzz, c5.x
ahaaaaaaabaaaiacaaaaaaaaacaaaaaaaeaaaaaaabaaaaaa max r1.w, r0.x, c4.x
aaaaaaaaaaaaapadabaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r1
"
}

}
	}

#LINE 83


}
	
FallBack "Reflective/VertexLit"
} 

