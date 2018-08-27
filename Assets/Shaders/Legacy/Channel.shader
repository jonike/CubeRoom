Shader "Custom/Channel" {
	Properties {
		_ColorR ("Color R", Color) = (1,1,1,1)
		_ColorG ("Color G", Color) = (1,1,1,1)
		_ColorB ("Color B", Color) = (1,1,1,1)
		_ColorA ("Color A", Color) = (1,1,1,1)
		_ChannelTex ("Channel Texture", 2D) = "white" {}
	}
	SubShader {
		Tags {
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
		}
		
		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma multi_compile_fwdbase

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			fixed4 _ColorR;
			fixed4 _ColorG;
			fixed4 _ColorB;
			fixed4 _ColorA;
			sampler2D _ChannelTex;
			float4 _ChannelTex_ST;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
	
				SHADOW_COORDS(4)
			};

			v2f vert(a2v v) {
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy * _ChannelTex_ST.xy + _ChannelTex_ST.zw;
				
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				// float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				// float3 worldTangent = UnityObjectToWorldDir(v.tangent);
				// float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

				// o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				// o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				// o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				TRANSFER_SHADOW(o);

				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET {
				// float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);

				fixed3 worldLight = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 worldView = normalize(UnityWorldSpaceViewDir(i.worldPos));

				// fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				// bump.xy *= _BumpScale;
				// bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
				// bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));

				fixed4 texel = tex2D(_ChannelTex, i.uv);
				fixed3 albedo = saturate(texel.r * _ColorR.rgb + texel.g * _ColorG.rgb + texel.b * _ColorB.rgb + (1-texel.a) * _ColorA.rgb);

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(i.worldNormal, worldLight));

				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

				fixed3 color = ambient + diffuse * atten;
				return fixed4(color, 1.0);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
