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
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_ground_lerp("ground_lerp", 2D) = "white" {}
		_ground_lerp2("ground_lerp2", 2D) = "white" {}
		_metallic("metallic", Float) = 0
		_smothness("smothness", Float) = 0
		_Color0("Color 0", Color) = (0,0,0,0)
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
		uniform float4 _Color0;
		uniform sampler2D _desert_ground_mid1_basecolor;
		uniform float4 _desert_ground_mid1_basecolor_ST;
		uniform sampler2D _desert_ground_mid_basecolor;
		uniform float4 _desert_ground_mid_basecolor_ST;
		uniform sampler2D _desert_white_sand_mid_basecolor;
		uniform float4 _desert_white_sand_mid_basecolor_ST;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _metallic;
		uniform float _smothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_desert_ground_mid1_normal = i.uv_texcoord * _desert_ground_mid1_normal_ST.xy + _desert_ground_mid1_normal_ST.zw;
			float2 uv_desert_ground_mid_normal = i.uv_texcoord * _desert_ground_mid_normal_ST.xy + _desert_ground_mid_normal_ST.zw;
			float2 uv_ground_lerp = i.uv_texcoord * _ground_lerp_ST.xy + _ground_lerp_ST.zw;
			float4 tex2DNode9 = tex2D( _ground_lerp, uv_ground_lerp );
			float4 lerpResult15 = lerp( tex2D( _desert_ground_mid1_normal, uv_desert_ground_mid1_normal ) , float4( UnpackNormal( tex2D( _desert_ground_mid_normal, uv_desert_ground_mid_normal ) ) , 0.0 ) , tex2DNode9.r);
			float2 uv_desert_white_sand_mid_normal = i.uv_texcoord * _desert_white_sand_mid_normal_ST.xy + _desert_white_sand_mid_normal_ST.zw;
			float2 uv_ground_lerp2 = i.uv_texcoord * _ground_lerp2_ST.xy + _ground_lerp2_ST.zw;
			float4 tex2DNode12 = tex2D( _ground_lerp2, uv_ground_lerp2 );
			float4 lerpResult16 = lerp( lerpResult15 , tex2D( _desert_white_sand_mid_normal, uv_desert_white_sand_mid_normal ) , tex2DNode12.r);
			o.Normal = lerpResult16.rgb;
			float2 uv_desert_ground_mid1_basecolor = i.uv_texcoord * _desert_ground_mid1_basecolor_ST.xy + _desert_ground_mid1_basecolor_ST.zw;
			float2 uv_desert_ground_mid_basecolor = i.uv_texcoord * _desert_ground_mid_basecolor_ST.xy + _desert_ground_mid_basecolor_ST.zw;
			float4 lerpResult11 = lerp( tex2D( _desert_ground_mid1_basecolor, uv_desert_ground_mid1_basecolor ) , tex2D( _desert_ground_mid_basecolor, uv_desert_ground_mid_basecolor ) , tex2DNode9.r);
			float2 uv_desert_white_sand_mid_basecolor = i.uv_texcoord * _desert_white_sand_mid_basecolor_ST.xy + _desert_white_sand_mid_basecolor_ST.zw;
			float4 lerpResult13 = lerp( lerpResult11 , tex2D( _desert_white_sand_mid_basecolor, uv_desert_white_sand_mid_basecolor ) , tex2DNode12.r);
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 lerpResult19 = lerp( ( _Color0 * lerpResult13 ) , lerpResult13 , tex2D( _TextureSample0, uv_TextureSample0 ));
			o.Albedo = lerpResult19.rgb;
			o.Metallic = _metallic;
			o.Smoothness = _smothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
1971;116;1187;822;1908.42;1229.849;2.103113;True;False
Node;AmplifyShaderEditor.SamplerNode;5;-1288.572,-477.9401;Float;True;Property;_desert_ground_mid1_basecolor;desert_ground_mid1_basecolor;2;0;Create;True;0;0;False;0;fa5fcf642d2aacc41ac813a00a6b8d63;36f7a61174c269e43adae9fef250c4fd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;-1256.343,-92.0625;Float;True;Property;_desert_ground_mid_basecolor;desert_ground_mid_basecolor;4;0;Create;True;0;0;False;0;060c1be3b4f6dcc4ea02a0d8e844fffd;d5936dff6ee9b264cbc29f5e232d06a0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-1306.795,-837.8165;Float;True;Property;_ground_lerp;ground_lerp;7;0;Create;True;0;0;False;0;1ea21dc7a1ac67348bec5d683ec9da39;1ea21dc7a1ac67348bec5d683ec9da39;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-351.0638,-815.0291;Float;True;Property;_ground_lerp2;ground_lerp2;8;0;Create;True;0;0;False;0;3a99c0269da5f304a9620fed0cf3df08;3a99c0269da5f304a9620fed0cf3df08;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1275.012,333.2159;Float;True;Property;_desert_white_sand_mid_basecolor;desert_white_sand_mid_basecolor;0;0;Create;True;0;0;False;0;1a47f91548c4e9243b26d56354760f68;e6ddb68a539e27a44832e9c8deef2d74;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;11;-725.9,-233.7854;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;22;-705.4383,-844.9777;Float;False;Property;_Color0;Color 0;11;0;Create;True;0;0;False;0;0,0,0,0;0.7352941,0.7352941,0.7352941,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-1284.752,-292.384;Float;True;Property;_desert_ground_mid1_normal;desert_ground_mid1_normal;3;0;Create;True;0;0;False;0;726e34b87a1fe2f43a21e07e26aae314;c247b020343168744ad7d6f2dbf60d32;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;13;-527.9863,-167.3163;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;8;-1258.788,125.5825;Float;True;Property;_desert_ground_mid_normal;desert_ground_mid_normal;5;0;Create;True;0;0;False;0;8ea471a8c98356c4e8fd3b289843ee32;a15794eb1e489c842a888fafbc087793;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-444.6532,-487.4508;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;24;-1020.906,-1044.775;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;0;0;False;0;1ea21dc7a1ac67348bec5d683ec9da39;1ea21dc7a1ac67348bec5d683ec9da39;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1249.513,532.567;Float;True;Property;_desert_white_sand_mid_normal;desert_white_sand_mid_normal;1;0;Create;True;0;0;False;0;983152a692893a645ba6c742def61e0a;c640d774ab0f05545a6a5448e2581221;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;15;-717.9047,-41.45943;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;16;-354.1439,365.6941;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-448.0412,34.0378;Float;False;Property;_metallic;metallic;9;0;Create;True;0;0;False;0;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-444.565,122.0511;Float;False;Property;_smothness;smothness;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;19;-137.5983,-249.7982;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Unlit/NewUnlitShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;5;0
WireConnection;11;1;7;0
WireConnection;11;2;9;0
WireConnection;13;0;11;0
WireConnection;13;1;3;0
WireConnection;13;2;12;0
WireConnection;23;0;22;0
WireConnection;23;1;13;0
WireConnection;15;0;6;0
WireConnection;15;1;8;0
WireConnection;15;2;9;0
WireConnection;16;0;15;0
WireConnection;16;1;4;0
WireConnection;16;2;12;0
WireConnection;19;0;23;0
WireConnection;19;1;13;0
WireConnection;19;2;24;0
WireConnection;0;0;19;0
WireConnection;0;1;16;0
WireConnection;0;3;17;0
WireConnection;0;4;18;0
ASEEND*/
//CHKSM=99A9520AFAB2CB451F032EBCB65A808024131E09