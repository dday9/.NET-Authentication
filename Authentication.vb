Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Security.Cryptography

Public Class Authentication

	''' <summary>
	''' Gets the length of the salt and hash
	''' </summary>
	Private Shared ReadOnly _byteLength As Integer = 32

	''' <summary>
	''' Gets the number of iterations to derive the key using Rfc2898
	''' </summary>
	Private Shared ReadOnly _rcfIterations As Integer = 100000

	''' <summary>
	''' Compares a plaintext <see cref="String"/> against a previously encrypted plaintext value
	''' </summary>
	''' <param name="plaintext">The <see cref="String"/> to encrypt</param>
	''' <param name="encryptedText">A collection of <see cref="Byte"/> representing the previously encrypted plaintext value</param>
	''' <returns><see cref="Boolean"/></returns>
	''' <remarks>This method takes steps to prevent timing attacks by not immediately returning a False value if the password does not authenticate. For more information visit: https://en.wikipedia.org/wiki/Timing_attack</remarks>
	Public Shared Function Authenticate(plaintext As String, encryptedText As Byte()) As Boolean
		Dim authenticated = True

		Dim salt(_byteLength - 1) As Byte
		If (encryptedText.Length > _byteLength) Then
			For index = 0 To _byteLength - 1
				salt(index) = encryptedText(index)
			Next
		Else
			For index = 0 To _byteLength - 1
				salt(index) = [Byte].MinValue
			Next
			authenticated = False
		End If

		Dim hash() As Byte = {}
		Try
			hash = Authentication.GenerateHash(plaintext, salt)
		Catch
			For index = 0 To _byteLength - 1
				hash(index) = [Byte].MinValue
			Next
			authenticated = False
		End Try

		If (encryptedText.Length - _byteLength = hash.Length) Then
			For index = 0 To hash.Length - 1
				If (authenticated) Then
					authenticated = hash(index) = encryptedText(_byteLength + index)
				Else
					authenticated = [Byte].MinValue = [Byte].MaxValue
				End If
			Next
		Else
			For index = 0 To hash.Length - 1
				authenticated = [Byte].MinValue = [Byte].MaxValue
			Next
		End If

		Return authenticated
	End Function

	''' <summary>
	''' Prepends one <see cref="Byte"/> array representing the salt to another <see cref="Byte"/> array representing the hash
	''' </summary>
	''' <param name="salt">A collection of <see cref="Byte"/> representing the salt</param>
	''' <param name="hash">A collection of <see cref="Byte"/> representing the hash</param>
	''' <returns><see cref="IEnumerable(Of Byte)"/></returns>
	''' <exception cref="ArgumentOutOfRangeException"><see cref="_byteLength"/> cannot be less than 1</exception>
	''' <exception cref="ArgumentOutOfRangeException"><param name="salt"/> is invalid because the length does not exactly match <see cref="_byteLength"/></exception>
	''' <exception cref="ArgumentOutOfRangeException"><param name="hash"/> is invalid because the length does not exactly match <see cref="_byteLength"/></exception>
	Public Shared Function CombineSaltAndHash(salt As Byte(), hash As Byte()) As IEnumerable(Of Byte)
		If (_byteLength < 1) Then
			Throw New ArgumentOutOfRangeException($"{NameOf(_byteLength)} cannot be less than 1")
		End If
		If (salt.Length <> _byteLength) Then
			Throw New ArgumentOutOfRangeException(NameOf(salt), "The incoming salt is invalid")
		End If
		If (hash.Length <> _byteLength) Then
			Throw New ArgumentOutOfRangeException(NameOf(hash), "The incoming hash is invalid")
		End If

		Return salt.Concat(hash).ToArray()
	End Function

	''' <summary>
	''' Encrypts a plaintext <see cref="String"/> to a collection of <see cref="Byte"/>
	''' </summary>
	''' <param name="plaintext">The <see cref="String"/> to encrypt</param>
	''' <returns><see cref="IEnumerable(Of Byte)"/></returns>
	Public Shared Function EncryptPlainText(plaintext As String) As IEnumerable(Of Byte)
		Dim salt = Authentication.GenerateSalt()
		Dim hash = Authentication.GenerateHash(plaintext, salt)
		Dim combinedHash = Authentication.CombineSaltAndHash(salt, hash)
		Return combinedHash
	End Function

	''' <summary>
	''' Uses <see cref="Rfc2898DeriveBytes"/> to generate a hash
	''' </summary>
	''' <param name="plaintext">The <see cref="String"/> representing the password used to derive the key</param>
	''' <param name="salt">The collection of <see cref="Byte"/> representing the salt used to derive the key</param>
	''' <returns>Collection of <see cref="Byte"/></returns>
	''' <exception cref="ArgumentOutOfRangeException"><see cref="_byteLength"/> cannot be less than 1</exception>
	''' <exception cref="ArgumentOutOfRangeException"><see cref="_rcfIterations"/> cannot be less than 1</exception>
	''' <exception cref="ArgumentNullException"><paramref name="plaintext"/> cannot be null</exception>
	''' <exception cref="ArgumentOutOfRangeException"><paramref name="salt"/> is invalid because the length does not exactly match <see cref="_byteLength"/></exception>
	Public Shared Function GenerateHash(plaintext As String, salt As Byte()) As Byte()
		If (_byteLength < 1) Then
			Throw New ArgumentOutOfRangeException($"{NameOf(_byteLength)} cannot be less than 1")
		End If
		If (_rcfIterations < 1) Then
			Throw New ArgumentOutOfRangeException($"{NameOf(_rcfIterations)} cannot be less than 1")
		End If
		If (String.IsNullOrWhiteSpace(plaintext)) Then
			Throw New ArgumentNullException(NameOf(plaintext))
		End If
		If (salt.Length <> _byteLength) Then
			Throw New ArgumentOutOfRangeException(NameOf(salt), "The incoming salt is invalid")
		End If

		Dim hash() As Byte = {}
		Using rcf = New Rfc2898DeriveBytes(plaintext, salt, _rcfIterations)
			hash = rcf.GetBytes(_byteLength)
		End Using

		Return hash
	End Function

	''' <summary>
	''' Uses <see cref="RandomNumberGenerator"/> to generate a salt
	''' </summary>
	''' <returns>Collection of <see cref="Byte"/></returns>
	''' <exception cref="ArgumentOutOfRangeException"><see cref="_byteLength"/> cannot be less than 1</exception>
	Public Shared Function GenerateSalt() As Byte()
		If (_byteLength < 1) Then
			Throw New ArgumentOutOfRangeException($"{NameOf(_byteLength)} cannot be less than 1")
		End If

		Dim salt(_byteLength - 1) As Byte
		Using provider = RandomNumberGenerator.Create()
			provider.GetBytes(salt)
		End Using
		Return salt
	End Function

End Class
