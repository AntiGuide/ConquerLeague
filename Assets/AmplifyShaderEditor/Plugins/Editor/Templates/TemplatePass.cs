using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AmplifyShaderEditor
{
	public enum TemplateModuleDataType
	{
		ModuleShaderModel,
		ModuleBlendMode,
		ModuleBlendOp,
		ModuleCullMode,
		ModuleColorMask,
		ModuleStencil,
		ModuleZwrite,
		ModuleZTest,
		ModuleZOffset,
		ModuleTag,
		ModuleGlobals,
		ModuleFunctions,
		ModulePragma,
		ModulePass,
		ModuleInputVert,
		ModuleInputFrag,
		PassVertexFunction,
		PassFragmentFunction,
		PassVertexData,
		PassInterpolatorData,
		PassNameData
	}

	public enum TemplateSRPType
	{
		BuiltIn,
		HD,
		Lightweight
	}

	[Serializable]
	public class TemplateModules
	{
		[SerializeField]
		private TemplateBlendData m_blendData = new TemplateBlendData();

		[SerializeField]
		private TemplateCullModeData m_cullModeData = new TemplateCullModeData();

		[SerializeField]
		private TemplateColorMaskData m_colorMaskData = new TemplateColorMaskData();

		[SerializeField]
		private TemplateStencilData m_stencilData = new TemplateStencilData();

		[SerializeField]
		private TemplateDepthData m_depthData = new TemplateDepthData();

		[SerializeField]
		private TemplateTagsModuleData m_tagData = new TemplateTagsModuleData();

		[SerializeField]
		private TemplateTagData m_globalsTag = new TemplateTagData( TemplatesManager.TemplateGlobalsTag, true );

		[SerializeField]
		private TemplateTagData m_functionsTag = new TemplateTagData( TemplatesManager.TemplateFunctionsTag, true );

		[SerializeField]
		private TemplateTagData m_pragmaTag = new TemplateTagData( TemplatesManager.TemplatePragmaTag, true );

		[SerializeField]
		private TemplateTagData m_passTag = new TemplateTagData( TemplatesManager.TemplatePassTag, true );

		[SerializeField]
		private TemplateTagData m_inputsVertTag = new TemplateTagData( TemplatesManager.TemplateInputsVertParamsTag, false );

		[SerializeField]
		private TemplateTagData m_inputsFragTag = new TemplateTagData( TemplatesManager.TemplateInputsFragParamsTag, false );

		[SerializeField]
		private TemplateShaderModelData m_shaderModel = new TemplateShaderModelData();

		[SerializeField]
		private TemplateSRPType m_srpType = TemplateSRPType.BuiltIn;

		[SerializeField]
		private string m_uniquePrefix;

		public void Destroy()
		{
			m_blendData = null;
			m_cullModeData = null;
			m_colorMaskData = null;
			m_stencilData = null;
			m_depthData = null;
			m_tagData.Destroy();
			m_tagData = null;
			m_globalsTag = null;
			m_functionsTag = null;
			m_pragmaTag = null;
			m_passTag = null;
			m_inputsVertTag = null;
			m_inputsFragTag = null;
		}

		public void ConfigureCommonTag( TemplateTagData tagData, TemplatePropertyContainer propertyContainer, TemplateIdManager idManager, string uniquePrefix, int offsetIdx, string subBody )
		{
			int id = subBody.IndexOf( tagData.Id );
			if( id >= 0 )
			{
				tagData.StartIdx = offsetIdx + id;
				idManager.RegisterId( tagData.StartIdx, uniquePrefix + tagData.Id, tagData.Id );
				propertyContainer.AddId( subBody, tagData.Id, tagData.SearchIndentation );
			}
		}

		public TemplateModules( TemplateIdManager idManager, TemplatePropertyContainer propertyContainer, string uniquePrefix, int offsetIdx, string subBody, bool isSubShader )
		{
			if( string.IsNullOrEmpty( subBody ) )
				return;

			m_uniquePrefix = uniquePrefix;

			//COMMON TAGS
			ConfigureCommonTag( m_globalsTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_functionsTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_pragmaTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_passTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_inputsVertTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_inputsFragTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );

			//BlEND MODE
			{
				int blendModeIdx = subBody.IndexOf( "Blend" );
				if( blendModeIdx > 0 )
				{
					int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, blendModeIdx );
					string blendParams = subBody.Substring( blendModeIdx, end - blendModeIdx );
					m_blendData.BlendModeId = blendParams;
					m_blendData.BlendModeStartIndex = offsetIdx + blendModeIdx;
					idManager.RegisterId( m_blendData.BlendModeStartIndex, uniquePrefix + m_blendData.BlendModeId, m_blendData.BlendModeId );

					TemplateHelperFunctions.CreateBlendMode( blendParams, ref m_blendData );
					if( m_blendData.ValidBlendMode )
					{
						propertyContainer.AddId( subBody, blendParams, false );
					}
				}
			}
			//BLEND OP
			{
				int blendOpIdx = subBody.IndexOf( "BlendOp" );
				if( blendOpIdx > 0 )
				{
					int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, blendOpIdx );
					string blendOpParams = subBody.Substring( blendOpIdx, end - blendOpIdx );
					m_blendData.BlendOpId = blendOpParams;
					BlendData.BlendOpStartIndex = offsetIdx + blendOpIdx;
					idManager.RegisterId( m_blendData.BlendOpStartIndex, uniquePrefix + m_blendData.BlendOpId, m_blendData.BlendOpId );
					TemplateHelperFunctions.CreateBlendOp( blendOpParams, ref m_blendData );
					if( m_blendData.ValidBlendOp )
					{
						propertyContainer.AddId( subBody, blendOpParams, false );
					}
				}

				m_blendData.DataCheck = ( m_blendData.ValidBlendMode || m_blendData.ValidBlendOp ) ? TemplateDataCheck.Valid : TemplateDataCheck.Invalid;
			}
			//CULL MODE
			{
				int cullIdx = subBody.IndexOf( "Cull" );
				if( cullIdx > 0 )
				{
					int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, cullIdx );
					string cullParams = subBody.Substring( cullIdx, end - cullIdx );
					m_cullModeData.CullModeId = cullParams;
					m_cullModeData.StartIdx = offsetIdx + cullIdx;
					idManager.RegisterId( m_cullModeData.StartIdx, uniquePrefix + m_cullModeData.CullModeId, m_cullModeData.CullModeId );
					TemplateHelperFunctions.CreateCullMode( cullParams, ref m_cullModeData );
					if( m_cullModeData.DataCheck == TemplateDataCheck.Valid )
						propertyContainer.AddId( subBody, cullParams, false, string.Empty );
				}
			}
			//COLOR MASK
			{
				int colorMaskIdx = subBody.IndexOf( "ColorMask" );
				if( colorMaskIdx > 0 )
				{
					int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, colorMaskIdx );
					string colorMaskParams = subBody.Substring( colorMaskIdx, end - colorMaskIdx );
					m_colorMaskData.ColorMaskId = colorMaskParams;
					m_colorMaskData.StartIdx = offsetIdx + colorMaskIdx;
					idManager.RegisterId( m_colorMaskData.StartIdx, uniquePrefix + m_colorMaskData.ColorMaskId, m_colorMaskData.ColorMaskId );
					TemplateHelperFunctions.CreateColorMask( colorMaskParams, ref m_colorMaskData );
					if( m_colorMaskData.DataCheck == TemplateDataCheck.Valid )
						propertyContainer.AddId( subBody, colorMaskParams, false );
				}
			}
			//STENCIL
			{
				int stencilIdx = subBody.IndexOf( "Stencil" );
				if( stencilIdx > -1 )
				{
					int stencilEndIdx = subBody.IndexOf( "}", stencilIdx );
					if( stencilEndIdx > 0 )
					{
						string stencilParams = subBody.Substring( stencilIdx, stencilEndIdx + 1 - stencilIdx );
						m_stencilData.StencilBufferId = stencilParams;
						m_stencilData.StartIdx = offsetIdx + stencilIdx;
						idManager.RegisterId( m_stencilData.StartIdx, uniquePrefix + m_stencilData.StencilBufferId, m_stencilData.StencilBufferId );
						TemplateHelperFunctions.CreateStencilOps( stencilParams, ref m_stencilData );
						if( m_stencilData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, stencilParams, true );
						}
					}
				}
			}
			//ZWRITE
			{
				int zWriteOpIdx = subBody.IndexOf( "ZWrite" );
				if( zWriteOpIdx > -1 )
				{
					int zWriteEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zWriteOpIdx );
					if( zWriteEndIdx > 0 )
					{
						m_depthData.ZWriteModeId = subBody.Substring( zWriteOpIdx, zWriteEndIdx + 1 - zWriteOpIdx );
						m_depthData.ZWriteStartIndex = offsetIdx + zWriteOpIdx;
						idManager.RegisterId( m_depthData.ZWriteStartIndex, uniquePrefix + m_depthData.ZWriteModeId, m_depthData.ZWriteModeId );
						TemplateHelperFunctions.CreateZWriteMode( m_depthData.ZWriteModeId, ref m_depthData );
						if( m_depthData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, m_depthData.ZWriteModeId, true );
						}
					}
				}
			}

			//ZTEST
			{
				int zTestOpIdx = subBody.IndexOf( "ZTest" );
				if( zTestOpIdx > -1 )
				{
					int zTestEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zTestOpIdx );
					if( zTestEndIdx > 0 )
					{
						m_depthData.ZTestModeId = subBody.Substring( zTestOpIdx, zTestEndIdx + 1 - zTestOpIdx );
						m_depthData.ZTestStartIndex = offsetIdx + zTestOpIdx;
						idManager.RegisterId( m_depthData.ZTestStartIndex, uniquePrefix + m_depthData.ZTestModeId, m_depthData.ZTestModeId );
						TemplateHelperFunctions.CreateZTestMode( m_depthData.ZTestModeId, ref m_depthData );
						if( m_depthData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, m_depthData.ZTestModeId, true );
						}
					}
				}
			}

			//ZOFFSET
			{
				int zOffsetIdx = subBody.IndexOf( "Offset" );
				if( zOffsetIdx > -1 )
				{
					int zOffsetEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zOffsetIdx );
					if( zOffsetEndIdx > 0 )
					{
						m_depthData.OffsetId = subBody.Substring( zOffsetIdx, zOffsetEndIdx + 1 - zOffsetIdx );
						m_depthData.OffsetStartIndex = offsetIdx + zOffsetIdx;
						idManager.RegisterId( m_depthData.OffsetStartIndex, uniquePrefix + m_depthData.OffsetId, m_depthData.OffsetId );
						TemplateHelperFunctions.CreateZOffsetMode( m_depthData.OffsetId, ref m_depthData );
						if( m_depthData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, m_depthData.OffsetId, true );
						}
					}
				}
			}
			//TAGS
			{
				int tagsIdx = subBody.IndexOf( "Tags" );
				if( tagsIdx > -1 )
				{
					int tagsEndIdx = subBody.IndexOf( "}", tagsIdx );
					if( tagsEndIdx > -1 )
					{
						m_tagData.Reset();
						m_tagData.TagsId = subBody.Substring( tagsIdx, tagsEndIdx + 1 - tagsIdx );
						m_tagData.StartIdx = offsetIdx + tagsIdx;
						idManager.RegisterId( m_tagData.StartIdx, uniquePrefix + m_tagData.TagsId, m_tagData.TagsId );
						m_srpType = TemplateHelperFunctions.CreateTags( ref m_tagData, isSubShader );
						propertyContainer.AddId( subBody, m_tagData.TagsId, false );
						m_tagData.DataCheck = TemplateDataCheck.Valid;
					}
					else
					{
						m_tagData.DataCheck = TemplateDataCheck.Invalid;
					}
				}
				else
				{
					m_tagData.DataCheck = TemplateDataCheck.Invalid;
				}
			}

			//SHADER MODEL
			{
				Match match = Regex.Match( subBody, TemplateHelperFunctions.ShaderModelPattern );
				if( match != null && match.Groups.Count > 1 )
				{
					if( TemplateHelperFunctions.AvailableInterpolators.ContainsKey( match.Groups[ 1 ].Value ) )
					{
						m_shaderModel.Id = match.Groups[ 0 ].Value;
						m_shaderModel.StartIdx = offsetIdx + match.Index;
						m_shaderModel.Value = match.Groups[ 1 ].Value;
						m_shaderModel.InterpolatorAmount = TemplateHelperFunctions.AvailableInterpolators[ match.Groups[ 1 ].Value ];
						m_shaderModel.DataCheck = TemplateDataCheck.Valid;
						idManager.RegisterId( m_shaderModel.StartIdx, uniquePrefix + m_shaderModel.Id, m_shaderModel.Id );
					}
					else
					{
						m_shaderModel.DataCheck = TemplateDataCheck.Invalid;
					}
				}
			}
		}

		public bool HasValidData
		{
			get
			{
				return m_blendData.DataCheck == TemplateDataCheck.Valid ||
						m_cullModeData.DataCheck == TemplateDataCheck.Valid ||
						m_colorMaskData.DataCheck == TemplateDataCheck.Valid ||
						m_stencilData.DataCheck == TemplateDataCheck.Valid ||
						m_depthData.DataCheck == TemplateDataCheck.Valid ||
						m_tagData.DataCheck == TemplateDataCheck.Valid ||
						m_shaderModel.DataCheck == TemplateDataCheck.Valid ||
						m_globalsTag.IsValid ||
						m_functionsTag.IsValid ||
						m_pragmaTag.IsValid ||
						m_passTag.IsValid ||
						m_inputsVertTag.IsValid ||
						m_inputsFragTag.IsValid;
			}
		}

		public TemplateBlendData BlendData { get { return m_blendData; } }
		public TemplateCullModeData CullModeData { get { return m_cullModeData; } }
		public TemplateColorMaskData ColorMaskData { get { return m_colorMaskData; } }
		public TemplateStencilData StencilData { get { return m_stencilData; } }
		public TemplateDepthData DepthData { get { return m_depthData; } }
		public TemplateTagsModuleData TagData { get { return m_tagData; } }
		public TemplateTagData GlobalsTag { get { return m_globalsTag; } }
		public TemplateTagData FunctionsTag { get { return m_functionsTag; } }
		public TemplateTagData PragmaTag { get { return m_pragmaTag; } }
		public TemplateTagData PassTag { get { return m_passTag; } }
		public TemplateTagData InputsVertTag { get { return m_inputsVertTag; } }
		public TemplateTagData InputsFragTag { get { return m_inputsFragTag; } }
		public TemplateShaderModelData ShaderModel { get { return m_shaderModel; } }
		public TemplateSRPType SRPType { get { return m_srpType; } }
		public string UniquePrefix { get { return m_uniquePrefix; } }
	}

	[Serializable]
	public class TemplatePass
	{
		private const string DefaultPassNameStr = "SubShader {0} Pass {1}";
		
		[SerializeField]
		private bool m_isInvisible = false;

		[SerializeField]
		private bool m_isMainPass = false;

		[SerializeField]
		private TemplateModules m_modules;

		[SerializeField]
		private List<TemplateInputData> m_inputDataList = new List<TemplateInputData>();
		private Dictionary<int, TemplateInputData> m_inputDataDict = new Dictionary<int, TemplateInputData>();

		[SerializeField]
		private TemplateFunctionData m_vertexFunctionData;

		[SerializeField]
		private TemplateFunctionData m_fragmentFunctionData;

		[SerializeField]
		private VertexDataContainer m_vertexDataContainer;

		[SerializeField]
		private TemplateInterpData m_interpolatorDataContainer;

		[SerializeField]
		private List<TemplateLocalVarData> m_localVarsList = new List<TemplateLocalVarData>();

		[SerializeField]
		private string m_uniquePrefix;

		[SerializeField]
		private TemplatePropertyContainer m_templateProperties = new TemplatePropertyContainer();

		[SerializeField]
		private List<TemplateShaderPropertyData> m_availableShaderGlobals = new List<TemplateShaderPropertyData>();

		[SerializeField]
		TemplateInfoContainer m_passNameContainer = new TemplateInfoContainer();

		public TemplatePass( int subshaderIdx, int passIdx, TemplateIdManager idManager, string uniquePrefix, int offsetIdx, TemplatePassInfo passInfo, ref Dictionary<string, TemplateShaderPropertyData> duplicatesHelper )
		{
			m_uniquePrefix = uniquePrefix;

			m_isMainPass = passInfo.Data.Contains( TemplatesManager.TemplateMainPassTag );
			if( !m_isMainPass ) 
				m_isInvisible = passInfo.Data.Contains( TemplatesManager.TemplateExcludeFromGraphTag );

			FetchPassName( offsetIdx, passInfo.Data );
			if( m_passNameContainer.Index > -1 )
			{
				idManager.RegisterId( m_passNameContainer.Index, uniquePrefix + m_passNameContainer.Id, m_passNameContainer.Id );
			}
			else
			{
				m_passNameContainer.Data = string.Format( DefaultPassNameStr, subshaderIdx, passIdx );
			}

			m_modules = new TemplateModules( idManager, m_templateProperties, uniquePrefix + "Module", offsetIdx, passInfo.Data, false );

			Dictionary<string, TemplateShaderPropertyData> ownDuplicatesDict = new Dictionary<string, TemplateShaderPropertyData>( duplicatesHelper );
			TemplateHelperFunctions.CreateShaderGlobalsList( passInfo.Data, ref m_availableShaderGlobals, ref ownDuplicatesDict );

			// Vertex and Interpolator data
			FetchVertexAndInterpData( offsetIdx, passInfo.Data );
			if( m_vertexDataContainer != null )
				idManager.RegisterId( m_vertexDataContainer.VertexDataStartIdx, uniquePrefix + m_vertexDataContainer.VertexDataId, m_vertexDataContainer.VertexDataId );

			if( m_interpolatorDataContainer != null )
				idManager.RegisterId( m_interpolatorDataContainer.InterpDataStartIdx, uniquePrefix + m_interpolatorDataContainer.InterpDataId, m_interpolatorDataContainer.InterpDataId );

			//Fetch function code areas
			FetchCodeAreas( offsetIdx, TemplatesManager.TemplateVertexCodeBeginArea, MasterNodePortCategory.Vertex, passInfo.Data );
			if( m_vertexFunctionData != null )
				idManager.RegisterId( m_vertexFunctionData.Position, uniquePrefix + m_vertexFunctionData.Id, m_vertexFunctionData.Id );

			FetchCodeAreas( offsetIdx, TemplatesManager.TemplateFragmentCodeBeginArea, MasterNodePortCategory.Fragment, passInfo.Data );
			if( m_fragmentFunctionData != null )
				idManager.RegisterId( m_fragmentFunctionData.Position, uniquePrefix + m_fragmentFunctionData.Id, m_fragmentFunctionData.Id );

			//Fetching inputs
			FetchInputs( offsetIdx, MasterNodePortCategory.Fragment, passInfo.Data );
			FetchInputs( offsetIdx, MasterNodePortCategory.Vertex, passInfo.Data );

			//Fetch local variables must be done after fetching code areas as it needs them to see is variable is on vertex or fragment
			TemplateHelperFunctions.FetchLocalVars( passInfo.Data, ref m_localVarsList, m_vertexFunctionData, m_fragmentFunctionData );

			int localVarCount = m_localVarsList.Count;
			if( localVarCount > 0 )
			{
				idManager.RegisterTag( TemplatesManager.TemplateLocalVarTag );
				for( int i = 0; i < localVarCount; i++ )
				{
					if( m_localVarsList[ i ].IsSpecialVar )
					{
						idManager.RegisterTag( m_localVarsList[ i ].Id );
					}
				}
			}

			int inputsCount = m_inputDataList.Count;
			for( int i = 0; i < inputsCount; i++ )
			{
				if( m_inputDataList[ i ] != null )
					idManager.RegisterId( m_inputDataList[ i ].TagGlobalStartIdx, uniquePrefix + m_inputDataList[ i ].TagId, m_inputDataList[ i ].TagId );
			}

			ownDuplicatesDict.Clear();
			ownDuplicatesDict = null;
		}

		public void Destroy()
		{
			m_passNameContainer = null;
			if( m_templateProperties != null )
				m_templateProperties.Destroy();
			m_templateProperties = null;

			if( m_modules != null )
				m_modules.Destroy();

			m_modules = null;

			if( m_inputDataList != null )
				m_inputDataList.Clear();

			m_inputDataList = null;

			if( m_inputDataDict != null )
				m_inputDataDict.Clear();

			m_inputDataDict = null;

			m_vertexFunctionData = null;
			m_fragmentFunctionData = null;

			if( m_vertexDataContainer != null )
				m_vertexDataContainer.Destroy();

			m_vertexDataContainer = null;

			if( m_interpolatorDataContainer != null )
				m_interpolatorDataContainer.Destroy();

			if( m_localVarsList != null )
			{
				m_localVarsList.Clear();
				m_localVarsList = null;
			}

			m_interpolatorDataContainer = null;

			if( m_availableShaderGlobals != null )
				m_availableShaderGlobals.Clear();

			m_availableShaderGlobals = null;
		}

		public TemplateInputData InputDataFromId( int id )
		{
			if( m_inputDataDict == null )
				m_inputDataDict = new Dictionary<int, TemplateInputData>();

			if( m_inputDataDict.Count != m_inputDataList.Count )
			{
				m_inputDataDict.Clear();
				for( int i = 0; i < m_inputDataList.Count; i++ )
				{
					m_inputDataDict.Add( m_inputDataList[ i ].PortUniqueId, m_inputDataList[ i ] );
				}
			}

			if( m_inputDataDict.ContainsKey( id ) )
				return m_inputDataDict[ id ];

			return null;
		}

		void FetchPassName( int offsetIdx, string body )
		{
			Match match = Regex.Match( body, TemplateHelperFunctions.PassNamePattern );
			if( match != null && match.Groups.Count > 1 )
			{
				m_passNameContainer.Id = match.Groups[ 0 ].Value;
				m_passNameContainer.Data = match.Groups[ 1 ].Value;
				m_passNameContainer.Index = offsetIdx + match.Index;
			}
		}

		void FetchVertexAndInterpData( int offsetIdx, string body )
		{
			// Vertex Data
			try
			{
				int vertexDataTagBegin = body.IndexOf( TemplatesManager.TemplateVertexDataTag );
				if( vertexDataTagBegin > -1 )
				{
					m_vertexDataContainer = new VertexDataContainer();
					m_vertexDataContainer.VertexDataStartIdx = offsetIdx + vertexDataTagBegin;
					int vertexDataTagEnd = body.IndexOf( TemplatesManager.TemplateEndOfLine, vertexDataTagBegin );
					m_vertexDataContainer.VertexDataId = body.Substring( vertexDataTagBegin, vertexDataTagEnd + TemplatesManager.TemplateEndOfLine.Length - vertexDataTagBegin );
					int dataBeginIdx = body.LastIndexOf( '{', vertexDataTagBegin, vertexDataTagBegin );
					string vertexData = body.Substring( dataBeginIdx + 1, vertexDataTagBegin - dataBeginIdx );

					int parametersBegin = vertexDataTagBegin + TemplatesManager.TemplateVertexDataTag.Length;
					string parameters = body.Substring( parametersBegin, vertexDataTagEnd - parametersBegin );
					m_vertexDataContainer.VertexData = TemplateHelperFunctions.CreateVertexDataList( vertexData, parameters );
					m_templateProperties.AddId( body, m_vertexDataContainer.VertexDataId );
				}
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}

			// Available interpolators
			try
			{
				int interpDataBegin = body.IndexOf( TemplatesManager.TemplateInterpolatorBeginTag );
				if( interpDataBegin > -1 )
				{
					int interpDataEnd = body.IndexOf( TemplatesManager.TemplateEndOfLine, interpDataBegin );
					string interpDataId = body.Substring( interpDataBegin, interpDataEnd + TemplatesManager.TemplateEndOfLine.Length - interpDataBegin );

					int dataBeginIdx = body.LastIndexOf( '{', interpDataBegin, interpDataBegin );
					string interpData = body.Substring( dataBeginIdx + 1, interpDataBegin - dataBeginIdx );

					m_interpolatorDataContainer = TemplateHelperFunctions.CreateInterpDataList( interpData, interpDataId, m_modules.ShaderModel.InterpolatorAmount );
					m_interpolatorDataContainer.InterpDataId = interpDataId;
					m_interpolatorDataContainer.InterpDataStartIdx = offsetIdx + interpDataBegin;
					m_templateProperties.AddId( body, interpDataId );

				}
			}
			catch( Exception e )
			{
				Debug.LogException( e );
			}
		}

		void FetchCodeAreas( int offsetIdx, string begin, MasterNodePortCategory category, string body )
		{
			int areaBeginIndexes = body.IndexOf( begin );
			if( areaBeginIndexes > -1 )
			{
				int beginIdx = areaBeginIndexes + begin.Length;
				int endIdx = body.IndexOf( TemplatesManager.TemplateEndOfLine, beginIdx );
				int length = endIdx - beginIdx;

				string parameters = body.Substring( beginIdx, length );

				string[] parametersArr = parameters.Split( IOUtils.FIELD_SEPARATOR );

				string id = body.Substring( areaBeginIndexes, endIdx + TemplatesManager.TemplateEndOfLine.Length - areaBeginIndexes );
				string inParameters = parametersArr[ 0 ];
				string outParameters = ( parametersArr.Length > 1 ) ? parametersArr[ 1 ] : string.Empty;
				if( category == MasterNodePortCategory.Fragment )
				{
					string mainBodyName = string.Empty;
					int mainBodyLocalIndex = -1;

					Match mainBodyNameMatch = Regex.Match( body, TemplateHelperFunctions.FragmentPragmaPattern );
					if( mainBodyNameMatch != null && mainBodyNameMatch.Groups.Count == 2 )
					{
						mainBodyName = mainBodyNameMatch.Groups[ 1 ].Value;
						string pattern = string.Format( TemplateHelperFunctions.FunctionBodyStartPattern, mainBodyName );
						Match mainBodyIdMatch = Regex.Match( body, pattern );
						if( mainBodyIdMatch != null && mainBodyIdMatch.Groups.Count > 0 )
						{
							mainBodyLocalIndex = mainBodyIdMatch.Index;
						}

					}

					m_fragmentFunctionData = new TemplateFunctionData( mainBodyLocalIndex, mainBodyName, id, offsetIdx + areaBeginIndexes, inParameters, outParameters, category );
				}
				else
				{
					string mainBodyName = string.Empty;
					int mainBodyLocalIndex = -1;

					Match mainBodyNameMatch = Regex.Match( body, TemplateHelperFunctions.VertexPragmaPattern );
					if( mainBodyNameMatch != null && mainBodyNameMatch.Groups.Count == 2 )
					{
						mainBodyName = mainBodyNameMatch.Groups[ 1 ].Value;
						string pattern = string.Format( TemplateHelperFunctions.FunctionBodyStartPattern, mainBodyName );
						Match mainBodyIdMatch = Regex.Match( body, pattern );
						if( mainBodyIdMatch != null && mainBodyIdMatch.Groups.Count > 0 )
						{
							mainBodyLocalIndex = mainBodyIdMatch.Index;
						}
					}

					m_vertexFunctionData = new TemplateFunctionData( mainBodyLocalIndex, mainBodyName, id, offsetIdx + areaBeginIndexes, inParameters, outParameters, category );
				}
				m_templateProperties.AddId( body, id, true );
			}
		}

		void FetchInputs( int offset, MasterNodePortCategory portCategory, string body )
		{
			string beginTag = ( portCategory == MasterNodePortCategory.Fragment ) ? TemplatesManager.TemplateInputsFragBeginTag : TemplatesManager.TemplateInputsVertBeginTag;
			int[] inputBeginIndexes = body.AllIndexesOf( beginTag );
			if( inputBeginIndexes != null && inputBeginIndexes.Length > 0 )
			{
				for( int i = 0; i < inputBeginIndexes.Length; i++ )
				{
					int inputEndIdx = body.IndexOf( TemplatesManager.TemplateEndSectionTag, inputBeginIndexes[ i ] );
					int defaultValueBeginIdx = inputEndIdx + TemplatesManager.TemplateEndSectionTag.Length;
					int endLineIdx = body.IndexOf( TemplatesManager.TemplateFullEndTag, defaultValueBeginIdx );

					string defaultValue = body.Substring( defaultValueBeginIdx, endLineIdx - defaultValueBeginIdx );
					string tagId = body.Substring( inputBeginIndexes[ i ], endLineIdx + TemplatesManager.TemplateFullEndTag.Length - inputBeginIndexes[ i ] );

					int beginIndex = inputBeginIndexes[ i ] + beginTag.Length;
					int length = inputEndIdx - beginIndex;
					string inputData = body.Substring( beginIndex, length );
					string[] inputDataArray = inputData.Split( IOUtils.FIELD_SEPARATOR );
					
					if( inputDataArray != null && inputDataArray.Length > 0 )
					{
						try
						{
							string portName = inputDataArray[ (int)TemplatePortIds.Name ];
							WirePortDataType dataType = (WirePortDataType)Enum.Parse( typeof( WirePortDataType ), inputDataArray[ (int)TemplatePortIds.DataType ].ToUpper() );
							if( inputDataArray.Length == 3 )
							{
								int portOrderId = m_inputDataList.Count;
								int portUniqueId = -1;
								bool isInt = int.TryParse( inputDataArray[ 2 ], out portUniqueId );
								if( isInt )
								{
									if( portUniqueId < 0 )
										portUniqueId = m_inputDataList.Count;

									m_inputDataList.Add( new TemplateInputData( inputBeginIndexes[ i ], offset + inputBeginIndexes[ i ], tagId, portName, defaultValue, dataType, portCategory, portUniqueId, portOrderId, string.Empty ) );
									m_templateProperties.AddId( body, tagId, false );
								}
								else
								{
									portUniqueId = m_inputDataList.Count;
									m_inputDataList.Add( new TemplateInputData( inputBeginIndexes[ i ], offset + inputBeginIndexes[ i ], tagId, portName, defaultValue, dataType, portCategory, portUniqueId, portOrderId, inputDataArray[ 2 ] ) );
									m_templateProperties.AddId( body, tagId, false );
								}
							}
							else
							{
								int portUniqueIDArrIdx = (int)TemplatePortIds.UniqueId;
								int portUniqueId = ( portUniqueIDArrIdx < inputDataArray.Length ) ? Convert.ToInt32( inputDataArray[ portUniqueIDArrIdx ] ) : -1;
								if( portUniqueId < 0 )
									portUniqueId = m_inputDataList.Count;

								int portOrderArrayIdx = (int)TemplatePortIds.OrderId;
								int portOrderId = ( portOrderArrayIdx < inputDataArray.Length ) ? Convert.ToInt32( inputDataArray[ portOrderArrayIdx ] ) : -1;
								if( portOrderId < 0 )
									portOrderId = m_inputDataList.Count;

								int portLinkIdx = (int)TemplatePortIds.Link;
								string linkId = ( portLinkIdx < inputDataArray.Length ) ? inputDataArray[ portLinkIdx ] : string.Empty;
								m_inputDataList.Add( new TemplateInputData( inputBeginIndexes[ i ], offset + inputBeginIndexes[ i ], tagId, portName, defaultValue, dataType, portCategory, portUniqueId, portOrderId, linkId ) );
								m_templateProperties.AddId( body, tagId, false );
							}
						}
						catch( Exception e )
						{
							Debug.LogException( e );
						}
					}
				}
			}
		}

		public TemplateModules Modules { get { return m_modules; } }
		public List<TemplateInputData> InputDataList { get { return m_inputDataList; } }
		public TemplateFunctionData VertexFunctionData { get { return m_vertexFunctionData; } }
		public TemplateFunctionData FragmentFunctionData { get { return m_fragmentFunctionData; } }
		public VertexDataContainer VertexDataContainer { get { return m_vertexDataContainer; } }
		public TemplateInterpData InterpolatorDataContainer { get { return m_interpolatorDataContainer; } }
		public string UniquePrefix { get { return m_uniquePrefix; } }
		public TemplatePropertyContainer TemplateProperties { get { return m_templateProperties; } }
		public List<TemplateShaderPropertyData> AvailableShaderGlobals { get { return m_availableShaderGlobals; } }
		public List<TemplateLocalVarData> LocalVarsList { get { return m_localVarsList; } }
		public TemplateInfoContainer PassNameContainer { get { return m_passNameContainer; } }
		public bool IsMainPass { get { return m_isMainPass; } set { m_isMainPass = value; } }
		public bool IsInvisible { get { return m_isInvisible; } set { m_isInvisible = value; } }
		public bool AddToList
		{
			get
			{
				if( m_isInvisible )
				{
					return ( m_inputDataList.Count > 0 );
				}

				return true;
			}
		}
		public bool HasValidFunctionBody
		{
			get
			{
				if( m_fragmentFunctionData != null || m_vertexFunctionData != null )
					return true;
				return false;
			}
		}
	}
}
