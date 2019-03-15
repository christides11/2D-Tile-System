// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Transparent Texture2D" {
Properties {
    _MainTex ("Tex", 2DArray) = "" {}
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#pragma target 2.0
            #pragma multi_compile_fog
			#pragma require 2darray

            #include "UnityCG.cginc"

            struct appdata_t {
				float3 texcoord : TEXCOORD0;
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
				float3 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            //sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.texcoord.z = v.texcoord.z;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			UNITY_DECLARE_TEX2DARRAY(_MainTex);

            half4 frag (v2f i) : SV_Target
            {
                fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_MainTex, i.texcoord);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
        ENDCG
    }
}

}
