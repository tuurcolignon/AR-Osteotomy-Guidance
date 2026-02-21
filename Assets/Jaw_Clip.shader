Shader "Custom/JawClip"
{
    Properties
    {
        // New slider to adjust cut height manually
        _CutOffset ("Cut Offset", Range(-0.05, 0.05)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _SlicePlane;
            float _CutOffset; // Variable for the slider

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Add the offset to the distance calculation
                float d = dot(_SlicePlane.xyz, i.worldPos) + _SlicePlane.w + _CutOffset;
                
                clip(-d); // Discards the top part
                return fixed4(1,1,1,1);
            }
            ENDCG
        }
    }
}