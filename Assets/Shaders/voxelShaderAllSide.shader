Shader "Custom/voxelShaderAllSide" {
	Properties {
	   _MainTex ("Base (RGB)", 2D) = "white" {}
   }
   
   SubShader {
      Pass {	
         Tags { "LightMode" = "ForwardBase" } 
            // make sure that all uniforms are correctly set
 
         CGPROGRAM
         #pragma vertex vert  
         #pragma fragment frag 

         uniform float4 _LightColor0; 

         uniform sampler2D _MainTex;
         uniform half4 _MainTex_ST;

         static half3 normArray[6] = 
         {
         	half3(0,0,1), half3(0,0,-1), half3(-1,0,0), half3(1,0,0), half3(0,1,0), half3(0,-1,0)
         };

 
         struct vertexInput {
            half4 vertex : POSITION;
            half4 col	  : COLOR0;
         };
         struct vertexOutput {
            half4 pos : SV_POSITION;
            half4 col : COLOR;
            half2 uv  : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
 			half4 params = input.col * 255;
			uint normIndex = (uint)(params.w);
			
 			output.uv = normIndex < 2 ?  input.vertex.xy : normIndex < 4 ? input.vertex.zy : input.vertex.xz;
			//HACK HACK HACK 10 is my voxel res
			output.uv = output.uv * 10 * _MainTex_ST.xy + _MainTex_ST.zw;

            half3 normalDirection = normArray[ normIndex ];
            half3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
 
            half3 diffuseReflection = _LightColor0.rgb
               * max(0.0, dot(normalDirection, lightDirection)) + UNITY_LIGHTMODEL_AMBIENT;
 
 			diffuseReflection = min(diffuseReflection, 1.0);
 
            output.col = half4(diffuseReflection, 1.0);
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         half4 frag(vertexOutput input) : COLOR
         {            	
         	half4 c = tex2D(_MainTex, input.uv);
         	
            return input.col * c;
         }
 
         ENDCG
      }
   }
   Fallback "Mobile/Diffuse"
}
