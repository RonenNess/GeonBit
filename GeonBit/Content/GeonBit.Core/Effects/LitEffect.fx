#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// world / view / projection matrix
matrix WorldViewProjection;

// ambient light value
float4 AmbientColor = float4(1, 1, 1, 1);

// diffuse color
float4 DiffuseColor = float4(1, 1, 1, 1);

// rendering alpha
float Alpha = 1.0f;

// main texture
texture MainTexture;

// main texture sampler
sampler2D MainTextureSampler = sampler_state {
	Texture = (MainTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};


// vertex shader input
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

// vertex shader output
struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float3 Normal : TEXCOORD0;
	float2 TextureCoordinate : TEXCOORD1;
};

// do basic vertex processing that is relevant for all lighting techniques.
VertexShaderOutput BasicVertexProcessing(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;
	output.Normal = input.Normal.xyz;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

// main vertex shader for flat lighting
VertexShaderOutput FlatLightingMainVS(in VertexShaderInput input)
{
	// get basic output
	VertexShaderOutput output = BasicVertexProcessing(input);

	// return vertex output
	return BasicVertexProcessing(input);
}

// main pixel shader for flat lighting
float4 FlatLightingMainPS(VertexShaderOutput input) : COLOR
{
	// get texture color
	float4 textureColor = tex2D(MainTextureSampler, input.TextureCoordinate);

	// store original texture alpha
	float originalAlpha = textureColor.a;

	// multiply by diffuse color, ambient, etc.
	float4 ret = saturate(textureColor * input.Color * AmbientColor * DiffuseColor);

	// reset original alpha so it won't be affected by lighting
	ret.a = originalAlpha * Alpha;

	// return final
	return ret;
}

// default technique with flat lighting 
technique FlatLighting
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL FlatLightingMainVS();
		PixelShader = compile PS_SHADERMODEL FlatLightingMainPS();
	}
};