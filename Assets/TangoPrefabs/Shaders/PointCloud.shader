Shader "Tango/PointCloud" {
  SubShader {
  	 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
     Lighting Off
     Blend One Zero
     ZWrite Off
     ZTest Always
     
     Pass {
  
        GLSLPROGRAM

        #ifdef VERTEX
        varying vec4 v_color;
        void main()
        {   
           gl_PointSize = 30.0;
           gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
           
           // Color should be based on pose relative info
           v_color = vec4(0.0,1.0,1.0,1.0);
        }
        #endif

        #ifdef FRAGMENT
        varying vec4 v_color;
        void main()
        {
           gl_FragColor = v_color;
        }
        #endif

        ENDGLSL
     }
  }
}
