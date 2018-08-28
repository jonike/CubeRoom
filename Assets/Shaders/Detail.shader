Shader "CubeRoom/Detail"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ColorShadow ("Shadow Color", Color) = (0, 0, 0, 1)
        _AOColor ("AO Color", Color) = (0, 0, 0, 1)

        _AOTex ("AO Texture", 2D) = "white" { }

        _DetailTex ("Detail Texture", 2D) = "white" { }
        _BumpMap ("Normal Map", 2D) = "bump" { }
        _BumpScale ("Bump Scale", Float) = 1.0

    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        
        CGPROGRAM
        
        #pragma surface surf ColorShadow addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass
        #pragma target 3.0

        sampler2D _AOTex;
        sampler2D _DetailTex;
        sampler2D _BumpMap;
        fixed4 _Color;
        fixed4 _ColorShadow;
        fixed4 _AOColor;
        float _BumpScale;
        
        struct Input
        {
            float2 uv_AOTex;
             float2 uv_DetailTex;
            float3 worldNormal;
        };


        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        struct SurfaceOutputAO
        {
            fixed3 Albedo;
            fixed3 Normal;
            fixed3 Emission;
            half Specular;
            fixed Gloss;
            fixed Alpha;
            fixed3 Occlusion;
        };

        inline fixed4 LightingColorShadow(SurfaceOutputAO s, half3 lightDir, half atten)
        {
            half3 normalDir = normalize(s.Normal);
            float ndl = max(0, dot(normalDir, lightDir));

            fixed3 occlusion = s.Occlusion.r;
            fixed3 lightColor = _LightColor0.rgb;
            s.Albedo = lerp(s.Albedo * _ColorShadow.rgb, s.Albedo, ndl * atten);
            s.Albedo = lerp(s.Albedo * _AOColor.rgb, s.Albedo, occlusion);
            fixed3 diffuse = s.Albedo * lightColor;
            
            fixed4 color;
            color.rgb = diffuse;
            color.a = s.Alpha;
            return color;
        }

        void surf(Input IN, inout SurfaceOutputAO o)
        {
            o.Albedo = _Color.rgb * tex2D(_DetailTex, IN.uv_DetailTex);;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_DetailTex)) * _BumpScale;
            o.Alpha = _Color.a;
            o.Occlusion = tex2D(_AOTex, IN.uv_AOTex);
        }
        ENDCG
        
    }
    FallBack "Diffuse"
}
