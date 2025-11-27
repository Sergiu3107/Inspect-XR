Shader "Unlit/LayerShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {} 
        _Textures ("Layer Array", 2DArray) = "" {}
        _Count ("Number of layers", Integer) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD;
                
            };
            
            UNITY_DECLARE_TEX2DARRAY(_Textures);
            int _Count;
            float4 _LayerColors[128];
            float _Weigths[128];
            sampler2D _MainTex;

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
                float4 color = tex2D(_MainTex, i.uv);
                if (_Count == 0) return color;
                
                float4 result = float4(0, 0, 0, 0);
                
                for (int index = 0; index < _Count; ++index)
                {
                    float4 layer = UNITY_SAMPLE_TEX2DARRAY(_Textures, float3(i.uv, index));
                    layer *= _LayerColors[index];
                    // layer.a = mask(layer.a);
                    result = lerp(result, layer, layer.a);
                }

                return lerp(color, result, result.a);
            }

            ENDCG
        }
    }
}
