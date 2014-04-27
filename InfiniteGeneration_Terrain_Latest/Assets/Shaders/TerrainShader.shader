Shader "Custom/TerrainShader" 
{
	Properties 
	{
		_TexBaseSize("TexBaseSize", Float) = 1024
		_BorderSize("BorderSize", Float) = 2
		_LightMapStr("LightMapStr", Float) = 0.5
		_RockAlphaTiling("RockAlphaTiling", Vector) = (1,1,1,1)
		_GrassAlphaTiling("GrassAlphaTiling", Vector) = (1,1,1,1)
		_NoiseAlphaTiling("NoiseAlphaTiling", Vector) = (1,1,1,1)
		_MainTex("ColorMap", 2D) = "white" {}
		_RockAlpha("RockAlpha", 2D) = "white" {}
		_GrassAlpha("GrassAlpha", 2D) = "white" {}
		_DryGrassTex("DryGrassTex", 2D) = "white" {}
		_WetGrassTex("WetGrassTex", 2D) = "white" {}
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_DirtTex("DirtTex", 2D) = "white" {}
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque"}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert nolightmap
		#pragma target 3.0
		
		sampler2D _MainTex;
		uniform sampler2D _NormalMap, _LightMap, _SplatMap;
		sampler2D _RockAlpha, _GrassAlpha, _DryGrassTex, _WetGrassTex, _DirtTex;
		sampler2D _NoiseTex;

		float _TexBaseSize, _BorderSize, _LightMapStr;
		float3 _NoiseAlphaTiling, _RockAlphaTiling, _GrassAlphaTiling;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;
		};
		
		//To reduce texture tiling a alpha map is sampled at 3 different frequencies and multiplied by the textures color		
		float3 SampleRockAlpha(float3 worldPosition)
		{
			//Each channel contains a rock like grayscale texture
			float3 rockAlpha;
			rockAlpha.r = tex2D(_RockAlpha, worldPosition.xz * 1.0/_RockAlphaTiling.r).r;
			rockAlpha.g = tex2D(_RockAlpha, worldPosition.xz * 1.0/_RockAlphaTiling.g).g;
			rockAlpha.b = tex2D(_RockAlpha, worldPosition.xz * 1.0/_RockAlphaTiling.b).b;
			
			return rockAlpha;
		}
		
		float3 SampleGrassAlpha(float3 worldPosition)
		{
			//Each channel contains a grass like grayscale texture
			float3 grassAlpha;
			grassAlpha.r = tex2D(_GrassAlpha, worldPosition.xz * 1.0/_GrassAlphaTiling.r).r;
			grassAlpha.g = tex2D(_GrassAlpha, worldPosition.xz * 1.0/_GrassAlphaTiling.g).g;
			grassAlpha.b = tex2D(_GrassAlpha, worldPosition.xz * 1.0/_GrassAlphaTiling.b).b;
			
			return grassAlpha;
		}
		
		float3 SampleNoiseAlpha(float3 worldPosition)
		{
			//This noise alpha is just for general purpose and is used when adding a little noise improves the results. b channel is not used atm.
			float3 noiseAlpha;
			noiseAlpha.r = tex2D(_NoiseTex, worldPosition.xz * 1.0/_NoiseAlphaTiling.r).r;
			noiseAlpha.g = tex2D(_NoiseTex, worldPosition.xz * 1.0/_NoiseAlphaTiling.g).g;
			noiseAlpha.b = tex2D(_NoiseTex, worldPosition.xz * 1.0/_NoiseAlphaTiling.b).b;
			
			return noiseAlpha;
		}
		
		float3 SampleRockColor(float3 rockAlpha, float3 colorMap)
		{
			float alpha = (rockAlpha.r + rockAlpha.g + rockAlpha.b)/3.0;
			//Take the base color map, which is mosty gray colors and times it by the average of the alpha map channels
			//This tends to darken the colors so times by some factor to lighten colors
			return colorMap * alpha * 1.4;
		}
		
		float3 SampleDirtColor(float3 grassAlpha, float3 worldPos)
		{
			float alpha = (grassAlpha.r + grassAlpha.g + grassAlpha.b)/3.0;
			//Take the dirt tex and times it by the average of the alpha map channels
			//Just use the grass alphas as it looks fine and saves having another alpha map for dirt
			//This tends to darken the colors so times by some factor to lighten colors
			return tex2D(_DirtTex, worldPos.xz).rgb * alpha * 2.0; 
		}
		
		float3 SampleGrassColor(float3 grassAlpha, float3 worldPos, float3 splatMap, float3 noiseTex)
		{
			float alpha = (grassAlpha.r + grassAlpha.g + grassAlpha.b)/3.0;
			
			//Theres two grass textures a dry and wet one. They are slightly different color tones
			float3 dryGrass = tex2D(_DryGrassTex, worldPos.xz).rgb;
			float3 wetGrass = tex2D(_WetGrassTex, worldPos.xz).rgb;
		
			float noise = 0.5 + noiseTex.r * 0.5;
			//blend dry and wet textures based on the b channel of splat map
			float3 grassColor = lerp(dryGrass, wetGrass, splatMap.b);
			//Take the grass color and times it by the average of the alpha map channels and some noise
			//I find the noises stops the grass from looking very flat and dull
			//This tends to darken the colors so times by some factor to lighten colors
			return grassColor * alpha * 2.0 * noise;
		}
		
		void surf(Input IN, inout SurfaceOutput o) 
		{
			//There is a border of 2 pixels around the color, light, normal and splat map that contains the vaules of the
			//neigbouring maps. This is to remove the seam created by the bilieanr filter not having the
			//correct values to sample from at the edge of the textures.This border means that the bilieanr filter can be performed correctly
			//but the texcoords need to be adjusted
			float u = 1.0/(_TexBaseSize+_BorderSize*2.0);
			float2 uv = IN.uv_MainTex * (1.0-u*_BorderSize*2.0) + float2(u,u)*_BorderSize;
			
			o.Alpha = 1.0;
			o.Normal = UnpackNormal(tex2D(_NormalMap, uv));
		
			//This is just a rough way to apply a light map that was not created in Unity
			float lightMap = tex2D(_LightMap, uv).r;
			lightMap = (1.0-_LightMapStr) + lightMap * _LightMapStr;
			
			float3 colorMap = tex2D(_MainTex, uv).rgb;
			float3 splatMap = tex2D(_SplatMap, uv).rgb;
			
			float3 noiseAlpha = SampleNoiseAlpha(IN.worldPos);
			float3 rockAlpha = SampleRockAlpha(IN.worldPos);
			float3 grassAlpha = SampleGrassAlpha(IN.worldPos);
			
			//I find adding a little noise to the splat map helps remove the blur from bilinear filtering
			//ands adds a little extra bit of detail
			splatMap -= noiseAlpha.g*0.4;
			splatMap = clamp(splatMap, 0, 1);
			
			float3 rockColor = SampleRockColor(rockAlpha, colorMap);
			float3 grassColor = SampleGrassColor(grassAlpha, IN.worldPos, splatMap, noiseAlpha);
			float3 dirtColor = SampleDirtColor(grassAlpha, IN.worldPos);
			
			//blend rock and grass based on b channel of splatmap
			o.Albedo = lerp(rockColor, grassColor, splatMap.b);
			//blend rock/grass and dirt based on g channel of splatmap
			o.Albedo = lerp(o.Albedo, dirtColor, splatMap.g);
			//blend rock/grass/dirt and grass based on b channel of splatmap, create a dull effect
			o.Albedo = lerp(o.Albedo, grassColor, splatMap.b)*0.3;
			
			//modulate by lightmap
			//o.Albedo *= lightMap;
	
		}
		ENDCG
	}
	FallBack "Diffuse"
}