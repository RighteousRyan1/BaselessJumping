float2 oCoordinates;
float oLightPower;
float oLightDistance;
float4 oLightColor;

sampler TexSampler : register(s0);
float4 LightingShader(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(TexSampler, coords);
    
    color *= oLightColor * oLightPower;
    
    color.rgba -= distance(coords, oCoordinates) * oLightDistance;
    
    return color;
}

technique Lighting
{
    pass LightingPass
    {
        PixelShader = compile ps_2_0 LightingShader();
    }
}