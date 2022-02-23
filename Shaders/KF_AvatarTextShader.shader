// VRChat Avatar Text Shader by KillFrenzy

Shader "Unlit/KF_VRChatAvatarTextShader"
{
    Properties
    {
		[Enum(Off,0,Front,1,Back,2)] _Culling ("Culling Mode", Int) = 2
        _MainTex("Texture", 2D) = "white" {}
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
    }
	
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull [_Culling]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
			
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
				if (!facing) {
					i.uv.x = 1.0 - i.uv.x;
				}
				
				// Really, I'd like to have arrays using shader properties
				float chars[64];
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
				
				float2 uvSize = float2(_TileX, _TileY);
				float2 charSize = float2(_RowLength, _RowColumns);
				float2 uvTile = 1.0 / uvSize;
				float2 charTile = 1.0 / charSize;
				
				float charPosition = floor(i.uv.x * charSize.x) + floor((1.0 - i.uv.y) * charSize.y) * charSize.x;
				float charCurrent = round(chars[max(min(charPosition, 63), 0)]);
				
				charCurrent = max(min(charCurrent, uvSize.x * uvSize.y), 0.0);
				
				float2 uvPosition = (fmod(i.uv * charSize, 1.0) / uvSize);
				float2 uvOffset = float2(fmod(charCurrent, uvSize.x) * uvTile.x, 1.0 - ((floor(charCurrent / uvSize.x) + 1.0) * uvTile.y));
				float2 uv = uvPosition + uvOffset;
				
                fixed4 col = tex2D(_MainTex, uv);
				clip(col.a - 0.01);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
			
            ENDCG
        }
    }
	FallBack "Unlit/Transparent"
}
