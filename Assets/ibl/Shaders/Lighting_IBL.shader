Shader "Unlit/LightingTestMulti" {
    Properties {
        _RockAlbedo ("Albedo", 2D) = "white" {}
        [NoScaleOffset] _RockNormals ("Normals", 2D) = "bump" {}
        [NoScaleOffset] _RockHeight ("Height", 2D) = "gray" {}
        [NoScaleOffset] _DiffuseIBL ("Diffuse IBL", 2D) = "black" {}
        [NoScaleOffset] _SpecularIBL ("Specular IBL", 2D) = "black" {}
        _Gloss ("Gloss", Range(0,1)) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _AmbientLight ("Ambient Light", Color) = (0,0,0,0)
        _SpecIBLIntensity ("Spec IBL Intensity", Range(0,1)) = 1
        _NormalIntensity ("Normal Intensity", Range(0,1)) = 1
        _DispStrength ("Displacement Strength", Range(0,0.2)) = 0
        _F0 ("F0", Range(0,1)) = 1
        _Shadow ("Shadow Attenuation", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        // Base pass
        Pass {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #define IS_IN_BASE_PASS
            #include "CustomIBL.cginc"
            ENDCG
        }
        
        // Add pass
        Pass {
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One // src*1 + dst*1
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
            #include "CustomIBL.cginc"
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"        
    }
}
