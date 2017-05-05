using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misuzilla.Net.Irc;

namespace Misuzilla.Applications.TwitterIrcGateway.AddIns
{
    class ExtractQuotedTweet : AddInBase
    {
        public override void Initialize()
        {
            CurrentSession.PostProcessTimelineStatus += (sender, e) =>
            {
                if (e.Status.QuotedStatus != null)
                {
                    String body = (String)e.Status.QuotedStatus.Text.Clone();
                    CurrentSession.Send(new NoticeMessage(Config.Default.ChannelName, body.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " "))
                    {
                        SenderHost = "twitter@" + Server.ServerName,
                        SenderNick = e.Status.QuotedStatus.User.ScreenName
                    });
                }
            };
        }
    }
}
