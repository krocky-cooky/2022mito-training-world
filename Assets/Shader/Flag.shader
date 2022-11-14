Shader "Custom/Flag" {
    Properties {
        _Amplitude ("Amplitude", Float) = 0.0
        _AngularFrequency ("AngularFrequency", Float) = 100.0

        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;
        float _Amplitude;
        float _AngularFrequency;

        struct Input {
            float2 uv_MainTex;
        };

        void vert(inout appdata_full v, out Input o )
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            // float amp = 1.0f*sin(_Time*100 + v.vertex.x * 100);
            // float amp = _Amplitude * sin(_Time * _AngularFrequency + v.vertex.x * 100);
            float amp = _Amplitude * sin(_Time * _AngularFrequency + v.vertex.x * 100) * (v.vertex.x - 5);
            v.vertex.xyz = float3(v.vertex.x, v.vertex.y+amp, v.vertex.z);            
            //v.normal = normalize(float3(v.normal.x+offset_, v.normal.y, v.normal.z));
        }

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}