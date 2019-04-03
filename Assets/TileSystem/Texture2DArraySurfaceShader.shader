// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Texture2DArraySurfaceShader"
{
	Properties
	{
		_MainTex("Tex", 2DArray) = "" {}
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.5
		#include "UnityCG.cginc"

		UNITY_DECLARE_TEX2DARRAY(_MainTex);

		struct Input
		{
			fixed2 uv_Textures;
		};

		UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
#define _Color_arr Props
			UNITY_DEFINE_INSTANCED_PROP(float, _TextureIndex)
#define _TextureIndex_arr Props
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_Textures, UNITY_ACCESS_INSTANCED_PROP(_TextureIndex_arr, _TextureIndex)) * UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color));
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		ENDCG
				}
				FallBack "Diffuse"
}

