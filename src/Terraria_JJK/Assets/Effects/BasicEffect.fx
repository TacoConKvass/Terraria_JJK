#pragma warning (disable : 4717)

matrix WorldViewProjection;

struct PixelShaderInput
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
    float2 TextureCoord : TEXCOORD0;
};

struct VertexShaderInput
{

    float4 Position : POSITION;
    float4 Color : COLOR0;
    float2 TextureCoord : TEXCOORD0;
};

PixelShaderInput MainVertexShader(VertexShaderInput input)
{
    PixelShaderInput output = (PixelShaderInput) 0;
    output.Color = input.Color;
	output.TextureCoord = input.TextureCoord;
    output.Position = mul(input.Position, WorldViewProjection);
    
    return output;
}

float4 TextureturePixelShader(PixelShaderInput input) : COLOR0
{
    return input.Color;
}

technique Technique1
{
    pass Texture
    {
        VertexShader = compile vs_2_0 MainVertexShader();
        PixelShader = compile ps_2_0 TextureturePixelShader();
    }
}