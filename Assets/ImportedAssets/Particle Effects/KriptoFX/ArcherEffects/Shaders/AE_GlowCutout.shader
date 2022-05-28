// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "KriptoFX/AE/GlowCutout" {
	Properties{
	[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,1)
	_TimeScale("Time Scale", Vector) = (1,1,1,1)
	_MainTex("Noise Texture", 2D) = "white" {}
	_Threshold("Threshold", Float) = 0.77
	_ThresholdStep("Threshold step", Float) = 0.08
	_Cutout("_Cutout", Range(0, 1)) = 1
	}

		Category{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
						Blend SrcAlpha One
						Cull Off
						ZWrite Off
						//Offset -1, -1

			SubShader {
				Pass {

					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag

					#include "UnityCG.cginc"

					sampler2D _MainTex;
					float4 _TintColor;
					float4 _TimeScale;
					float _Threshold;
					float _ThresholdStep;
					float _Cutout;

					struct appdata_t {
						float4 vertex : POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
						float3 normal : NORMAL;
					};

					struct v2f {
						float4 vertex : POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
					};

					float4 _MainTex_ST;

					v2f vert(appdata_t v)
					{
						v2f o;
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.color = v.color;
						o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
						return o;
					}

					half4 frag(v2f i) : COLOR
					{
						half2 mask = tex2D(_MainTex, _Time.xx * _TimeScale.xy+ i.texcoord);
						half2 tex = tex2D(_MainTex, mask + _Time.xx * _TimeScale.zw+ i.texcoord);
						mask.r = tex2D(_MainTex, i.texcoord).g;
						float4 res = 0;
						res.r = step(tex.r, _Threshold);
						res.r -= step(tex.r, _Threshold - _ThresholdStep);
						res.r *= tex.g;
						res = i.color * res.r * _TintColor;
						res.a = res.a * step(mask.r, _Cutout);
						return  res;
					}
					ENDCG
				}
			}
	}

}
