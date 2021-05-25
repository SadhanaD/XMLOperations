using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace Employee
{
    class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            var xmldoc = new XmlDataDocument();
            XmlNodeList xmlnode;

            #region Get exect folder path
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            #endregion

            var path = appRoot + @"\data\organization.xml";
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            xmldoc.Load(fs);
            xmlnode = xmldoc.GetElementsByTagName("Employee");

            for (int i = 0; i <= xmlnode.Count - 1; i++)
            {
                string Name = xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                string Title = xmlnode[i].Attributes["Title"].Value;
                string Unit = "-";
                if (xmlnode[i].ParentNode.Name == "Unit")
                    Unit = xmlnode[i].ParentNode.Attributes["Name"].Value;

                Console.WriteLine($"Name: {Name} , Title: {Title} , Unit: {Unit}");
            }

            XmlElement root = xmldoc.DocumentElement;
            XmlNode a = root.SelectSingleNode("//Unit[@Name='Platform Team']");
            XmlNode b = root.SelectSingleNode("//Unit[@Name='Maintenance Team']");
            SwapNodesChild(a, b);

            string jsonData = JsonConvert.SerializeXmlNode(xmldoc);
            System.IO.File.WriteAllText(appRoot + @".\data\" + "Output.json", jsonData);
            Console.WriteLine("--------------------");
            Console.WriteLine("Json File Created at " + appRoot + @"\data\Output.json");
            Console.ReadLine();
        }

        static void SwapNodesChild(XmlNode a, XmlNode b)
        {
            // swap elements
            ArrayList tempNodeOfa = new ArrayList();
            while (a.HasChildNodes)
            {
                tempNodeOfa.Add(a.FirstChild); // Add in temp from a
                a.RemoveChild(a.FirstChild);  // remove from a
            }
            while (b.HasChildNodes)
            {
                a.AppendChild(b.FirstChild);  // swap from b to a
            }
            for (int i = 0; i < tempNodeOfa.Count; i++)
            {
                XmlNode childNodeListOfplatformTeam = (XmlNode)tempNodeOfa[i];
                b.AppendChild(childNodeListOfplatformTeam); // swap from temp to b
            }
        }
    }
}
