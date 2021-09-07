// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/NewImageEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PlayerPosition ("Player Position", vector) = (0,0,0,0) // The location of the player - will be set by script
        _VisibleDistance ("Visibility Distance", float) = 4.0 // How close does the player have to be to make object visible
        _ShadedDistance ("Shaded Distance", float) = 5.0 // How close does the player have to be to make object partially visible
        //_SecondaryTex("Secondary Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+1"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform float4 _PlayerPosition;
            uniform float _VisibleDistance;
            uniform float _ShadedDistance;
            uniform sampler2D _MainTex;
            //uniform sampler2D _SecondaryTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            // Input to vertex shader
            struct vertexInput {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            // Input to fragment shader
            struct vertexOutput {
                float4 pos : SV_POSITION;
                float4 position_in_world_space : TEXCOORD0;
                float4 tex : TEXCOORD1;
            };

            // VERTEX SHADER
            vertexOutput vert (vertexInput i)
            {
                vertexOutput o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.position_in_world_space = mul(unity_ObjectToWorld, i.vertex);
                o.tex = i.texcoord;
                return o;
            }

            fixed4 frag (vertexOutput i) : COLOR
            {
                //fixed4 frag (vertexOutput i) : SV_Target
                fixed4 col = tex2D(_MainTex, i.position_in_world_space);
                // fixed4 col = tex2D(_MainTex, i.position_in_world_space) + tex2D(_SecondaryTex, i.position_in_world_space);

                // Calculate distance to player position
                float dist = distance(i.position_in_world_space, _PlayerPosition);

                // Return appropriate colour
                if (dist < _VisibleDistance) {
                    col.a = 0.0; // Visible
                }
                else if (dist < _ShadedDistance) {
                    col.a = 0.0 + ((dist - _VisibleDistance) / (_ShadedDistance -_VisibleDistance)); // Nuanced
                }
                else {
                    col.a = 1.0; // Invisible
                }
                
                //col.a = 1.0f - col.r;
                // col.a = 0.96f - col.r;

                return col;
            }
            ENDCG
        }
    }
}
