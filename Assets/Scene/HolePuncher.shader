Shader "HolePuncher"
{
    Properties
    {
        // No properties needed for this depth-only shader
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        // Disable color writes to avoid any color buffer output
        ColorMask 0

        Pass
        {
            ZWrite On
            ZTest GEqual
            // Ensure the shader doesn't output anything to color
            ColorMask 0

            // Use a minimal vertex shader to avoid any fragment output
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            // Empty fragment shader to ensure no color is written
            fixed4 frag() : SV_Target
            {
                return fixed4(0, 0, 0, 0); // No color output
            }
            ENDCG
        }
    }
}
