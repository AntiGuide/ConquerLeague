// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Unlit/NewUnlitShader"
{
	Properties
	{
		_desert_white_sand_mid_basecolor("desert_white_sand_mid_basecolor", 2D) = "white" {}
		_desert_white_sand_mid_normal("desert_white_sand_mid_normal", 2D) = "white" {}
		_desert_ground_mid1_basecolor("desert_ground_mid1_basecolor", 2D) = "white" {}
		_desert_ground_mid1_normal("desert_ground_mid1_normal", 2D) = "white" {}
		_desert_ground_mid_basecolor("desert_ground_mid_basecolor", 2D) = "white" {}
		_desert_ground_mid_normal("desert_ground_mid_normal", 2D) = "bump" {}
		_ground_lerp("ground_lerp", 2D) = "white" {}
		_ground_lerp2("ground_lerp2", 2D) = "white" {}
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

		uniform sampler2D _desert_ground_mid1_normal;
		uniform float4 _desert_ground_mid1_normal_ST;
		uniform sampler2D _desert_ground_mid_normal;
		uniform float4 _desert_ground_mid_normal_ST;
		uniform sampler2D _ground_lerp;
		uniform float4 _ground_lerp_ST;
		uniform sampler2D _desert_white_sand_mid_normal;
		uniform float4 _desert_white_sand_mid_normal_ST;
		uniform sampler2D _ground_lerp2;
		uniform float4 _ground_lerp2_ST;
		uniform sampler2D _desert_ground_mid1_basecolor;
		uniform float4 _desert_ground_mid1_basecolor_ST;
		uniform sampler2D _desert_ground_mid_basecolor;
		uniform float4 _desert_ground_mid_basecolor_ST;
		uniform sampler2D _desert_white_sand_mid_basecolor;
		uniform float4 _desert_white_sand_mid_basecolor_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_desert_ground_mid1_normal = i.uv_texcoord * _desert_ground_mid1_normal_ST.xy + _desert_ground_mid1_normal_ST.zw;
			float2 uv_desert_ground_mid_normal = i.uv_texcoord * _desert_ground_mid_normal_ST.xy + _desert_ground_mid_normal_ST.zw;
			float2 uv_ground_lerp = i.uv_texcoord * _ground_lerp_ST.xy + _ground_lerp_ST.zw;
			float4 tex2DNode9 = tex2D( _ground_lerp, uv_ground_lerp );
			float4 lerpResult15 = lerp( tex2D( _desert_ground_mid1_normal, uv_desert_ground_mid1_normal ) , float4( UnpackNormal( tex2D( _desert_ground_mid_normal, uv_desert_ground_mid_normal ) ) , 0.0 ) , tex2DNode9);
			float2 uv_desert_white_sand_mid_normal = i.uv_texcoord * _desert_white_sand_mid_normal_ST.xy + _desert_white_sand_mid_normal_ST.zw;
			float2 uv_ground_lerp2 = i.uv_texcoord * _ground_lerp2_ST.xy + _ground_lerp2_ST.zw;
			float4 tex2DNode12 = tex2D( _ground_lerp2, uv_ground_lerp2 );
			float4 lerpResult16 = lerp( lerpResult15 , tex2D( _desert_white_sand_mid_normal, uv_desert_white_sand_mid_normal ) , tex2DNode12);
			o.Normal = lerpResult16.rgb;
			float2 uv_desert_ground_mid1_basecolor = i.uv_texcoord * _desert_ground_mid1_basecolor_ST.xy + _desert_ground_mid1_basecolor_ST.zw;
			float2 uv_desert_ground_mid_basecolor = i.uv_texcoord * _desert_ground_mid_basecolor_ST.xy + _desert_ground_mid_basecolor_ST.zw;
			float4 lerpResult11 = lerp( tex2D( _desert_ground_mid1_basecolor, uv_desert_ground_mid1_basecolor ) , tex2D( _desert_ground_mid_basecolor, uv_desert_ground_mid_basecolor ) , tex2DNode9);
			float2 uv_desert_white_sand_mid_basecolor = i.uv_texcoord * _desert_white_sand_mid_basecolor_ST.xy + _desert_white_sand_mid_basecolor_ST.zw;
			float4 lerpResult13 = lerp( lerpResult11 , tex2D( _desert_white_sand_mid_basecolor, uv_desert_white_sand_mid_basecolor ) , tex2DNode12);
			o.Albedo = lerpResult13.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
314;272;1187;822;2355.388;682.4249;2.235394;True;False
Node;AmplifyShaderEditor.SamplerNode;7;-1256.343,-92.0625;Float;True;Property;_desert_ground_mid_basecolor;desert_ground_mid_basecolor;4;0;Create;True;0;0;False;0;060c1be3b4f6dcc4ea02a0d8e844fffd;060c1be3b4f6dcc4ea02a0d8e844fffd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-1284.752,-292.384;Float;True;Property;_desert_ground_mid1_normal;desert_ground_mid1_normal;3;0;Create;True;0;0;False;0;726e34b87a1fe2f43a21e07e26aae314;726e34b87a1fe2f43a21e07e26aae314;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;8;-1258.788,125.5825;Float;True;Property;_desert_ground_mid_normal;desert_ground_mid_normal;5;0;Create;True;0;0;False;0;8ea471a8c98356c4e8fd3b289843ee32;8ea471a8c98356c4e8fd3b289843ee32;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-1288.572,-477.9401;Float;True;Property;_desert_ground_mid1_basecolor;desert_ground_mid1_basecolor;2;0;Create;True;0;0;False;0;fa5fcf642d2aacc41ac813a00a6b8d63;fa5fcf642d2aacc41ac813a00a6b8d63;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-897.5992,-621.1692;Float;True;Property;_ground_lerp;ground_lerp;6;0;Create;True;0;0;False;0;1ea21dc7a1ac67348bec5d683ec9da39;1ea21dc7a1ac67348bec5d683ec9da39;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;15;-548.174,124.4755;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;11;-537.5059,-24.8486;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;12;-898.7352,-828.4416;Float;True;Property;_ground_lerp2;ground_lerp2;7;0;Create;True;0;0;False;0;3a99c0269da5f304a9620fed0cf3df08;3a99c0269da5f304a9620fed0cf3df08;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1275.012,333.2159;Float;True;Property;_desert_white_sand_mid_basecolor;desert_white_sand_mid_basecolor;0;0;Create;True;0;0;False;0;1a47f91548c4e9243b26d56354760f68;1a47f91548c4e9243b26d56354760f68;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1249.513,532.567;Float;True;Property;_desert_white_sand_mid_normal;desert_white_sand_mid_normal;1;0;Create;True;0;0;False;0;983152a692893a645ba6c742def61e0a;983152a692893a645ba6c742def61e0a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;13;-240.8807,-12.30468;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;16;-245.1761,162.5537;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Unlit/NewUnlitShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;6;0
WireConnection;15;1;8;0
WireConnection;15;2;9;0
WireConnection;11;0;5;0
WireConnection;11;1;7;0
WireConnection;11;2;9;0
WireConnection;13;0;11;0
WireConnection;13;1;3;0
WireConnection;13;2;12;0
WireConnection;16;0;15;0
WireConnection;16;1;4;0
WireConnection;16;2;12;0
WireConnection;0;0;13;0
WireConnection;0;1;16;0
ASEEND*/
//CHKSM=514C9FB0AAC438ABF82469943E8AF8486DE4DDEF