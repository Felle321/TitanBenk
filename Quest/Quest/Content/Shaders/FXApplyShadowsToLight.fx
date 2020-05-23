#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture : register(t0);

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float Zoom;
float CameraX;
float CameraY;
float depth;
float MaskWidth;
float MaskHeight;
float TextureWidth;
float TextureHeight;
float TextureX;
float TextureY;
float FrameOffsetX;
float FrameOffsetY;

Texture2D Mask;

sampler2D MaskSampler = sampler_state
{
	Texture = <Mask>;
};


struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 maskCoordinates = float2((TextureX + (input.TextureCoordinates.x * TextureWidth - FrameOffsetX) / MaskWidth) * Zoom, (TextureY + (input.TextureCoordinates.y * TextureHeight - FrameOffsetY) / MaskHeight) * Zoom);
	float maskDepth = tex2D(MaskSampler, maskCoordinates).r;	
	float bleedThrough = tex2D(MaskSampler, maskCoordinates).g;
	float delta = depth - maskDepth;

	if (delta < 0)
		return lerp(float4(0, 0, 0, 0), tex2D(SpriteTextureSampler, input.TextureCoordinates), 1 - bleedThrough) * input.Color;
	else if(delta < 0.015)
		return lerp(float4(0, 0, 0, 0), tex2D(SpriteTextureSampler, input.TextureCoordinates), (delta / 0.015)) * input.Color * bleedThrough;
	else
		return tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color * bleedThrough;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};