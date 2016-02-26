namespace SecretLibrary
{
    using ElementAtLibrary;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Tells secrets.
    /// </summary>
    public sealed class SecretTeller : IDisposable
    {
        /// <summary>
        /// Optional salt append on input before hashing.
        /// </summary>
        private readonly byte[] _salt;

        /// <summary>
        /// Alphabet of consonants.
        /// </summary>
        private readonly char[] _consonants;

        /// <summary>
        /// Alphabet of vowels.
        /// </summary>
        private readonly char[] _vowels;

        /// <summary>
        /// Hash algorithm to use.
        /// </summary>
        private readonly HashAlgorithm _hashAlgorithm;

        /// <summary>
        /// Ctor.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "It's disposed on by the class itself.")]
        public SecretTeller()
            : this("bcdfghjklmnpqrstvwxz", "aeiou", SHA1.Create(), string.Empty) { }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="consonants">Alphabet of consonants</param>
        /// <param name="vowels">Alphabet of vowels</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "It's disposed on by the class itself.")]
        public SecretTeller(string consonants, string vowels) :
            this(consonants, vowels, SHA1.Create(), string.Empty) { }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="consonants">Alphabet of consonants</param>
        /// <param name="vowels">Alphabet of vowels</param>
        /// <param name="hashAlgorithm">Hash algorithm to use</param>
        /// <param name="salt">Salt appended on input before hashing</param>
        public SecretTeller(string consonants, string vowels, HashAlgorithm hashAlgorithm, string salt)
        {
            if (consonants == null)
                throw new ArgumentNullException("consonants");

            if (!consonants.Any())
                throw new ArgumentException("An empty consonant alphabet was given.");

            if (vowels == null)
                throw new ArgumentNullException("vowels");

            if (!vowels.Any())
                throw new ArgumentException("An empty vowel alphabet was given.");

            if (hashAlgorithm == null)
                throw new ArgumentNullException("hashAlgorithm");

            _consonants = consonants.ToCharArray();
            _vowels = vowels.ToCharArray();
            _hashAlgorithm = hashAlgorithm;
            _salt = string.IsNullOrEmpty(salt) ? new byte[0] : Encoding.UTF8.GetBytes(salt);
        }

        public void Dispose()
        {
            _hashAlgorithm.Dispose();
        }

        /// <summary>
        /// Tells a secret.
        /// </summary>
        /// <param name="something">Something to turn into secret (UTF-8 encoded)</param>
        /// <param name="length">Secret length</param>
        /// <returns>A secret</returns>
        public string Tell(string something, int length = 8)
        {
            return Tell(Encoding.UTF8.GetBytes(something), length);
        }

        /// <summary>
        /// Tells a secret.
        /// </summary>
        /// <param name="something">Something to turn into secret</param>
        /// <param name="length">Secret length</param>
        /// <returns>A secret</returns>
        public string Tell(byte[] something, int length = 8)
        {
            if (something == null)
                throw new ArgumentNullException("something");

            if (length <= 0)
                throw new ArgumentException("A negative length was given.");

            if ((_hashAlgorithm.HashSize / 8) < length)
                throw new ArgumentException("A length shorter than the hash size was given.");

            // Salting the given input
            var salted = SaltIt(something);

            // Computing the hash
            var hash = HashIt(salted);

            // Getting alphabets in order
            var alphabets = GetAlphabets(length);

            // Building the secret and returning it
            return SecretIt(hash, length, alphabets);
        }

        private byte[] SaltIt(byte[] something)
        {
            return _salt.Length > 0 ? something.Concat(_salt).ToArray() : something;
        }

        private byte[] HashIt(byte[] something)
        {
            return _hashAlgorithm.ComputeHash(something);
        }

        private Tuple<char[], char[]> GetAlphabets(int secretLength)
        {
            var oddLength = secretLength % 2 != 0;
            var current = oddLength ? _vowels : _consonants;
            var next = oddLength ? _consonants : _vowels;
            return new Tuple<char[], char[]>(current, next);
        }

        private static string SecretIt(byte[] hash, int secretLength, Tuple<char[], char[]> alphabets)
        {
            var current = alphabets.Item1;
            var next = alphabets.Item2;
            var secret = new char[secretLength];
            foreach (var i in Enumerable.Range(0, secretLength))
            {
                secret[i] = current.SafeElementAt(hash[i]);
                current = Interlocked.Exchange(ref next, current);
            }
            return new string(secret);
        }
    }
}
