Shader "Custom/BubbleShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Alpha ("Alpha", Range(0.0,1.0)) = 1.0
	}
	SubShader {
	Pass{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 300

		Blend SrcAlpha OneMinusSrcAlpha

	 	CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
 
 		 uniform sampler2D _MainTex;
 		 uniform half _Alpha;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float2 uv	  : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;           
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            output.uv = input.uv;
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	float4 c = tex2D(_MainTex, input.uv);
         	clip(c.a - 0.5);
         	clip(_Alpha - 0.5);
            return float4(c.rgb,_Alpha * c.a);
         }
 
         ENDCG
}
}
	Fallback "Mobile/VertexLit"
}
