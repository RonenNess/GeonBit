#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// world matrix
matrix World;

// world / view / projection matrix
matrix WorldViewProjection;

// how many active lights we have
int ActiveLightsCount = 0;

// ambient light value
float4 AmbientColor = float4(1, 1, 1, 1);

// diffuse color
float4 DiffuseColor = float4(1, 1, 1, 1);

// rendering alpha
float Alpha = 1.0f;

// main texture
texture MainTexture;

// max lights count
#define MAX_LIGHTS_COUNT 7

// light sources.
// note: 
//	- lights with range 0 = directional lights (in which case light pos is direction).
//	- lights with intensity 0 = disabled lights.
float3 LightColor[MAX_LIGHTS_COUNT];
float3 LightPosition[MAX_LIGHTS_COUNT];
float LightIntensity[MAX_LIGHTS_COUNT];
float LightRange[MAX_LIGHTS_COUNT];
float LightSpecular[MAX_LIGHTS_COUNT];

// main texture sampler
sampler2D MainTextureSampler = sampler_state {
	Texture = (MainTexture);
};


// vertex shader input
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

// vertex shader output
struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;
	float2 TextureCoordinate : TEXCOORD1;

	// the position in world space (in oppose to "Position" which is world-view-projection space) + SV_POSITION is unreachable from pixel shader.
	float3 WorldPos : TEXCOORD2;
};

// do basic vertex processing that is relevant for all lighting techniques.
VertexShaderOutput BasicVertexProcessing(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.Normal = input.Normal.xyz;
	output.TextureCoordinate = input.TextureCoordinate;
	output.WorldPos = input.Position; // mul(input.Position, World).xyz;
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

	// get pixel position
	float3 position = input.WorldPos;

	// calc lights strength
	float4 LightsColor = AmbientColor;
	for (int i = 0; i < ActiveLightsCount; ++i) {

		// calc light strength based on range
		float disFactor = 1.0f - (distance(position, LightPosition[i]) / LightRange[i]);

		// out of range? skip this light.
		if (disFactor <= 0) { continue; }

		// add light to pixel
		LightsColor.rgb += (LightColor[i]) * (LightIntensity[i] * (disFactor * disFactor));
	}

	// multiply by diffuse color, ambient, etc.
	float4 ret = saturate(textureColor * LightsColor * DiffuseColor);

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