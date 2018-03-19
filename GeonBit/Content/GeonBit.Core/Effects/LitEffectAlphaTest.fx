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

// world matrix
matrix World;

// define white color value
float3 WhiteColor = float3(1, 1, 1);

// ambient light value
float3 AmbientColor = float3(1, 1, 1);

// diffuse color
float3 DiffuseColor = float3(1, 1, 1);

// emissive
float3 EmissiveColor = float3(0, 0, 0);

// max intensity, eg allowing lights to overflow original color
float MaxLightIntensity = 1.0f;

// rendering alpha
float Alpha = 1.0f;

// main texture
texture MainTexture;

// are we using texture?
bool TextureEnabled = false;

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

// how many active lights we have
int ActiveLightsCount = 0;

// how many of the active lights are directional (direction lights come first)
int DirectionalLightsCount = 0;

// main texture sampler
sampler2D MainTextureSampler = sampler_state {
	Texture = (MainTexture);
};

// vertex shader input
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

// vertex shader output
struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;
	float2 TextureCoordinate : TEXCOORD1;
	float4 WorldPos : TEXCOORD2;
};

// main vertex shader for flat lighting
VertexShaderOutput FlatLightingMainVS(in VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.WorldPos = mul(input.Position, World);
	output.Normal = normalize(mul(input.Normal, (float3x3)World));
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

// main pixel shader for flat lighting
float4 FlatLightingMainPS(VertexShaderOutput input) : COLOR
{
	// pixel color to return
	float4 retColor;

	// set color either from texture if enabled or white
	retColor = TextureEnabled ? tex2D(MainTextureSampler, input.TextureCoordinate) : 1.0f;

	// apply alpha
	retColor.a *= Alpha;

	// do alpha test
	if (retColor.a == 0)
		discard;

	// start calcing lights strength
	float3 LightsColor = AmbientColor + EmissiveColor;

	// process directional lights
	int i = 0;
	for (i = 0; i < DirectionalLightsCount; ++i)
	{
		// calculate angle factor
		float cosTheta = saturate(dot(LightPosition[i], input.Normal));

		// add light to pixel
		LightsColor.rgb += (LightColor[i]) * (cosTheta * LightIntensity[i]);
	}

	// now process all point lights
	for (i = DirectionalLightsCount; i < ActiveLightsCount; ++i)
	{
		// if fully lit stop here
		if (LightsColor.r > 1 && LightsColor.g > 1 && LightsColor.b > 1) { break; }

		// calculate distance and angle factors for point light
		float disFactor = 1.0f - (distance(input.WorldPos, LightPosition[i]) / LightRange[i]);

		// out of range? skip this light.
		if (disFactor > 0)
		{
			// power distance factor
			disFactor = pow(disFactor, 2);

			// calc with normal factor
			float3 lightDir = normalize(input.WorldPos - LightPosition[i]);
			float cosTheta = saturate(dot(-lightDir, input.Normal));

			// add light to pixel
			LightsColor.rgb += (LightColor[i]) * (cosTheta * LightIntensity[i] * disFactor);
		}
	}

	// make sure lights doesn't overflow
	LightsColor.rgb = min(LightsColor.rgb, MaxLightIntensity);

	// apply lighting and diffuse on return color
	retColor.rgb = saturate(retColor.rgb * LightsColor * DiffuseColor);

	// return final
	return retColor;
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
