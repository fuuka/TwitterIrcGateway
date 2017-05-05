using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Misuzilla.Applications.TwitterIrcGateway.AddIns
{
    class ResolveShortUrlServices : AddInBase
    {
        public override void Initialize()
        {
            CurrentSession.PreFilterProcessTimelineStatus += new EventHandler<TimelineStatusEventArgs>(Session_PreFilterProcessTimelineStatus);
            //Session.PreSendUpdateStatus += new EventHandler<StatusUpdateEventArgs>(Session_PreSendUpdateStatus);
        }

        //void Session_PreSendUpdateStatus(object sender, StatusUpdateEventArgs e)
        //{
        //    e.Text = Utility.UrlToTinyUrlInMessage(e.Text);
        //}
        
        void Session_PreFilterProcessTimelineStatus(object sender, TimelineStatusEventArgs e)
        {
            // t.co (Twitter Url Shortener)
            List<Entities> entities_list = new List<Entities>();
            if (e.Status.Extended != null && e.Status.Extended.Entities != null)
            {
                entities_list.Add(e.Status.Extended.Entities);
            }
            if (e.Status.Entities != null)
            {
                entities_list.Add(e.Status.Entities);
            }
            foreach (var entities in entities_list)
            {
                if (entities.Urls != null && entities.Urls.Length > 0)
                {
                    foreach (var urlEntity in entities.Urls)
                    {
                        if (!String.IsNullOrEmpty(urlEntity.ExpandedUrl))
                        {
                            e.Text = Regex.Replace(e.Text, Regex.Escape(urlEntity.Url), urlEntity.ExpandedUrl);
                        }
                    }
                }
                if (entities.Media != null && entities.Media.Length > 0)
                {
                    foreach (var urlEntity in entities.Media)
                    {
                        if (!String.IsNullOrEmpty(urlEntity.ExpandedUrl))
                        {
                            String expandedUrl = Regex.Replace(urlEntity.ExpandedUrl, "^http:", "https:");
                            e.Text = Regex.Replace(e.Text, Regex.Escape(urlEntity.Url), expandedUrl);
                        }
                    }
                }
            }

            // TinyURL
            e.Text = (CurrentSession.Config.ResolveTinyUrl) ? Utility.ResolveShortUrlInMessage(Utility.ResolveTinyUrlInMessage(e.Text))
                                                            : e.Text;
        }
    }
}
