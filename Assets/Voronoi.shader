// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Voronoi"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}

    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION; // vertex position
                float2 uv : TEXCOORD0; // texture coordinate
            };

            struct v2f
            {
                float4 vertex : SV_POSITION; // clip space position
                float4 positionInWorldSpace : TEXCOORD0;
            };

            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.positionInWorldSpace = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            sampler2D _MainTex;
			fixed4 _ReferencePoints[10];
			float _NumberOfPoints;
			fixed4 _ReferenceColors[10];

            fixed4 frag (v2f i) : COLOR
            {
				fixed4 col;
				int index = 0;	
                float4 pos = i.positionInWorldSpace;

                float mindist = 100000;
				for (int i = 0; i < _NumberOfPoints; ++i)
				{   
                    float4 refPoint = _ReferencePoints[i];
					if (distance(pos.xy,refPoint.xy) < mindist)
					{
						mindist = distance(pos.xy, refPoint.xy);
						index = i;
					}
				}
				col = _ReferenceColors[index];
				return col;
            }
            ENDCG
        }
    }
}