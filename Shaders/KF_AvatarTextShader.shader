// Avatar Text for VRChat
// Copyright (C) 2022 KillFrenzy / Evan Tran

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.


Shader "Unlit/KF_VRChatAvatarTextShader"
{
	Properties
	{
		[Enum(Off,0,Front,1,Back,2)] _Culling ("Culling Mode", Int) = 2
		_MainTex("Texture", 2D) = "white" {}
		_MainColor("Main Colour", Color) = (1, 1, 1, 1)
		_ShadowColor("Shadow Colour", Color) = (0, 0, 0, 1)
		[Space]
		_TileX("Text Tile Count X", Float) = 16
		_TileY("Text Tile Count Y", Float) = 6
		_RowLength("Text Output Row Length", Float) = 32
		_RowColumns("Text Output Row Columns", Float) = 12

		[Space]
		[Toggle(_SUNDISK_NONE)]_TextShadow("Parallax Text Shadow", Range(0, 1)) = 0
		[Gamma]_ShadowDistance("Shadow Distance", Range(0, 0.03)) = 0.001

		[Space]
		_Char0("Character 0", Float) = 0
		_Char1("Character 1", Float) = 0
		_Char2("Character 2", Float) = 0
		_Char3("Character 3", Float) = 0
		_Char4("Character 4", Float) = 0
		_Char5("Character 5", Float) = 0
		_Char6("Character 6", Float) = 0
		_Char7("Character 7", Float) = 0
		_Char8("Character 8", Float) = 0
		_Char9("Character 9", Float) = 0
		_Char10("Character 10", Float) = 0
		_Char11("Character 11", Float) = 0
		_Char12("Character 12", Float) = 0
		_Char13("Character 13", Float) = 0
		_Char14("Character 14", Float) = 0
		_Char15("Character 15", Float) = 0
		_Char16("Character 16", Float) = 0
		_Char17("Character 17", Float) = 0
		_Char18("Character 18", Float) = 0
		_Char19("Character 19", Float) = 0
		_Char20("Character 20", Float) = 0
		_Char21("Character 21", Float) = 0
		_Char22("Character 22", Float) = 0
		_Char23("Character 23", Float) = 0
		_Char24("Character 24", Float) = 0
		_Char25("Character 25", Float) = 0
		_Char26("Character 26", Float) = 0
		_Char27("Character 27", Float) = 0
		_Char28("Character 28", Float) = 0
		_Char29("Character 29", Float) = 0
		_Char30("Character 30", Float) = 0
		_Char31("Character 31", Float) = 0
		_Char32("Character 32", Float) = 0
		_Char33("Character 33", Float) = 0
		_Char34("Character 34", Float) = 0
		_Char35("Character 35", Float) = 0
		_Char36("Character 36", Float) = 0
		_Char37("Character 37", Float) = 0
		_Char38("Character 38", Float) = 0
		_Char39("Character 39", Float) = 0
		_Char40("Character 40", Float) = 0
		_Char41("Character 41", Float) = 0
		_Char42("Character 42", Float) = 0
		_Char43("Character 43", Float) = 0
		_Char44("Character 44", Float) = 0
		_Char45("Character 45", Float) = 0
		_Char46("Character 46", Float) = 0
		_Char47("Character 47", Float) = 0
		_Char48("Character 48", Float) = 0
		_Char49("Character 49", Float) = 0
		_Char50("Character 50", Float) = 0
		_Char51("Character 51", Float) = 0
		_Char52("Character 52", Float) = 0
		_Char53("Character 53", Float) = 0
		_Char54("Character 54", Float) = 0
		_Char55("Character 55", Float) = 0
		_Char56("Character 56", Float) = 0
		_Char57("Character 57", Float) = 0
		_Char58("Character 58", Float) = 0
		_Char59("Character 59", Float) = 0
		_Char60("Character 60", Float) = 0
		_Char61("Character 61", Float) = 0
		_Char62("Character 62", Float) = 0
		_Char63("Character 63", Float) = 0
		_Char64("Character 64", Float) = 0
		_Char65("Character 65", Float) = 0
		_Char66("Character 66", Float) = 0
		_Char67("Character 67", Float) = 0
		_Char68("Character 68", Float) = 0
		_Char69("Character 69", Float) = 0
		_Char70("Character 70", Float) = 0
		_Char71("Character 71", Float) = 0
		_Char72("Character 72", Float) = 0
		_Char73("Character 73", Float) = 0
		_Char74("Character 74", Float) = 0
		_Char75("Character 75", Float) = 0
		_Char76("Character 76", Float) = 0
		_Char77("Character 77", Float) = 0
		_Char78("Character 78", Float) = 0
		_Char79("Character 79", Float) = 0
		_Char80("Character 80", Float) = 0
		_Char81("Character 81", Float) = 0
		_Char82("Character 82", Float) = 0
		_Char83("Character 83", Float) = 0
		_Char84("Character 84", Float) = 0
		_Char85("Character 85", Float) = 0
		_Char86("Character 86", Float) = 0
		_Char87("Character 87", Float) = 0
		_Char88("Character 88", Float) = 0
		_Char89("Character 89", Float) = 0
		_Char90("Character 90", Float) = 0
		_Char91("Character 91", Float) = 0
		_Char92("Character 92", Float) = 0
		_Char93("Character 93", Float) = 0
		_Char94("Character 94", Float) = 0
		_Char95("Character 95", Float) = 0
		_Char96("Character 96", Float) = 0
		_Char97("Character 97", Float) = 0
		_Char98("Character 98", Float) = 0
		_Char99("Character 99", Float) = 0
		_Char100("Character 100", Float) = 0
		_Char101("Character 101", Float) = 0
		_Char102("Character 102", Float) = 0
		_Char103("Character 103", Float) = 0
		_Char104("Character 104", Float) = 0
		_Char105("Character 105", Float) = 0
		_Char106("Character 106", Float) = 0
		_Char107("Character 107", Float) = 0
		_Char108("Character 108", Float) = 0
		_Char109("Character 109", Float) = 0
		_Char110("Character 110", Float) = 0
		_Char111("Character 111", Float) = 0
		_Char112("Character 112", Float) = 0
		_Char113("Character 113", Float) = 0
		_Char114("Character 114", Float) = 0
		_Char115("Character 115", Float) = 0
		_Char116("Character 116", Float) = 0
		_Char117("Character 117", Float) = 0
		_Char118("Character 118", Float) = 0
		_Char119("Character 119", Float) = 0
		_Char120("Character 120", Float) = 0
		_Char121("Character 121", Float) = 0
		_Char122("Character 122", Float) = 0
		_Char123("Character 123", Float) = 0
		_Char124("Character 124", Float) = 0
		_Char125("Character 125", Float) = 0
		_Char126("Character 126", Float) = 0
		_Char127("Character 127", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Overlay"
			"DisableBatching" = "True"
			"IgnoreProjector" = "True"
			"PreviewType"="Plane"
			"Queue" = "AlphaTest+549"
		}

		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Cull [_Culling]
		ZWrite Off
		AlphaToMask On

		CGINCLUDE
		// Common code, for all passes
		// UNITY_SHADER_NO_UPGRADE
		#pragma shader_feature_local _ _SUNDISK_NONE

		cbuffer UnityPerMaterial
		{
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float4 _MainColor;
			float4 _ShadowColor;
			float _TileX;
			float _TileY;
			float _RowLength;
			float _RowColumns;
			float _TextShadow;
			float _ShadowDistance;
		}

		CBUFFER_START(CharacterBuffer)
			float _Chars[128]  : packoffset(c00);
			float _Char0  : packoffset(c00);
			float _Char1  : packoffset(c01);
			float _Char2  : packoffset(c02);
			float _Char3  : packoffset(c03);
			float _Char4  : packoffset(c04);
			float _Char5  : packoffset(c05);
			float _Char6  : packoffset(c06);
			float _Char7  : packoffset(c07);
			float _Char8  : packoffset(c08);
			float _Char9  : packoffset(c09);
			float _Char10  : packoffset(c10);
			float _Char11  : packoffset(c11);
			float _Char12  : packoffset(c12);
			float _Char13  : packoffset(c13);
			float _Char14  : packoffset(c14);
			float _Char15  : packoffset(c15);
			float _Char16  : packoffset(c16);
			float _Char17  : packoffset(c17);
			float _Char18  : packoffset(c18);
			float _Char19  : packoffset(c19);
			float _Char20  : packoffset(c20);
			float _Char21  : packoffset(c21);
			float _Char22  : packoffset(c22);
			float _Char23  : packoffset(c23);
			float _Char24  : packoffset(c24);
			float _Char25  : packoffset(c25);
			float _Char26  : packoffset(c26);
			float _Char27  : packoffset(c27);
			float _Char28  : packoffset(c28);
			float _Char29  : packoffset(c29);
			float _Char30  : packoffset(c30);
			float _Char31  : packoffset(c31);
			float _Char32  : packoffset(c32);
			float _Char33  : packoffset(c33);
			float _Char34  : packoffset(c34);
			float _Char35  : packoffset(c35);
			float _Char36  : packoffset(c36);
			float _Char37  : packoffset(c37);
			float _Char38  : packoffset(c38);
			float _Char39  : packoffset(c39);
			float _Char40  : packoffset(c40);
			float _Char41  : packoffset(c41);
			float _Char42  : packoffset(c42);
			float _Char43  : packoffset(c43);
			float _Char44  : packoffset(c44);
			float _Char45  : packoffset(c45);
			float _Char46  : packoffset(c46);
			float _Char47  : packoffset(c47);
			float _Char48  : packoffset(c48);
			float _Char49  : packoffset(c49);
			float _Char50  : packoffset(c50);
			float _Char51  : packoffset(c51);
			float _Char52  : packoffset(c52);
			float _Char53  : packoffset(c53);
			float _Char54  : packoffset(c54);
			float _Char55  : packoffset(c55);
			float _Char56  : packoffset(c56);
			float _Char57  : packoffset(c57);
			float _Char58  : packoffset(c58);
			float _Char59  : packoffset(c59);
			float _Char60  : packoffset(c60);
			float _Char61  : packoffset(c61);
			float _Char62  : packoffset(c62);
			float _Char63  : packoffset(c63);
			float _Char64  : packoffset(c64);
			float _Char65  : packoffset(c65);
			float _Char66  : packoffset(c66);
			float _Char67  : packoffset(c67);
			float _Char68  : packoffset(c68);
			float _Char69  : packoffset(c69);
			float _Char70  : packoffset(c70);
			float _Char71  : packoffset(c71);
			float _Char72  : packoffset(c72);
			float _Char73  : packoffset(c73);
			float _Char74  : packoffset(c74);
			float _Char75  : packoffset(c75);
			float _Char76  : packoffset(c76);
			float _Char77  : packoffset(c77);
			float _Char78  : packoffset(c78);
			float _Char79  : packoffset(c79);
			float _Char80  : packoffset(c80);
			float _Char81  : packoffset(c81);
			float _Char82  : packoffset(c82);
			float _Char83  : packoffset(c83);
			float _Char84  : packoffset(c84);
			float _Char85  : packoffset(c85);
			float _Char86  : packoffset(c86);
			float _Char87  : packoffset(c87);
			float _Char88  : packoffset(c88);
			float _Char89  : packoffset(c89);
			float _Char90  : packoffset(c90);
			float _Char91  : packoffset(c91);
			float _Char92  : packoffset(c92);
			float _Char93  : packoffset(c93);
			float _Char94  : packoffset(c94);
			float _Char95  : packoffset(c95);
			float _Char96  : packoffset(c96);
			float _Char97  : packoffset(c97);
			float _Char98  : packoffset(c98);
			float _Char99  : packoffset(c99);
			float _Char100 : packoffset(c100);
			float _Char101 : packoffset(c101);
			float _Char102 : packoffset(c102);
			float _Char103 : packoffset(c103);
			float _Char104 : packoffset(c104);
			float _Char105 : packoffset(c105);
			float _Char106 : packoffset(c106);
			float _Char107 : packoffset(c107);
			float _Char108 : packoffset(c108);
			float _Char109 : packoffset(c109);
			float _Char110 : packoffset(c110);
			float _Char111 : packoffset(c111);
			float _Char112 : packoffset(c112);
			float _Char113 : packoffset(c113);
			float _Char114 : packoffset(c114);
			float _Char115 : packoffset(c115);
			float _Char116 : packoffset(c116);
			float _Char117 : packoffset(c117);
			float _Char118 : packoffset(c118);
			float _Char119 : packoffset(c119);
			float _Char120 : packoffset(c120);
			float _Char121 : packoffset(c121);
			float _Char122 : packoffset(c122);
			float _Char123 : packoffset(c123);
			float _Char124 : packoffset(c124);
			float _Char125 : packoffset(c125);
			float _Char126 : packoffset(c126);
			float _Char127 : packoffset(c127);
		CBUFFER_END

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;

			#ifndef UNITY_PASS_SHADOWCASTER
			#endif

			// Attributes for parallax shadows
			#if defined(_SUNDISK_NONE)
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			#endif

			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			UNITY_FOG_COORDS(1)
			#ifndef UNITY_PASS_SHADOWCASTER
			float4 vertex : SV_POSITION;
			#else
			V2F_SHADOW_CASTER;
			#endif

			#if defined(_SUNDISK_NONE)
			float3 tangentViewDir : TANGENT;
			#endif
			UNITY_VERTEX_OUTPUT_STEREO
		};

		// Helper functions

		// This function serves one purpose - to touch every shader property so Unity knows they're used.
		float sumCharacters()
		{
			return
			_Char0 + _Char1 + _Char2 + _Char3 + _Char4 + _Char5 + _Char6 + _Char7 +
			_Char8 + _Char9 + _Char10 + _Char11 + _Char12 + _Char13 + _Char14 + _Char15 +
			_Char16 + _Char17 + _Char18 + _Char19 + _Char20 + _Char21 + _Char22 + _Char23 +
			_Char24 + _Char25 + _Char26 + _Char27 + _Char28 + _Char29 + _Char30 + _Char31 +
			_Char32 + _Char33 + _Char34 + _Char35 + _Char36 + _Char37 + _Char38 + _Char39 +
			_Char40 + _Char41 + _Char42 + _Char43 + _Char44 + _Char45 + _Char46 + _Char47 +
			_Char48 + _Char49 + _Char50 + _Char51 + _Char52 + _Char53 + _Char54 + _Char55 +
			_Char56 + _Char57 + _Char58 + _Char59 + _Char60 + _Char61 + _Char62 + _Char63 +
			_Char64 + _Char65 + _Char66 + _Char67 + _Char68 + _Char69 + _Char70 + _Char71 +
			_Char72 + _Char73 + _Char74 + _Char75 + _Char76 + _Char77 + _Char78 + _Char79 +
			_Char80 + _Char81 + _Char82 + _Char83 + _Char84 + _Char85 + _Char86 + _Char87 +
			_Char88 + _Char89 + _Char90 + _Char91 + _Char92 + _Char93 + _Char94 + _Char95 +
			_Char96 + _Char97 + _Char98 + _Char99 + _Char100 + _Char101 + _Char102 + _Char103 +
			_Char104 + _Char105 + _Char106 + _Char107 + _Char108 + _Char109 + _Char110 + _Char111 +
			_Char112 + _Char113 + _Char114 + _Char115 + _Char116 + _Char117 + _Char118 + _Char119 +
			_Char120 + _Char121 + _Char122 + _Char123 + _Char124 + _Char125 + _Char126 + _Char127;
		}

		float2 ParallaxOffset(float height, float2 parallaxUV, float scale, float2 uvs = 0)
		{
			return ((height - 1) * parallaxUV * scale) + uvs;
		}

		float2 getParallaxUVs(float2 parallaxUV, float height, float scale, float4 scaleOffset, float2 uv)
		{
			float2 offset = ParallaxOffset(height, parallaxUV, scale);
			float2 centre = 0.5-offset;

			uv -= centre;
			uv *= scaleOffset.xy;
			uv += centre;
			uv += scaleOffset.zw;
			uv += offset;
			return uv;
		}

		float3 Heatmap(float v) {
			float3 r = v * 2.1 - float3(1.8, 1.14, 0.3);
			return 1.0 - r * r;
		}

		// Triangle Wave
		float T(float z) {
			return z >= 0.5 ? 2.-2.*z : 2.*z;
		}

		// R dither mask
		float intensity(float2 pixel) {
			const float a1 = 0.75487766624669276;
			const float a2 = 0.569840290998;
			return frac(a1 * float(pixel.x) + a2 * float(pixel.y));
		}

		float computeMaskedAlpha(float a, float t) {
			// Use derivatives to smooth alpha tested edges
			return (a - t) / max(fwidth(a), 1e-3) + 0.5;
		}

		// Main code

		v2f vert (appdata v)
		{
			v2f o;
			UNITY_INITIALIZE_OUTPUT(v2f, o);
			UNITY_SETUP_INSTANCE_ID(v);

			float3 posVS = v.vertex.xyz;
			float3 centerEye = _WorldSpaceCameraPos;
			#ifdef USING_STEREO_MATRICES
			centerEye = .5 * (unity_StereoWorldSpaceCameraPos[0] + unity_StereoWorldSpaceCameraPos[1]);
			#endif
			float3 objPos = unity_ObjectToWorld._14_24_34;
			v.vertex *= 1 + smoothstep(0, 1, distance(centerEye, objPos)-0.5);


			#ifdef UNITY_PASS_SHADOWCASTER
			TRANSFER_SHADOW_CASTER(o);
			#else
			float4 posCS = UnityObjectToClipPos(v.vertex);
			o.vertex = posCS;
			#endif

			#if defined(_SUNDISK_NONE)
			// Get tangent-space view direction
			float3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) ) * v.tangent.w;
			float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
			o.tangentViewDir = mul(rotation,  ObjSpaceViewDir(v.vertex));
			#endif

			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			UNITY_TRANSFER_FOG(o,o.vertex);
			UNITY_TRANSFER_INSTANCE_ID(v, o);
			return o;
		}



		fixed4 frag (v2f i, uint facing: SV_IsFrontFace) : SV_Target
		{

			#ifdef UNITY_PASS_SHADOWCASTER
			float4 posCS = i.pos;
			#else
			float4 posCS = i.vertex;
			#endif

			// Flip text if looking at the backface
			if (!facing) {
				i.uv.x = 1.0 - i.uv.x;
				#if defined(_SUNDISK_NONE)
				i.tangentViewDir *= -1;
				#endif
			}

			// Flip text if looking at the mirror
			if (unity_CameraProjection[2][0] != 0.0 || unity_CameraProjection[2][1] != 0.0) {
				i.uv.x = 1.0 - i.uv.x;
			}

			// Get derivatives from the original texture coordinates,
			// so the texture can be read with mipmaps.
			float2 dX = (ddx(i.uv.x));
			float2 dY = (ddx(i.uv.y));

			// Setup parallax shadow stuff.
			#if defined(_SUNDISK_NONE)
			// Re-normalize the tangent space view direction
			const float2 parallaxUV = i.tangentViewDir.xy / max(i.tangentViewDir.z, 0.0001);
			#endif

			float dither = T(intensity(posCS + _Time.y));
			i.uv += dither * _MainTex_TexelSize.xy * dot(dX, dY);

			float2 uvSize = float2(_TileX, _TileY);
			float2 charSize = float2(_RowLength, _RowColumns);
			float2 uvTile = 1.0 / uvSize;
			float2 charTile = 1.0 / charSize;

			float texRatio = _MainTex_TexelSize.x / _MainTex_TexelSize.y;
			float2 cellTexRatio = float2(texRatio, 1.0);

			float charLimit = uvSize.x * uvSize.y;
			float charPosition = floor(i.uv.x * charSize.x) + floor((1.0 - i.uv.y) * charSize.y) * charSize.x;
			float charCurrent = round(_Chars[clamp(charPosition, 0, 127)]);
			charCurrent = min(charCurrent, charLimit);
			if (charCurrent < 0) {
				charCurrent += floor(charCurrent / charLimit) * charLimit;
			}

			if (ceil(i.uv.x) != 1) discard;
			if (ceil(i.uv.y) != 1) discard;

			float2 uvPosition = (fmod(i.uv * charSize, 1.0) / uvSize);
			float2 uvOffset = float2(fmod(charCurrent, uvSize.x) * uvTile.x, 1.0
				- ((floor(charCurrent / uvSize.x) + 1.0) * uvTile.y));

			float2 uv = uvPosition + uvOffset;

			fixed4 col = tex2Dgrad(_MainTex, uv, dX, dY);
			float cutoutValue = 0.5;
			col.a = (col.a - cutoutValue) / max(fwidth(col.a), 1e-3) + 0.5;

			// Add parallax shadow
			float2 offsets[] =
			{
				float2(-0.666, 0.666),
				float2(0, 1),
				float2(0.666, 0.666),
				float2(1, 0),
				float2(-0.666, -0.666),
				float2(0, -1),
				float2(0.666, -0.666),
				float2(-1, 0),
			};

			#if defined(_SUNDISK_NONE)
			[unroll]
			for (int c = 0; c < 8; c+=1)
			{
				float2 origUV = i.uv;
				float2 offsetUV = offsets[c].xy * _ShadowDistance * float2(1, 4);
				float2 paraPos = parallaxUV * _ShadowDistance;
				origUV -= offsetUV;
				origUV -= paraPos;
				float charPosition = floor(origUV.x * charSize.x)
					+ floor((1.0 - origUV.y) * charSize.y) * charSize.x;
				float charCurrent = round(_Chars[clamp(charPosition, 0, 127)]);
				charCurrent = min(charCurrent, charLimit);
				if (charCurrent < 0) {
					charCurrent += floor(charCurrent / charLimit) * charLimit;
				}

				if (ceil(origUV.x) != 1) continue;
				if (ceil(origUV.y) != 1) continue;

				float2 uvPosition = (fmod(origUV * charSize, 1.0) / uvSize);
				float2 uvOffset = float2(fmod(charCurrent, uvSize.x) * uvTile.x, 1.0
					- ((floor(charCurrent / uvSize.x) + 1.0) * uvTile.y));

				float2 uv = uvPosition + uvOffset;
				fixed4 colShadow = tex2Dgrad(_MainTex, uv, dX, dY);

				float2 shadowMask = (frac(i.uv * charSize)-parallaxUV * _ShadowDistance);
				shadowMask = shadowMask == saturate(shadowMask);
				colShadow.a *= shadowMask.x * shadowMask.y;

				col.a = max(col.a, colShadow.a);
			}
			#endif

			col = smoothstep(0.25, 0.75, col);

			col.rgb = lerp(_ShadowColor, _MainColor, col);

			if (col.a <= 0.0) {
				discard;
			}

			UNITY_APPLY_FOG(i.fogCoord, col);

			// Force a branch, so this can be skipped fully.
			[branch]
			// This is only to make sure Unity populates the array. If the _CharXX properties
			// aren't read in the shader, Unity will not send them.
			if ((col.a+posCS.w) == 65536) return sumCharacters();

			#if defined(UNITY_PASS_SHADOWCASTER)
			SHADOW_CASTER_FRAGMENT(i)
			#endif

			return col;
		}

		ENDCG

		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "Always" "Queue" = "Overlay" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			ENDCG
		}

		Pass
		{
			Name "ShadowCaster"
			Tags { "RenderType" = "TreeLeaf" "LightMode" = "ShadowCaster" }
			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			CGPROGRAM
			#pragma multi_compile_shadowcaster

			#ifndef UNITY_PASS_SHADOWCASTER
			#define UNITY_PASS_SHADOWCASTER
			#endif

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	FallBack "Unlit/Transparent"
}
