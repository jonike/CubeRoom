Shader "CubeRoom/ColorShadow"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ColorShadow ("Shadow Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        
        CGPROGRAM
        
        #pragma surface surf CRLambert addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass
        #pragma target 3.0

        #include "CubeRoom.cginc"

        struct Input
        {
            float3 worldNormal;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputCR o)
        {
            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a;
        }
        ENDCG
        
    }
    FallBack "Diffuse"
}
