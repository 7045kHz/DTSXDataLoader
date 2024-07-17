using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace DTSXDataLoader.Models
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
