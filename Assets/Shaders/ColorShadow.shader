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
        
        #pragma surface surf ColorShadow addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass
        #pragma target 3.0

        fixed4 _Color;
        fixed4 _ColorShadow;

        struct Input
        {
            float3 worldNormal;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        inline fixed4 LightingColorShadow(SurfaceOutput s, half3 lightDir, half atten)
        {
            half3 normalDir = normalize(s.Normal);
            float ndl = max(0, dot(normalDir, lightDir));

            fixed3 lightColor = _LightColor0.rgb;
            s.Albedo = lerp(s.Albedo * _ColorShadow.rgb, s.Albedo, ndl * atten);
            fixed3 diffuse = s.Albedo * lightColor;
            
            fixed4 color;
            color.rgb = diffuse;
            color.a = s.Alpha;
            return color;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a;
        }
        ENDCG
        
    }
    FallBack "Diffuse"
}
