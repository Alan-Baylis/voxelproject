﻿Shader "Unlit/VoxeliseV2"
{
	Properties
	{

		//object Texture
		_MainTex ("Texture", 2D) = "white" {}

		//Size of the cubes
		_DefinitionSize("Definition", float) = 1

	}
	SubShader
	{
		 Tags {"LightMode"="ForwardBase"}

		Pass
		{
			CGPROGRAM

			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			 #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"


			// array for knowing position of all the vertices
			float4 allMyVertex[1] = {float4(0,0,0,0)};
			float4 allMyVertex2[1] = {float4(0,0,0,0)};
			float arraySize = 1;


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				 SHADOW_COORDS(1) //TEXCOORD1
				float4 pos : SV_POSITION;
				float3 diff : COLOR0;
				fixed3 ambient : COLOR1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;



			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); //convert to camera space
				
				//displace the position of the vertices in the camera space
				o.pos.x = round(o.pos.x);
				o.pos.y = round(o.pos.y);


				o.uv = TRANSFORM_TEX(v.uv, _MainTex);


				////////////////////////////////////////////////////////// take all vertex in an array to check the distance between them and know hom many are near (cube or something unwanted)
				//if(allMyVertex[0].x == 0 && allMyVertex[0].w == 0){
				//	allMyVertex[0] = o.pos;
				//}else{
				//	allMyVertex2 = allMyVertex;
				//	arraySize ++;
				//	allMyVertex[arraySize];

				//	for(int i = 0; i < arraySize; i++){

				//		if(arraySize-1 == i){
				//			allMyVertex[i] = o.pos;
				//		}else{
				//			allMyVertex[i] = allMyVertex2[i];
				//		}

				//	}

				//}

				//////////////////////////////////////////////////////////

				//light and shadows
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                float nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                TRANSFER_SHADOW(o)
			
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{

				
				fixed4 col = tex2D(_MainTex, i.uv); //color of the texture
				//col = float4(0.8,0.3,0.3,1); //redColor

				//light and shadows change the color
                fixed shadow = SHADOW_ATTENUATION(i);
                fixed3 lighting = i.diff * shadow + i.ambient;
                col.rgb *= lighting;
				return col; 
			}




			
			ENDCG
		}

		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

	}
}