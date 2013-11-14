Shader "Custom/GrayTexture" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque"  "IgnoreProjector"="True" "ForceNoShadowCasting"="True" }
		LOD 200
		Pass {
		Tags { "Lightmode"="Always" }
		ZTest Always Cull Off ZWrite Off
	  	Fog { Mode off }    
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		#pragma fragmentoption ARB_precision_hint_fastest
		
		#include "UnityCG.cginc"
		
		uniform sampler2D _MainTex;
		uniform fixed2 _MainTex_ST;
		fixed3 _Color;

		struct v2f 
		{
    		fixed4 pos : SV_POSITION;
		    fixed2 uv : TEXCOORD0;
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
 			half2 col=tex2D(_MainTex, IN.uv).rg;
 			return fixed4(col.xxx,1);
 		}
		ENDCG
		
		}
	} 
	FallBack "Diffuse"
}
