Shader "Custom/SliceCap"
{
    SubShader
    {
        Tags { "Queue"="Geometry+1" }
        Cull Off
        ZWrite Off

        Stencil
        {
            Ref 1
            Comp Equal
        }

        Pass
        {
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

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1,1,1,1); // white cap
            }
            ENDCG
        }
    }
}
