﻿Shader "Judgement Night/FOV Object" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Contrast ("Color Contrast", Range(1,4)) = 1
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
		_EmissionTex ("Emission Texture (RGB)", 2D) = "white" {}
		_EmissionStrength ("Emission Strength", Range(0,4)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Stencil {
			Ref 1
			Comp equal
		}
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _EmissionTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_EmissionTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color, _EmissionColor;
		half _Contrast, _EmissionStrength;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			fixed4 e = tex2D(_EmissionTex, IN.uv_EmissionTex) * _EmissionColor * _EmissionStrength;

			// Metallic and smoothness come from slider variables
			o.Emission = e;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
