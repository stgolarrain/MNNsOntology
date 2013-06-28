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
            DateTime initial = DateTime.Now;
            Console.Out.WriteLine("Reading XML");

            XMLParser xmlParser = new XMLParser("LabelMe");
            xmlParser.ReadFile();

            Extractor extractor = new Extractor();
            extractor.generateMNN(xmlParser.getOutput());

            DateTime final = DateTime.Now;
            Console.Out.WriteLine(String.Format("T. Inicial: {0}; T. Final: {1}", initial, final));
            Console.Read();
        }
    }
}
