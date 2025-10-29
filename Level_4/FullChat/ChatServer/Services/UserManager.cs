using System.Collections.Concurrent;
using ChatServer.Models;

namespace ChatServer.Services
{
    public class UserManager
    {
        private readonly ConcurrentDictionary<string, User> _users = new();
        private readonly ConcurrentDictionary<string, string> _usernameToId = new();

        public event Action<User>? UserConnected;
        public event Action<User>? UserDisconnected;

        public bool AddUser(User user, string username)
        {
            // Check if username is already taken
            if (_usernameToId.ContainsKey(username))
                return false;

            user.Username = username;
            
            if (_users.TryAdd(user.Id, user) && _usernameToId.TryAdd(username, user.Id))
            {
                UserConnected?.Invoke(user);
                return true;
            }

            return false;
        }

        public void RemoveUser(string userId)
        {
            if (_users.TryRemove(userId, out var user))
            {
                _usernameToId.TryRemove(user.Username, out _);
                UserDisconnected?.Invoke(user);
                user.Disconnect();
            }
        }

        public User? GetUser(string userId)
        {
            return _users.TryGetValue(userId, out var user) ? user : null;
        }

        public User? GetUserByUsername(string username)
        {
            return _usernameToId.TryGetValue(username, out var userId) ? GetUser(userId) : null;
        }

        public List<User> GetOnlineUsers()
        {
            return _users.Values.Where(u => u.IsOnline).ToList();
        }

        public List<string> GetOnlineUsernames()
        {
            return _users.Values.Where(u => u.IsOnline).Select(u => u.Username).ToList();
        }

        public bool IsUsernameTaken(string username)
        {
            return _usernameToId.ContainsKey(username);
        }

        public int GetOnlineCount()
        {
            return _users.Values.Count(u => u.IsOnline);
        }
    }
}
