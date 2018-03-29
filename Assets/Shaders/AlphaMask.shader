 Shader "Custom/CloudShader"
 {
     Properties
     {
         _MainTex("Base (A)", 2D) = "white" {}
         _Color ("Color", Color) = (1.0, 0, 1.0, 1.0)
     }
     
     SubShader
     {
         Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
         Blend SrcAlpha OneMinusSrcAlpha, One One
         Cull Off
         ZWrite Off
         LOD 200
  
         CGPROGRAM
         #pragma surface surf NoLighting
         
         fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
         {
             fixed4 c;
             c.rgb = s.Albedo; 
             c.a = s.Alpha;
             return c;
         }
         
         
         uniform sampler2D _ViewMask;
         fixed4 _Color;
  
         struct Input
         {
             float2 uv_ViewMask;
             float3 viewDir;
             float4 color : COLOR;
             float3 worldPos;
         };
  
         void surf (Input IN, inout SurfaceOutput o)
         {
             const float FADEOUT = 20;
             float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
             float factor = saturate(dist / FADEOUT);
         
             o.Albedo = _Color.rgb * IN.color;
             o.Alpha = _Color.a * tex2D (_ViewMask, IN.uv_ViewMask).a;
             
             o.Alpha *= factor * saturate(abs(dot(o.Normal, normalize(IN.viewDir))) - 0.3);            
         }
         ENDCG
     } 
     FallBack "Diffuse"
 }