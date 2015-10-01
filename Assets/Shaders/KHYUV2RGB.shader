Shader "Tango/YUV2RGB" {
    Properties 
    {
          _YTex ("Y channel texture", 2D) = "white" {}
          _UTex ("U channel texture", 2D) = "white" {}
          _VTex ("V channel texture", 2D) = "white" {}
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
         
         uniform sampler2D _YTex;
            uniform sampler2D _UTex;
            uniform sampler2D _VTex;
            
            struct vertexInput 
            {
            	float4 vertex : POSITION;
            	float3 normal : NORMAL;
         	};
         	
         	struct vertexOutput 
         	{
            	float4 pos : SV_POSITION;
            	float4 col : COLOR;
            	float2 uv : TEXCOORD0;
         	};
 
	         vertexOutput vert(vertexInput input) 
	         {
	            vertexOutput output;
	 
	            float4x4 modelMatrix = _Object2World;
	            float4x4 modelMatrixInverse = _World2Object;
	 
	            float3 normalDirection = normalize(
	               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
	            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
	 
	            float3 diffuseReflection = _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection)) + 0.3;
	 
	            output.col = float4(diffuseReflection, 1.0);
	            
	            float4 p = mul(UNITY_MATRIX_MVP, input.vertex);
	            
	            output.pos = p;
	            
	            p.xy /= p.w;
                float2 px = (p.xy + 1) * 0.5;
                
                output.uv = px;
	            
	            return output;
	         }

			 uint mod(uint a, uint b) 
			 {
                return a - ((a / b) * b);
            }
            
             float4 frag(vertexOutput input) : COLOR
         {
            // In this example, we are using HAL_PIXEL_FORMAT_YCrCb_420_SP format
                // the data packing is: texture_y will pack 1280x720 pixels into
                // a 320x720 RGBA8888 texture.
                // texture_Cb and texture_Cr will contain copies of the 2x2 downsampled
                // interleaved UV planes packed similarly.
                float y_value, u_value, v_value;
                // Computing index in the color texture space (expected result). The target
                // texture size is 1280 x 720.
                uint x = int(input.uv.x * 1280.0);
                uint y = int((1.0 - input.uv.y) * 720.0);
                
                // Compute the Y value.
                uint x_y_image = uint(x / 4);
                uint x_y_offset = mod(x, 4);
                uint y_y_image = y;
                
                float4 c_y = tex2D(_YTex, float2(float(x_y_image) / float(320.0), float(y_y_image) / float(720.0)));
                if (x_y_offset == 0) {
                    y_value = c_y.r;
                } else if (x_y_offset == 1) {
                    y_value = c_y.g;
                } else if (x_y_offset == 2) {
                    y_value = c_y.b;
                } else if (x_y_offset == 3) {
                    y_value = c_y.a;
                }
                
                // Compute the U,V value.
                uint x_uv_image = uint(x / 4);
                uint x_uv_offset = mod(x, 4);
                uint y_uv_image = uint(y / 2);

                float4 c_uv = tex2D(_UTex, float2(float(x_uv_image) / float(320.0), float(y_uv_image) / float(360.0)));

                if (x_uv_offset == 0 || x_uv_offset == 1) {
                    v_value = c_uv.r;
                    u_value = c_uv.g;
                } else  if (x_uv_offset == 2 || x_uv_offset == 3) {
                    v_value = c_uv.b;
                    u_value = c_uv.a;
                }
                
                // The YUV to RBA conversion, please refer to: http://en.wikipedia.org/wiki/YUV
                // Y'UV420sp (NV21) to RGB conversion (Android) section.
                float r = y_value + 1.370705 * (v_value - 0.5);
                float g = y_value - 0.698001 * (v_value - 0.5) - (0.337633 * (u_value - 0.5));
                float b = y_value + 1.732446 * (u_value - 0.5);

                float4 _color = float4(r, g, b, 1.0) * input.col;
                
                return _color;
                
         }


            ENDCG
        }

      
    }
}