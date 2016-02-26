<p align="center">
    <a href="#secret">
        <img alt="logo" src="Assets/logo-200x200.png">
    </a>
</p>

# Secret

[![][build-img]][build]
[![][nuget-img]][nuget]

You tell me something and I give you back a secret.

[build]:     https://ci.appveyor.com/project/TallesL/net-secret
[build-img]: https://ci.appveyor.com/api/projects/status/github/tallesl/net-secret?svg=true
[nuget]:     https://www.nuget.org/packages/Secret
[nuget-img]: https://badge.fury.io/nu/Secret.svg

## Usage

```cs
using SecretLibrary;

using (var teller = new SecretTeller())
{
    teller.Tell("Hello world!");  // returns "pijozabe"
    teller.Tell("Hello secret?"); // returns "qoxefewe"
}
```

The defaults are:

* **consonants**: `"bcdfghjklmnpqrstvwxz"`;
* **vowels**: `"aeiou"`;
* **hashAlgorithm**: [SHA1];
* **salt**: [empty string](https://msdn.microsoft.com/library/system.string.empty.aspx).

[SHA1]:              https://msdn.microsoft.com/library/System.Security.Cryptography.SHA1
[empty string]:      https://msdn.microsoft.com/library/System.String.Empty

## How it works

It's a cheap trick: it hashes your input and, with the hash bytes, picks consonant + vogal pairs forming the so called
secret (no fancy [NLP] here).

[NLP]: http://en.wikipedia.org/wiki/Natural_language_processing