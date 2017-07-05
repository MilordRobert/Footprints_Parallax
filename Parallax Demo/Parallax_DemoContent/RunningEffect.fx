float4x4 World;
texture NormalMap;
texture SecondMap;
texture Extract;
int Toggle;
// TODO: add effect parameters here.

sampler NormalSampler = sampler_state
{
	Texture = <NormalMap>;
	addressu = Wrap;
	addressv = Wrap;
};

sampler SecondSampler = sampler_state
{
	Texture = <SecondMap>;
	addressu = Wrap;
	addressv = Wrap;
};

sampler ExtractSampler = sampler_state
{
	Texture = <Extract>;
	addressu = Wrap;
	addressv = Wrap;
};


struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexC : TEXCOORD0;
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexC : TEXCOORD0;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	worldPosition = float4(worldPosition.xz * float2(0.5, 0.5), 0.5, 1);
	//float4 viewPosition = mul(worldPosition, View);
	output.Position = worldPosition;
	output.TexC = input.TexC;

	//output.Position = input.Position;
	// TODO: add your vertex shader code here.

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	float4 secondTex = tex2D(SecondSampler, input.TexC);
	float4 firstTex = tex2D(NormalSampler, input.TexC);
	float4 extractTex = tex2D(ExtractSampler, input.TexC);

	if(Toggle)
	return float4 (firstTex.rgb, firstTex.a / 3 + 0.5);
	float multiplier = 1 - input.TexC.y;
	float depth1 = firstTex.a / 4;
	float depth2 = secondTex.a;
	float depth3 = extractTex.a / 3;

	float depth = depth2 < 0.0006 ? depth1 : depth1 + depth2 * 1.5;
	depth = depth1 >= 0.0006 ? depth + multiplier * multiplier * multiplier *depth * 8: depth;
	depth = depth3 > 0.0004 ? depth - depth3 : depth;
	float3 normal = depth2 < 0.0006 ? firstTex.rgb : secondTex.rgb;
	normal = depth3 > 0.0004 ? extractTex.rgb : normal;
    return float4(normal, depth + 0.5);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
