Shader "Unlit/KF_UnlitDualTexture"
{
	Properties
	{
		[Enum(Off,0,Front,1,Back,2)] _Culling ("Culling Mode", Int) = 2
		_MainTex ("Texture Main", 2D) = "white" {}
		_MainTex2 ("Texture 2", 2D) = "white" {}
		_TextureBlend ("Texture Blend", Range(0,1)) = 0
		_Dither ("Dither", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull [_Culling]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 spos : TEXCOORD2;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _MainTex2;
			float4 _MainTex2_ST;

			float _TextureBlend;
			float _Dither;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _MainTex2);
				o.spos = ComputeScreenPos(o.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = lerp(tex2D(_MainTex, i.uv), tex2D(_MainTex2, i.uv2), _TextureBlend);

				float DITHER_THRESHOLDS[16] =
				{
					1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
					13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
					4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
					16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
				};

				float2 pos = i.spos.xy / i.spos.w;
				pos *= _ScreenParams.xy;
				int index = (int(pos.x) % 4) * 4 + int(pos.y) % 4;
				if (col.a > 0.5) {
					col.a = 1.0;
				} else {
					col.a = 0.0;
				}
				clip((col.a * 1.0 - _Dither) - DITHER_THRESHOLDS[index]);

				// clip(col.a - 0.1);

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
