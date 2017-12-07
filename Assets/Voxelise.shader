Shader "Unlit/Voxelise"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DefinitionSize("Definition", float) = 1

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float4 allMyVertex[1] = {float4(0,0,0,0)};
			float4 allMyVertex2[1] = {float4(0,0,0,0)};

			float arraySize = 1;

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
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.x = round(o.vertex.x);
				o.vertex.y = round(o.vertex.y);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				if(allMyVertex[0].x == 0 && allMyVertex[0].w == 0){
					allMyVertex[0] = o.vertex;
				}else{
					allMyVertex2 = allMyVertex;
					arraySize ++;
					allMyVertex[arraySize];

					for(int i = 0; i < arraySize; i++){

						if(arraySize-1 == i){
							allMyVertex[i] = o.vertex;
						}else{
							allMyVertex[i] = allMyVertex2[i];
						}

					}

				}
			
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}




			
			ENDCG
		}
	}
}
