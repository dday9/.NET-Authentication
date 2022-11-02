# .NET-Authentication
A Visual Basic .NET (VB.NET) implementation that generates salts and hashes used to encrypt passwords.

## Add to project
The [Authentication.vb](Authentication.vb) code file is uncompiled. Follow these instructions to add the file to your project:

 1. Project > Add Existing Item (shift + alt + a)
 2. Select the file in the browse dialog
 3. Click Add

## Demo
Fiddle: https://dotnetfiddle.net/sVvYeS

## Fields
### \_byteLength
Gets the length of the salt and hash

``` vb
Private Shared ReadOnly _byteLength As Integer = 32
```

### \_rcfIterations
Gets the number of iterations to derive the key using Rfc2898

``` vb
Private Shared ReadOnly _rcfIterations As Integer = 100000
```

## Methods
### Authenticate
Compares a plaintext String against a previously encrypted plaintext value

``` vb
Public Shared Function Authenticate(plaintext As String, encryptedText As Byte()) As Boolean
```

#### Parameters
`plaintext` The String to encrypt
`encryptedText` A collection of Byte representing the previously encrypted plaintext value

#### Returns
Boolean

#### Example
``` vb
Dim existingPassword = Authentication.EncryptPlainText("Password123456789")
Dim incomingPassword = Console.ReadLine()
Dim authenticated = Authentication.Authenticate(incomingPassword, existingPassword)
```

### CombineSaltAndHash
Prepends one Byte array representing the salt to another Byte array representing the hash

``` vb
Public Shared Function CombineSaltAndHash(salt As Byte(), hash As Byte()) As IEnumerable(Of Byte)
```

#### Parameters
`salt` A collection of Byte representing the salt

`hash` A collection of Byte representing the hash

#### Returns
IEnumerable(Of Byte)

#### Exceptions
`ArgumentOutOfRangeException` \_byteLength cannot be less than 1

`ArgumentOutOfRangeException` salt is invalid because the length does not exactly match byteLength

`ArgumentOutOfRangeException` hash is invalid because the length does not exactly match byteLength

#### Example
``` vb
Dim salt = Authentication.GenerateSalt()
Dim hash = Authentication.GenerateHash("Password123456789", salt)
Dim combinedHash = Authentication.CombineSaltAndHash(salt, hash)
```

### EncryptPlainText
Encrypts a plaintext String to a collection of Byte

``` vb
Public Shared Function EncryptPlainText(plaintext As String) As IEnumerable(Of Byte)
```

#### Parameters
`plaintext` The String to encrypt

#### Returns
IEnumerable(Of Byte)

#### Example
``` vb
Dim existingPassword = Authentication.EncryptPlainText("Password123456789")
```

### GenerateHash
Uses Rfc2898DeriveBytes To generate a hash

``` vb
Public Shared Function GenerateHash(plaintext As String, salt As Byte()) As Byte()
```

#### Parameters
`plaintext` The String representing the password used to derive the key

`salt` The collection of Byte representing the salt used to derive the key

#### Returns
Collection of Byte

#### Exceptions
`ArgumentOutOfRangeException` \_byteLength cannot be less than 1
`ArgumentOutOfRangeException` \_rcfIterations cannot be less than 1
`ArgumentNullException` plaintext cannot be null
`ArgumentOutOfRangeException` salt Is invalid because the length does Not exactly match byteLength

#### Example
``` vb
Dim salt = Authentication.GenerateSalt()
Dim hash = Authentication.GenerateHash("Password123456789", salt)
```

### GenerateSalt
Uses RandomNumberGenerator To generate a salt

``` vb
Public Shared Function GenerateSalt() As Byte()
```

#### Returns
Collection of Byte

#### Exceptions
`ArgumentOutOfRangeException` \_byteLength cannot be less than 1

#### Example
``` vb
Dim salt = Authentication.GenerateSalt()
```

## Donate
Show your support! Your (non-tax deductible) donation of Monero cryptocurrency is a sign of solidarity among web developers.

Being self taught, I have come a long way over the years. I certainly do not intend on making a living from this free feature, but my hope is to earn a few dollars to validate all of my hard work.

Monero Address: 447SPi8XcexZnF7kYGDboKB6mghWQzRfyScCgDP2r4f2JJTfLGeVcFpKEBT9jazYuW2YG4qn51oLwXpQJ3oEXkeXUsd6TCF

![447SPi8XcexZnF7kYGDboKB6mghWQzRfyScCgDP2r4f2JJTfLGeVcFpKEBT9jazYuW2YG4qn51oLwXpQJ3oEXkeXUsd6TCF](monero.png)
