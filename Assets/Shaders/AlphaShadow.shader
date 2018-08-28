Shader "CubeRoom/AlphaShadow"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ShadowTex ("Shadow Texture", 2D) = "white" { }
        _InLow ("In Low", range(0, 1)) = 0
        _InHigh ("In High", range(0, 1)) = 1
        _OutLow ("Out Low", range(0, 1)) = 0
        _OutHigh ("Out High", range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        
        CGPROGRAM
        
        #pragma surface surf Lambert addshadow fullforwardshadows alpha:blend exclude_path:deferred exclude_path:prepass
        #pragma target 3.0

        sampler2D _ShadowTex;
        fixed4 _Color;
        fixed _InLow;
        fixed _InHigh;
        fixed _OutLow;
        fixed _OutHigh;

        struct Input
        {
            float2 uv_ShadowTex;
            float3 worldNormal;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = _Color.rgb;
            fixed alpha = tex2D(_ShadowTex, IN.uv_ShadowTex).r;
            alpha = (_OutHigh - _OutLow) * smoothstep(_InLow, _InHigh, alpha) + _OutLow;
            o.Alpha = (1 - alpha) * _Color.a;
        }
        ENDCG
        
    }
    FallBack "Transparent/VertexLit"
}
