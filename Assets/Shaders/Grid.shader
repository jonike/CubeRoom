Shader "CubeRoom/Grid"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" { }
        _LineColor ("LineColor", Color) = (1, 1, 1, 1)
        _AlphaColor ("AlphaColor", Color) = (1, 1, 1, 1)
        _AlphaScale ("Alpha Scale", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        
        Pass
        {
            ZWrite On
            ColorMask 0
        }
        
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _LineColor;
            fixed4 _AlphaColor;
            float _AlphaScale;

            struct a2v
            {
                float4 vertex: POSITION;
                float3 normal: NORMAL;
                float4 texcoord: TEXCOORD0;
            };

            struct v2f
            {
                float4 pos: SV_POSITION;
                float3 worldNormal: TEXCOORD0;
                float3 worldPos: TEXCOORD1;
                float2 uv: TEXCOORD2;
            };

            v2f vert(a2v v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                return o;
            }

            fixed4 frag(v2f i): SV_TARGET
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLight = normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed4 texColor = tex2D(_MainTex, i.uv);

                fixed3 albedo = texColor.r * _LineColor.rgb + (1 - texColor.r) * _AlphaColor.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT * albedo;
                fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldNormal, worldLight));
                fixed3 color = ambient + diffuse;
                fixed alpha = texColor.a * saturate(texColor.r + _AlphaScale);

                return fixed4(color, alpha);
            }
            ENDCG
            
        }
    }
    FallBack "Transparent/VertexLit"
}