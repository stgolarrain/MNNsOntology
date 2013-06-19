using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNNsOntology
{
    class Edge
    {
        public string endLemmas { get; set; }
        public string rel { get; set; }
        public string end { get; set; }

        public string[] features { get; set; }
        public string license { get; set; }
        public string[] sources { get; set; }
        public string startLemmas { get; set; }
        public string[] text { get; set; }
        public string uri { get; set; }
        public string weight { get; set; }
        public string dataset { get; set; }
        public string start { get; set; }
        public double score { get; set; }
        public string context { get; set; }
        public string timestamp { get; set; }
        public string[] nodes { get; set; }
        public string id { get; set; }
    }
}
