float4x4 World;
texture NormalMap;
texture SecondMap;
texture Extract;
int Running;
float Time;
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
	//float4 secondTex = tex2D(SecondSampler, input.TexC);
	float4 firstTex = tex2D(NormalSampler, input.TexC);
	float4 extractTex = tex2D(ExtractSampler, input.TexC);
	//return float4(extractTex.rgb, extractTex.a + 0.5);
	if (Running)
	{
		float multiplier = 1 - input.TexC.y - 0.5;
		if (multiplier < -0.1)
			multiplier = 0;
		float depth1 = firstTex.a / 4;
		//float depth2 = secondTex.a;
		float depth3 = extractTex.a / 3;
		float depth = firstTex.a / 3;
		//float depth = depth2 < 0.0006 ? depth1 : depth1 + depth2 * 1.5;
		depth = depth1 >= 0.0006 ? depth + (multiplier * multiplier * multiplier) * depth * 100: depth;
		depth = depth3 > 0.0004 ? depth - depth3 : depth;
		//return float4((depth + 0.5).xxx, 1);
		//return float4((multiplier).xxx, 1);
		float3 normal = firstTex.rgb;
		// float3 normal = depth2 < 0.0006 ? firstTex.rgb : secondTex.rgb;
		normal = depth3 > 0.0004 ? extractTex.rgb : normal;
		firstTex = float4(normal, depth);
	}
	else
	{
		firstTex = float4 (firstTex.rgb, firstTex.a / 3);
	}
	//return float4 (firstTex.rgb, firstTex.a + 0.5);
	/*if (firstTex.a < 0.0032)
		return float4 (firstTex.rgb, firstTex.a + 0.5);*/

	float4 nUp = tex2D(NormalSampler, input.TexC + float2(0, 10/512));
	float4 nDown = tex2D(NormalSampler, input.TexC - float2(0, 10/512));
	float4 nLeft = tex2D(NormalSampler, input.TexC - float2(0.01, 0));
	float4 nRight = tex2D(NormalSampler, input.TexC + float2(0.01, 0));



	float timeSegment = Time / 8;
	float3 windDirection = float3(1, 1, 0.5);
	float3 normal = firstTex.rgb * 2 - 1;
	float depth = firstTex.a;
	
	for (int i = 0; i < 8; i++)
	{
		float C = dot(-normal, windDirection) * depth + depth * 4;
		float cUp = abs(dot(-nUp.rgb, windDirection) * nUp.a + nUp.a * 4);
		float cDown = abs(dot(-nDown.rgb, windDirection) * nDown.a + nDown.a * 4);
		//float cLeft = abs(dot(-nLeft.rgb, windDirection) * nLeft.a + nLeft.a * 4);
		//float cRight = abs(dot(-nRight.rgb, windDirection) * nLeft.a + nLeft.a * 4);

		C = abs(C);
		//C = C == 0.1 ? 0.11 : C;
		float limit = 0.1;
		float newDepth = depth - timeSegment * (C - limit);
		nUp.a = nUp.a - timeSegment * (cUp - limit);
		nDown.a = nDown.a - timeSegment * (cDown - limit);
		//nLeft.a = nLeft.a - timeSegment * (cLeft - limit);
		//nRight.a = nRight.a - timeSegment * (cRight - limit);
		newDepth = max(newDepth, 0.001);
		depth = newDepth;
		//normal = normalize(normal + float3(0, 0, 10 * timeSegment * (C - limit)));
		nUp.xyz = normalize(nUp.xyz + float3(0, 0, 10 * timeSegment * (cUp - limit)));
		nDown.xyz = normalize(nDown.xyz + float3(0, 0, 10 * timeSegment * (cDown - limit)));
		//nLeft.xyz = normalize(nLeft.xyz + float3(0, 0, 10 * timeSegment * (cLeft - limit)));
		//nRight.xyz = normalize(nRight.xyz + float3(0, 0, 10 * timeSegment *(cRight - limit)));

		normal = float3(normal.x, (nUp.a - nDown.a), normal.z + 10 * timeSegment * (C - limit));
		normal = normalize(normal);
	}

	//normal = normalize(normal + nUp.xyz + nDown.xyz);
	

	return float4 ((normal + 1) / 2, depth + 0.5);
	
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
