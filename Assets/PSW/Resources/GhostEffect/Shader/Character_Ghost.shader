// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SGame/Character/Character_Ghost" 
{
	Properties
	{
		_MainTex ("Base Texture", 2D) = "white" {}
		_Color("Color", Color) = (0.5, 0.5, 0.5, 1.0)

		_FresnelStrength ("Fresnel Strength", Range(0.01, 5)) = 0.5
		_FresnelColorStrength ("Fresnel Color Strength", Range(0, 5)) = 1
		_Alpha ("Alpha", Range(0, 5)) = 1
	}

	CGINCLUDE
	ENDCG

	SubShader
	{
		Tags { "Queue" = "Transparent+10" "RenderType" = "Transparent" }

        Pass 
		{
			Tags { "LightMode" = "DepthOnly" }
			ZWrite On
			ColorMask 0
		}

        Pass 
        {
			Blend One OneMinusSrcAlpha			
			ZWrite Off

			CGPROGRAM

			struct a2v_ghost
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_ghost
			{
				UNITY_POSITION(pos);
				float4 uvTex0 : TEXCOORD0;
				half3 worldNormal : TEXCOORD1;
				half3 worldViewDir : TEXCOORD2;
			};

			#pragma vertex VertexGhost
			#pragma fragment FragGhost

			#include "UnityCG.cginc"	
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _Color;
			half _MainTexStrength;
			half _FresnelStrength;
			half _FresnelColorStrength;
			half _Alpha;

			v2f_ghost VertexGhost(a2v_ghost v)
			{
				v2f_ghost o;
				UNITY_INITIALIZE_OUTPUT(v2f_ghost,o);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uvTex0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldNormal = worldNormal;
				o.worldViewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);

				return o;
			}

			half4 FragGhost(v2f_ghost i) : SV_Target
			{
				half4 mainColor = tex2D(_MainTex, i.uvTex0);

				half3 worldNormal = normalize(i.worldNormal);
				half3 viewDirection = normalize(i.worldViewDir);

				half fresnelStrength = pow(1.002 - saturate(dot(worldNormal, viewDirection)), _FresnelStrength);
				half3 finalColor = _Color * fresnelStrength * _FresnelColorStrength;
				finalColor = lerp(finalColor, mainColor, _MainTexStrength);

				return half4(finalColor, saturate(fresnelStrength * _Alpha));
			}

			ENDCG
        }
	}
}