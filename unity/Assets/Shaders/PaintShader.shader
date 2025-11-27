Shader "Unlit/PaintShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _SplatTex ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
        LOD 100
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _SplatTex;

            float mask(float alpha)
            {
                alpha *= 0.5;
                alpha = 1 - alpha;
                float alpha1 = step(alpha, 0.90);
                float alpha2 = step(alpha, 0.85);
                
                float border = alpha1 - alpha2;
                float inside = alpha1 * 0.50;

                // border *= 0.8;
                return border + inside;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed4 main = tex2D(_MainTex, i.uv);
                fixed4 splat = tex2D(_SplatTex, i.uv);

                float alpha = mask(splat.a);

                fixed4 color;
                color.rgb = lerp(main.rgb, splat.rgb, alpha);
                color.a = alpha;
                return color;
            }
            ENDCG
        }
    }
}
