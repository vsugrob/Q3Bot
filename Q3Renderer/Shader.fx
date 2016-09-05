struct VS_OUT {
	float4 pos_wvp : POSITION;
	float2 tex0 : TEXCOORD0;
	float2 tex1 : TEXCOORD1;
	float3 normal : TEXCOORD2;
	float3 pos : TEXCOORD3;
};

float4x4 worldViewProj : WORLDVIEWPROJECTION;

VS_OUT Transform0 ( float4 pos : POSITION, float3 normal: NORMAL, float2 tex0 : TEXCOORD0, float2 tex1 : TEXCOORD1 ) {
	VS_OUT output = ( VS_OUT ) 0;
	output.pos_wvp = mul ( pos, worldViewProj );
	output.tex0 = tex0;
	output.tex1 = tex1;
	output.normal = normal;
	output.pos = pos;
	
	return	output;
}

Texture tex;
Texture lightmap;

sampler texSampler      = sampler_state { texture = <tex>     ; mipFilter = LINEAR; };
sampler lightmapSampler = sampler_state { texture = <lightmap>; mipFilter = LINEAR; };

float4 LightPos;
float4 EyePos;
float Time = 0.0f;
bool LightmapsEnabled = false;
bool LightEnabled = false;

void Shade0 ( in float2 tex0 : TEXCOORD0, in float2 tex1 : TEXCOORD1, in float3 normal : TEXCOORD2, in float3 pos : TEXCOORD3, out float4 color : COLOR0 ) {
	float4 texColor = tex2D ( texSampler, tex0 );
	
	if ( LightmapsEnabled ) {
		float4 lightmapColor = tex2D ( lightmapSampler, tex1 );
		color = texColor * lightmapColor * 4;
	} else {
		color = texColor;
	}
	
	if ( LightEnabled ) {
		float3 lightRay = LightPos - pos;
		float dist = length ( lightRay );
		
		normal = normalize ( normal );
		lightRay = normalize ( lightRay );
		float lnDot = dot ( normal, lightRay );
		float diff = 0.0f;
		float spec = 0.0f;
		
		if ( lnDot > 0.0f ) {
			// Diffuse
			diff = lnDot;
			
			// Specular
			float3 refl = normalize ( 2 * normal - normalize ( lightRay ) );
			float3 eyeRay = normalize ( EyePos - pos );
			float reDot = dot ( eyeRay, refl );
			
			if ( reDot > 0 )
				spec = pow ( reDot, 8 );
		}
		
		color = saturate ( ( color * diff + spec ) * pow ( 200.0f / dist, 2 ) );
	}
}

technique VertexShake {
	pass P0 {
		CullMode = None;
		VertexShader = compile vs_3_0 Transform0 ();
		PixelShader = compile ps_3_0 Shade0 ();
	}
}