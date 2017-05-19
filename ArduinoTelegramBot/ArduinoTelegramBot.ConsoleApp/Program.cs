using System;
using ArduinoTelegramBot.Core;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ArduinoTelegramBot.ConsoleApp
{
    class Program
    {
        //Arduino COM-port
        private static string _comPort = "COM9";
        //Telegram bot token
        private static string _telegramToken = "345214822:AAEuu4VJEpQe0tN5Hj6x0Fh5MoH-5VGGpKo";
        private static int _telegramMessageOffset = -1;
        //Request to Telegram bot delay in milliseconds
        private static int _telegramRequestDelay = 3000;
        //Loop exit flag
        private static bool _exit = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello to Arduino-Telegramm bot!" + Environment.NewLine);

            Arduino Arduino = new Arduino(_comPort);
            Arduino.DataReceived += Arduino_DataReceived;

            Arduino.Initialize();
            WaitForArduino();

            Telegram Telegram = new Telegram(_telegramToken);

            string Command = String.Empty;

            while (Command.ToLowerInvariant() != "quit")
            {
                Console.WriteLine("Waiting for Telegram bot command...");
                Task<string> TelegramMessagesRequest = Telegram.GetData("getUpdates", "offset=" + _telegramMessageOffset);
                TelegramMessagesRequest.Wait();
                JObject TelegramMessages = JObject.Parse(TelegramMessagesRequest.Result);

                if (TelegramMessages.HasValues && TelegramMessages["ok"].ToString().ToLowerInvariant() == "true" && TelegramMessages["result"].HasValues)
                {
                    JToken Message = TelegramMessages["result"][0];
                    Command = Message["message"]["text"].ToString();
                    Console.WriteLine("Bot: " + Command);

                    if (_telegramMessageOffset < 0 && Command.ToLowerInvariant().Equals("quit"))
                    {
                        Command = String.Empty;
                        Console.WriteLine("First message, quit declined.");
                    }

                    int.TryParse(Message["update_id"].ToString(), out _telegramMessageOffset);
                    _telegramMessageOffset++;

                    if (!String.IsNullOrWhiteSpace(Command) && Command != "quit")
                    {
                        Arduino.SendCommand(Command);
                        WaitForArduino();
                    }
                }
                else
                {
                    Console.WriteLine("No new commands.");
                }

                Console.WriteLine("Sleeping for: " + _telegramRequestDelay/1000 + " seconds." + Environment.NewLine);
                if (Command != "quit")
                {
                    Thread.Sleep(_telegramRequestDelay);
                }
            }
        }

        private static void WaitForArduino()
        {
            Console.WriteLine("Waiting for Arduino responce:");
            _exit = false;
            while (!_exit)
            {
                Thread.Sleep(1);
            }
        }

        private static void Arduino_DataReceived(string obj)
        {
            Console.WriteLine("Arduino: " + obj.Trim());
            _exit = true;
        }
    }
}