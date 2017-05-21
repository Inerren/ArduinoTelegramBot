using System;
using ArduinoTelegramBot.Core;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ArduinoTelegramBot.ConsoleApp
{
    class Program
    {
        private static Arduino Arduino;
        private static Telegram Telegram;

        //Arduino COM-port
        private static string _comPort = "COM9";
        //Telegram bot token
        private static string _telegramToken = "345214822:AAEuu4VJEpQe0tN5Hj6x0Fh5MoH-5VGGpKo";
        //Telegram last message offset (+1)
        private static int _telegramMessageOffset = -1;
        //Telegram active chat ID
        private static int _telegramChatId = 0;
        //Telegram command execution try counts
        private static int _telegramTryCount = 0;
        //Request to Telegram bot delay in milliseconds
        private static int _telegramRequestDelay = 3000;
        //Loop exit flag
        private static bool _exit = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello to Arduino-Telegramm bot!" + Environment.NewLine);

            Arduino = new Arduino(_comPort);
            Arduino.DataReceived += Arduino_DataReceived;

            Telegram = new Telegram(_telegramToken);

            Arduino.Initialize();
            WaitForArduino();

            string Command = String.Empty;

            while (Command.ToLowerInvariant() != "quit")
            {
                if (_telegramChatId != 0)
                {
                    if (_telegramTryCount > 10)
                    {
                        Console.WriteLine("Command timeout. Continue.");
                        SendTelegramMessage("Command timeout.");
                        _telegramTryCount = 0;
                        _telegramChatId = 0;
                    }
                    else
                    {
                        Console.WriteLine("Working on command...");
                        _telegramTryCount++;
                        Thread.Sleep(500);
                        continue;
                    }
                }

                Console.WriteLine("Waiting for Telegram bot command...");
                Task<string> TelegramMessagesRequest = Telegram.GetData("getUpdates", "offset=" + _telegramMessageOffset);
                TelegramMessagesRequest.Wait();
                JObject TelegramMessages = JObject.Parse(TelegramMessagesRequest.Result);

                if (TelegramMessages.HasValues && TelegramMessages["ok"].ToString().ToLowerInvariant() == "true" && TelegramMessages["result"].HasValues)
                {
                    JToken Message = TelegramMessages["result"][0];
                    _telegramChatId = int.Parse(Message["message"]["chat"]["id"].ToString());
                    Command = Message["message"]["text"].ToString();
                    Console.WriteLine("Bot: " + Command);

                    if (_telegramMessageOffset < 0)
                    {
                        Command = String.Empty;
                        _telegramChatId = 0;
                        Console.WriteLine("Outdated command, declined.");
                    }
                    else if (!String.IsNullOrWhiteSpace(Command) && Command != "quit")
                    {
                        Arduino.SendCommand(Command);
                        WaitForArduino();
                    }

                    if (int.TryParse(Message["update_id"].ToString(), out _telegramMessageOffset))
                    {
                        _telegramMessageOffset++;
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
                else
                {
                    SendTelegramMessage("Switching off. Bye.");
                    _telegramChatId = 0;
                }
            }
        }

        private static bool SendTelegramMessage(string message)
        {
            if (Telegram == null || _telegramChatId == 0) return false;

            Task<string> TelegramMessagesRequest = Telegram.GetData("sendMessage", "chat_id=" + _telegramChatId + "&text=" + message);
            TelegramMessagesRequest.Wait();
            JObject TelegramMessages = JObject.Parse(TelegramMessagesRequest.Result);

            return TelegramMessages.HasValues && TelegramMessages["ok"].ToString().ToLowerInvariant() == "true";
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
            SendTelegramMessage(obj.Trim());
            Regex regex = new Regex("^done", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (regex.IsMatch(obj))
            {
                _telegramChatId = 0;
            }
            _exit = true;
        }
    }
}