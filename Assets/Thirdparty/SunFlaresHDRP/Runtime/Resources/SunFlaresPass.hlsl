#ifndef SUN_FLARES_HDRP
#define SUN_FLARES_HDRP		

	// Copyright 2020 Kronnect - All Rights Reserved.

    TEXTURE2D_X(_InputTexture);
	TEXTURE2D(_FlareNoiseTex);
    TEXTURE2D(_OcclusionTex);

	float4 _MainTex_ST;
	float4 _MainTex_TexelSize;
	float4 _SunPos;
	float4 _SunData;	// x = sunIntensity, y = disk size, z = ray difraction, w = ray difraction amount
	float4 _SunCoronaRays1;  // x = length, y = streaks, z = spread, w = angle offset
	float4 _SunCoronaRays2;  // x = length, y = streaks, z = spread, w = angle offset
	float4 _SunGhosts1;  // x = sharpness, y = size, 2 = pos offset, 3 = brightness
	float4 _SunGhosts2;  // x = sharpness, y = size, 2 = pos offset, 3 = brightness
	float4 _SunGhosts3;  // x = sharpness, y = size, 2 = pos offset, 3 = brightness
	float4 _SunGhosts4;  // x = sharpness, y = size, 2 = pos offset, 3 = brightness
   	float4 _SunHalo;  // x = offset, y = amplitude, z = intensity, w = clouds occlusion
   	float4 _SunTint;
	float3 _SunPosRightEye;

    #define OCCLUSION_SPEED _SunTint.a
    #define CLOUDS_OCCLUSION _SunHalo.w

    SamplerState sampler_LinearRepeat;

  struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }



	inline float getLuma(float3 rgb) {
		const float3 lum = float3(0.299, 0.587, 0.114);
		return dot(rgb, lum);
	}


	void rotate(inout float2 uv, float ang) {
		float2 sico;
		sincos(ang, sico.x, sico.y);
		float2 cosi = float2(sico.y, -sico.x);
		uv = float2(dot(cosi, uv), dot(sico, uv));
	}

    float sdHexagon( float2 p, float r ) {
        const float3 k = float3(-0.866025404,0.5,0.577350269);
        p = abs(p);
        p -= 2.0*min(dot(k.xy,p),0.0)*k.xy;
        p -= float2(clamp(p.x, -k.z*r, k.z*r), r);
        return length(p)*sign(p.y);
    }

    float sdPentagon( float2 p, float r) {
        const float3 k = float3(0.809016994,0.587785252,0.726542528);
        p.x = abs(p.x);
        p -= 2.0*min(dot(float2(-k.x,k.y),p),0.0)*float2(-k.x,k.y);
        p -= 2.0*min(dot(float2( k.x,k.y),p),0.0)*float2( k.x,k.y);
        p -= float2(clamp(p.x,-r*k.z,r*k.z),r);    
        return length(p)*sign(p.y);
    }

    float sdOctogon( float2 p, float r) {
        const float3 k = float3(-0.9238795325, 0.3826834323, 0.4142135623);
        p = abs(p);
        p -= 2.0*min(dot(float2( k.x,k.y),p),0.0)*float2( k.x,k.y);
        p -= 2.0*min(dot(float2(-k.x,k.y),p),0.0)*float2(-k.x,k.y);
        p -= float2(clamp(p.x, -k.z*r, k.z*r), r);
        return length(p)*sign(p.y);
    }

    float addGhost(float4 ghostData, float2 sunPos, float aspectRatio, float2 uv, float len) {
        float2 ghost1Pos  = 1.0 - sunPos;
   		float2 grd = uv - ghost1Pos + (ghost1Pos - 0.5) * ghostData.z;
		grd.y *= aspectRatio;

    #if SF_HEXAGONAL_SHAPE
        float g0 = sdHexagon(grd, ghostData.y);
        float flare = saturate(g0 * ghostData.x) * ghostData.w / len;
    #elif SF_PENTAGONAL_SHAPE
        float g0 = sdPentagon(grd, ghostData.y);
        float flare = saturate(g0 * ghostData.x) * ghostData.w / len;
    #elif SF_OCTOGONAL_SHAPE
        float g0 = sdOctogon(grd, ghostData.y);
        float flare = saturate(g0 * ghostData.x) * ghostData.w / len;
    #else
		float g0 = saturate(ghostData.y / length(grd)); 
		g0 = pow(g0, ghostData.x);
   		float flare = g0 * ghostData.w / len;
    #endif
        return flare;
    }

    float GetDepth(float2 sunPos, float2 offset) {
        float2 pos = clamp(sunPos + offset, float2(0,0), _ScreenSize.xy - float2(1,1));
        return LoadCameraDepth(uint2(pos));
    }

   	float occlusionTest(float2 uv) {

		// general params
   		float2 sunPos = unity_StereoEyeIndex == 0 ? _SunPos.xy : _SunPosRightEye.xy;
        float2 positionSS = sunPos * _ScreenSize.xy;

		float  depth1 = Linear01Depth(GetDepth(positionSS, float2(-1, -1)), _ZBufferParams);
        float  depth2 = Linear01Depth(GetDepth(positionSS, float2( 1,  1)), _ZBufferParams);
        float  depth3 = Linear01Depth(GetDepth(positionSS, float2( 3,  3)), _ZBufferParams);
        float  depth4 = Linear01Depth(GetDepth(positionSS, float2(-3, -3)), _ZBufferParams);
		float occlusion = (depth1 + depth2 + depth3 + depth4) * 0.25;
        return occlusion;
    }


   	float3 sunflare(float2 uv) {

        float occlusion = saturate(SAMPLE_TEXTURE2D(_OcclusionTex, sampler_LinearRepeat, float2(0,0)).r);
        if (occlusion <= 0.02) return 0.0.xxx;

		// general params
   		float2 sunPos = unity_StereoEyeIndex == 0 ? _SunPos.xy : _SunPosRightEye.xy;

        float2 positionSS = sunPos * _ScreenSize.xy;

   		float2 grd = uv - sunPos;
		float aspectRatio = _ScreenSize.y / _ScreenSize.x;
   		grd.y *= aspectRatio; 
   		float len = length(grd);

   		// sun disk
   		float s0 = pow( 1.0 + saturate(_SunData.y - len), 75) - 1.0;
        
   		// corona rays
		float gang = _SunPos.w;
   		float ang = atan2(grd.y, grd.x) + gang;
   		float ray1 = _SunCoronaRays1.z + abs(_SunCoronaRays1.x * cos(_SunCoronaRays1.w + ang * _SunCoronaRays1.y));	// design
   		ray1 *= pow( 1.0 + len, 1.0 / _SunCoronaRays1.x);	
   		s0 += 1.0 / ray1;

   		float ray2 = _SunCoronaRays2.z + abs(_SunCoronaRays2.x * sin(_SunCoronaRays2.w + ang * _SunCoronaRays2.y));	// design
   		ray2 *= pow( 1.0 + len, 1.0 / _SunCoronaRays2.x);	
   		s0 += 1.0 / ray2;
   		
   		s0 *= _SunData.x;
   		
		#if !defined(UNITY_SINGLE_PASS_STEREO) && !defined(UNITY_STEREO_INSTANCING_ENABLED) && !defined(UNITY_STEREO_MULTIVIEW_ENABLED)
       		// ghosts circular (not compatible with single pass stereo due to how projection works)
            s0 += addGhost(_SunGhosts1, sunPos, aspectRatio, uv, len);
            s0 += addGhost(_SunGhosts2, sunPos, aspectRatio, uv, len);
            s0 += addGhost(_SunGhosts3, sunPos, aspectRatio, uv, len);
            s0 += addGhost(_SunGhosts4, sunPos, aspectRatio, uv, len);
   		#endif

        float3 flare = s0.xxx;

		// light rays
		float2 uv2 = uv - sunPos;
		float clen = length(uv2);
		rotate(uv2, gang);
		uv2.x *= aspectRatio;
		uv2.x *= 0.1;
		uv2 /= len;
		float lr = saturate(SAMPLE_TEXTURE2D(_FlareNoiseTex, sampler_LinearRepeat, uv2 + _SunPos.zz).r - _SunData.w);
		float3 rays = lr * sin(float3(len, len + 0.1, len + 0.2) * 3.1415927);
		float atten = pow(1.0 + clen, 13.0);
		rays *= _SunData.z / atten;
		flare += rays;

		// halo
		float hlen = clamp( (len - _SunHalo.x) * _SunHalo.y, 0, 3.1415927);
		float3 halo = pow(sin(float3(hlen, hlen + 0.1, hlen + 0.2)), 12.0.xxx);
		halo *= _SunHalo.z / atten;
		flare += halo; 

		return saturate(flare * _SunTint.rgb * occlusion);
   	}  

  	float4 FragSF (Varyings input) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        uint2 positionSS = input.texcoord * _ScreenSize.xy;
        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;
        float3 flare = sunflare(input.texcoord);
        flare *= 1.0 - saturate( dot(outColor, 1.0.xxx) * outColor.g * CLOUDS_OCCLUSION);
        return float4(flare + outColor, 1.0);
   	}

  	float4 FragSFOcclusion (Varyings input) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float occlusion = occlusionTest(input.texcoord);

        #if SF_OCCLUSION_INIT
            return float4(occlusion, occlusion, occlusion, 1.0);
        #else
            return float4(occlusion, occlusion, occlusion, unity_DeltaTime.x * OCCLUSION_SPEED);
        #endif
   	}

#endif // SUN_FLARES_HDRP