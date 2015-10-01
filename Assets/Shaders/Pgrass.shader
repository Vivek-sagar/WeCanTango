Shader "Custom/Pgrass" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Ambient ("Ambient", Color) = (1,1,1,1)
		_MainTex ("Main", 2D) = "white" {}
	}
	SubShader 
    {
   		 Tags { "Queue" = "Overlay" } // render after everything else
        // Setting the z write off to make sure our video overlay is always rendered at back.
        //ZWrite Off
        
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
             #include "UnityCG.cginc"
 
         	uniform float4 _LightColor0; 
         	sampler2D _MainTex;
         	float4 _Color;
            float4 _Ambient;
            
            struct vertexInput 
            {
            	float4 vertex : POSITION;
            	float3 normal : NORMAL;
            	float2 uv	  : TEXCOORD0;
         	};
         	
         	struct vertexOutput 
         	{
            	float4 pos : SV_POSITION;
            	float4 col : COLOR;
            	float2 uv  : TEXCOORD0;
         	};
 
	         vertexOutput vert(vertexInput input) 
	         {
	            vertexOutput output;
	 
	            float4x4 modelMatrix = _Object2World;
	            float4x4 modelMatrixInverse = _World2Object;
	 
	            float3 normalDirection = normalize(
	               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
	            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
	 
	            float3 diffuseReflection = _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection)) + _Ambient;
	 
	            output.col = float4(diffuseReflection, 1.0);
	            
	            float4 p = mul(UNITY_MATRIX_MVP, input.vertex);
	            output.pos = p;
	            output.uv = input.uv;
	            
	            return output;
	         }

            
            float4 frag(vertexOutput input) : COLOR
         	{
         		float4 color = _Color;
         		float twopi = 3.142 * 2;
         		float2 uv0 = (1 - input.uv.xy) * twopi;
         		float2 uv1 = (float2(1,0) - input.uv.xy) * twopi;
         		float2 uv2 = (float2(0,1) - input.uv.xy) * twopi;
         		float2 uv3 = (0 - input.uv.xy) * twopi;
         		
         		color = color * abs( sin(uv3.x * 2) * sin(uv1.y) + sin(uv0.x * 1.5) + sin(uv2.y * 2) ) * 0.1 + color * 0.9;
         	
                return color * input.col;
         	}


            ENDCG
        }

      
    }
}
