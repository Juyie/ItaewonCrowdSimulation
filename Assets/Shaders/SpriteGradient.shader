// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SpriteGradient" {
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _ColorLeft("Left Color", Color) = (1,1,1,1)
        _ColorLeftMid("Left Mid Color", Color) = (1,1,1,1)
        _ColorMid("Mid Color", Color) = (1,1,1,1)
        _ColorRightMid("Right Mid Color", Color) = (1,1,1,1)
        _ColorRight("Right Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"Queue" = "Background"  "IgnoreProjector" = "True"}
        LOD 100

        ZWrite On

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _ColorLeft;
            fixed4 _ColorLeftMid;
            fixed4 _ColorMid;
            fixed4 _ColorRightMid;
            fixed4 _ColorRight;

                struct v2f
            {
                float4 pos : SV_POSITION;
                float4 texcoord : TEXCOORD0;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                fixed4 c = lerp(_ColorLeft, _ColorLeftMid, i.texcoord.x / 0.25) * step(i.texcoord.x, 0.25);
                c += lerp(_ColorLeftMid, _ColorMid, (i.texcoord.x - 0.25) / 0.25) * step(0.25, i.texcoord.x) * step(i.texcoord.x, 0.5);
                c += lerp(_ColorMid, _ColorRightMid, (i.texcoord.x - 0.5) / 0.25) * step(0.5, i.texcoord.x) * step(i.texcoord.x, 0.75);
                c += lerp(_ColorRightMid, _ColorRight, (i.texcoord.x - 0.75) / 0.25) * step(0.75, i.texcoord.x);
                c.a = 1;
                return c;
            }
            ENDCG
        }
     }
}