float4x4 World;
float4x4 View;
float4x4 Projection;
texture DiffuseTex;
texture NormalTex;
float3 LightDirection;  // Direction from light source to surface
float3 CameraPosition;
int Toggle;
float2 Ratio;
float Time;
float3 WindDirection;

sampler DiffuseSampler = sampler_state
{
	Texture = <DiffuseTex>;
	addressu = Wrap;
	addressv = Wrap;
};

sampler NormalSampler = sampler_state
{
	Texture = <NormalTex>;
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
	float2 WorldTexC : TEXCOORD1;
	float2 Ratio : TEXCOORD2;
};

struct VertexShaderOutput
{
	float4 Position : POSITION;
	float3 Normal : TEXCOORD0;
	float3 Tangent : TEXCOORD1;
	float3 Bitangent : TEXCOORD2;
	float2 TexC : TEXCOORD3;
	float3 WorldPosition : TEXCOORD4;
	float2 WorldTexC : TEXCOORD5;
	float2 Ratio : TEXCOORD6;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	output.Position = mul(mul(worldPosition, View), Projection);

	output.Normal = mul(input.Normal, (float3x3)World);
	output.Tangent = mul(input.Tangent, (float3x3)World);
	output.Bitangent = mul(input.Bitangent, (float3x3)World);
	output.TexC = input.TexC;
	output.WorldTexC = input.WorldTexC;
	output.WorldPosition = worldPosition.xyz;
	output.Ratio = input.Ratio;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	//if(Toggle)
	//return float4(tex2D(NormalSampler, input.TexC).aaa, 1);
	const float delta = 0.02;
const float maxheight = 0;
const float maxDepth = 2;
float d = -1;

float3 SN = normalize(input.Normal);
float3 ST = normalize(input.Tangent);
float3 SB = normalize(input.Bitangent);

float3x3 Tangent2World = float3x3(ST, SB, SN);
float3x3 World2Tangent = transpose(Tangent2World);

float3 SN1 = normalize(float3(0,1,0));
float3 ST1 = normalize(float3(1,0,0));
float3 SB1 = normalize(float3(0,0,1));

float3x3 Tangent2World1 = float3x3(ST1, SB1, SN1);
float3x3 World2Tangent1 = transpose(Tangent2World1);

float3 VIn = mul(normalize(input.WorldPosition - CameraPosition), World2Tangent);
float2 dVInS = VIn.xy / (-VIn.z) * float2(1, -1) * 0.1;
float3 VIns2 = mul(normalize(input.WorldPosition - CameraPosition), World2Tangent1);
float2 dVIns2 = VIns2.xy / (-VIns2.z) * float2(1, -1) * 0.1;

float3 erosion = mul(normalize(input.WorldPosition - WindDirection), World2Tangent);
float2 erosionDirection = erosion.xy / (-erosion.z) * float2(1, -1);
//return float4(VIn.xxx, 1);

bool above = true;
bool indent = false;
float dabove = 0, dbelow = 0;
float probeabove, probebelow;
probeabove = maxheight + tex2D(NormalSampler, input.TexC + dVInS * d).a - 0.0;
//return float4(probeabove.xxx, 1);
//for (d = delta; d <= maxheight + delta + delta / 10; d += delta)
for (d = delta; d <= maxDepth; d += delta)
{
	if (above)
	{
		float neighbour = tex2D(NormalSampler, input.TexC + dVInS * d + Time * erosionDirection * 0.1).a;
		float probe1 = tex2D(NormalSampler, input.TexC + dVInS * d).a - 0.0;

		float probe = probe1;// (probe1 + neighbour) / 2 < probe1 ? (probe1 + neighbour) / 2 : probe1;
		//if (probe == (probe1 + neighbour) / 2) indent = true;
		/*if (indent)
			probe = (probe + tex2D(NormalSampler, input.TexC + dVInS * d + Time).a) / 2;*/
		if (/*sample depth is below the surface of height map*/d > probe)
		{   // Our vector has gone below the surface
			dbelow = d;
			probebelow = probe;
			above = false;
			
		}
		else
		{   // Our vector is still above the surface
			dabove = d;
			probeabove = probe;
		}
	}
}
//if(probebelow == 0)
//return float4(probebelow/4, 0, 0, 1);
d = (dabove * (probebelow - probeabove) - delta * probeabove) / (probebelow - probeabove - delta);
//return float4(d.xxx, 1);
//if (Toggle) d = 0;  // Gives Normal map
float2 tex = input.TexC + dVInS * d;

tex.x = tex.x < 0 ? 0 : tex.x > 1 ? 1 : tex.x;
tex.y = tex.y < 0 ? 0 : tex.y > 1 ? 1 : tex.y;

float3 neighbourNormal = normalize(tex2D(NormalSampler, input.TexC + dVInS * d + Time * erosionDirection * 0.1).rgb * 2 - 1);
float3 neighbourNormal1 = normalize(tex2D(NormalSampler, input.TexC + dVInS * d - Time * erosionDirection * 0.1).rgb * 2 - 1);
float3 N = normalize(tex2D(NormalSampler, input.TexC + dVInS * d).rgb * 2 - 1);   // Get replacement normal

float3 combine = normalize(N + neighbourNormal + neighbourNormal1);

N = combine;

//if (indent)
//{
//	N = combine;
//}
N = mul(N, Tangent2World);

//return float4(N, 1);
float brightness = 0.4 + 0.6 * saturate(dot(N, -LightDirection));
//return float4(brightness.xxx, 1);

float2 worldTex = input.WorldTexC + dVIns2 * d * Ratio;
worldTex.x = worldTex.x < 0 ? 0 : worldTex.x > 1 ? 1 : worldTex.x;
worldTex.y = worldTex.y < 0 ? 0 : worldTex.y > 1 ? 1 : worldTex.y;


float2 newTex = input.TexC + dVInS * d;


float up = tex2D(NormalSampler, newTex + float2(0, 0.01)).a;
float down = tex2D(NormalSampler, newTex - float2(0, 0.01)).a;
float left = tex2D(NormalSampler, newTex - float2(0.01, 0)).a;
float right = tex2D(NormalSampler, newTex + float2(0.01, 0)).a;

float hoz = abs(left - right);
float vert = abs(up - down);
float slope = max(hoz, vert);
float deep = tex2D(NormalSampler, newTex).a;
deep = deep > 0.5 ? deep - 0.5 : 0;
//float darkener = hoz > 0.3 ? hoz - 0.3 : vert > 0.3 ? vert - 0.3 : 0;
float darkener = slope > 0.3 ? slope - 0.3 : 0;
darkener /= 2.0;
deep /= 2.5;


//float normal = (tex2D(DiffuseSampler, (input.WorldTexC + dVIns2 * d * Ratio)).rgb + tex2D(DiffuseSampler, (input.WorldTexC + dVIns2 * d * Ratio + erosionDirection * Time)).rgb)/2;
if(!Toggle)
return float4(tex2D(DiffuseSampler, (input.WorldTexC + dVIns2 * d * Ratio) ).rgb * brightness - float3(darkener.xx, darkener * 0.8) - float3(deep.xx, deep * 0.8), 1);
//return float4(brightness.xxx, 1);                              // Show lighting or
return float4(tex2D(NormalSampler, input.TexC + dVInS * d).aaa ,1);
//return float4(tex2D(DiffuseSampler, input.TexC + dVInS * d).rgb * brightness, 1);   // Show lighted texture
}



technique TechniqueParallax
{
	pass Pass1
	{
		//FillMode = WireFrame;
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
