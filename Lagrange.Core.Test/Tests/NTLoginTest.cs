using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;

namespace Lagrange.Core.Test.Tests;

// ReSharper disable once InconsistentNaming

public class NTLoginTest
{
    public async Task LoginByPassword()
    {
        var deviceInfo = WtLoginTest.GetDeviceInfo();
        var keyStore = WtLoginTest.LoadKeystore();
        
        if (keyStore == null)
        {
            Console.WriteLine("Please login by QrCode first");
            return;
        }

        var bot = BotFactory.Create(new BotConfig() 
        {
            UseIPv6Network = false,
            GetOptimumServer = true,
            AutoReconnect = true,
            Protocol = Protocols.Linux
        }, deviceInfo, keyStore);
        
        bot.Invoker.OnBotLogEvent += (context, @event) =>
        {
            Utility.Console.ChangeColorByTitle(@event.Level);
            Console.WriteLine(@event.ToString());
        };
        
        bot.Invoker.OnBotOnlineEvent += (context, @event) =>
        {
            Console.WriteLine(@event.ToString());
            WtLoginTest.SaveKeystore(bot.UpdateKeystore());
        };

        await bot.LoginByPassword();

        var cookies = await bot.FetchCookies(new List<string> { "qun.qq.com" });
        Console.WriteLine(cookies[0]);
        Console.WriteLine(bot.GetCsrfToken());

        var friends = await bot.FetchFriends();
        
        await bot.GetHighwayAddress();

        var friendChain = MessageBuilder.Friend(1925648680);
        friendChain.Text("This is the friend message sent by Lagrange.Core");
        await bot.SendMessage(friendChain.Build());
    }
}