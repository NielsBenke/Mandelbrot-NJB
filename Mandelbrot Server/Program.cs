

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using System.Net;

namespace Mandelbrot_Server {
    struct json {
        public IEnumerable<int> response;
    }

    class Program {
        string process(string input) {
            // read JSON directly from a file
            JObject j;
            var inputBuffer = input.Select(c => Convert.ToByte(c)).ToArray();
            // using (StreamReader file = File.OpenText(@"C:\Users\BBW-BENUTZER\Documents\Visual Studio 2015\Projects\Mandelbrot Server\Mandelbrot Server\mandelbrot.json"))
            using (JsonTextReader reader = new JsonTextReader(new StreamReader(new MemoryStream(inputBuffer)))) { 
                j = (JObject)JToken.ReadFrom(reader);
                System.Console.WriteLine(j.ToString());
            }

            double real_from = (double)j.GetValue("RealFrom");
            double real_to = (double)j.GetValue("RealTo");
            double imaginary_from = (double)j.GetValue("ImaginaryFrom");
            double imaginary_to = (double)j.GetValue("ImaginaryTo");
            double intervall = (double)j.GetValue("Intervall");
            int maxiteration = (int)j.GetValue("MaxIteration");

            List<int> iterations_list = new List<int>();

            var epsilon = intervall / 2;
            for (double i = real_from; i - epsilon < real_to; i += intervall) {
                for (double k = imaginary_to; k + epsilon > imaginary_from; k -= intervall) {
                    var iterations = Mandelbrot.calc_iterations(new System.Numerics.Complex(i, k), maxiteration);
                    iterations_list.Add(iterations);
                }
            }
            System.Console.WriteLine("/n");
            json output_json = new json { response = iterations_list };

            string json_string = JsonConvert.SerializeObject(output_json);

            return json_string;
        }
        static void Main(string[] args) {
            Program test = new Program();
            //Console.WriteLine(test.process());
            System.Console.ReadKey();

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://0.0.0.0:8080/");
            listener.Start();
            var ctx = listener.GetContext();
            var inputStream = ctx.Request.InputStream;
            string input;
            using (var reader = new StreamReader(inputStream)) {
                input = reader.ReadToEnd();
            }

            test.process(input);
        }

    }
}
