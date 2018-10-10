using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum TemplateDataType
	{
		LegacySinglePass,
		MultiPass
	}

	[Serializable]
	public class TemplateInfoContainer
	{
		public string Id = string.Empty;
		public string Data = string.Empty;
		public int Index = -1;
	}

	[Serializable]
	public class TemplateDataParent
	{
		[SerializeField]
		private TemplateDataType m_templateType;

		[ SerializeField]
		protected string m_name;

		[SerializeField]
		protected string m_guid;

		[SerializeField]
		protected int m_orderId;

		[SerializeField]
		protected string m_defaultShaderName = string.Empty;

		[SerializeField]
		protected bool m_isValid = true;

		[SerializeField]
		protected bool m_communityTemplate = false;

		public TemplateDataParent( TemplateDataType templateType )
		{
			m_templateType = templateType;
		}
		public virtual void Destroy() { }
		public virtual void Reload() { }
		public string Name { get { return m_name; } set { m_name = value; } }
		public string GUID { get { return m_guid; } set { m_guid = value; } }
		public int OrderId { get { return m_orderId; } set { m_orderId = value; } }
		public string DefaultShaderName { get { return m_defaultShaderName; } set { m_defaultShaderName = value; } }
		public bool IsValid { get { return m_isValid; } }
		public TemplateDataType TemplateType { get { return m_templateType; } }
	}
}


