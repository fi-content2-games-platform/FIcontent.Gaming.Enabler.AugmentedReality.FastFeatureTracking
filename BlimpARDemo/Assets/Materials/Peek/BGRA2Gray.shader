Shader "Custom/BGRA2Gray" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque"  "IgnoreProjector"="True" "ForceNoShadowCasting"="True" }
		LOD 200
		Pass {
		Tags { "Lightmode"="Always" }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		#pragma fragmentoption ARB_precision_hint_fastest
		
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform fixed2 _MainTex_ST;

		struct v2f {
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
 			fixed3 c = tex2D(_MainTex, IN.uv);
 #if SHADER_API_MOBILE
 			fixed gray = dot(c.rgb, fixed3(0.299f, 0.587f, 0.114f));
 #else
 			fixed gray = dot(c.rgb, fixed3(0.114f, 0.587f, 0.299f));
 #endif
 			return fixed4(gray,gray,gray,1);
		}
		ENDCG
		
		}
	} 
	FallBack "Diffuse"
}
