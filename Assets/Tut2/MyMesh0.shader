Shader "Custom/MyMesh0" {
	Properties {
	    _clrbase ("clrbase", Color) = (1,1,1,1)
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		//_AltTex ("Alt (RGB)", 2D) = "white" {}
		_nx ("NX", Range(1,500)) = 10.0
		_ny ("NY", Range(1,500)) = 10.0

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

half4 _clrbase;
float _nx, _ny; 
float _alpha;
float _cosalpha;
float _sinalpha;


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
		    half4 c = {0,0,0,0};
		    //float nx=10; float ny=10;
//uniform float sin_alpha=sin(_alpha/100f);
//uniform float cos_alpha=cos(_alpha/100f);
		    
		    float cx=i.uv.x*_nx;
		    float cy=i.uv.y*_ny;
		    float dx=_cosalpha*cx+_sinalpha*cy;
		    float dy=-_sinalpha*cx+_cosalpha*cy;
		    if ((fmod(1000f+dx,1.0f)<0.1f) || (fmod(1000f+dy,1.0f)<0.1)) { 
			   c=_clrbase;
			} 
            return c; 
} 
 
		
ENDCG
		}
	} 
	FallBack "Diffuse"
}
