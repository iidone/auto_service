using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace Auto_Service.Services;

public class TelegramService
{
    private readonly TelegramBotClient _bot;
    private readonly string _chatId;

    public TelegramService(string botToken, string chatId)
    {
        _bot = new TelegramBotClient(botToken);
        _chatId = chatId;
    }

    public async Task<bool> SendPdfReceipt(byte[] pdfBytes, string fileName)
    {
        try
        {
            using var stream = new MemoryStream(pdfBytes);
            stream.Position = 0;

            await _bot.SendDocumentAsync(
                chatId: _chatId,
                document: new InputOnlineFile(stream, fileName),
                caption: "Ваш чек на обслуживание");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return false;
        }
    }
}