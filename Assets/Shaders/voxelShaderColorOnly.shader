Shader "Custom/voxelShaderColorOnly" {
	Properties {
	   _Color ("Color", Color) = (0,0,0,0)
	   _SideColor("Side Color", Color) = (0,0,0,0)
   }
   
   SubShader {
      Pass {	
         Tags { "LightMode" = "ForwardBase" } 
            // make sure that all uniforms are correctly set
 
         CGPROGRAM
         #pragma vertex vert  
         #pragma fragment frag 

         uniform float4 _LightColor0; 

         uniform half4 _Color;
         uniform half4 _SideColor;

         static half3 normArray[6] = 
         {
         	half3(0,0,1), half3(0,0,-1), half3(-1,0,0), half3(1,0,0), half3(0,1,0), half3(0,-1,0)
         };

 
         struct vertexInput {
            float4 vertex : POSITION;
            half4 col	  : COLOR0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            half4 col : COLOR;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
 			half4 params = input.col * 255;
			uint normIndex = (uint)(params.w);
			
            half3 normalDirection = normArray[ normIndex ];
            half3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
 
 			half4 vcolor;
 			
 			if(normIndex < 4)
            	vcolor = _SideColor;
            else
            	vcolor = _Color;
 
            half3 diffuseReflection = _LightColor0.rgb
               * max(0.0, dot(normalDirection, lightDirection)) + UNITY_LIGHTMODEL_AMBIENT;
 
 			//diffuseReflection = min(diffuseReflection, 1.0);
 
            output.col = half4(diffuseReflection, params.w + 0.01) * vcolor;
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         half4 frag(vertexOutput input) : COLOR
         {
            return input.col;
         }
 
         ENDCG
      }
   }
   Fallback "Mobile/Diffuse"
}
