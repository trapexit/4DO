////////////////////////////////
// JMK NOTES
//
// This portable settings provider found at http://www.codeproject.com/KB/vb/CustomSettingsProvider.aspx
// The original was in VB, but it was easy enough to convert it to C#
//
// I have seriously hacked up how this provider determines the filename to
// save to. But, here's the result.
//  * FourDO's default save always goes to FourDO.settings.
//  * All other settings using the provider will use the class name.
////////////////////////////////
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Configuration;
using System.Configuration.Provider;
using System.Windows.Forms;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.Xml;

namespace FourDO.UI
{
	public class PortableSettingsProvider : SettingsProvider
	{
		//XML Root Node
		const string SETTINGSROOT = "Settings";

		public override void Initialize(string name, NameValueCollection col)
		{
			base.Initialize(this.ApplicationName, col);
		}

		public override string ApplicationName
		{
			get
			{
				if (Application.ProductName.Trim().Length > 0)
				{
					return Application.ProductName;
				}
				else
				{
					System.IO.FileInfo fi = new System.IO.FileInfo(Application.ExecutablePath);
					return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
				}
			}
			//Do nothing
			set { }
		}

		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
		{
			this.CalcSettingsFilename(context);
			
			//Iterate through the settings to be stored
			//Only dirty settings are included in propvals, and only ones relevant to this provider
			foreach (SettingsPropertyValue propval in propvals)
			{
				SetValue(propval);
			}

			try
			{
				string directoryName = Path.GetDirectoryName(m_xmlFileName);
				if (!Directory.Exists(directoryName))
					Directory.CreateDirectory(directoryName);

				SettingsXML.Save(m_xmlFileName);
			}
			catch
			{
				//Ignore if cant save, device been ejected
			}
		}

		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
		{
			this.CalcSettingsFilename(context);

			//Create new collection of values
			SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

			//Iterate through the settings to be retrieved

			foreach (SettingsProperty setting in props)
			{
				SettingsPropertyValue value = new SettingsPropertyValue(setting);
				value.IsDirty = false;
				value.SerializedValue = GetValue(setting);
				values.Add(value);
			}
			return values;
		}

		private string m_xmlFileName = null;
		private System.Xml.XmlDocument m_SettingsXML = null;

		private void CalcSettingsFilename(SettingsContext context)
		{
			if (m_xmlFileName != null)
				return;

			////////////////
			// Determine the settings file name.
			string directoryName = FourDO.Utilities.Global.Constants.SettingsPath;

			// We'll leave fourdo's main settings as FourDo.settings.
			// Everything else will use the class name.
			string className;
			string groupName = (string)context["GroupName"];
			if (groupName == Properties.Settings.Default.GetType().FullName)
			{
				className = "FourDO";
			}
			else
			{
				string[] groupParts = groupName.Split('.');
				string lastPart = groupParts[groupParts.Length - 1];
				className = lastPart;
			}

			m_xmlFileName = System.IO.Path.Combine(directoryName, className + ".settings");
		}

		private XmlDocument SettingsXML
		{
			get
			{
				//If we dont hold an xml document, try opening one.  
				//If it doesnt exist then create a new one ready.
				if (m_SettingsXML == null)
				{
					m_SettingsXML = new System.Xml.XmlDocument();

					try
					{
						m_SettingsXML.Load(m_xmlFileName);
					}
					catch
					{
						//Create new document
						XmlDeclaration dec = m_SettingsXML.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
						m_SettingsXML.AppendChild(dec);

						XmlNode nodeRoot = null;

						nodeRoot = m_SettingsXML.CreateNode(XmlNodeType.Element, SETTINGSROOT, "");
						m_SettingsXML.AppendChild(nodeRoot);
					}
				}

				return m_SettingsXML;
			}
		}

		private string GetValue(SettingsProperty setting)
		{
			string ret = "";

			try
			{
				if (IsRoaming(setting))
				{
					ret = SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + setting.Name).InnerText;
				}
				else
				{
					ret = SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + System.Environment.MachineName + "/" + setting.Name).InnerText;
				}

			}
			catch
			{
				if ((setting.DefaultValue != null))
				{
					ret = setting.DefaultValue.ToString();
				}
				else
				{
					ret = "";
				}
			}

			return ret;
		}


		private void SetValue(SettingsPropertyValue propVal)
		{
			System.Xml.XmlElement MachineNode = null;
			System.Xml.XmlElement SettingNode = null;

			//Determine if the setting is roaming.
			//If roaming then the value is stored as an element under the root
			//Otherwise it is stored under a machine name node 
			try
			{
				if (IsRoaming(propVal.Property))
				{
					SettingNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + propVal.Name);
				}
				else
				{
					SettingNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + System.Environment.MachineName + "/" + propVal.Name);
				}
			}
			catch
			{
				SettingNode = null;
			}

			//Check to see if the node exists, if so then set its new value
			if ((SettingNode != null))
			{
				object tempVal = propVal.SerializedValue;
				SettingNode.InnerText = tempVal == null ? null : tempVal.ToString();
			}
			else
			{
				if (IsRoaming(propVal.Property))
				{
					//Store the value as an element of the Settings Root Node
					SettingNode = SettingsXML.CreateElement(propVal.Name);
					SettingNode.InnerText = propVal.SerializedValue.ToString();
					SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(SettingNode);
				}
				else
				{
					//Its machine specific, store as an element of the machine name node,
					//creating a new machine name node if one doesnt exist.
					try
					{
						MachineNode = (XmlElement)SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + System.Environment.MachineName);
					}
					catch
					{
						MachineNode = SettingsXML.CreateElement(System.Environment.MachineName);
						SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(MachineNode);
					}

					if (MachineNode == null)
					{
						MachineNode = SettingsXML.CreateElement(System.Environment.MachineName);
						SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(MachineNode);
					}

					SettingNode = SettingsXML.CreateElement(propVal.Name);
					SettingNode.InnerText = propVal.SerializedValue.ToString();
					MachineNode.AppendChild(SettingNode);
				}
			}
		}

		private bool IsRoaming(SettingsProperty prop)
		{
			//Determine if the setting is marked as Roaming
			foreach (DictionaryEntry d in prop.Attributes)
			{
				Attribute a = (Attribute)d.Value;
				if (a is System.Configuration.SettingsManageabilityAttribute)
				{
					return true;
				}
			}
			return false;
		}
	}
}
