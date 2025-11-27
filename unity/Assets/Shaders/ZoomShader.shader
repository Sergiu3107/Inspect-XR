Shader "Unlit/ZoomShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Coordinates("Coordinates", Vector) = (0,0,0,0)
        _Scale("Scale", Range(1,50)) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Coordinates;
            half _Scale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 diff = i.uv - _Coordinates;
                float2 uv = _Coordinates + diff / _Scale;

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
