﻿Shader "CrossFade"
{
  Properties
  {
    _Blend ( "Blend", Range ( 0, 1 ) ) = 0.1
    _Color ("Color Tint", Color) = (1,1,1,1)
    _MainTex ("SelfIllum Color (RGB) Alpha (A)", 2D) = "white"
    _Texture2 ( "Texture 2", 2D ) = ""
  }
  //Tags {Queue=Transparent}
  SubShader
  {
    Lighting On
           ZWrite On
       Cull Back
       Blend SrcAlpha OneMinusSrcAlpha
    Tags {"Queue"="Transparent"}
    LOD 300
    
    
    //Alphatest Greater 0 ZWrite Off ColorMask RGB
    
    Material {
               Emission [_Color]
            }
    
    Pass
    {
    Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }
		//Blend SrcAlpha OneMinusSrcAlpha
      SetTexture[_MainTex]
      SetTexture[_Texture2]
      {
      Combine Texture * Primary, Texture * Primary
        ConstantColor ( 0, 0, 0, [_Blend] )
        Combine texture Lerp( constant ) previous
      }    
    }
  
    CGPROGRAM
    #pragma surface surf Lambert
    
    sampler2D _MainTex;
    sampler2D _Texture2;
    fixed4 _Color;
    float _Blend;
    
    struct Input
    {
      float2 uv_MainTex;
      float2 uv_Texture2;
    };
    
    void surf ( Input IN, inout SurfaceOutput o )
    {
      fixed4 t1  = tex2D( _MainTex, IN.uv_MainTex ) * _Color;
      fixed4 t2  = tex2D ( _Texture2, IN.uv_Texture2 ) * _Color;
      o.Albedo  = lerp( t1, t2, _Blend );
    }
    ENDCG
  }
  FallBack "Diffuse"
 }
