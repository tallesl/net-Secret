using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TellMeASecret.Tests
{
    [TestClass]
    public class Exceptions
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NullConsonants()
        {
            // Arrange
            string consonants = null;
            var vowels = "aeiou";

            // Act
            using (var secretTeller = new SecretTeller(consonants, vowels))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NullVowels()
        {
            // Arrange
            var consonants = "xyz";
            string vowels = null;

            // Act
            using (var secretTeller = new SecretTeller(consonants, vowels))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyConsonants()
        {
            // Arrange
            string consonants = string.Empty;
            var vowels = "aeiou";

            // Act
            using (var secretTeller = new SecretTeller(consonants, vowels))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyVowels()
        {
            // Arrange
            var consonants = "xyz";
            var vowels = string.Empty;

            // Act
            using (var secretTeller = new SecretTeller(consonants, vowels))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NullHashAlgorithm()
        {
            // Arrange
            HashAlgorithm hashAlgorithm = null;

            // Act
            using (var secretTeller = new SecretTeller(hashAlgorithm))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeLength()
        {
            // Arrange
            var something = "something";
            using (var secretTeller = new SecretTeller())
            {
                // Act
                secretTeller.Tell(something, -1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ZeroLength()
        {
            // Arrange
            var something = "something";
            using (var secretTeller = new SecretTeller())
            {
                // Act
                secretTeller.Tell(something, 0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LengthGreaterThanHash()
        {
            // Arrange
            var hashAlgorithm = MD5.Create();
            var something = "something";
            var length = (hashAlgorithm.HashSize / 8) + 1;
            using (var secretTeller = new SecretTeller(hashAlgorithm))
            {
                // Act
                secretTeller.Tell(something, 21);
            }
        }
    }
}
