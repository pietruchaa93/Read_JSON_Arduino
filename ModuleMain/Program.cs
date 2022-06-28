using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Collections;
using System.Text.Json;
//using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace Read_Json_Arduino
{
    public class Program
    {
        public static ArrayList dataTemperature = new ArrayList();
        public static ArrayList dataButton1 = new ArrayList();
        public static ArrayList dataButton2 = new ArrayList();
        public static ArrayList dataAnalog = new ArrayList();

        public static void Main(string[] args)
        {

            SerialPort port = new SerialPort();
            port.PortName = "COM3";
            port.BaudRate = 9600;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            port.RtsEnable = true;

            port.DataReceived += new SerialDataReceivedEventHandler(DataReceiver);

            port.Open();
            Console.ReadKey();
            port.Close();

        }

        public static void DataReceiver(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort myport = (SerialPort)sender;
                String data = myport.ReadExisting();

                if (data.Contains(" "))
                {
                    int x;
                    string correct;
                    x = data.IndexOf(" ");
                    correct = data.Remove(x);

                    Adder(correct);
                }
                else
                {
                    Adder(data);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("No data!!!");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            }

            Display();

        }
        public class GlobalBase
        {
            public double Temperature { get; set; }
            public int Button1 { get; set; }
            public int Button2 { get; set; }
            public double Analog { get; set; }
        }

        public static void Display()
        {
            Console.WriteLine("Current value: ");
            Console.WriteLine(Program.dataTemperature[Program.dataTemperature.Count - 1]);
            Console.WriteLine(Program.dataButton1[Program.dataButton1.Count - 1]);
            Console.WriteLine(Program.dataButton2[Program.dataButton2.Count - 1]);
            Console.WriteLine(Program.dataAnalog[Program.dataAnalog.Count - 1]);

            Console.WriteLine();
        }

        public static void Adder(string x)
        {
            GlobalBase? globalBase = JsonSerializer.Deserialize<GlobalBase>(x);
            Program.dataTemperature.Add($"Temperature: {globalBase?.Temperature}");
            Program.dataButton1.Add($"Button1: {globalBase?.Button1}");
            Program.dataButton2.Add($"Button1: {globalBase?.Button2}");
            Program.dataAnalog.Add($"Button1: {globalBase?.Analog}");
        }

        
        public class Worker : BackgroundService
        {
            private readonly ILogger<Worker> _logger;

            public Worker(ILogger<Worker> logger) =>
                _logger = logger;

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.UtcNow);
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }

    }


