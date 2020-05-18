﻿/*
Much of the code for this shader was either auto-generated or is code that I found at the following link
https://answers.unity.com/questions/244837/shader-help-adding-transparency-to-a-shader.html
I did not write this code nor do I take any credit for this code.
*/

Shader "Custom/UnderwaterShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		//_Transparency("Transparency", Range(0.0,0.5)) = 0.25
		_MainTex("Color (RGB) Alpha (A)", 2D) = "white"
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
	}
		SubShader
	{

		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Cull Off
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		//#pragma surface surf Standard fullforwardshadows

		#pragma surface surf Lambert alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutput o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * c.a;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
		}

		ENDCG
	}
		FallBack "Diffuse"
}
