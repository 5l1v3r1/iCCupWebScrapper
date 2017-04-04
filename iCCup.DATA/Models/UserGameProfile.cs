﻿namespace iCCup.DATA.Models
{
    public class UserGameProfile
    {
        public string Url { get; set; }

        public string Nickname { get; set; }

        public string GamesListUrl { get; set; }

        public UserGameProfile(string url, string nickname)
        {
            this.Url = url;
            this.Nickname = nickname;
        }
    }
}
