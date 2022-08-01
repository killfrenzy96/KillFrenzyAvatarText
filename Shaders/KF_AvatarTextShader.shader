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
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.1
		_TileX("Text Tile Count X", Float) = 16
		_TileY("Text Tile Count Y", Float) = 6
		_RowLength("Text Output Row Length", Float) = 32
		_RowColumns("Text Output Row Columns", Float) = 12

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
		Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest+100" "IgnoreProjector"="True" }
		LOD 100
		AlphaToMask On
		// Blend SrcAlpha OneMinusSrcAlpha
		Cull [_Culling]

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;

			float _TileX;
			float _TileY;
			float _RowLength;
			float _RowColumns;

			// Please send me support for arrays using shader properties
			float _Char0;
			float _Char1;
			float _Char2;
			float _Char3;
			float _Char4;
			float _Char5;
			float _Char6;
			float _Char7;
			float _Char8;
			float _Char9;
			float _Char10;
			float _Char11;
			float _Char12;
			float _Char13;
			float _Char14;
			float _Char15;
			float _Char16;
			float _Char17;
			float _Char18;
			float _Char19;
			float _Char20;
			float _Char21;
			float _Char22;
			float _Char23;
			float _Char24;
			float _Char25;
			float _Char26;
			float _Char27;
			float _Char28;
			float _Char29;
			float _Char30;
			float _Char31;
			float _Char32;
			float _Char33;
			float _Char34;
			float _Char35;
			float _Char36;
			float _Char37;
			float _Char38;
			float _Char39;
			float _Char40;
			float _Char41;
			float _Char42;
			float _Char43;
			float _Char44;
			float _Char45;
			float _Char46;
			float _Char47;
			float _Char48;
			float _Char49;
			float _Char50;
			float _Char51;
			float _Char52;
			float _Char53;
			float _Char54;
			float _Char55;
			float _Char56;
			float _Char57;
			float _Char58;
			float _Char59;
			float _Char60;
			float _Char61;
			float _Char62;
			float _Char63;
			float _Char64;
			float _Char65;
			float _Char66;
			float _Char67;
			float _Char68;
			float _Char69;
			float _Char70;
			float _Char71;
			float _Char72;
			float _Char73;
			float _Char74;
			float _Char75;
			float _Char76;
			float _Char77;
			float _Char78;
			float _Char79;
			float _Char80;
			float _Char81;
			float _Char82;
			float _Char83;
			float _Char84;
			float _Char85;
			float _Char86;
			float _Char87;
			float _Char88;
			float _Char89;
			float _Char90;
			float _Char91;
			float _Char92;
			float _Char93;
			float _Char94;
			float _Char95;
			float _Char96;
			float _Char97;
			float _Char98;
			float _Char99;
			float _Char100;
			float _Char101;
			float _Char102;
			float _Char103;
			float _Char104;
			float _Char105;
			float _Char106;
			float _Char107;
			float _Char108;
			float _Char109;
			float _Char110;
			float _Char111;
			float _Char112;
			float _Char113;
			float _Char114;
			float _Char115;
			float _Char116;
			float _Char117;
			float _Char118;
			float _Char119;
			float _Char120;
			float _Char121;
			float _Char122;
			float _Char123;
			float _Char124;
			float _Char125;
			float _Char126;
			float _Char127;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i, uint facing: SV_IsFrontFace) : SV_Target
			{
				// Flip text if looking at the backface
				if (!facing) {
					i.uv.x = 1.0 - i.uv.x;
				}

				// Flip text if looking at the mirror
				if (unity_CameraProjection[2][0] != 0.0 || unity_CameraProjection[2][1] != 0.0) {
					i.uv.x = 1.0 - i.uv.x;
				}

				// Really, I'd like to have arrays using shader properties
				float chars[128];
				chars[0] = _Char0;
				chars[1] = _Char1;
				chars[2] = _Char2;
				chars[3] = _Char3;
				chars[4] = _Char4;
				chars[5] = _Char5;
				chars[6] = _Char6;
				chars[7] = _Char7;
				chars[8] = _Char8;
				chars[9] = _Char9;
				chars[10] = _Char10;
				chars[11] = _Char11;
				chars[12] = _Char12;
				chars[13] = _Char13;
				chars[14] = _Char14;
				chars[15] = _Char15;
				chars[16] = _Char16;
				chars[17] = _Char17;
				chars[18] = _Char18;
				chars[19] = _Char19;
				chars[20] = _Char20;
				chars[21] = _Char21;
				chars[22] = _Char22;
				chars[23] = _Char23;
				chars[24] = _Char24;
				chars[25] = _Char25;
				chars[26] = _Char26;
				chars[27] = _Char27;
				chars[28] = _Char28;
				chars[29] = _Char29;
				chars[30] = _Char30;
				chars[31] = _Char31;
				chars[32] = _Char32;
				chars[33] = _Char33;
				chars[34] = _Char34;
				chars[35] = _Char35;
				chars[36] = _Char36;
				chars[37] = _Char37;
				chars[38] = _Char38;
				chars[39] = _Char39;
				chars[40] = _Char40;
				chars[41] = _Char41;
				chars[42] = _Char42;
				chars[43] = _Char43;
				chars[44] = _Char44;
				chars[45] = _Char45;
				chars[46] = _Char46;
				chars[47] = _Char47;
				chars[48] = _Char48;
				chars[49] = _Char49;
				chars[50] = _Char50;
				chars[51] = _Char51;
				chars[52] = _Char52;
				chars[53] = _Char53;
				chars[54] = _Char54;
				chars[55] = _Char55;
				chars[56] = _Char56;
				chars[57] = _Char57;
				chars[58] = _Char58;
				chars[59] = _Char59;
				chars[60] = _Char60;
				chars[61] = _Char61;
				chars[62] = _Char62;
				chars[63] = _Char63;
				chars[64] = _Char64;
				chars[65] = _Char65;
				chars[66] = _Char66;
				chars[67] = _Char67;
				chars[68] = _Char68;
				chars[69] = _Char69;
				chars[70] = _Char70;
				chars[71] = _Char71;
				chars[72] = _Char72;
				chars[73] = _Char73;
				chars[74] = _Char74;
				chars[75] = _Char75;
				chars[76] = _Char76;
				chars[77] = _Char77;
				chars[78] = _Char78;
				chars[79] = _Char79;
				chars[80] = _Char80;
				chars[81] = _Char81;
				chars[82] = _Char82;
				chars[83] = _Char83;
				chars[84] = _Char84;
				chars[85] = _Char85;
				chars[86] = _Char86;
				chars[87] = _Char87;
				chars[88] = _Char88;
				chars[89] = _Char89;
				chars[90] = _Char90;
				chars[91] = _Char91;
				chars[92] = _Char92;
				chars[93] = _Char93;
				chars[94] = _Char94;
				chars[95] = _Char95;
				chars[96] = _Char96;
				chars[97] = _Char97;
				chars[98] = _Char98;
				chars[99] = _Char99;
				chars[100] = _Char100;
				chars[101] = _Char101;
				chars[102] = _Char102;
				chars[103] = _Char103;
				chars[104] = _Char104;
				chars[105] = _Char105;
				chars[106] = _Char106;
				chars[107] = _Char107;
				chars[108] = _Char108;
				chars[109] = _Char109;
				chars[110] = _Char110;
				chars[111] = _Char111;
				chars[112] = _Char112;
				chars[113] = _Char113;
				chars[114] = _Char114;
				chars[115] = _Char115;
				chars[116] = _Char116;
				chars[117] = _Char117;
				chars[118] = _Char118;
				chars[119] = _Char119;
				chars[120] = _Char120;
				chars[121] = _Char121;
				chars[122] = _Char122;
				chars[123] = _Char123;
				chars[124] = _Char124;
				chars[125] = _Char125;
				chars[126] = _Char126;
				chars[127] = _Char127;

				float2 uvSize = float2(_TileX, _TileY);
				float2 charSize = float2(_RowLength, _RowColumns);
				float2 uvTile = 1.0 / uvSize;
				float2 charTile = 1.0 / charSize;

				float charLimit = uvSize.x * uvSize.y;
				float charPosition = floor(i.uv.x * charSize.x) + floor((1.0 - i.uv.y) * charSize.y) * charSize.x;
				float charCurrent = round(chars[clamp(charPosition, 0, 127)]);
				charCurrent = min(charCurrent, charLimit);
				if (charCurrent < 0) {
					charCurrent += floor(charCurrent / charLimit) * charLimit;
				}

				float2 uvPosition = (fmod(i.uv * charSize, 1.0) / uvSize);
				float2 uvOffset = float2(fmod(charCurrent, uvSize.x) * uvTile.x, 1.0 - ((floor(charCurrent / uvSize.x) + 1.0) * uvTile.y));
				float2 uv = uvPosition + uvOffset;

				fixed4 col = tex2D(_MainTex, uv);
				clip(col.a - _Cutoff);
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}

			ENDCG
		}
	}
	FallBack "Unlit/Transparent"
}
