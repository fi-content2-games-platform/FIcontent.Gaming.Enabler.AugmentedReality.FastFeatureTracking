Shader "Custom/Alpha2Gray" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass {
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		sampler2D _MainTex;

		struct v2f {
    		float4 pos : SV_POSITION;
		    half2 uv : TEXCOORD0;
		};

		v2f vert (appdata_base v)
		{
		    v2f o;
		    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord;
		    return o;
		}

		fixed4 frag (v2f IN) : COLOR 
		{
 			fixed c = tex2D(_MainTex, IN.uv).a;
 			return c;
		}
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
