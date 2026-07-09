Shader "Unlit/CurvedRotationEmission"
{ 
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_EmissionMap ("Emission", 2D) = "black" {}
		_EmissionStrength ("Emission Strength", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "DisableBatching" = "True"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work 
			#pragma multi_compile_fog
				
#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float4 color : COLOR;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 color : TEXCOORD1;
			UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float _CurveStrength;

		sampler2D _EmissionMap;
		float _EmissionStrength;

		v2f vert(appdata v)
		{
			v2f o;

			float _Horizon = 100.0f;
			float _FadeDist = 50.0f;

			float4 rotVert = v.vertex;
			rotVert.z = v.vertex.z * cos(_Time.y * 3.14f) - v.vertex.x * sin(_Time.y * 3.14f);
			rotVert.x = v.vertex.z * sin(_Time.y * 3.14f) + v.vertex.x * cos(_Time.y * 3.14f);

			o.vertex = UnityObjectToClipPos(rotVert);

			float dist = UNITY_Z_0_FAR_FROM_CLIPSPACE(o.vertex.z);

			o.vertex.y -= _CurveStrength * dist * dist * _ProjectionParams.x;

			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			o.color = v.color;

			UNITY_TRANSFER_FOG(o, o.vertex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			// Sample the texture
			fixed4 col = tex2D(_MainTex, i.uv) * i.color;

			// Sample the emission map
			fixed4 emission = tex2D(_EmissionMap, i.uv);
			col.rgb += emission.rgb * _EmissionStrength;

			// Apply fog
			UNITY_APPLY_FOG(i.fogCoord, col);

			return col;
		}

			ENDCG
		}
	}
}
