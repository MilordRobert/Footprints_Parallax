//float4x4 World;
//texture NormalMap;
//texture SecondMap;
//float4x4 SecondWorld;
//float2 TopRight;
//float2 TopLeft;
//float2 BottomLeft;
//float Length;
//float4x4 SecondWorldInverse;
//
//// TODO: add effect parameters here.
//sampler NormalSampler = sampler_state
//{
//	Texture = <NormalMap>;
//	addressu = Wrap;
//	addressv = Wrap;
//};
//
//sampler SecondSampler = sampler_state
//{
//	Texture = <SecondMap>;
//	addressu = Wrap;
//	addressv = Wrap;
//};
//
//
//struct VertexShaderInput
//{
//	float4 Position : POSITION;
//	float3 Normal : NORMAL0;
//	float3 Tangent : TANGENT0;
//	float3 Bitangent : BINORMAL0;
//	float2 TexC : TEXCOORD0;
//	// TODO: add input channels such as texture
//	// coordinates and vertex colors here.
//};
//
//struct VertexShaderOutput
//{
//	float4 Position : POSITION;
//	float3 Normal : TEXCOORD0;
//	float3 Tangent : TEXCOORD1;
//	float3 Bitangent : TEXCOORD2;
//	float2 TexC : TEXCOORD3;
//	float4 Pos : TEXCOORD4;
//	float3 NormalSec : TEXCOORD5;
//	float3 TangentSec : TEXCOORD6;
//	float3 BitangentSec : TEXCOORD7;
//
//	// TODO: add vertex shader outputs such as colors and texture
//	// coordinates here. These values will automatically be interpolated
//	// over the triangle, and provided as input to your pixel shader.
//};
//
//VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
//{
//	VertexShaderOutput output;
//	float4 worldPosition = mul(input.Position, World);
//	worldPosition = float4(worldPosition.xz * float2(0.5, 0.5), 0.5, 1);
//	//float4 viewPosition = mul(worldPosition, View);
//	output.Position = worldPosition;
//	output.TexC = input.TexC;
//	output.Pos = input.Position;
//
//	output.Normal = mul(input.Normal, (float3x3)World);
//	output.Tangent = mul(input.Tangent, (float3x3)World);
//	output.Bitangent = mul(input.Bitangent, (float3x3)World);
//
//	output.NormalSec = (mul(input.Normal, ((float3x3)SecondWorldInverse)));
//	output.TangentSec = (mul(input.Tangent, ((float3x3)SecondWorldInverse)));
//	output.BitangentSec = (mul(input.Bitangent, ((float3x3)SecondWorldInverse)));
//
//	output.TexC = input.TexC;
//	//output.Position = input.Position;
//	// TODO: add your vertex shader code here.
//
//	return output;
//}
//
//float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
//{
//
//	float4 pos = mul(input.Pos, SecondWorld);
//
//	float2 newTex = float2(((pos + 2) / Length).xz * float2(1, -1));
//	float4 secondTex = tex2D(SecondSampler, newTex);
//	float4 firstTex = tex2D(NormalSampler, input.TexC);
//
//	//float left = tex2D(NormalSampler, input.TexC - float2(1.0 / 512, 0)).a - 0.1356;
//	//float right = tex2D(NormalSampler, input.TexC + float2(1.0 / 512, 0)).a - 0.1356;
//	//float up = tex2D(NormalSampler, input.TexC - float2(0, 1.0 / 512)).a - 0.1356;
//	//float down = tex2D(NormalSampler, input.TexC + float2(0, 1.0 / 512)).a - 0.1356;
//	//float left2 = tex2D(NormalSampler, newTex - float2(1.0 / 512, 0)).a - 0.1356;
//	//float right2 = tex2D(NormalSampler, newTex + float2(1.0 / 512, 0)).a - 0.1356;
//	//float up2 = tex2D(NormalSampler, newTex - float2(0, 1.0 / 512)).a - 0.1356;
//	//float down2 = tex2D(NormalSampler, newTex + float2(0, 1.0 / 512)).a - 0.1356;
//
//	//if(false)
//	if (pos.x <= 2 && pos.x >= -2 && pos.z <= 2 && pos.z >= -2)
//	{
//		//return float4(0, 1, 0, 1);
//		float3 SN = normalize(input.Normal);
//		float3 ST = normalize(input.Tangent);
//		float3 SB = normalize(input.Bitangent);
//		float3x3 Tangent2World = float3x3(-ST,  SB, SN);
//		float3x3 World2Tangent = transpose(Tangent2World);
//		float3x3 transform = mul(mul(Tangent2World, SecondWorldInverse), World2Tangent);
//		//return float4(ST.zzz, 1);
//		/*if (secondTex.a - firstTex.a >= 0.05)
//			return secondTex;
//
//		if (firstTex.a - secondTex.a >= 0.05)
//			return firstTex;*/
//
//		
//		/*float3 horizontal = normalize(float3(1 , 0, (left - right) * 10));
//		float3 vertical = normalize(float3(0, 1 , -(up - down) * 10));*/
//		//float3 horizontal2 = mul(normalize(float3(1, 0, (left2 - right2) * 10)), first2Second);
//		//float3 vertical2 = mul(normalize((float3(0, 1, -(up2 - down2) * 10))), first2Second);
//		//float3 normalise = (normalize(cross(horizontal, vertical)) + (normalize(cross(horizontal2, vertical2))) + 1) / 2;
//
//		//float3 colour = secondTex.a > 0.13 ? secondTex.rgb : firstTex.rgb;
//		// secondTex;
//
//		float depth = secondTex.a - 0.1356 + firstTex.a - 0.1356;
//		//depth /= 2;
//		//if (Toggle)
//		//	depth = 1 - exp(-depth * 2);
//		//else
//		//	depth = max(secondTex.a - 0.1356, firstTex.a - 0.1356);
//		//depth /= 2;
//		//depth *= exp(-depth);
//		//float3 normal = secondTex.a > firstTex.a ? secondTex.rgb * 2 - 1 : firstTex.rgb * 2 - 1;
//		//float3 normal = secondTex.a > firstTex.a ? mul(secondTex.rgb * 2 - 1, transform) : firstTex.rgb * 2 - 1;
//		//float3 normal = float3(0, 1, 0);
//		float3 normal = normalize(secondTex.rgb + firstTex.rgb);
//		float depth1 = firstTex.a;// - 0.1;
//		float depth2 = secondTex.a;// -0.1;
//		float diff = depth1 - depth2;
//
//		depth =  depth1 <= 0 ? depth2 : depth2 <= 0 ? depth1 : diff > 5 ? depth1 : max(depth1, depth2) + 1 * min(depth1, depth2);
//		float3 secondNormal =  mul(secondTex.rgb * 2 - 1, transform);
//		//float3 secondNormal = secondTex.rgb * 2 - 1;
//		/*float diff = secondTex.a - firstTex.a;
//		float cap = 0.2;
//		bool similar = diff < cap && diff > -cap;
//		normal = similar ? secondTex.rgb + cap * firstTex.rgb : diff < 0 ? firstTex.rgb : secondTex.rgb;*/
//		//normal = depth1 <= 0.28 ? secondNormal : depth2 <= 0.28 ? firstTex.rgb * 2 - 1 : 1 * secondNormal + 0.3 * (firstTex.rgb * 2 - 1);
//		normal = depth1 <= 0.0432 ? secondNormal  : depth2 <= 0.0432 ? firstTex.rgb * 2 - 1 : 1 * secondNormal + 0.3 * (firstTex.rgb * 2 - 1);
//		//if (abs(secondTex.a - firstTex.a) <= 0.1)
//		//{
//			//normal = normalize(mul(secondTex.rgb * 2 - 1, transform) + (firstTex.rgb * 2 - 1));
//			//normal = normalize(mul(secondTex.rgb * 2 - 1, transform) *  (secondTex.a / (secondTex.a + firstTex.a)) + (firstTex.rgb * 2 - 1) * (firstTex.a / (secondTex.a + firstTex.a)));
//		/*}
//		else
//		{
//			normal = secondTex.a > firstTex.a ? mul(secondTex.rgb * 2 - 1, transform) : firstTex.rgb * 2 - 1;
//		}*/
//		//normal = secondTex.a > firstTex.a ? mul(secondTex.rgb * 2 - 1, transform) : firstTex.rgb * 2 - 1;
//		//if (depth1 == 0)
//		//{
//		//	normal = float3(1, 0, 0);
//		//	depth = 1;
//		//}
//		normal = normalize(normal);
//		normal = (normal + 1) / 2;
//		return float4(normal, depth/2);
//		//return float4(1, 0, 0, 1);
//	}
//	//return float4(1,0,0,1);
//	// TODO: add your pixel shader code here.
//
//	return (tex2D(NormalSampler, input.TexC));
//	/*float4 colour = secondTex;
//	colour.a = colour.a <= 0.2769 ? 0.042 : colour.a - 0.27;
//	return colour;*/
//}
//
//technique Technique1
//{
//	pass Pass1
//	{
//		// TODO: set renderstates here.
//
//		VertexShader = compile vs_3_0 VertexShaderFunction();
//		PixelShader = compile ps_3_0 PixelShaderFunction();
//	}
//}
float4x4 World;
texture NormalMap;
texture SecondMap;
float4x4 SecondWorld;
float2 TopRight;
float2 TopLeft;
float2 BottomLeft;
float Length;
float size;
float Osize;
float4x4 SecondWorldInverse;

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


struct VertexShaderInput
{
	float4 Position : POSITION;
	float3 Normal : NORMAL0;
	float3 Tangent : TANGENT0;
	float3 Bitangent : BINORMAL0;
	float2 TexC : TEXCOORD0;
	// TODO: add input channels such as texture
	// coordinates and vertex colors here.
};

struct VertexShaderOutput
{
	float4 Position : POSITION;
	float3 Normal : TEXCOORD0;
	float3 Tangent : TEXCOORD1;
	float3 Bitangent : TEXCOORD2;
	float2 TexC : TEXCOORD3;
	float4 Pos : TEXCOORD4;
	float3 NormalSec : TEXCOORD5;
	float3 TangentSec : TEXCOORD6;
	float3 BitangentSec : TEXCOORD7;

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
	output.Pos = input.Position;

	output.Normal = mul(input.Normal, (float3x3)World);
	output.Tangent = mul(input.Tangent, (float3x3)World);
	output.Bitangent = mul(input.Bitangent, (float3x3)World);

	output.NormalSec = (mul(input.Normal, ((float3x3)SecondWorldInverse)));
	output.TangentSec = (mul(input.Tangent, ((float3x3)SecondWorldInverse)));
	output.BitangentSec = (mul(input.Bitangent, ((float3x3)SecondWorldInverse)));

	output.TexC = input.TexC;
	//output.Position = input.Position;
	// TODO: add your vertex shader code here.

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	//return float4(1,0,0,1);
	float4 pos = mul(input.Pos, SecondWorld);

	float2 newTex = float2(((pos + 1) * 2 / Length).xz * float2(1, -1));
	float2 newTexA = float2(((input.Pos + 1) * 2 / Length).xz * float2(1, -1));
	float4 secondTex = tex2D(SecondSampler, newTex);
	float4 firstTex = tex2D(NormalSampler, newTexA);

	float3 SN = normalize(input.Normal);
	float3 ST = normalize(input.Tangent);
	float3 SB = normalize(input.Bitangent);
	float3x3 Tangent2World = float3x3(-ST, SB, SN);
	float3x3 World2Tangent = transpose(Tangent2World);
	float3x3 transform = mul(mul(Tangent2World, SecondWorld), World2Tangent);

	//float left = tex2D(NormalSampler, input.TexC - float2(1.0 / 512, 0)).a - 0.1356;
	//float right = tex2D(NormalSampler, input.TexC + float2(1.0 / 512, 0)).a - 0.1356;
	//float up = tex2D(NormalSampler, input.TexC - float2(0, 1.0 / 512)).a - 0.1356;
	//float down = tex2D(NormalSampler, input.TexC + float2(0, 1.0 / 512)).a - 0.1356;
	//float left2 = tex2D(NormalSampler, newTex - float2(1.0 / 512, 0)).a - 0.1356;
	//float right2 = tex2D(NormalSampler, newTex + float2(1.0 / 512, 0)).a - 0.1356;
	//float up2 = tex2D(NormalSampler, newTex - float2(0, 1.0 / 512)).a - 0.1356;
	//float down2 = tex2D(NormalSampler, newTex + float2(0, 1.0 / 512)).a - 0.1356;

	//if(false)
	bool inA = input.Pos.x <= 1 && input.Pos.x >= -1 && input.Pos.z <= 1 && input.Pos.z >= -1;
	bool inB = pos.x <= 1 && pos.x >= -1 && pos.z <= 1 && pos.z >= -1;

	//return float4(1, 0, 0, 1);
	if (inA && inB)
		//if (pos.x <= 2 && pos.x >= -2 && pos.z <= 2 && pos.z >= -2)
	{
		
		//return firstTex;
		//return float4(0, 1, 0, 1);

		//return float4(ST.zzz, 1);
		/*if (secondTex.a - firstTex.a >= 0.05)
		return secondTex;

		if (firstTex.a - secondTex.a >= 0.05)
		return firstTex;*/


		/*float3 horizontal = normalize(float3(1 , 0, (left - right) * 10));
		float3 vertical = normalize(float3(0, 1 , -(up - down) * 10));*/
		//float3 horizontal2 = mul(normalize(float3(1, 0, (left2 - right2) * 10)), first2Second);
		//float3 vertical2 = mul(normalize((float3(0, 1, -(up2 - down2) * 10))), first2Second);
		//float3 normalise = (normalize(cross(horizontal, vertical)) + (normalize(cross(horizontal2, vertical2))) + 1) / 2;

		//float3 colour = secondTex.a > 0.13 ? secondTex.rgb : firstTex.rgb;
		// secondTex;

		float depth = secondTex.a + firstTex.a;
		//depth /= 2;
		//if (Toggle)
		//	depth = 1 - exp(-depth * 2);
		//else
		//	depth = max(secondTex.a - 0.1356, firstTex.a - 0.1356);
		//depth /= 2;
		//depth *= exp(-depth);
		//float3 normal = secondTex.a > firstTex.a ? secondTex.rgb * 2 - 1 : firstTex.rgb * 2 - 1;
		//float3 normal = secondTex.a > firstTex.a ? mul(secondTex.rgb * 2 - 1, transform) : firstTex.rgb * 2 - 1;
		//float3 normal = float3(0, 1, 0);
		float3 normal = normalize(secondTex.rgb + firstTex.rgb);
		float depth1 = firstTex.a/6;// - 0.1;
		float depth2 = secondTex.a/6;// -0.1;
		float diff = depth1 - depth2;
		//return float4(0, 0, 0, diff);
		depth = depth1 <= 0 ? depth2 : depth2 <= 0 ? depth1 : diff > 5 ? 1 : max(depth1, depth2) + 0.6 * min(depth1, depth2);
		float3 secondNormal = mul(secondTex.rgb * 2 - 1, transform);
		//float3 secondNormal = secondTex.rgb * 2 - 1;
		/*float diff = secondTex.a - firstTex.a;
		float cap = 0.2;
		bool similar = diff < cap && diff > -cap;
		normal = similar ? secondTex.rgb + cap * firstTex.rgb : diff < 0 ? firstTex.rgb : secondTex.rgb;*/
		//normal = depth1 <= 0.28 ? secondNormal : depth2 <= 0.28 ? firstTex.rgb * 2 - 1 : 1 * secondNormal + 0.3 * (firstTex.rgb * 2 - 1);
		normal = depth1 <= 0.002 ? secondNormal : depth2 <= 0.002 ? firstTex.rgb * 2 - 1 : 0.8 * secondNormal + 0.3 * (firstTex.rgb * 2 - 1);
		//if (abs(secondTex.a - firstTex.a) <= 0.1)
		//{
		//normal = normalize(mul(secondTex.rgb * 2 - 1, transform) + (firstTex.rgb * 2 - 1));
		//normal = normalize(mul(secondTex.rgb * 2 - 1, transform) *  (secondTex.a / (secondTex.a + firstTex.a)) + (firstTex.rgb * 2 - 1) * (firstTex.a / (secondTex.a + firstTex.a)));
		/*}
		else
		{
		normal = secondTex.a > firstTex.a ? mul(secondTex.rgb * 2 - 1, transform) : firstTex.rgb * 2 - 1;
		}*/
		//normal = secondTex.a > firstTex.a ? mul(secondTex.rgb * 2 - 1, transform) : firstTex.rgb * 2 - 1;
		//if (depth1 == 0)
		//{
		//	normal = float3(1, 0, 0);
		//	depth = 1;
		//}
		normal = normalize(normal);
		normal = (normal + 1) / 2;
		return float4(normal, depth + 0.5);
		//return float4(1, 0, 0, 1);
	}
	else if (inA)
		//return firstTex;
	return float4(firstTex.rgb, firstTex.a/6 + 0.5);
	else if (inB)
	{

		secondTex.rgb = mul(secondTex.rgb * 2 - 1, transform);
		return float4((secondTex.rgb + 1) / 2, secondTex.a/6 + 0.5);
	}

	else return float4(0.5, 0.5, 1, 0.5);
	//return float4(1,0,0,1);
	// TODO: add your pixel shader code here.

	return (tex2D(NormalSampler, input.TexC));
	/*float4 colour = secondTex;
	colour.a = colour.a <= 0.2769 ? 0.042 : colour.a - 0.27;
	return colour;*/
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
