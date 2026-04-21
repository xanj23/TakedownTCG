namespace TakedownTCGApplication.Models.UserAccounts
{
    public sealed class User
    {
        private string _userName = string.Empty;
        private string _userEmail = string.Empty;
        private string _passwordHash = string.Empty;

        public string UserName
        {
            get => _userName;
            init => _userName = Require(value, nameof(UserName));
        }

        public string UserEmail
        {
            get => _userEmail;
            init => _userEmail = Require(value, nameof(UserEmail));
        }

        public string PasswordHash
        {
            get => _passwordHash;
            init => _passwordHash = Require(value, nameof(PasswordHash));
        }

        public bool UserNotifications { get; init; }

        private static string Require(string? value, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{propertyName} is required.", propertyName);
            }

            return value.Trim();
        }
    }
}
