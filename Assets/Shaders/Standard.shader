Shader "CubeRoom/Standard"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ColorShadow ("Shadow Color", Color) = (0, 0, 0, 1)

        // AO
        [MaterialToggle] Ao ("Ambient Occlusion", Float) = 0
        _AOColor ("AO Color", Color) = (0, 0, 0, 1)
        _AOTex ("AO Texture", 2D) = "white" { }

        // Detial
        _DetailTex ("Detail Texture", 2D) = "white" { }

        _BumpMap ("Normal Map", 2D) = "bump" { }
        _BumpScale ("Bump Scale", Float) = 1.0

        // Smoothness
        _Smoothness ("Smoothness", Range(0, 1)) = 1
        
        // Reflection
        [MaterialToggle] Rf ("Reflection", Float) = 0
        _Cube ("Reflection Cubemap", Cube) = "_Skybox" { }
        _ReflectPower ("Reflect Power", Range(0.0, 1)) = 0.0
        _FresnelScale ("Fresnel Scale", Range(0, 3.0)) = 0.5

        // Metal
        [MaterialToggle] Mt ("Metal", Float) = 0
        _Metallic ("Metallic", Range(0.0, 1)) = 0.1
        _MetalTex ("Metalic Map", 2D) = "white" { }

        // Specular
        [MaterialToggle] Sp ("Specular", Float) = 0
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("Shininess", Range(0.01, 10)) = 0.2
        _SpecTex ("Specular Map", 2D) = "white" { }
        
        // Emission
        [MaterialToggle] Em ("Emission", Float) = 0
        _EmissionPower ("Emission Power", Range(0, 20)) = 0
        _EmissionTex ("Emission Map", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        
        CGPROGRAM
        
        #pragma surface surf CRBlinnPhong addshadow fullforwardshadows
        #pragma target 3.0
        #pragma multi_compile _ RF_ON
        #pragma multi_compile _ MT_ON
        #pragma multi_compile _ SP_ON
        #pragma multi_compile _ AO_ON
        #pragma multi_compile _ EM_ON

        #include "CubeRoom.cginc"

        sampler2D _DetailTex;
        
        sampler2D _BumpMap;
        float _BumpScale;
        
        float _Smoothness;

        samplerCUBE _Cube;
        float _ReflectPower;
        float _FresnelScale;

        float _Metallic;
        sampler2D _MetalTex;

        float _Shininess;
        sampler2D _SpecTex;

        fixed4 _AOColor;
        sampler2D _AOTex;

        float _EmissionPower;
        sampler2D _EmissionTex;


        struct Input
        {
            float2 uv_AOTex;
            float2 uv_DetailTex;
            float3 viewDir;
            float3 worldRefl;
            INTERNAL_DATA
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)
        


        void surf(Input IN, inout SurfaceOutputCR o)
        {
            fixed4 tex = tex2D(_DetailTex, IN.uv_DetailTex);
            fixed3 baseColor = _Color.rgb * tex.rgb;
            fixed alpha = _Color.a * tex.a;
            fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_DetailTex));
            normal.z = normal.z / _BumpScale;

            o.Normal = normalize(normal);
            o.Albedo = baseColor;
            o.Alpha = alpha;
            // o.Albedo += _LightAdd;

            // reflect
            float3 reflectDir = WorldReflectionVector(IN, o.Normal);
            fixed4 reflection = texCUBElod(_Cube, float4(reflectDir, (1 - _Smoothness) * 9));

            #if defined(MT_ON)
                fixed metal = tex2D(_MetalTex, IN.uv_DetailTex).r;
                fixed3 mat = 1 - metal * _Metallic;
                fixed3 met = reflection.rgb * baseColor.rgb * metal * _Metallic;

                o.Albedo *= met * mat * _Metallic;
                o.Albedo += baseColor.rgb * mat + met;
            #endif

            #if defined(RF_ON)
                half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
                fixed3 reflect = reflection.rgb * reflection.a * _ReflectPower;
                o.Albedo += reflect * pow(rim, _FresnelScale);
            #endif

            #if defined(SP_ON)
                fixed spec = tex2D(_SpecTex, IN.uv_DetailTex).r;
                o.Gloss = _SpecColor.a;
                o.Specular = spec * _Shininess;
            #endif

            #if defined(EM_ON)
                fixed emission = tex2D(_EmissionTex, IN.uv_DetailTex).r;
                o.Emission = emission * _Color * _EmissionPower;
            #endif

            #if defined(AO_ON)
                float occlusion = tex2D(_AOTex, IN.uv_AOTex);
                _AOColor.rgb = lerp(o.Albedo, _AOColor.rgb, _AOColor.a);
                o.Albedo = lerp(o.Albedo * _AOColor.rgb, o.Albedo, occlusion);
            #endif
        }
        ENDCG
        
    }
    FallBack "Shaders/Reflective/Bumped Specular"
}
