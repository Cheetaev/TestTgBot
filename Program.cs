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
                    cts = new CancellationTokenSource();

                    CheckFileCommand(message.Chat.Id, cts.Token);
                }

                if (message.Text.ToLower().Contains("stop monitoring"))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "File monitoring stopped");
                    cts?.Cancel();
                }

                Console.WriteLine(message.Chat.Id);
            }

            return;

        }

        private async void CheckFileCommand(long chatId, CancellationToken token)
        {
            var delta = 15;
            bool fileChecked = false;

            var startDateOne = DateTime.ParseExact(new DateTime().AddHours(11).AddMinutes(30).ToString("HH.mm.ss"), "HH.mm.ss", System.Globalization.CultureInfo.InvariantCulture);
            var endDateOne = DateTime.ParseExact(new DateTime().AddHours(14).ToString("HH.mm.ss"), "HH.mm.ss", System.Globalization.CultureInfo.InvariantCulture);

            var startDateTwo = DateTime.ParseExact(new DateTime().AddHours(18).ToString("HH.mm.ss"), "HH.mm.ss", System.Globalization.CultureInfo.InvariantCulture);
            var endDateTwo = DateTime.ParseExact(new DateTime().AddHours(19).AddMinutes(30).ToString("HH.mm.ss"), "HH.mm.ss", System.Globalization.CultureInfo.InvariantCulture);


            while (!token.IsCancellationRequested)
            {
                var now = DateTime.ParseExact(DateTime.Now.ToString("HH.mm.ss"), "HH.mm.ss", System.Globalization.CultureInfo.InvariantCulture);
                
                if (!((now >= startDateOne && now < endDateOne) || (now >= startDateTwo && now < endDateTwo)))
                {
                    await Task.Delay(5000);
                    continue;
                }

                var files = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "../QrCodes"));

                var lastFilename = files.GetFiles("*.txt").Last().Name;
                var lastFileDateString = new string(files.GetFiles("*.txt").Last().Name.Substring(0, lastFilename.Length - 4).TakeLast(19).ToArray());
                
                var dateFromLastFile = DateTime.ParseExact(lastFileDateString, "dd.MM.yyyy.HH.mm.ss", System.Globalization.CultureInfo.InvariantCulture);

                var totalMinutes = DateTime.Now.Subtract(dateFromLastFile).TotalMinutes;

                if (totalMinutes >= delta && !fileChecked)
                {
                    await _client.SendTextMessageAsync(chatId, "Last QrCode was created more than 15 min ago.");
                    fileChecked = true;
                    continue;
                }
                else if (totalMinutes < delta && fileChecked)
                {
                    await _client.SendTextMessageAsync(chatId, "Found new QrCode after last message.");
                    fileChecked = false;
                }
                await Task.Delay(5000);
            }
            cts.Dispose();
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }
    }
}
