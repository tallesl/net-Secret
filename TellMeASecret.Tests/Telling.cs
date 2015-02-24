using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TellMeASecret.Tests
{
    [TestClass]
    public class Telling
    {
        [TestMethod]
        public void TellWithNoCustom()
        {
            // Arrange
            var something = "something";
            var expected = "jejasuni";
            var actual = string.Empty;
            using (var secretTeller = new SecretTeller())
            {
                // Act
                actual = secretTeller.Tell(something);
            }

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TellWithCustomLength()
        {
            // Arrange
            var something = "something";
            var length = 15;
            var actual = string.Empty;
            using (var secretTeller = new SecretTeller())
            {
                // Act
                actual = secretTeller.Tell(something, length);
            }

            // Assert
            Assert.AreEqual(length, actual.Length);
        }

        [TestMethod]
        public void TellWithCustomSalt()
        {
            // Arrange
            var salt = "some salt";
            var something = "something";
            var expected = "somojeno";
            var actual = string.Empty;
            using (var secretTeller = new SecretTeller(salt))
            {
                // Act
                actual = secretTeller.Tell(something);
            }

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TellWithCustomAlphabets()
        {
            // Arrange
            var consonants = "bcd";
            var vowels = "aei";
            var something = "something";
            var expected = "debebica";
            var actual = string.Empty;
            using (var secretTeller = new SecretTeller(consonants, vowels))
            {
                // Act
                actual = secretTeller.Tell(something);
            }

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TellWithCustomHashAlgorithm()
        {
            // Arrange
            var hashAlgorithm = MD5.Create();
            var something = "something";
            var expected = "ifirutocu";
            var actual = string.Empty;
            using (var secretTeller = new SecretTeller(hashAlgorithm))
            {
                // Act
                actual = secretTeller.Tell(something, 9);
            }

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TellWithAllCustom()
        {
            // Arrange
            var consonants = "grf";
            var vowels = "aeo";
            var hashAlgorithm = MD5.Create();
            var salt = "saltydasdad";
            var something = "something";
            var expected = "gefagefo";
            var actual = string.Empty;
            using (var secretTeller = new SecretTeller(consonants, vowels, hashAlgorithm, salt))
            {
                // Act
                actual = secretTeller.Tell(something);
            }

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
