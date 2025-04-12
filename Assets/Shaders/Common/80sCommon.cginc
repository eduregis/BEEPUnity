#ifndef _80S_COMMON_INCLUDED
#define _80S_COMMON_INCLUDED

// TV static noise effect
float TVNoise(float2 uv, float time, float intensity)
{
    float noise = frac(sin(dot(uv * time, float2(12.9898, 78.233))) * 43758.5453);
    noise = lerp(0.5, noise, intensity);
    return noise;
}

#endif
