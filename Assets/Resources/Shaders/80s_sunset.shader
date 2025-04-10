Shader "UI/80sSunsetUI"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1, 0.2, 0.4, 1)
        _SecondaryColor ("Secondary Color", Color) = (1, 0.6, 0.2, 1)
        _SunSize ("Sun Size", Range(0, 1)) = 0.5
        _SunEdgeSoftness ("Sun Edge Softness", Range(0, 1)) = 0.01
        _StripesColor ("Stripes Color", Color) = (0, 0.8, 1, 1)
        _StripesWidth ("Stripes Width", Range(0, 0.5)) = 0.03
        _StripesSoftness ("Stripes Softness", Range(0, 0.5)) = 0
        _StripesSpeed ("Stripes Speed", Float) = 1.0
        _StripesSpacing ("Stripes Spacing", Float) = 0.5
        
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
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
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            fixed4 _MainColor;
            fixed4 _SecondaryColor;
            float _SunSize;
            float _SunEdgeSoftness;
            fixed4 _StripesColor;
            float _StripesWidth;
            float _StripesSoftness;
            float _StripesSpeed;
            float _StripesSpacing;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Centered coordinates
                float2 uv = i.uv - 0.5;
                float distanceFromCenter = length(uv) * 2;
                
                // Sun gradient
                float sun = smoothstep(_SunSize + _SunEdgeSoftness, _SunSize - _SunEdgeSoftness, distanceFromCenter);
                fixed4 sunColor = lerp(_SecondaryColor, _MainColor, distanceFromCenter / _SunSize);
                
                // Animated stripes
                float stripePos = (uv.y + 0.5) + _Time.y * _StripesSpeed;
                float stripe = frac(stripePos / _StripesSpacing);
                float stripeMask = smoothstep(_StripesWidth + _StripesSoftness, _StripesWidth - _StripesSoftness, abs(stripe - 0.5) * 2);
                
                // Combine sun with stripes
                fixed4 col = sunColor * sun;
                col = lerp(_StripesColor, col, stripeMask);
                
                // Apply transparency based on sun
                col.a = sun;
                
                // Apply UI masking
                col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                
                // Apply vertex color
                col *= i.color;
                
                return col;
            }
            ENDCG
        }
    }
}