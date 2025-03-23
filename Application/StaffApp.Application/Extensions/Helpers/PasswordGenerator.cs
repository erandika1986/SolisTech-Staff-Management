using System.Security.Cryptography;

namespace StaffApp.Application.Extensions.Helpers
{
    public class PasswordGenerator
    {
        private static readonly char[] LowercaseChars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] NumberChars = "0123456789".ToCharArray();
        private static readonly char[] SpecialChars = "!@#$%^&*()-_=+[]{};:,.<>/?".ToCharArray();

        private readonly bool _useLowercase;
        private readonly bool _useUppercase;
        private readonly bool _useNumbers;
        private readonly bool _useSpecial;
        private readonly int _minLength;
        private readonly int _maxLength;
        private readonly bool _requireAll;

        /// <summary>
        /// Initializes a new instance of the PasswordGenerator class with default settings
        /// </summary>
        public PasswordGenerator() : this(true, true, true, true, 12, 16, true) { }

        /// <summary>
        /// Initializes a new instance of the PasswordGenerator class with custom settings
        /// </summary>
        /// <param name="useLowercase">Include lowercase letters</param>
        /// <param name="useUppercase">Include uppercase letters</param>
        /// <param name="useNumbers">Include numbers</param>
        /// <param name="useSpecial">Include special characters</param>
        /// <param name="minLength">Minimum password length</param>
        /// <param name="maxLength">Maximum password length</param>
        /// <param name="requireAll">Require at least one character from each selected character set</param>
        public PasswordGenerator(
            bool useLowercase = true,
            bool useUppercase = true,
            bool useNumbers = true,
            bool useSpecial = true,
            int minLength = 12,
            int maxLength = 16,
            bool requireAll = true)
        {
            if (!useLowercase && !useUppercase && !useNumbers && !useSpecial)
                throw new ArgumentException("At least one character set must be selected");

            if (minLength < 1)
                throw new ArgumentException("Minimum length must be at least 1", nameof(minLength));

            if (maxLength < minLength)
                throw new ArgumentException("Maximum length must be greater than or equal to minimum length", nameof(maxLength));

            _useLowercase = useLowercase;
            _useUppercase = useUppercase;
            _useNumbers = useNumbers;
            _useSpecial = useSpecial;
            _minLength = minLength;
            _maxLength = maxLength;
            _requireAll = requireAll;
        }

        /// <summary>
        /// Generates a random password based on the configured settings
        /// </summary>
        /// <returns>A randomly generated password</returns>
        public string GeneratePassword()
        {
            var availableChars = new List<char>();
            var requiredCharSets = new List<char[]>();

            if (_useLowercase)
            {
                availableChars.AddRange(LowercaseChars);
                requiredCharSets.Add(LowercaseChars);
            }

            if (_useUppercase)
            {
                availableChars.AddRange(UppercaseChars);
                requiredCharSets.Add(UppercaseChars);
            }

            if (_useNumbers)
            {
                availableChars.AddRange(NumberChars);
                requiredCharSets.Add(NumberChars);
            }

            if (_useSpecial)
            {
                availableChars.AddRange(SpecialChars);
                requiredCharSets.Add(SpecialChars);
            }

            // Determine actual password length
            int length;
            using (var rng = RandomNumberGenerator.Create())
            {
                var buffer = new byte[4];
                rng.GetBytes(buffer);
                length = _minLength + (BitConverter.ToInt32(buffer, 0) & 0x7FFFFFFF) % (_maxLength - _minLength + 1);
            }

            // Generate the password
            var password = new char[length];
            var requiredCharCount = _requireAll ? requiredCharSets.Count : 0;

            if (requiredCharCount > length)
                throw new InvalidOperationException("Password length is too short to include required characters from all sets");

            using (var rng = RandomNumberGenerator.Create())
            {
                // First add required characters from each set
                if (_requireAll)
                {
                    var positions = GetRandomPositions(length, requiredCharCount, rng);
                    for (int i = 0; i < requiredCharCount; i++)
                    {
                        password[positions[i]] = GetRandomCharFromSet(requiredCharSets[i], rng);
                    }
                }

                // Fill the rest with random characters
                for (int i = 0; i < length; i++)
                {
                    if (password[i] == '\0') // If this position is not filled yet
                    {
                        password[i] = GetRandomCharFromSet(availableChars.ToArray(), rng);
                    }
                }
            }

            return new string(password);
        }

        /// <summary>
        /// Generates multiple random passwords
        /// </summary>
        /// <param name="count">Number of passwords to generate</param>
        /// <returns>An array of randomly generated passwords</returns>
        public string[] GeneratePasswords(int count)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than zero", nameof(count));

            var passwords = new string[count];
            for (int i = 0; i < count; i++)
            {
                passwords[i] = GeneratePassword();
            }
            return passwords;
        }

        private static char GetRandomCharFromSet(char[] charSet, RandomNumberGenerator rng)
        {
            var buffer = new byte[4];
            rng.GetBytes(buffer);
            var index = BitConverter.ToInt32(buffer, 0) & 0x7FFFFFFF;
            return charSet[index % charSet.Length];
        }

        private static int[] GetRandomPositions(int length, int count, RandomNumberGenerator rng)
        {
            var positions = new List<int>();
            for (int i = 0; i < length; i++)
            {
                positions.Add(i);
            }

            var result = new int[count];
            for (int i = 0; i < count; i++)
            {
                var buffer = new byte[4];
                rng.GetBytes(buffer);
                var index = BitConverter.ToInt32(buffer, 0) & 0x7FFFFFFF;
                index %= positions.Count;

                result[i] = positions[index];
                positions.RemoveAt(index);
            }

            return result;
        }
    }
}
