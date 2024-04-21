Shader "Unlit/SpiceShimmer"
{
    Properties
    {
        _Color ("Main Color", Color) = (1, 0.92, 0.016, 1) // Golden color
        _ShimmerSpeed ("Shimmer Speed", Float) = 1.0 // Speed of shimmering effect
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType"="Transparent" }
        ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _ShimmerSpeed;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = _Color;
                // Calculate shimmer offset using sine wave
                float shimmer = sin(_Time.y * _ShimmerSpeed) * 0.1; // Adjust amplitude as needed

                // Apply color with shimmer offset
                col.rgb += shimmer;
                return col;
            }
            ENDCG
        }
    }

    FallBack "Sprite/Default"
}
