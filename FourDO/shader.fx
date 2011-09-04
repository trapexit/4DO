Texture2D <float4> xTexture;
sampler TextureSampler;

struct VS_IN
{	
	float4 pos : POSITION;
	float2 cords : textcoord;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float2 cords : textcoord;
};

PS_IN vs_main(VS_IN input)
{	
	PS_IN output = (PS_IN)0;	
	output.pos = input.pos;
	output.cords = input.cords;	
	return output;
}

float4 ps_main(PS_IN input) : SV_Target
{
	float2 temp;
	temp = float2(input.cords[0] ,input.cords[1]);
	return xTexture.Sample(TextureSampler, temp);
}