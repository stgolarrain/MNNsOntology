using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MNNsOntology
{
    class Test
    {
        public Test()
        {
            Directory.CreateDirectory("./datatest");
        }

        public void GenerateTestData()
        {
            string[] filePaths = Directory.GetFiles("./LabelMe/05june05_static_street_boston", "*.xml");
            int i = 1;
            foreach (string path in filePaths)
            {
                XElement xelement = XElement.Load(path);
                RemoveElement(xelement).Save(String.Format("datatest/{0}.xml", i));
                Console.WriteLine("Test file generated " + i);
                i++;
            }
        }

        private XElement RemoveElement(XElement input)
        {
            XElement output = new XElement("objects", new XAttribute("filename", input.Element("filename").Value));
            Random rand = new Random();
            int outElement = rand.Next(input.Elements("object").Count());

            XElement element = input.Element("object");
            for (int i = 0; i < input.Elements("object").Count(); i++)
            {
                if (element.Element("name") != null)
                {

                    if (i != outElement)
                    {
                        output.Add(new XElement("object", new XAttribute("name", element.Element("name").Value.ToLower().Replace("\n", "").Replace("\b", ""))));
                    }
                    else
                    {
                        output.Add(new XElement("result", new XAttribute("name", element.Element("name").Value.ToLower().Replace("\n", "").Replace("\b", ""))));
                    }
                }
                element = (XElement)element.NextNode;
            }

            return output;
        }
    }
}
