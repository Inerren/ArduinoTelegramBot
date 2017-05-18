using System;
using ArduinoTelegramBot.Core;
using System.Threading;

namespace ArduinoTelegramBot.ConsoleApp
{
    class Program
    {
        private static string _comPort = "COM9";
        private static bool _exit = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello to Arduino-Telegramm bot!");

            string command = String.Empty;

            Arduino Arduino = new Arduino(_comPort);
            Arduino.DataReceived += Arduino_DataReceived;

            WaitForAnswer();

            while (command.ToLowerInvariant() != "quit")
            {
                Console.WriteLine("Waiting for command ['quit' for exit]:");
                command = Console.ReadLine();
                if (command != "quit")
                {
                    Arduino.SendCommand(command);
                    WaitForAnswer();
                }
            }
        }

        private static void WaitForAnswer()
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
            Console.WriteLine(obj);
            _exit = true;
        }
    }
}