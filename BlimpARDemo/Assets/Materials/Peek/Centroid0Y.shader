Shader "Custom/Centroid0Y" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Reference (RGB)", Color) = (0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque"  "IgnoreProjector"="True" "ForceNoShadowCasting"="True" }
		LOD 200
		Pass {
		ZTest Always Cull Off ZWrite Off
	  	Fog { Mode off }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		#pragma fragmentoption ARB_precision_hint_fastest
		
		#include "UnityCG.cginc"
		
		uniform sampler2D _MainTex;
		uniform fixed2 _MainTex_ST;
		fixed3 _Color;
		fixed4 offsets;

		struct v2f 
		{
			float4 pos : POSITION;
			fixed4 uv01 : TEXCOORD0;
			fixed4 uv23 : TEXCOORD1;
			fixed4 uv45 : TEXCOORD2;
			fixed4 uv67 : TEXCOORD3;
		};

		v2f vert (appdata_base v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			fixed4 uv=v.texcoord.xyxy;

			o.uv01 =  uv + offsets * fixed4(0,0,0,1);
			o.uv23 =  uv + offsets * fixed4(0,2,0,3);
			o.uv45 =  uv + offsets * fixed4(0,4,0,5);
			o.uv67 =  uv + offsets * fixed4(0,6,0,7);
	
		    return o;
		}

		fixed colorDistance(fixed3 c)
		{
 			fixed3 delta=(_Color-c);
 			return dot(delta,delta)<0.05;
		}
		
		fixed distanceSample(fixed2 uv)
		{
			return colorDistance(tex2D(_MainTex, uv).rgb);
		}

		fixed4 frag (v2f i) : COLOR 
		{
			fixed s = distanceSample(i.uv01.xy);
 			fixed t = distanceSample(i.uv01.zw);
 			fixed v = s+t;
 			fixed d = i.uv01.y*s+i.uv01.w*t;
 			s = distanceSample(i.uv23.xy);
			t = distanceSample(i.uv23.zw); v+=s+t; d+=i.uv23.y*s+i.uv23.w*t;
			s = distanceSample(i.uv45.xy);
			t = distanceSample(i.uv45.zw); v+=s+t; d+=i.uv45.y*s+i.uv45.w*t;
			s = distanceSample(i.uv67.xy);
			t = distanceSample(i.uv67.zw); v+=s+t; d+=i.uv67.y*s+i.uv67.w*t;
			d*=0.125;
			v*=0.125;

  			return fixed4(d,v,0,1);
 		}
 		
 		ENDCG
		}
	} 
	FallBack "Diffuse"
}
