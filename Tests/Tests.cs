namespace SecretLibrary.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Security.Cryptography;

    [TestClass]
    public class Tests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConsonants()
        {
            new SecretTeller(null, "aeiou");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullVowels()
        {
            new SecretTeller("xyz", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyConsonants()
        {
            new SecretTeller(string.Empty, "aeiou");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyVowels()
        {
            new SecretTeller("xyz", string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeLength()
        {
            using (var secretTeller = new SecretTeller())
            {
                secretTeller.Tell("something", -1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ZeroLength()
        {
            using (var secretTeller = new SecretTeller())
            {
                secretTeller.Tell("something", 0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LengthGreaterThanHash()
        {
            using (var teller = new SecretTeller())
            {
                teller.Tell("something", 21);
            }
        }

        [TestMethod]
        public void Vanilla()
        {
            using (var secretTeller = new SecretTeller())
            {
                Assert.AreEqual("jejasuni", secretTeller.Tell("something"));
            }
        }

        [TestMethod]
        public void CustomLength()
        {
            using (var teller = new SecretTeller())
            {
                Assert.AreEqual(15, teller.Tell("something", 15).Length);
            }
        }

        [TestMethod]
        public void CustomAlphabets()
        {
            using (var teller = new SecretTeller("bcd", "aei"))
            {
                Assert.AreEqual("debebica", teller.Tell("something"));
            }
        }

        [TestMethod]
        public void CustomEverything()
        {
            using (var teller = new SecretTeller("grf", "aeo", MD5.Create(), "saltydasdad"))
            {
                Assert.AreEqual("gefagefo", teller.Tell("something"));
            }
        }
    }
}
