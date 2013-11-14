Shader "Custom/ColorDistance" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Reference (RGB)", Color) = (0,0,1)
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

		fixed colorDistance(fixed3 c)
		{
			//fixed3 delta=_Color-c;
			//fixed dist=3-dot(delta,delta);
 			//return dist/3;
 			
 			//fixed3 delta=abs(_Color-c);	
 			//return delta.x+delta.y+delta.z;
 			
 			fixed3 delta=(_Color-c);	
 			fixed dist=dot(delta,delta);
 			return dist<0.05;
		}
		
		fixed4 frag (v2f IN) : COLOR 
		{
 			fixed3 c = tex2D(_MainTex, IN.uv);
 			fixed d=colorDistance(c);
 			//d=d>0.9?1:0;
 			return fixed4(d,d,d,1);
 		}
		ENDCG
		
		}
	} 
	FallBack "Diffuse"
}
