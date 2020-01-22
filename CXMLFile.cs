using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace XMLModel
{
    public class CXMLFile
    {
        private string m_XmlFile="";
        private string m_Error;

        private XmlDocument m_pDoc;
        private int m_Count;
        public CXMLFile()
        {

        }
        public CXMLFile(string sXmlFile)
        {
            m_XmlFile = sXmlFile;
        }
        public string XmlFile
        {
            get { return m_XmlFile; }
            set { m_XmlFile = value; }
        }
        public string ErrorMessage
        {
            get { return m_Error; }
        }

        public int ElementCount
        {
            get { return m_Count; }
        }
        public int newXml(string sStartElement)
        {
            int nRet = 0;
            try
            {
                if (File.Exists(m_XmlFile)) { File.Delete(m_XmlFile); }

                XmlWriterSettings mysets = new XmlWriterSettings();
                mysets.Indent = true;
                mysets.IndentChars = ("  ");
                XmlWriter mywriters = XmlWriter.Create(m_XmlFile, mysets);
                mywriters.WriteStartElement(sStartElement);
                mywriters.WriteEndElement();
                mywriters.Flush();
                mywriters.Close();
            }
            catch (System.Exception ex)
            {
                m_Error = ex.Message;
                nRet = 1;
            }
            if (nRet == 0) { nRet = loadXml(); }
            return nRet;
        }
        public int loadXml()
        {
            int nRet = 0;
            try
            {
                m_pDoc = new XmlDocument();
                m_pDoc.Load(m_XmlFile);
                XmlElement element=m_pDoc.DocumentElement;
                XmlNodeList nods = element.ChildNodes;
                m_Count = nods.Count;
                nRet = 0;
            }
            catch (System.Exception ex)
            {
                m_Error = ex.Message;
                nRet = -1;
                m_pDoc = null;
            }
            return nRet;
        }
        public string GettagName()
        {
            string sText = "";
            if (m_pDoc != null)
            {
                try
                {
                    XmlElement element = m_pDoc.DocumentElement;
                    sText = element.Name;
                }
                catch (System.Exception)
                {
                	
                }                
            }
            return sText;
        }
        public XmlNode getNode(XmlNode parentNode,int iNodeIndex)
        {
            XmlNode node=null;

            if (m_pDoc!=null) {
                try
                {
                    if (parentNode == null)
                    {
                        XmlElement element = m_pDoc.DocumentElement;
                        XmlNodeList lstNode = element.ChildNodes;
                        if (iNodeIndex >= 0 && iNodeIndex < lstNode.Count)
                        {
                            node = lstNode[iNodeIndex];
                        }
                    }
                    else
                    {
                        XmlNodeList lstNode = parentNode.ChildNodes;
                        if (iNodeIndex >= 0 && iNodeIndex < lstNode.Count)
                        {
                            node = lstNode[iNodeIndex];
                        }
                    }
                }
                catch (System.Exception)
                {
                	
                }                
            }
            return node;
        }
        public XmlNode getNode(XmlNode parentNode, string sNodeName)
        {
            XmlNode node = null,nodeRet=null;
            int i;

            if (m_pDoc != null)
            {
                try
                {
                    if (parentNode == null)
                    {
                        XmlElement element = m_pDoc.DocumentElement;
                        XmlNodeList lstNode = element.ChildNodes;
                        for (i = 0; i < lstNode.Count; i++)
                        {
                            node = lstNode[i];
                            if (string.Compare(node.Name,sNodeName,true)==0) {
                                nodeRet = node;
                                break;
                            }
                        }                            
                    }
                    else
                    {
                        XmlNodeList lstNode = parentNode.ChildNodes;
                        for (i = 0; i < lstNode.Count; i++)
                        {
                            node = lstNode[i];
                            if (string.Compare(node.Name, sNodeName, true) == 0)
                            {
                                nodeRet = node;
                                break;
                            }
                        }
                    }
                }
                catch (System.Exception)
                {

                }
            }
            return nodeRet;
        }
        public string getNodeName(XmlNode node)
        {
            string sText="";
            try
            {
                if (node == null)
                {
                    sText = m_pDoc.DocumentElement.Name;
                }
                else { sText = node.Name; }
            }
            catch (System.Exception)
            {
            	
            }
            return sText;
        }
        public string getNodeText(XmlNode node)
        {
            string sText = "";
            try
            {
                if (node == null)
                {
                    sText = m_pDoc.DocumentElement.Value;
                }
                else
                {
                    sText = node.Value;
                }
            }
            catch (System.Exception)
            {

            }
            return sText;
        }
        public string getNodeAttrValue(XmlNode node,string sAttrName)
        {
            int i;
            string sAttrValue="";

            XmlAttribute attr;
            try
            {
                if (node == null)
                {
                    XmlElement element = m_pDoc.DocumentElement;
                    for (i = 0; i < element.Attributes.Count; i++)
                    {
                        attr = element.Attributes[i];
                        if (string.Compare(attr.Name,sAttrName,true)==0) {
                            sAttrValue = attr.Value.ToString();
                        }
                    }
                }
                else
                {
                    for (i = 0; i < node.Attributes.Count; i++)
                    {
                        attr = node.Attributes[i];
                        if (string.Compare(attr.Name, sAttrName, true) == 0)
                        {
                            sAttrValue = attr.Value.ToString();
                        }
                    }
                }
            }
            catch (System.Exception)
            {

            }
            return sAttrValue;
        }
        public int getNodeAttr(XmlNode node,out List<string> ayAttrName,out List<string> ayAttrValue)
        {
            ayAttrName = new List<string>();
            ayAttrValue = new List<string>();
            int i;

            XmlAttribute attr;            
            try
            {
                if (node == null)
                {
                    XmlElement element = m_pDoc.DocumentElement;
                    for (i = 0; i < element.Attributes.Count; i++)
                    {
                        attr = element.Attributes[i];
                        ayAttrName.Add(attr.Name);
                        ayAttrValue.Add(attr.Value);
                    }
                }
                else
                {
                    for (i = 0; i < node.Attributes.Count; i++)
                    {
                        attr = node.Attributes[i];
                        ayAttrName.Add(attr.Name);
                        ayAttrValue.Add(attr.Value);
                    }
                }
            }
            catch (System.Exception)
            {
            	
            }
            
            return ayAttrName.Count;
        }
        public void setNodeAttr(XmlNode node,string sAttrName,string sAttrValue)
        {
            int i;
            bool bExists=false;

            XmlAttribute attr;
            try
            {
                if (node == null)
                {
                    XmlElement element = m_pDoc.DocumentElement;
                    for (i = 0; i < element.Attributes.Count; i++)
                    {
                        attr = element.Attributes[i];
                        if (string.Compare(attr.Name, sAttrName, true) == 0)
                        {
                            bExists = true;
                            if (attr.Value != sAttrValue)
                            {
                                attr.Value = sAttrValue;
                            }
                            break;
                        }
                    }
                    if (!bExists)
                    {
                        attr = m_pDoc.CreateAttribute(sAttrName);
                        attr.Value = sAttrValue;
                        element.Attributes.Append(attr);
                    }
                }
                else {
                    for (i = 0; i < node.Attributes.Count; i++)
                    {
                        attr = node.Attributes[i];
                        if (string.Compare(attr.Name, sAttrName, true) == 0)
                        {
                            bExists = true;
                            if (attr.Value != sAttrValue)
                            {
                                attr.Value = sAttrValue;
                            }
                            break;
                        }
                    }
                    if (!bExists)
                    {
                        attr = m_pDoc.CreateAttribute(sAttrName);
                        attr.Value = sAttrValue;
                        node.Attributes.Append(attr);
                    }
                }
            }
            catch (System.Exception)
            {
            	
            }
        }
        public void clearNodeAttr(XmlNode node)
        {
            try
            {
                if (node == null)
                {
                    XmlElement element = m_pDoc.DocumentElement;
                    element.Attributes.RemoveAll();
                }
                else
                {
                    node.Attributes.RemoveAll();
                }
            }
            catch (System.Exception)
            {
            	
            }
        }
        public void setNodeAttr(XmlNode node, List<string> ayAttrName, List<string> ayAttrValue)
        {
            int i,n;
            string sName,sValue;
            bool bExists;

            XmlAttribute attr;
            try
            {
                if (node == null)
                {
                    XmlElement element = m_pDoc.DocumentElement;
                    for (n = 0; n < ayAttrName.Count && n < ayAttrValue.Count; n++)
                    {
                        sName = ayAttrName[n];
                        sValue = ayAttrValue[n];
                        bExists = false;
                        for (i = 0; i < element.Attributes.Count; i++)
                        {                            
                            attr = element.Attributes[i];
                            if (string.Compare(attr.Name,sName,true)==0) {
                                bExists = true;
                                if (attr.Value!=sValue) {
                                    attr.Value = sValue;
                                }
                                break;
                            }                            
                        }
                        if (!bExists)
                        {
                            attr = m_pDoc.CreateAttribute(sName);
                            attr.Value = sValue;
                            element.Attributes.Append(attr);
                        }
                    }
                }
                else
                {
                    for (n = 0; n < ayAttrName.Count && n < ayAttrValue.Count; n++)
                    {
                        sName = ayAttrName[n];
                        sValue = ayAttrValue[n];
                        bExists = false;
                        for (i = 0; i < node.Attributes.Count; i++)
                        {
                            attr = node.Attributes[i];
                            if (string.Compare(attr.Name, sName, true) == 0)
                            {
                                bExists = true;
                                if (attr.Value != sValue)
                                {
                                    attr.Value = sValue;
                                }
                                break;
                            } 
                        }
                        if (!bExists)
                        {
                            attr = m_pDoc.CreateAttribute(sName);
                            attr.Value = sValue;
                            node.Attributes.Append(attr);
                        }
                    }
                }
            }
            catch (System.Exception)
            {

            }            
        }
        public XmlNode createNode(XmlNode nodParent,string sNodeName)
        {
            XmlNode obj,nodRet=null;            
            string sError;
            try
            {
                obj = m_pDoc.CreateNode(XmlNodeType.Element, sNodeName, "");
                if (nodParent==null) {
                    nodRet=m_pDoc.DocumentElement.AppendChild(obj);
                }
                else {
                    nodRet=nodParent.AppendChild(obj);
                }
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
            }
            return nodRet;
        }
        public XmlNode insertNode(XmlNode nodParent, string sNodeName,XmlNode nodRef,string sInsertPos)
        {
            XmlNode obj, nodRet = null;
            string sError;
            try
            {
                obj = m_pDoc.CreateNode(XmlNodeType.Element, sNodeName, "");
                if (nodParent == null)
                {
                    if (nodRef == null)
                    {
                        nodRet = m_pDoc.DocumentElement.AppendChild(obj);
                    }
                    else
                    {
                        if (string.Compare(sInsertPos, "after", true) == 0)
                        {
                            nodRet = m_pDoc.DocumentElement.InsertAfter(obj, nodRef);
                        }
                        else if (string.Compare(sInsertPos, "before", true) == 0)
                        {
                            nodRet = m_pDoc.DocumentElement.InsertBefore(obj, nodRef);
                        }
                        else
                        {
                            nodRet = m_pDoc.DocumentElement.AppendChild(obj);
                        }
                    }
                }
                else
                {
                    if (nodRef == null)
                    {
                        nodRet = nodParent.AppendChild(obj);
                    }
                    else {
                        if (string.Compare(sInsertPos,"after",true)==0) {
                            nodRet=nodParent.InsertAfter(obj,nodRef);
                        }
                        else if (string.Compare(sInsertPos,"before",true)==0) {
                            nodRet=nodParent.InsertBefore(obj,nodRef);
                        }
                        else {
                            nodRet = nodParent.AppendChild(obj);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
            }
            return nodRet;
        }
        public void save()
        {
            try
            {
                m_pDoc.Save(m_XmlFile);
                              
            }
            catch (System.Exception)
            {
            	
            }
        }
        public int getChildNodeCount(XmlNode parentNode)
        {
            int n = 0;
            if (m_pDoc != null)
            {
                try
                {
                    if (parentNode == null)
                    {
                        n = m_pDoc.DocumentElement.ChildNodes.Count;
                    }
                    else
                    {
                        n = parentNode.ChildNodes.Count;
                    }
                }
                catch (System.Exception)
                {

                }
            }
            return n;
        }
        public void deleteChildNode(XmlNode nodParent, string sNodeName)
        {
            string sError;
            try
            {
                XmlNode nod = getNode(nodParent, sNodeName);
                if (nod != null)
                {
                    if (nodParent == null)
                    {
                        m_pDoc.DocumentElement.RemoveChild(nod);
                    }
                    else
                    {
                        nodParent.RemoveChild(nod);
                    }
                }
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
            }
        }
        public void deleteChildNode(XmlNode nodParent, XmlNode nodDelete)
        {
            string sError;
            try
            {
                if (nodParent == null)
                {
                    m_pDoc.DocumentElement.RemoveChild(nodDelete);
                }
                else
                {
                    nodParent.RemoveChild(nodDelete);
                }
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
            }
        }
    }
}
