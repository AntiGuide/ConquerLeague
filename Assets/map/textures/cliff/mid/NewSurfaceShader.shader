// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/NewSurfaceShader"
{
	Properties
	{
		_cliff_mid_basecolor("cliff_mid_basecolor", 2D) = "white" {}
		_cliff_mid_normal("cliff_mid_normal", 2D) = "bump" {}
		_cliff_mid_roughness("cliff_mid_roughness", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _cliff_mid_normal;
		uniform float4 _cliff_mid_normal_ST;
		uniform sampler2D _cliff_mid_basecolor;
		uniform float4 _cliff_mid_basecolor_ST;
		uniform sampler2D _cliff_mid_roughness;
		uniform float4 _cliff_mid_roughness_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_cliff_mid_normal = i.uv_texcoord * _cliff_mid_normal_ST.xy + _cliff_mid_normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _cliff_mid_normal, uv_cliff_mid_normal ) );
			float2 uv_cliff_mid_basecolor = i.uv_texcoord * _cliff_mid_basecolor_ST.xy + _cliff_mid_basecolor_ST.zw;
			o.Albedo = tex2D( _cliff_mid_basecolor, uv_cliff_mid_basecolor ).rgb;
			float2 uv_cliff_mid_roughness = i.uv_texcoord * _cliff_mid_roughness_ST.xy + _cliff_mid_roughness_ST.zw;
			o.Smoothness = tex2D( _cliff_mid_roughness, uv_cliff_mid_roughness ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
1927;29;1266;948;1005;258;1;True;False
Node;AmplifyShaderEditor.SamplerNode;4;-662,396;Float;True;Property;_cliff_mid_roughness;cliff_mid_roughness;2;0;Create;True;0;0;False;0;83134baa949e2b549bdad1b48ba7f536;83134baa949e2b549bdad1b48ba7f536;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-547,158;Float;True;Property;_cliff_mid_normal;cliff_mid_normal;1;0;Create;True;0;0;False;0;dfc95ccf93de0c94eb80c6daac8f9e19;dfc95ccf93de0c94eb80c6daac8f9e19;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-538,-90;Float;True;Property;_cliff_mid_basecolor;cliff_mid_basecolor;0;0;Create;True;0;0;False;0;5e5c9a2ee1cce894196df2b63153c8c0;5e5c9a2ee1cce894196df2b63153c8c0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/NewSurfaceShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;0;0;1;0
WireConnection;0;1;2;0
WireConnection;0;4;4;0
ASEEND*/
//CHKSM=22FA0C9878F3859A9EA5FE50082DE69CB90D02B2