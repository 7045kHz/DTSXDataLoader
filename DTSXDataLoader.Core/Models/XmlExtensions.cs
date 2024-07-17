using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
namespace DTSXDataLoaderCore.Models
{
    static class XmlExtensions
    {
        /// <summary>
        /// GetLevel(node) returnse the number of parent nodes to the current node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
         public static int GetLevel(this XPathNavigator node)
        {
            var n = node.Clone();
            if (n.MoveToParent() == false) return 0;
            return 1 + n.GetLevel();
        }
        /*
                    'R',    // Root
                    'E',    // Element
                    'A',    // Attribute
                    'N',    // Namespace
                    'T',    // Text
                    'S',    // SignificantWhitespace
                    'W',    // Whitespace
                    'P',    // ProcessingInstruction
                    'C',    // Comment
        */
       
        public static string NewGUID(XPathNavigator node )
        {
            var n1 = node.Clone();
            char prefixGUID;
            
            switch (n1.NodeType)
            {
                case XPathNodeType.Namespace:
                    prefixGUID = 'N';
                    break;
                case XPathNodeType.Attribute:
                    prefixGUID = 'A';
                    break;
                case XPathNodeType.Element:
                    prefixGUID = 'E';
                    break;
                case XPathNodeType.Root:
                    prefixGUID = 'R';
                    break;
                case XPathNodeType.Text:
                    prefixGUID = 'T';
                    break;
                case XPathNodeType.SignificantWhitespace:
                    prefixGUID = 'S';
                    break;
                case XPathNodeType.Whitespace:
                    prefixGUID = 'W';
                    break;
                case XPathNodeType.ProcessingInstruction:
                    prefixGUID = 'P';
                    break;
                case XPathNodeType.Comment:
                    prefixGUID = 'C';
                    break;
                default:
                    prefixGUID = 'X';
                    break;
            }
            byte[]? ba = null;
            ba = Encoding.Default.GetBytes( GetPath(n1));
            var hexString = BitConverter.ToString(ba);
            return prefixGUID.ToString() + ':' + hexString.Replace("-","");


        }
        public static string NewGUID(XPathNavigator node, string label)
        {
            var n1 = node.Clone();
            char prefixGUID;

            switch (n1.NodeType)
            {
                case XPathNodeType.Namespace:
                    prefixGUID = 'N';
                    break;
                case XPathNodeType.Attribute:
                    prefixGUID = 'A';
                    break;
                case XPathNodeType.Element:
                    prefixGUID = 'E';
                    break;
                case XPathNodeType.Root:
                    prefixGUID = 'R';
                    break;
                case XPathNodeType.Text:
                    prefixGUID = 'T';
                    break;
                case XPathNodeType.SignificantWhitespace:
                    prefixGUID = 'S';
                    break;
                case XPathNodeType.Whitespace:
                    prefixGUID = 'W';
                    break;
                case XPathNodeType.ProcessingInstruction:
                    prefixGUID = 'P';
                    break;
                case XPathNodeType.Comment:
                    prefixGUID = 'C';
                    break;
                default:
                    prefixGUID = 'X';
                    break;
            }
            byte[]? ba = null;
            ba = Encoding.Default.GetBytes(GetPath(n1, label));
            var hexString = BitConverter.ToString(ba);
            return prefixGUID.ToString() + ':' + hexString.Replace("-", "");


        }

        /// <summary>
        /// GetPath(node) method attempts to derive the full node path to the current node. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetPath(this XPathNavigator node)
        {
            var n = node.Clone();
            string str = null;
            List<string> names = new List<string>();
            // Add potential object names
            names.Add("ObjectName");
            names.Add("Name");
            names.Add("name");
            names.Add("cachedName");
            names.Add("SqlStatementSource");

            foreach (var name in names)
            {
                if (!string.IsNullOrEmpty(node.GetAttribute(name, node.NamespaceURI)))
                {
                    str = $@"{node.Name}[@{name}={node.GetAttribute(name, node.NamespaceURI)}]";
                    break;
                }
            }
             
            if (string.IsNullOrEmpty(str))
            {
                str = $@"{node.Name}";
            }
             
            if (n.MoveToParent() == false)
            {

                return $@"ROOT";
            }
            else
            {
                return n.GetPath() + "/" + str;
            }
        }
        public static string GetPath(this XPathNavigator node, string label)
        {
            var n = node.Clone();
            string str = null;

            if (!string.IsNullOrEmpty(node.GetAttribute(label, node.NamespaceURI)))
            {
                str = $@"{node.Name}[@{label}={node.GetAttribute(label, node.NamespaceURI)}]";
            }
             
            if (string.IsNullOrEmpty(str))
            {
                str = $@"{node.Name}";
            }
            
            if (n.MoveToParent() == false)
            {

                return $@"ROOT";
            }
            else
            {
                return n.GetPath() + "/" + str;
            }
        }

    }
}
