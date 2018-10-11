// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Unlit/tornado"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_tornado_tex("tornado_tex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample0;
		uniform sampler2D _tornado_tex;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 panner9 = ( 1.0 * _Time.y * float2( 0.7,0.1 ) + v.texcoord.xy);
			float4 tex2DNode10 = tex2Dlod( _TextureSample0, float4( panner9, 0, 0.0) );
			float3 temp_cast_0 = (( tex2DNode10.b * 0.2 )).xxx;
			v.vertex.xyz += temp_cast_0;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 panner1 = ( 1.0 * _Time.y * float2( 0.5,0.1 ) + i.uv_texcoord);
			float4 tex2DNode6 = tex2D( _tornado_tex, panner1 );
			float2 panner9 = ( 1.0 * _Time.y * float2( 0.7,0.1 ) + i.uv_texcoord);
			float4 tex2DNode10 = tex2D( _TextureSample0, panner9 );
			o.Emission = ( ( ( tex2DNode6.r + tex2DNode10.g ) * float4(0.6470588,0.4380444,0.2141004,0) ) * 1.7 ).rgb;
			o.Alpha = 1;
			clip( pow( ( ( tex2DNode6.r + tex2DNode10.g ) * ( tex2DNode6.g + tex2DNode10.r ) ) , 5.0 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
-51;381;1906;1004;3470.111;1020.134;1.947487;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-2883.765,-239.0681;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2883.638,-19.44134;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;1;-2665.416,-237.4134;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;9;-2665.291,-17.78665;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.7,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;10;-2496.918,-23.50707;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;5f7fd656c7095cc4db2d40e9306d3b01;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-2497.044,-243.1338;Float;True;Property;_tornado_tex;tornado_tex;2;0;Create;True;0;0;False;0;5f7fd656c7095cc4db2d40e9306d3b01;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-2128.293,-550.7902;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-2149.35,-725.9789;Float;False;Constant;_Color0;Color 0;2;0;Create;True;0;0;False;0;0.6470588,0.4380444,0.2141004,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-1861.119,197.2896;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-1867.62,-21.00446;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1838.117,-630.6373;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-649.9098,980.3887;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1623.536,90.66022;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1312.249,-345.8044;Float;False;Constant;_Float1;Float 1;3;0;Create;True;0;0;False;0;1.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-443.9599,837.1616;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1119.199,-725.0093;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;25;-1391.544,84.47752;Float;True;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;5;-20.53293,141.8639;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Unlit/tornado;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;1;0;3;0
WireConnection;9;0;11;0
WireConnection;10;1;9;0
WireConnection;6;1;1;0
WireConnection;37;0;6;1
WireConnection;37;1;10;2
WireConnection;24;0;6;2
WireConnection;24;1;10;1
WireConnection;21;0;6;1
WireConnection;21;1;10;2
WireConnection;36;0;37;0
WireConnection;36;1;16;0
WireConnection;8;0;21;0
WireConnection;8;1;24;0
WireConnection;13;0;10;3
WireConnection;13;1;14;0
WireConnection;33;0;36;0
WireConnection;33;1;34;0
WireConnection;25;0;8;0
WireConnection;5;2;33;0
WireConnection;5;10;25;0
WireConnection;5;11;13;0
ASEEND*/
//CHKSM=732F93BC8E50B6F043552CEA79D9F19867DEE960