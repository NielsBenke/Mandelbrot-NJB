

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
using System.Threading;

namespace Mandelbrot_Server {

    

    struct json {
        public IEnumerable<int> response;
    }

    class Program {

        private static Boolean running = true;

        string process(string input) {
            // read JSON directly from a file
            JObject j;
            var inputBuffer = input.Select(c => Convert.ToByte(c)).ToArray();
            // using (StreamReader file = File.OpenText(@"C:\Users\BBW-BENUTZER\Documents\Visual Studio 2015\Projects\Mandelbrot Server\Mandelbrot Server\mandelbrot.json"))
            using (JsonTextReader reader = new JsonTextReader(new StreamReader(new MemoryStream(inputBuffer)))) { 
                j = (JObject)JToken.ReadFrom(reader);
                System.Console.WriteLine(j.ToString());
            }

            double real_from = (double)j.GetValue("realFrom");
            double real_to = (double)j.GetValue("realTo");
            double imaginary_from = (double)j.GetValue("imaginaryFrom");
            double imaginary_to = (double)j.GetValue("imaginaryTo");
            double intervall = (double)j.GetValue("interval");
            int maxiteration = (int)j.GetValue("maxIteration");

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

        private static void awaitTermination()
        {
            Console.WriteLine("Please press any key to exit the server after completing the next request");
            System.Console.ReadKey();
            Console.WriteLine("Exiting server on next run");
            running = false;
        }

        static void Main(string[] args) {
            Program brotcomputer = new Program();
            //Console.WriteLine(test.process());

            Console.WriteLine("Awaiting client");
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://+:8080/");
            listener.Start();

            Thread thread = new Thread(new ThreadStart(Program.awaitTermination));
            thread.Name = "Terminator";
            thread.IsBackground = true;
            thread.Start();

            while (running)
            {
                var ctx = listener.GetContext();
                var inputStream = ctx.Request.InputStream;
                string input;
                using (var reader = new StreamReader(inputStream))
                {
                    input = reader.ReadToEnd();
                }

                string result = brotcomputer.process(input);
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "text/json";
                ctx.Response.ContentEncoding = Encoding.UTF8;
                ctx.Response.OutputStream.Write(Encoding.UTF8.GetBytes(result), 0, result.Length);
                ctx.Response.OutputStream.Flush();
                try
                {
                    ctx.Response.Close();
                }
                catch (Exception e)
                {
                    ConsoleColor p = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("An exception occured while flushing data to the client:");
                    Console.WriteLine(e.ToString());
                    Console.ForegroundColor = p;
                }
            }
            
           
        }

    }
}
