using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace MNNsOntology
{
    class XMLParser
    {
        private string _path;
        private XElement _outputXml;

        public XMLParser(string path)
        {
            _path = path;
            Directory.CreateDirectory("./dataset");
            _outputXml = new XElement(new XElement("objects", ""));
        }

        public XElement getOutput() { return _outputXml; }

        public void ReadFile()
        {
            string[] filePaths = Directory.GetFiles(_path, "*.xml", SearchOption.AllDirectories);

            foreach (string file in filePaths)
            {
                XmlDocument _docXml = new XmlDocument();
                _docXml.Load(file);
                XmlNodeList objectList = _docXml.GetElementsByTagName("object");
                foreach (XmlNode n in objectList)
                {
                    if (n.ChildNodes[1].InnerText.Equals("0"))
                    {
                        string value = n.ChildNodes[0].InnerText.ToLower().Replace("\n", "").Replace("\b", "");
                        List<XElement> uniqueQuery = (from el in _outputXml.Elements("object")
                                                      where (string)el.Attribute("name") == value
                                                      select el).ToList();
                        if (uniqueQuery.Count == 0)
                        {
                            _outputXml.Add(new XElement("object", new XAttribute("name", n.ChildNodes[0].InnerText.ToLower().Replace("\n", "").Replace("\b", ""))));
                            Console.WriteLine("Adding Element " + value);
                        }
                    }
                }
            }

            _outputXml.Save("dataset/objects.xml");

        }
    }
}
