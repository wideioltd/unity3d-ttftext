Shader "Custom/MyShader0" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass {
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
 Lighting On
 ZWrite Off		
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
 
#include "UnityCG.cginc"
		

		sampler2D _MainTex;
		float _dx=0.2;
		float _dy=0.2;
		float _sx=0.3;
		float _sy=0.3;

		
		
struct v2f {
        float4  pos : SV_POSITION; 
        float2  uv : TEXCOORD0;
}; 

v2f vert (appdata_base v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    o.uv=v.texcoord;
    //o.color = v.normal * 0.5 + 0.5;
    return o;
}
 
		
half4 frag (v2f i) : COLOR {
		    float2 v=i.uv;
		    v.x=v.x*_sx+_dx;
		    v.y=v.y*_sy+_dy;
		    
			half4 c = tex2D (_MainTex, v);
			float hx=(i.uv.x-0.5)*2;
			float hy=(i.uv.y-0.5)*2;
			c.a=1.0-max(abs(hx),abs(hy));
			return c;
} 

		
		
ENDCG
}
	} 
	FallBack Off
}

