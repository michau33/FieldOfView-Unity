Shader "Custom/MaskedDiffuse" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MaskTex ("Lighting Mask (RGB)", 2D) = "black" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
     
        CGPROGRAM
     
        #pragma surface surf DiffuseMask fullforwardshadows
 
        #pragma target 3.0
 
        sampler2D _MainTex;
        sampler2D _MaskTex;
 
        struct Input {
            float2 uv_MainTex;
            float2 uv_MaskTex;
        };
 
        struct SurfaceOutputMask
        {
            fixed3 Albedo;
            fixed3 Normal;
            fixed3 Emission;
            half Specular;
            fixed Gloss;
            fixed Alpha;
            fixed Mask;
 
        };
 
        half4 LightingDiffuseMask (SurfaceOutputMask s, half3 lightDir,  half3 viewDir, half atten) {
            half NdotL = saturate(dot (s.Normal, lightDir));
            half4 c;
            c.rgb = lerp(s.Albedo * _LightColor0.rgb * (NdotL * atten * 2),s.Albedo,s.Mask);
            //c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
            c.a = s.Alpha;
            return c;
        }      
 
        fixed4 _Color;
 
        void surf (Input IN, inout SurfaceOutputMask o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed m = tex2D(_MaskTex,IN.uv_MaskTex).g;
            o.Albedo = c.rgb;
            o.Mask = m;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}