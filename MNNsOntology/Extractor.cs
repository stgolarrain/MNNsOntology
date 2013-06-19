using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace MNNsOntology
{
    class Extractor
    {
        HttpClient client;

        public Extractor()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:9000/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public double promScore(string startLemmas, string endLemmas)
        {
            if (startLemmas == "" || endLemmas == "")
                return 0;
            double prom = 0;
            Boolean fin = false;
            int cont = 0;
            while (!fin)
            {
                HttpResponseMessage response = client.GetAsync("http://conceptnet5.media.mit.edu/data/5.1/search?startLemmas=" + startLemmas + "&endLemmas=" + endLemmas + "&offset=" + cont + "&limit=" + (cont + 50)).Result;  // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body. Blocking!
                    var arco = response.Content.ReadAsAsync<Query>().Result;

                    if (cont == 0 && arco.edges.Count == 0)
                        return 0;

                    if (arco.edges.Count != 0)
                    {
                        foreach (Edge e in arco.edges)
                        {
                            prom = prom + e.score;
                            cont++;
                        }
                    }
                    else
                        fin = true;
                }
            }
            prom = prom / cont;

            return prom;
        }

        public void generateMNN(XElement input)
        {
            foreach (XElement startLemmas in input.Elements())
            {
                foreach (XElement endLemmas in input.Elements())
                {
                    double score = promScore(startLemmas.Element("name").Value, endLemmas.Element("name").Value);
                    startLemmas.Add(endLemmas.Element("name"), score);
                    Console.Out.WriteLine(String.Format("startLemmas: {0}; endLemmas: {1}; score: {2}", startLemmas.Element("name").Value, endLemmas.Element("name").Value, score));
                }
            }
            input.Save("dataset/mnn.xml"); ;
        }
    }
}
