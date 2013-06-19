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
                XmlNodeList objectList = _docXml.GetElementsByTagName("name");
                foreach (XmlNode n in objectList)
                {
                    List<XElement> uniqueQuery = (from el in _outputXml.Elements("object")
                                                  where (string)el == n.InnerText.Replace("\n", "").Replace("\b", "")
                                                  select el).ToList();
                    if (uniqueQuery.Count == 0)
                    {
                        _outputXml.Add(new XElement("object", new XElement("name", n.InnerText.Replace("\n", "").Replace("\b", ""))));
                        Console.WriteLine("Adding Element " + n.InnerText.Replace("\n", "").Replace("\b", ""));
                    }
                }
            }

            _outputXml.Save("dataset/objects.xml");

        }
    }
}
