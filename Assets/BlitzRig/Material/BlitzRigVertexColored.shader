Shader "Nukode/BlitzRig/Vertex Colored Lightning"
{
    Properties
    {
        // [HDR] _Color ("Main Color", Color) = (1,1,1,1)
        // _HDRAdd ("HDR Additive", Range(0,1)) = 0.1
        // _HDRMult ("HDR Multiplicative", Range(0,1)) = 0.1
        _RedAdd ("Red Additive", Range(0,8)) = 0
        _GreenAdd ("Green Additive", Range(0,8)) = 0
        _BlueAdd ("Blue Additive", Range(0,8)) = 0
        _AdaptiveIntensity ("Adaptive Intensity", Range(0,8)) = 0
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            // float4 _Color;
            // float _HDRAdd;
            // float _HDRMult;
            float _RedAdd;
            float _GreenAdd;
            float _BlueAdd;
            float _AdaptiveIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float adaptiveR = _AdaptiveIntensity;
                float adaptiveG = _AdaptiveIntensity;
                float adaptiveB = _AdaptiveIntensity;

                if (i.color.r < i.color.g || i.color.r < i.color.b)
                    adaptiveR = 0;

                if (i.color.g < i.color.r || i.color.g < i.color.b)
                    adaptiveG = 0;

                if (i.color.b < i.color.r || i.color.b < i.color.g)
                    adaptiveB = 0;

                float4 adaptiveIntensityColor = float4(adaptiveR, adaptiveG, adaptiveB, 0);

                return float4(i.color.r + _RedAdd, i.color.g + _GreenAdd, i.color.b + _BlueAdd, i.color.a) + adaptiveIntensityColor;
            }
            ENDCG
        }
    }
}