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
        
        #pragma surface surf CRLambert addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass
        #pragma target 3.0

        #include "CubeRoom.cginc"

        fixed4 _AOColor;
        sampler2D _AOTex;
        sampler2D _DetailTex;
        sampler2D _BumpMap;
        float _BumpScale;
        
        struct Input
        {
            float2 uv_AOTex;
            float2 uv_DetailTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputCR o)
        {
            float occlusion = tex2D(_AOTex, IN.uv_AOTex);
            fixed3 baseColor = _Color.rgb * tex2D(_DetailTex, IN.uv_DetailTex);
            o.Albedo = lerp(baseColor * _AOColor.rgb, baseColor, occlusion);
            fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_DetailTex));
            normal.z = normal.z / _BumpScale;
            o.Normal = normal;
            o.Alpha = _Color.a;
        }
        ENDCG
        
    }
    FallBack "Diffuse"
}
