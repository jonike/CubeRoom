Shader "CubeRoom/Basic"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ColorShadow ("Shadow Color", Color) = (0, 0, 0, 1)
        
        _AOColor ("AO Color", Color) = (0, 0, 0, 1)
        _AOTex ("AO Texture", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        
        CGPROGRAM
        
        #pragma surface surf CRLambert addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass
        #pragma target 3.0

        #include "CubeRoom.cginc"

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        sampler2D _AOTex;
        fixed4 _AOColor;

        struct Input
        {
            float2 uv_AOTex;
        };

        void surf(Input IN, inout SurfaceOutputCR o)
        {
            float occlusion = tex2D(_AOTex, IN.uv_AOTex);
            o.Albedo = lerp(_Color.rgb * _AOColor.rgb, _Color.rgb, occlusion);
            o.Alpha = _Color.a;
        }
        ENDCG
        
    }
    FallBack "Diffuse"
}