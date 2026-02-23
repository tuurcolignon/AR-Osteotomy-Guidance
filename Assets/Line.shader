Shader "Custom/LineOnly" {
    Properties {
        _PlanePosition ("Plane Position", Vector) = (0,0,0,0)
        _PlaneNormal ("Plane Normal", Vector) = (0,0,1,0)
        _PlaneRight ("Plane Right", Vector) = (1,0,0,0)
        _PlaneUp ("Plane Up", Vector) = (0,1,0,0)
        _PlaneScale ("Plane Scale", Vector) = (1,1,1,0)
        _LineThickness ("Line Thickness", Float) = 0.0005
        _LineColor ("Line Color", Color) = (1, 0, 0, 1) 
    }
    SubShader {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        LOD 200
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input {
            float3 worldPos;
        };

        float4 _PlanePosition;
        float4 _PlaneNormal;
        float4 _PlaneRight;
        float4 _PlaneUp;
        float4 _PlaneScale;
        float _LineThickness;
        fixed4 _LineColor;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            float3 offset = IN.worldPos - _PlanePosition.xyz;
            
            float distNormal = abs(dot(offset, normalize(_PlaneNormal.xyz)));
            float distRight = abs(dot(offset, normalize(_PlaneRight.xyz)));
            float distUp = abs(dot(offset, normalize(_PlaneUp.xyz)));
            
            bool withinX = distRight <= (_PlaneScale.x * 0.5);
            bool withinY = distUp <= (_PlaneScale.y * 0.5);

            if (distNormal >= _LineThickness || !withinX || !withinY) {
                clip(-1.0); 
            }
            
            o.Albedo = _LineColor.rgb;
            o.Emission = _LineColor.rgb; 
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}