using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using CyclicElementAt;

namespace TellMeASecret
{
    /// <summary>
    /// Tells secrets.
    /// </summary>
    public class SecretTeller : IDisposable
    {
        /// <summary>
        /// The default consonant alphabet assumed if none is given.
        /// </summary>
        private static readonly ReadOnlyCollection<char> _defaultConsonants = new List<char> { 
            'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'
        }.AsReadOnly();

        /// <summary>
        /// The default vowel alphabet assumed if none is given.
        /// </summary>
        private static readonly ReadOnlyCollection<char> _defaultVowels = new List<char> {
            'a', 'e', 'i', 'o', 'u'
        }.AsReadOnly();

        /// <summary>
        /// The default hash algorithm assumed if none is given.
        /// </summary>
        private static HashAlgorithm _defaultHashAlgorithm
        {
            get
            {
                return SHA1.Create();
            }
        }

        /// <summary>
        /// Optional salt append on input before hashing.
        /// </summary>
        private readonly byte[] _salt;

        /// <summary>
        /// Alphabet of consonants.
        /// </summary>
        private readonly ReadOnlyCollection<char> _consonants;

        /// <summary>
        /// Alphabet of vowels.
        /// </summary>
        private readonly ReadOnlyCollection<char> _vowels;

        /// <summary>
        /// Hash algorithm to use.
        /// </summary>
        private readonly HashAlgorithm _hashAlgorithm;

        /// <summary>
        /// Ctor.
        /// </summary>
        public SecretTeller()
            : this(_defaultConsonants, _defaultVowels, _defaultHashAlgorithm, string.Empty) { }
    
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="salt">Salt appended on input before hashing</param>
        public SecretTeller(string salt)
            : this(_defaultConsonants, _defaultVowels, _defaultHashAlgorithm, salt) { }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="consonants">Alphabet of consonants</param>
        /// <param name="vowels">Aliphabet of vowels</param>
        public SecretTeller(IEnumerable<char> consonants, IEnumerable<char> vowels)
            : this(consonants, vowels, _defaultHashAlgorithm, string.Empty) { }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="hashAlgorithm">Hash algorithm to use</param>
        public SecretTeller(HashAlgorithm hashAlgorithm)
            : this(_defaultConsonants, _defaultVowels, hashAlgorithm, string.Empty) { }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="consonants">Alphabet of consonants</param>
        /// <param name="vowels">Alphabet of vowels</param>
        /// <param name="hashAlgorithm">Hash algorithm to use</param>
        /// <param name="salt">Salt appended on input before hashing</param>
        public SecretTeller(IEnumerable<char> consonants, IEnumerable<char> vowels, HashAlgorithm hashAlgorithm,
            string salt)
        {
            if (consonants == null || !consonants.Any())
            {
                throw new ArgumentException("The given consonant alphabet is null or empty.");
            }

            if (vowels == null || !vowels.Any())
            {
                throw new ArgumentException("The given vowel alphabet is null or empty.");
            }

            if (hashAlgorithm == null)
            {
                throw new ArgumentException("The given hash algorithm is null.");
            }

            _salt = string.IsNullOrEmpty(salt) ? new byte[0] : Encoding.UTF8.GetBytes(salt);
            _consonants = consonants.ToList().AsReadOnly();
            _vowels = vowels.ToList().AsReadOnly();
            _hashAlgorithm = hashAlgorithm;
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
            if (length <= 0)
            {
                throw new ArgumentException("The given length must be greater than zero.");
            }

            if ((_hashAlgorithm.HashSize / 8) < length)
            {
                throw new ArgumentException(
                    "The hash size of the used algorithm must be greater than the provided length.");
            }

            // Salting the given input
            var salted = SaltIt(something);

            // Computing the hash
            var hash = HashIt(salted);

            // Getting alphabets in order
            var alphabets = GetAlphabets(length);

            // Building the secret
            var secret = SecretIt(hash, length, alphabets);

            return secret;
        }

        /// <summary>
        /// Salts the given input.
        /// </summary>
        /// <param name="something">Something to be salted</param>
        /// <returns>Salted input</returns>
        private byte[] SaltIt(byte[] something)
        {
            return _salt.Length > 0 ? something.Concat(_salt).ToArray() : something;
        }

        /// <summary>
        /// Hashes the given input.
        /// </summary>
        /// <param name="something">Something to be hashed.</param>
        /// <returns>Hashed input</returns>
        private byte[] HashIt(byte[] something)
        {
            return _hashAlgorithm.ComputeHash(something);
        }

        /// <summary>
        /// If the length is odd, returns vowel alphabet first then the consonant one.
        /// Else, if even, returns consonant first.
        /// </summary>
        /// <param name="secretLength">Secret length</param>
        /// <returns>Both alphabets properly ordered</returns>
        private Tuple<ReadOnlyCollection<char>, ReadOnlyCollection<char>> GetAlphabets(int secretLength)
        {
            var oddLength = secretLength % 2 != 0;
            var current = oddLength ? _vowels : _consonants;
            var next = oddLength ? _consonants : _vowels;
            return new Tuple<ReadOnlyCollection<char>, ReadOnlyCollection<char>>(current, next);
        }

        /// <summary>
        /// Does the actual "secreting".
        /// </summary>
        /// <param name="hash">The computed hash</param>
        /// <param name="secretLength">Secret length</param>
        /// <param name="alphabets">The alphabets to use</param>
        /// <returns>The secret</returns>
        private string SecretIt(byte[] hash, int secretLength,
            Tuple<ReadOnlyCollection<char>, ReadOnlyCollection<char>> alphabets)
        {
            var current = alphabets.Item1;
            var next = alphabets.Item2;
            var secret = new char[secretLength];
            foreach (var i in Enumerable.Range(0, secretLength))
            {
                secret[i] = current.CyclicElementAt(hash[i]);
                current = Interlocked.Exchange(ref next, current);
            }
            return new string(secret);
        }
    }
}
