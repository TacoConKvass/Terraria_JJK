#pragma warning (disable : 4717)

matrix WorldViewProjection;
texture2D inputTexture;

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

sampler2D textureSampler = sampler_state {
	Texture = <inputTexture>;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

PixelShaderInput MainVertexShader(VertexShaderInput input)
{
	PixelShaderInput output = (PixelShaderInput) 0;
	output.Color = input.Color;
	output.TextureCoord = input.TextureCoord;
	output.Position = mul(input.Position, WorldViewProjection);

	return output;
}

float4 MainPixelShader(PixelShaderInput input) : COLOR0
{
	float4 texture_sample = tex2D(textureSampler, input.TextureCoord);
	float lightness = max(texture_sample.r, max(texture_sample.g, texture_sample.b));
	return input.Color * lightness;
}

float4 TextureSample(PixelShaderInput input) : COLOR0
{
	float4 texture_sample = tex2D(textureSampler, input.TextureCoord);
	return input.Color * texture_sample;
}

technique Technique1
{
	pass Texture
	{
		VertexShader = compile vs_2_0 MainVertexShader();
		PixelShader = compile ps_2_0 MainPixelShader();
	}
	
	pass ColoredTexture
	{
		VertexShader = compile vs_2_0 MainVertexShader();
		PixelShader = compile ps_2_0 TextureSample();
	}
}	