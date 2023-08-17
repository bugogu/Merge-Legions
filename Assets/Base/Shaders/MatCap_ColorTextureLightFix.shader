// MatCap Shader, (c) 2015-2017 Jean Moreno
// WorldSpaced by (c) 2017 KumoKairo

Shader "MatCap/Vertex/ColorTextureLightFix"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MatCap ("MatCap (RGB)", 2D) = "white" {}
        _Border ("Border", Range (0.1, 0.5)) = 0.43
	}
	
	Subshader
	{
		Tags { "RenderType"="Opaque" }
		
		Pass
		{
			Tags { "LightMode" = "Always" }
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_fog
				#include "UnityCG.cginc"
				
				struct v2f
				{
					float4 pos	: SV_POSITION;
					float2 uv 	: TEXCOORD0;
					float3 cap	: TEXCOORD1;
					UNITY_FOG_COORDS(1)
				};

				uniform float4 _MainTex_ST;
                uniform float _Border;

				v2f vert (appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                    float3 referenceDirection = float3(0.0, 0.0, 1.0);
					
                    float3 worldNorm = mul((float3x3)unity_ObjectToWorld, v.normal);
                    o.cap = worldNorm * _Border + 0.5f;
					
					UNITY_TRANSFER_FOG(o, o.pos);

					return o;
				}
				uniform float4 _Color;
				uniform sampler2D _MainTex;
				uniform sampler2D _MatCap;
				
				float4 frag (v2f i) : COLOR
				{
					fixed4 tex = tex2D(_MainTex, i.uv);
					float4 mc = tex2D(_MatCap, i.cap.xy);
					mc = _Color * mc * unity_ColorSpaceDouble;
					//mc = float4(i.cap.xyy, 1.0);
					mc.rgb = tex.rgb + (mc.rgb * 2.0) - 1.0;
					UNITY_APPLY_FOG(i.fogCoord, mc);

					return mc;
				}
			ENDCG
		}
	}
	
	Fallback "VertexLit"
}