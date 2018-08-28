
fixed4 _Color;
fixed4 _ColorShadow;

inline fixed4 LightingCRLambertOld(SurfaceOutput s, half3 lightDir, half atten)
{
    fixed3 lightColor = _LightColor0.rgb;
    
    half3 normalDir = normalize(s.Normal);
    float ndl = max(0, dot(normalDir, lightDir));

    _ColorShadow = lerp(_Color, _ColorShadow, _ColorShadow.a);
    s.Albedo = lerp(s.Albedo * _ColorShadow.rgb, s.Albedo, ndl * atten * 2);
    fixed3 diffuse = s.Albedo * lightColor;
    
    fixed4 color;
    color.rgb = diffuse;
    color.a = s.Alpha;
    return color;
}


inline fixed4 LightingCRBlinnPhongOld(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
{
    half3 normalDir = normalize(s.Normal);
    lightDir = normalize(lightDir);
    half3 halfDir = normalize(lightDir + viewDir);

    float ndl = max(0, dot(normalDir, lightDir));
    float ndh = max(0, dot(normalDir, halfDir));

    atten = min(1, atten * 2);;
    fixed3 lightColor = _LightColor0.rgb;

    fixed3 diff = ndl * atten;
    
    _ColorShadow = lerp(_Color, _ColorShadow, _ColorShadow.a);
    fixed3 shadowColor = lerp(_ColorShadow.rgb, 1, diff);

    float spec = pow(ndh, s.Specular * 128.0) * s.Gloss;
    spec *= atten;

    fixed3 diffuse = s.Albedo * lightColor * shadowColor;
    fixed3 specular = _SpecColor.rgb * lightColor * spec;

    fixed4 color;
    color.rgb = diffuse + specular;
    color.a = s.Alpha;
    return color;
}

/**********/

struct SurfaceOutputCR
{
    half atten;
    fixed3 Albedo;
    fixed3 Normal;
    fixed3 Emission;
    half Specular;
    fixed Gloss;
    fixed Alpha;
    fixed Rim;
};


inline half4 LightingCRLambert(inout SurfaceOutputCR s, UnityGI gi)
{
    half3 lightDir = gi.light.dir;
    half3 lightColor = gi.light.color.rgb;

    #if defined(UNITY_PASS_FORWARDBASE)
        half atten = min(1, s.atten * 2);
    #else
        half atten = 1;
    #endif

    half3 normalDir = normalize(s.Normal);
    s.Normal = normalDir;

    fixed ndl = max(0, dot(normalDir, lightDir));

    half diff = ndl;

    #if !(POINT) && !(SPOT)
        diff *= atten;
    #endif

    _ColorShadow = lerp(_Color, _ColorShadow, _ColorShadow.a);
    fixed3 shadowColor = lerp(_ColorShadow.rgb, 1, diff);

    fixed3 diffuse = s.Albedo * lightColor.rgb * shadowColor;

    #if (POINT || SPOT)
        diffuse *= atten;
    #endif

    fixed4 color;
    color.rgb = diffuse;
    color.a = s.Alpha;

    #ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
        color.rgb += s.Albedo * gi.indirect.diffuse;
    #endif

    return color;
}

inline half4 LightingCRLambert_Deferred(SurfaceOutputCR s, UnityGI gi, out half4 outGBuffer0, out half4 outGBuffer1, out half4 outGBuffer2)
{
    UnityStandardData data;
    data.diffuseColor = s.Albedo;
    data.occlusion = 1;
    data.specularColor = 0;
    data.smoothness = 0;
    data.normalWorld = s.Normal;

    UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

    half4 emission = half4(s.Emission, 1);

    #ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
        emission.rgb += s.Albedo * gi.indirect.diffuse;
    #endif

    return emission;
}

inline void LightingCRLambert_GI(inout SurfaceOutputCR s, UnityGIInput data, inout UnityGI gi)
{
    gi = UnityGlobalIllumination(data, 1.0, s.Normal);

    s.atten = data.atten;	//transfer attenuation to lighting function
    gi.light.color = _LightColor0.rgb;	//remove attenuation
}

inline fixed4 LightingCRLambert_PrePass(SurfaceOutputCR s, half4 light)
{
    fixed4 c;
    c.rgb = s.Albedo * light.rgb;
    c.a = s.Alpha;
    return c;
}

inline half4 LightingCRBlinnPhong(inout SurfaceOutputCR s, half3 viewDir, UnityGI gi)
{
    half3 lightDir = gi.light.dir;
    half3 lightColor = gi.light.color.rgb;

    #if defined(UNITY_PASS_FORWARDBASE)
        half atten = min(1, s.atten * 2);
    #else
        half atten = 1;
    #endif

    half3 normalDir = normalize(s.Normal);
    half3 halfDir = normalize(lightDir + viewDir);

    fixed ndl = max(0, dot(normalDir, lightDir));
    float ndh = max(0, dot(normalDir, halfDir));

    fixed3 diff = ndl;

    #if !(POINT) && !(SPOT)
        diff *= atten;
    #endif

    _ColorShadow = lerp(_Color, _ColorShadow, _ColorShadow.a);
    fixed3 shadowColor = lerp(_ColorShadow.rgb, 1, diff);

    float spec = pow(ndh, s.Specular * 128.0) * s.Gloss;
    spec *= atten;

    fixed3 diffuse = s.Albedo * lightColor * shadowColor;
    fixed3 specular = _SpecColor.rgb * lightColor * spec;

    #if (POINT || SPOT)
        diffuse *= atten;
    #endif

    fixed4 color;
    color.rgb = diffuse + specular;
    color.a = s.Alpha;

    #ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
        color.rgb += s.Albedo * gi.indirect.diffuse;
    #endif

    return color;
}
inline half4 LightingCRBlinnPhong_Deferred(SurfaceOutputCR s, half3 viewDir, UnityGI gi, out half4 outGBuffer0, out half4 outGBuffer1, out half4 outGBuffer2)
{
    UnityStandardData data;
    data.diffuseColor = s.Albedo;
    data.occlusion = 1;
    // PI factor come from StandardBDRF (UnityStandardBRDF.cginc:351 for explanation)
    data.specularColor = _SpecColor.rgb * s.Gloss * (1 / UNITY_PI);
    data.smoothness = s.Specular;
    data.normalWorld = s.Normal;

    UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

    half4 emission = half4(s.Emission, 1);

    #ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
        emission.rgb += s.Albedo * gi.indirect.diffuse;
    #endif

    return emission;
}

inline void LightingCRBlinnPhong_GI(inout SurfaceOutputCR s, UnityGIInput data, inout UnityGI gi)
{
    gi = UnityGlobalIllumination(data, 1.0, s.Normal);

    s.atten = data.atten;	//transfer attenuation to lighting function
    gi.light.color = _LightColor0.rgb;	//remove attenuation
}

inline fixed4 LightingCRBlinnPhong_PrePass(SurfaceOutputCR s, half4 light)
{
    fixed spec = light.a * s.Gloss;

    fixed4 c;
    c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * spec);
    c.a = s.Alpha;
    return c;
}