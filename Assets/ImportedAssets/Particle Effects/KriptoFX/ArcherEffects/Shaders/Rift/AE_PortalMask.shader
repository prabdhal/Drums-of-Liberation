Shader "KriptoFX/AE/Portal/PortalMask"
{
Properties {
	_Mask ("Mask", 2D) = "white" {}
	_Cutout("Cutout", Range(0, 1)) = 1
}
	SubShader 
	{
		Tags { "RenderType"="Tranperent" "Queue"="Geometry-100" "IgnoreProjector" = "True" }
		ColorMask 0
		ZWrite Off
		ZTest Always
		Cull Back
		Stencil 
		{
			Ref 2
			Comp always
			Pass replace
		}
		
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed _Cutout;
			sampler2D _Mask;
			float4 _Mask_ST;

			struct appdata 
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f 
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata v) 
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			half4 frag(v2f i) : SV_Target 
			{
				fixed mask = tex2D(_Mask, i.uv);
				clip(mask - (1 - _Cutout));
				return 0;
			}
		ENDCG
		}
	}
}
