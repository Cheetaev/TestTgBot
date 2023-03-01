using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bot = new BotFlow();
            bot.Start();
            

            Console.ReadLine();
        }

    }

    public class BotFlow
    {
        TelegramBotClient _client = new TelegramBotClient("6116642980:AAEOMRK6Atq_Z2hwasK7Vw-hU0u1MODba-g");

        CancellationTokenSource cts;

        public void Start()
        {
            _client.StartReceiving(Update, Error);
        }

        private async Task Update(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            var message = update.Message;

            if (message?.Text != null)
            {
                if (message.Text.ToLower().Contains("start monitoring"))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "File monitoring running");

                }

                if (message.Text.ToLower().Contains("stop monitoring"))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "File monitoring stopped");
                }

                Console.WriteLine(message.Chat.Id);
            }

            return;

        }


        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
