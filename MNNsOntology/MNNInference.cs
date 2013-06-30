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
            Console.Out.WriteLine("** Inference Input **");
            foreach (string name in XElement.Load(input).Elements("object").Attributes("name"))
                Console.Out.WriteLine(name);

            foreach (XElement mnnElement in _mnn.Elements("object"))
            {
                var objectNotInputData = (from element in XElement.Load(input).Elements("object")
                                          where element.Attribute("name").Value == mnnElement.Attribute("name").Value
                                          select element);

                if (!objectNotInputData.Any())
                {
                    foreach (string name in XElement.Load(input).Elements("object").Attributes("name"))
                    {
                        var v = (from element in mnnElement.Elements("endLemmas")
                                 where element.Attribute("name").Value == name
                                 select element);
                        if (v.Any())
                        {
                            _score[mnnElement.Attribute("name").Value] += Convert.ToDouble(v.First().Element("score").Value);
                        }
                    }
                }
            }

            return Max().Key;
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
    }
}
