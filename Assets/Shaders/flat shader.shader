Shader "Custom/flat shader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			 float4 color : COLOR;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			fixed3 norm = float3(0,1,0);
			
			if(IN.color.r > 0.999)
				norm = float3(1,0,0);
			else if(IN.color.r < 0.001)
				norm = float3(-1,0,0);
			
			if(IN.color.g > 0.999)
				norm = float3(0,1,0);
			else if(IN.color.g < 0.001)
				norm = float3(0,-1,0);
				
			if(IN.color.b > 0.999)
				norm = float3(0,0,1);
			else if(IN.color.b < 0.001)
				norm = float3(0,0,-1);
			
			o.Normal = norm;
			o.Albedo = _Color.rgb;
		
			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
