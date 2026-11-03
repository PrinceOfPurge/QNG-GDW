Shader "PBR"
{
    Properties
    {
        //this brings a colour, metallic texture and a metallic range. the metallic texture is not a texture it is a map
        /** We are bringing in, color, a metallic texture and
        a metallic range. Note that the metallic texture,
        is not a texture per se, instead, it is a map to
        determine the parts we want to be metallic. **/
        _Color ("Color", Color) = (1,1,1,1)
        _MetallicTex ("Metallic (R)", 2D) = "white" {}
        _Metallic ("Metallic", Range(0.0,1.0)) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Geometry" }

        CGPROGRAM
        #pragma surface surf Standard //this is using standard shader and makes the surf standard
        /** Unlike our previous shaders, now we are
        using the standard shader, a this changes
        our surf function. **/

        sampler2D _MetallicTex;
        half _Metallic;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MetallicTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color.rgb;
            o.Smoothness = tex2D (_MetallicTex, IN.uv_MetallicTex).r;
            o.Metallic = _Metallic;
        }
        //this is using metallic and smoothness, smoothness is from the red channel
        /** This allows using the metallic and
        smoothness.
        Note that smoothness is taking the red
        channel
        Declare the values
        **/   
        ENDCG
    }
    FallBack "Diffuse"
}
