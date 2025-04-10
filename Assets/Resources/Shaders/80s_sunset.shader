Shader "UI/80sSunset"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _SunColor1 ("Sun Color 1", Color) = (1, 0.5, 0, 1)
        _SunColor2 ("Sun Color 2", Color) = (1, 0.9, 0.2, 1)
        
        _SunSize ("Sun Size", Range(0.1, 1)) = 0.3
        _VisiblePercentage ("Visible Percentage", Range(0.1, 1)) = 0.7
        _Opacity ("Opacity", Range(0, 1)) = 1

        _StripesDensity ("Stripes Density", Float) = 5
        _MinStripesSpeed ("Min Stripes Speed", Float) = 0.5
        _MaxStripesSpeed ("Max Stripes Speed", Float) = 2
        _MinStripesWidth ("Min Stripes Width", Float) = 0.01
        _MaxStripesWidth ("Max Stripes Width", Float) = 0.05
        _StripesFade ("Fade Intensity", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _SunColor1;
            fixed4 _SunColor2;
            float _SunSize;
            float _VisiblePercentage;
            float _Opacity;

            float _StripesDensity;
            float _MinStripesSpeed;
            float _MaxStripesSpeed;
            float _MinStripesWidth;
            float _MaxStripesWidth;
            float _StripesFade;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float distanceToCenter = distance(IN.texcoord, center);
                float sunCircle = smoothstep(_SunSize, _SunSize * 0.99, distanceToCenter);

                float horizonLine = 0.5 - (_SunSize * (1.0 - _VisiblePercentage));
                float visibility = step(horizonLine, IN.texcoord.y);
                
                float verticalPos = (IN.texcoord.y - horizonLine) / (_SunSize * 2 * _VisiblePercentage);
                fixed4 sunColor = lerp(_SunColor1, _SunColor2, verticalPos);

                // Calcula stripeY como base de deslocamento
                float stripeIndex = floor(IN.texcoord.y * _StripesDensity);
                float stripeBaseY = stripeIndex / _StripesDensity;
                float stripeOffset = frac(stripeIndex * 17.17);
                float stripeSpeed = lerp(_MaxStripesSpeed, _MinStripesSpeed, verticalPos);
                float stripeWidth = lerp(_MaxStripesWidth, _MinStripesWidth, verticalPos);

                float stripeY = frac(IN.texcoord.y * _StripesDensity - _Time.y * stripeSpeed + stripeOffset);

                // Corte seco da faixa
                float stripe = step(0.5 - stripeWidth * 0.5, stripeY) * step(stripeY, 0.5 + stripeWidth * 0.5);

                // Máscara do círculo do sol (corte total fora dele)
                float inSun = step(distanceToCenter, _SunSize); // 1 se dentro do círculo

                // Recorta tudo fora da área visível
                stripe *= inSun * visibility;

                fixed4 color = sunColor * sunCircle * visibility;
                color = lerp(color, fixed4(1,1,1,1), stripe);
                color.a *= _Opacity * IN.color.a;

                return color;
            }

            ENDCG
        }
    }
}
