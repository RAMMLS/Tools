using AutoCommentBot.Config;
using System.Collections.Generic;

namespace AutoCommentBot.Services
{
    public class CredentialsProvider
    {
        private readonly List<UserProfile> _userProfiles;
        private int _currentIndex = 0;

        public CredentialsProvider(BotConfig botConfig)
        {
            _userProfiles = botConfig.UserProfiles;
        }

        public UserProfile GetNextUserProfile()
        {
            if (_userProfiles == null || _userProfiles.Count == 0)
            {
                return null;
            }

            var profile = _userProfiles[_currentIndex];
            _currentIndex = (_currentIndex + 1) % _userProfiles.Count;
            return profile;
        }
    }
}

