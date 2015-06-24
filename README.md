<p align="center">
    <a href="#tell-me-a-secret">
        <img alt="logo" src="logo/150x120.png">
    </a>
</p>

# Tell me a secret

[![build](https://ci.appveyor.com/api/projects/status/github/tallesl/TellMeASecret)](https://ci.appveyor.com/project/TallesL/TellMeASecret)
[![nuget package](https://badge.fury.io/nu/TellMeASecret.png)](http://badge.fury.io/nu/TellMeASecret)

A friendly secret generator ([shouldn't be used for actual secure passwords](http://security.stackexchange.com/questions/211/how-to-securely-hash-passwords)).

## Example

```csharp
using TellMeASecret;

using (var secretTeller = new SecretTeller())
{
    secretTeller.Tell("some text"); // returns "tazutiwu"
}
```

## Usage

There are 5 constructors overload:

```csharp
SecretTeller()
SecretTeller(string salt)
SecretTeller(IEnumerable<char> consonants, IEnumerable<char> vowels)
SecretTeller(HashAlgorithm hashAlgorithm)
SecretTeller(IEnumerable<char> consonants, IEnumerable<char> vowels, HashAlgorithm hashAlgorithm, string salt)
```

With the defaults of:

* **consonants**: `b`, `c`, `d`, `f`, `g`, `h`, `j`, `k`, `l`, `m`, `n`, `p`, `q`, `r`, `s`, `t`, `v`, `w`, `x`, `z`;
* **vowels**: `a`, `e`, `i`, `o`, `u`;
* [**hashAlgorithm**](https://msdn.microsoft.com/library/system.security.cryptography.hashalgorithm.aspx): [SHA1](https://msdn.microsoft.com/library/system.security.cryptography.sha1.aspx);
* [**salt**](http://en.wikipedia.org/wiki/Salt_%28cryptography%29): [empty string](https://msdn.microsoft.com/library/system.string.empty.aspx).

## How it works

It's a cheap trick: it hashes your input and, with the hash bytes, picks *consonant + vogal* pairs forming the so called *secret* (no fancy [NLP](http://en.wikipedia.org/wiki/Natural_language_processing) here).

If the desired length is odd the word starts with a vowel.
