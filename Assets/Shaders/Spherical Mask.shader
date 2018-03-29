Shader "Judgement Night/Spherical Mask" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Contrast("Color Contrast", Range(1,4)) = 1
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MaskTex ("MaskTex (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		//_Position ("World position", Vector) = (0,0,0,0)
		_Radius ("Sphere Radius", Range(0,100)) = 0
		_Softness ("Sphere Softness", Range(0,100)) = 0
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
		_EmissionTex ("Emission Texture (RGB)", 2D) = "white" {} 
		_EmissionStrength ("Emission Strength", Range(0,4)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _EmissionTex, _MaskTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_EmissionTex;
			float2 uv_MaskTex;
			float3 worldPos; 
		};

		half _Glossiness;
		half _Metallic;
		half _Softness, _Radius;
		fixed4 _Color, _EmissionColor;
		half _Contrast, _EmissionStrength;

		// set those parameters as uniform to be accesible and shared by all materials with this shader.
		uniform float4 GlobalMask_Position;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
			// use half instead of float and multiplication instead of division for performance purposes
			half gray = (c.r + c.g + c.b) * 0.333; 
			fixed4 cGray = fixed4(gray, gray, gray, 1);

			// Emission
			fixed4 e = tex2D(_EmissionTex, IN.uv_EmissionTex) * _EmissionColor * _EmissionStrength;

			half dist = distance(GlobalMask_Position, IN.worldPos);
			// outcome will be always between 0 and 1
			half sum = saturate((dist - _Radius) / -_Softness);

			fixed4 lerpColor = lerp(cGray, c * _Contrast, sum);
			fixed4 lerpEmission = lerp(fixed4(0,0,0,0), e, sum);

			o.Albedo = lerpColor.rgb;
			o.Emission = lerpEmission.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
