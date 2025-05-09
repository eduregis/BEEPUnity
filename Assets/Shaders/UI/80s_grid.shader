Shader "UI/80sObliqueGrid"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // Cores
        _GridColor1 ("Grid Color 1", Color) = (0, 1, 1, 1) // Cyan
        _GridColor2 ("Grid Color 2", Color) = (1, 0, 1, 1) // Magenta
        _BackgroundColor ("Background Color", Color) = (0, 0, 0.1, 1)
        
        // Parâmetros do Grid
        _GridDensity ("Grid Density", Float) = 80
        _LineWidth ("Line Width", Float) = 0.03
        
        // Controles de Animação
        _ScrollSpeed ("Scroll Speed", Vector) = (0.3, 1, 0, 0)
        _ReverseAnimation ("Reverse Animation", Float) = 0
        _VerticalSpeedMultiplier ("Vertical Speed Multiplier", Float) = 2.0
        
        // Controles de Perspectiva
        _Perspective ("Perspective Strength", Range(0, 1)) = 0.7
        _HorizonPosition ("Horizon Position", Range(0, 1)) = 0.6
        _HorizonDirection ("Horizon Direction", Float) = 1 // 1 para cima, -1 para baixo
        _FadeLength ("Fade Length", Range(0.01, 0.5)) = 0.2
        _MinFadeAlpha ("Min Fade Alpha", Range(0, 0.5)) = 0.15
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

            fixed4 _GridColor1;
            fixed4 _GridColor2;
            fixed4 _BackgroundColor;
            float _GridDensity;
            float _LineWidth;
            float2 _ScrollSpeed;
            float _ReverseAnimation;
            float _VerticalSpeedMultiplier;
            float _Perspective;
            float _HorizonPosition;
            float _HorizonDirection;
            float _FadeLength;
            float _MinFadeAlpha;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                
                // Aplica direção do horizonte
                OUT.texcoord = v.texcoord;
                if (_HorizonDirection < 0)
                    OUT.texcoord.y = 1 - OUT.texcoord.y;
                    
                OUT.color = v.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Ajusta a direção da animação
                float2 scrollDirection = _ScrollSpeed;
                if (_ReverseAnimation > 0.5)
                    scrollDirection *= -1;
                
                // Coordenadas com scrolling (centralizadas em [-0.5, 0.5])
                float2 centeredUV = IN.texcoord - float2(0.5, 0);
                
                // Fator de velocidade baseado na posição Y (quanto mais perto da base, mais rápido)
                float speedFactor = 1.0 + (1.0 - IN.texcoord.y) * _VerticalSpeedMultiplier;
                float2 scrolledUV = IN.texcoord + scrollDirection * _Time.y * float2(speedFactor, 1.0);
                
                // Linhas horizontais (largura constante)
                float horizontalLines = abs(frac(scrolledUV.y * _GridDensity) - 0.5);
                float horizontalGrid = smoothstep(_LineWidth, _LineWidth + 0.01, horizontalLines);
                
                // Aplica perspectiva nas linhas verticais (simétrica a partir do centro)
                float perspective = 1.0 - _Perspective * (1.0 - IN.texcoord.y / _HorizonPosition);
                float verticalPos = centeredUV.x / perspective;
                
                // Linhas verticais (centralizadas) com velocidade variável
                float verticalLines = abs(frac((verticalPos + 0.5) * _GridDensity + scrollDirection.x * _Time.y * speedFactor) - 0.5);
                float verticalGrid = smoothstep(_LineWidth, _LineWidth + 0.01, verticalLines);
                
                // Combina as linhas
                float grid = min(horizontalGrid, verticalGrid);
                
                // Interpola entre as duas cores do grid baseado na posição Y
                fixed4 gridColor = lerp(_GridColor1, _GridColor2, IN.texcoord.y);
                
                // Efeito de fade no horizonte com alpha mínimo
                float fadeStart = _HorizonPosition - _FadeLength;
                float fadeEnd = _HorizonPosition;
                float distanceFade;
                
                if (_HorizonDirection > 0)
                    distanceFade = lerp(_MinFadeAlpha, 1.0, 1.0 - smoothstep(fadeStart, fadeEnd, IN.texcoord.y));
                else
                    distanceFade = lerp(_MinFadeAlpha, 1.0, smoothstep(1.0 - fadeEnd, 1.0 - fadeStart, IN.texcoord.y));
                
                // Cor final
                fixed4 color = lerp(gridColor, _BackgroundColor, grid);
                color.a *= distanceFade * IN.color.a;
                
                return color;
            }
            ENDCG
        }
    }
}