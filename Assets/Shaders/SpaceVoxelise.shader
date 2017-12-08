// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SpaceVoxelise"
{
	Properties
	{

		//object Texture
		_MainTex ("Texture", 2D) = "white" {}

		//Size of the cubes
		_Granularity("Definition", float) = 1


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
				 SHADOW_COORDS(1)
				float4 pos : SV_POSITION;

				float3 wpos : TEXCOORD2;
				float3 vpos : TEXCOORD3;
				
				float3 diff : COLOR0;
				fixed3 ambient : COLOR1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Granularity;

			
			v2f vert (appdata v)
			{
				v2f o;
				 //convert to camera space

				// voxelisation
				float lod = 256. - _Granularity;
				//float lod = 1. + length(v.vertex.xyz) * _Granularity;
				float3 grid = ceil(v.vertex.xyz * lod) / lod;
				v.vertex.xyz = grid;
				//v.vertex.xyz = lerp(v.vertex.xyz, grid, clamp(_Granularity,0,1));

				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				
				//Lighting
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


/*
				//////////////////////////////////////////////////////////
				if(allMyVertex[0].x == 0 && allMyVertex[0].w == 0){
					allMyVertex[0] = o.pos;
				}else{
					allMyVertex2 = allMyVertex;
					arraySize ++;
					allMyVertex[arraySize];

					for(int i = 0; i < arraySize; i++){

						if(arraySize-1 == i){
							allMyVertex[i] = o.pos;
						}else{
							allMyVertex[i] = allMyVertex2[i];
						}

					}

				}
*/
				//////////////////////////////////////////////////////////