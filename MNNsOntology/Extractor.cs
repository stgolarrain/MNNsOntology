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
        static int requesThread = 1000;
        List<Task> tasks;

        public Extractor()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public double promScore(string startLemmas, string endLemmas)
        {
            if (startLemmas.Equals("") || endLemmas.Equals(""))
                return 0;

            double prom = 0;
            Boolean fin = false;
            int cont_arcos = 0;
            int cont_consulta = 0;

            while (!fin)
            {
            request:
                try
                {
                    Console.WriteLine(startLemmas + " - " + endLemmas);
                    HttpResponseMessage response = client.GetAsync("http://conceptnet5.media.mit.edu/data/5.1/search?startLemmas=" + startLemmas + "&endLemmas=" + endLemmas + "&offset=" + cont_arcos + "&limit=5000").Result;  // Blocking call!
                    cont_consulta++;
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response body. Blocking!
                        var arco = response.Content.ReadAsAsync<Query>().Result;

                        if (arco.edges.Count != 0)
                            foreach (Edge e in arco.edges)
                            {
                                prom = prom + e.score;
                                cont_arcos++;
                            }
                        else
                            fin = true;
                    }
                    if (cont_consulta * 5000 > cont_arcos)
                        fin = true;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    Task.WaitAny(tasks.ToArray());
                    goto request;
                }

                
            }

            if (cont_arcos == 0)
                return 0;
            else
                prom = prom / cont_arcos;

            /*Console.WriteLine("El total de arcos fue: " + cont_arcos);
            Console.WriteLine("El total de consultas fue: " + cont_consulta);*/
            return prom;
        }

        public void generateMNN(XElement input)
        {
            int count = input.Descendants("object").Count();
            foreach (XElement startLemmas in input.Elements())
            {
                DateTime initial = DateTime.Now;
                Dictionary<string, double> nodes = new Dictionary<string, double>();
                tasks = new List<Task>();

                foreach (XElement endLemmas in input.Elements())
                {
                    tasks.Add(Task.Run(() => nodes.Add(endLemmas.Attribute("name").Value, promScore(startLemmas.Attribute("name").Value, endLemmas.Attribute("name").Value))));
                    
                    for (int i = 0; i < tasks.Count; i++)
                    {
                        if (tasks[i].Status == TaskStatus.RanToCompletion)
                            tasks.RemoveAt(i);
                    }

                    if (tasks.Count >= requesThread)
                    {
                        Console.Out.WriteLine(String.Format("{0} : waiting for any thread to complete...", startLemmas.Attribute("name")));
                        Task.WaitAny(tasks.ToArray(),-1);
                    }

                    //nodes.Add(endLemmas.Attribute("name").Value, promScore(startLemmas.Attribute("name").Value, endLemmas.Attribute("name").Value));
                }

                Console.Out.WriteLine("Waiting threads ...");
                Task.WaitAll(tasks.ToArray(), -1);

                foreach(KeyValuePair<string, double> pair in nodes)
                {
                    if (pair.Value > 0)
                    {
                        XElement proof1 = new XElement("endLemmas", new XAttribute("name", pair.Key));
                        proof1.Add(new XElement("score", pair.Value));
                        startLemmas.Add(proof1);
                        Console.Out.WriteLine(String.Format("startLemmas: {0}; endLemmas: {1}; score: {2}", startLemmas.Attribute("name").Value, pair.Key, pair.Value));
                    }
                }
                Console.Out.WriteLine(String.Format("{0}: T. Inicial: {1}; T. Final: {2}", startLemmas.Attribute("name"), initial, DateTime.Now));
            }
            input.Save("dataset/mnn.xml");
        }
    }
}
