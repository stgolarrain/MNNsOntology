using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MNNsOntology
{
    class MNNInference
    {
        private XElement _mnn;
        private Dictionary<string, double> _score;

        public MNNInference(string mnn, string objects)
        {
            _score = new Dictionary<string, double>();
            _mnn = XElement.Load(mnn);
            foreach (XElement obj in XElement.Load(objects).Elements("object"))
            {
                _score.Add(obj.Attribute("name").Value, 0);
            }
        }

        public string Inference(string input)
        {
            Console.Out.WriteLine("\n** Inference Input **");
            Console.Out.WriteLine("File Name: " + XElement.Load(input).Attribute("filename").Value);
            foreach (XElement inputElement in XElement.Load(input).Elements("object"))
                Console.Out.WriteLine(inputElement.Attribute("name").Value);

            Console.WriteLine("");
            foreach (XElement inputElement in XElement.Load(input).Elements("object"))
            {
                var objectsOnInputElement = (from element in _mnn.Elements("object")
                                            where element.Attribute("name").Value == inputElement.Attribute("name").Value
                                            select element);

                foreach (XElement endLemmas in objectsOnInputElement.Elements("endLemmas"))
                {
                    _score[endLemmas.Attribute("name").Value] += Convert.ToDouble(endLemmas.Element("score").Value);
                }
            }

            //ExcludeOnInput(input);
            KeyValuePair<string, double>[] sorted = (from kv in _score orderby kv.Value select kv).ToArray();

            for(int i = sorted.Length - 1; i > sorted.Length - 11; i--)
                Console.WriteLine(sorted[i].Key + " : " + sorted[i].Value);

            Console.WriteLine("========================================\n");
            Console.WriteLine("Therory Result : " + XElement.Load(input).Element("result").Attribute("name").Value);
            Console.WriteLine("Score of Therory Result : " + _score[XElement.Load(input).Element("result").Attribute("name").Value]);

            return Max().Key;
        }

        private void ExcludeOnInput(string input)
        {
            foreach (XElement inputElement in XElement.Load(input).Elements("object"))
            {
                _score[inputElement.Attribute("name").Value] = 0;   
            }
        }

        private KeyValuePair<string, double> Max()
        {
            KeyValuePair<string, double> max = new KeyValuePair<string, double>("no results", 0);
            foreach (KeyValuePair<string, double> pair in _score)
            {
                if(pair.Value > max.Value)
                    max = new KeyValuePair<string, double>(pair.Key, pair.Value);
            }
            return max;
        }

        public void GenerateSageGraph(string input)
        {
            string result = "g = DiGraph({";
            foreach (XElement obj in XElement.Load(input).Elements("object"))
            {
                result += String.Format("'{0}' : {{", obj.Attribute("name").Value);
                foreach (XElement node in obj.Elements("endLemmas"))
                {
                    result += String.Format("'{0}' : {1}", node.Attribute("name").Value, node.Element("score").Value);
                    if (node.NextNode != null)
                    {
                        result += ",";
                    }

                }

                
                result += "},";
            }
            result += "})";
            System.IO.StreamWriter file = new System.IO.StreamWriter("dataset/sage_graph.txt");
            file.WriteLine(result);
            file.Close();
        }
    }
}
