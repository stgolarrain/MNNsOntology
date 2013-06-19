using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using System.Xml;

namespace MNNsOntology
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Out.WriteLine("Reading XML");

            XMLParser xmlParser = new XMLParser("LabelMe");
            //xmlParser.ReadFile();

            Extractor extractor = new Extractor();
            XElement xe = XElement.Load(@"dataset/objects.xml");
            extractor.generateMNN(xe);
            //extractor.generateMNN(xmlParser.getOutput());

            Console.Read();
        }
    }
}
