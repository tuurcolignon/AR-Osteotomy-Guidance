Shader "Custom/Line" {
    Properties {
        _Color ("Jaw Color", Color) = (0.8, 0.8, 0.8, 1)
        _MainTex ("Jaw Texture", 2D) = "white" {}
        _PlanePosition ("Plane Position", Vector) = (0,0,0,0)
        _PlaneNormal ("Plane Normal", Vector) = (0,0,1,0)
        _PlaneRight ("Plane Right", Vector) = (1,0,0,0)
        _PlaneUp ("Plane Up", Vector) = (0,1,0,0)
        _PlaneScale ("Plane Scale", Vector) = (1,1,1,0)
        _LineThickness ("Line Thickness", Float) = 0.0005
        _LineColor ("Line Color", Color) = (1, 0, 0, 1) 
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        fixed4 _Color;
        float4 _PlanePosition;
        float4 _PlaneNormal;
        float4 _PlaneRight;
        float4 _PlaneUp;
        float4 _PlaneScale;
        float _LineThickness;
        fixed4 _LineColor;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            
            // Calculate vector from plane center to current pixel
            float3 offset = IN.worldPos - _PlanePosition.xyz;
            
            // Project offset onto local plane axes
            float distNormal = abs(dot(offset, normalize(_PlaneNormal.xyz)));
            float distRight = abs(dot(offset, normalize(_PlaneRight.xyz)));
            float distUp = abs(dot(offset, normalize(_PlaneUp.xyz)));
            
            // Assume 1x1 base Quad mesh. Multiply scale by 0.5 to get extents from center.
            bool withinX = distRight <= (_PlaneScale.x * 0.5);
            bool withinY = distUp <= (_PlaneScale.y * 0.5);

            // Render line if within thickness AND within physical bounds of the plane
            if (distNormal < _LineThickness && withinX && withinY) {
                o.Albedo = _LineColor.rgb;
                o.Emission = _LineColor.rgb; 
            } else {
                o.Albedo = c.rgb;
            }
            
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}