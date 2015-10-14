Shader "Custom/voxelShader" {
	Properties {
	  _MainTex ("Base (RGB)", 2D) = "white" {}
	   _Color ("Main Color", Color) = (1,1,1,1)
	   _TexTiling ("TexTiling", Float) = 1
	   _Tiling ("Tiling", Float) = 1
	   _OffsetX ("Offset X", Float) = 0
	   _OffsetY ("Offset Y", Float) = 0
	   
	   _SideOffsetX ("Side Offset X", Float) = 0
	   _SideOffsetY ("Side Offset Y", Float) = 0
	   
	   _Ambient ("Ambient Color", Color) = (0.2,0.2,0.2,0.2)
   }
   
   SubShader {
      Pass {	
         Tags { "LightMode" = "ForwardBase" } 
            // make sure that all uniforms are correctly set
 
         CGPROGRAM
         #pragma vertex vert  
         #pragma fragment frag 

         uniform float4 _LightColor0; 

 
         uniform float4 _Color;
         uniform float4 _Ambient;
         uniform float	_TexTiling;
         uniform float	_Tiling;
         uniform float	_OffsetX;
         uniform float	_OffsetY;
         uniform float	_SideOffsetX;
         uniform float	_SideOffsetY;
         uniform sampler2D _MainTex;

         static float3 normArray[6] = 
         {
         	float3(0,0,1), float3(0,0,-1), float3(-1,0,0), float3(1,0,0), float3(0,1,0), float3(0,-1,0)
         };

 
         struct vertexInput {
            float4 vertex : POSITION;
            float4 col	  : COLOR0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 col : COLOR;
            float2 uv  : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
 			float4 params = input.col * 255;
			output.uv = float2(params.x, params.y);
			
			uint normIndex = (uint)(params.w);
 
            float3 normalDirection = normArray[ normIndex ];
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
 
            float3 diffuseReflection = _LightColor0.rgb
               * max(0.0, dot(normalDirection, lightDirection));
 
            output.col = float4(max(diffuseReflection,0.2), params.w + 0.01);
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            uint normIndex = (uint)(input.col.a);

            float2 texuv;
            
            if(normIndex < 4)
            	texuv = fmod(input.uv * _TexTiling * _Tiling,_TexTiling) + float2(_SideOffsetX,_SideOffsetY);
            else
            	texuv = fmod(input.uv * _TexTiling * _Tiling,_TexTiling) + float2(_OffsetX,_OffsetY);
            	
         	float4 c = tex2D(_MainTex, texuv);
         	
            return float4(input.col.rgb,1.0) * c + _Ambient;
         }
 
         ENDCG
      }
   }
   Fallback "Mobile/Diffuse"
}
