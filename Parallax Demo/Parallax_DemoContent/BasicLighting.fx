float4x4 World;
float4x4 View;
float4x4 Projection;
texture DiffuseTex;
float3 LightDirection;

// TODO: add effect parameters here.


sampler DiffuseSampler = sampler_state
{
	Texture = <DiffuseTex>;
	addressu = Wrap;
	addressv = Wrap;
};


struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexC : TEXCOORD0;
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 Normal : TEXCOORD1;
	float2 TexC : TEXCOORD0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Normal = mul(input.Normal, (float3x3)World);
    // TODO: add your vertex shader code here.
	output.TexC = input.TexC;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	float brightness = 0.4 + 0.6 * saturate(dot(input.Normal, -LightDirection));
//return float4(input.Normal.zzz, 1);
	return float4(tex2D(DiffuseSampler, input.TexC).rgb * brightness, 1);
    //return float4(1, 0, 0, 1);
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
