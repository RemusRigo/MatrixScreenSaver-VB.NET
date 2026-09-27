'--------------------------------------------------------------------------------------------------
' libReg - Registry functions
'    © 2026 Remus Rigo
'       v1.0 2026-04-29
'
' add Imports Microsoft.Win32
'--------------------------------------------------------------------------------------------------

Imports Microsoft.Win32

Public Module libReg

	'-----------------------------------------------------------------------------------------------
	' Read Boolean
	Public Function RegReadBool(root As RegistryKey, path As String, name As String) As Boolean
		Using key As RegistryKey = root.OpenSubKey(path, False)
			If key Is Nothing Then Return False
			Dim val = key.GetValue(name, Nothing)
			If val Is Nothing Then Return False
			Return val.ToString() = "1"
		End Using
	End Function

	'-----------------------------------------------------------------------------------------------
	' Write Boolean
	Public Function RegWriteBool(root As RegistryKey, path As String, name As String, value As Boolean) As Boolean
		Try
			Using key As RegistryKey = root.CreateSubKey(path, True)
				key.SetValue(name, If(value, "1", "0"), RegistryValueKind.String)
			End Using
			Return True
		Catch ex As Exception
			Return False
		End Try
	End Function

	'-----------------------------------------------------------------------------------------------
	' Read DWord | Integer
	Public Function RegReadDWord(root As RegistryKey, path As String, name As String) As Integer
		Dim r As Integer = -1
		Using key As RegistryKey = root.OpenSubKey(path, False)
			If key IsNot Nothing Then
				r = Convert.ToInt32(key.GetValue(name, Nothing))
			End If
		End Using
		Return r
	End Function

	'-----------------------------------------------------------------------------------------------
	' Write DWord | Integer
	Public Function RegWriteDWord(root As RegistryKey, path As String, name As String, value As Integer) As Boolean
		Try
			Using key As RegistryKey = root.CreateSubKey(path, True)
				key.SetValue(name, value, RegistryValueKind.DWord)
			End Using
			Return True
		Catch ex As Exception
			Return False
		End Try
	End Function

	'-----------------------------------------------------------------------------------------------
	' Read SZ | String
	Public Function RegReadSZ(root As RegistryKey, path As String, name As String) As String
		Try
			' Open the key for reading
			Using key As RegistryKey = root.OpenSubKey(path, False)
				' Check if the KEY exists
				If key Is Nothing Then Return String.Empty

				' Check if the VALUE exists
				If key.GetValue(name) Is Nothing Then Return String.Empty

				' Check if the type is actually a String (SZ) to avoid type mismatch
				Select Case key.GetValueKind(name)
					Case RegistryValueKind.String
						' Normal REG_SZ
						Return key.GetValue(name).ToString()

					Case RegistryValueKind.ExpandString
						' REG_EXPAND_SZ → must expand %variables%
						Dim raw As String = key.GetValue(name).ToString()
						Return Environment.ExpandEnvironmentVariables(raw)

					Case Else
						Return String.Empty
				End Select
			End Using
		Catch ex As Exception
			Return String.Empty
		End Try
	End Function

	'-----------------------------------------------------------------------------------------------
	' Write SZ | String
	Public Function RegWriteSZ(root As RegistryKey, path As String, name As String, value As String) As Boolean
		Try
			Using key As RegistryKey = root.CreateSubKey(path, True)
				key.SetValue(name, value, RegistryValueKind.String)
			End Using
			Return True
		Catch ex As Exception
			Return False
		End Try
	End Function

	'-----------------------------------------------------------------------------------------------
	' Check if value exist
	Public Function RegValueExists(root As RegistryKey, path As String, valueName As String) As Boolean
		Try
			Using key As RegistryKey = root.OpenSubKey(path, False)
				If key IsNot Nothing Then
					' GetValue returns Nothing if the value name does not exist
					Return key.GetValue(valueName) IsNot Nothing
				End If
			End Using
		Catch ex As Exception
			Console.WriteLine("Error accessing registry: " & ex.Message)
		End Try

		Return False
	End Function

	Public Function RegCountValues(root As RegistryKey, path As String, valueName As String) As Integer
		Try
			Using key As RegistryKey = root.OpenSubKey(path, False)
				If key IsNot Nothing Then
					' GetValue returns Nothing if the value name does not exist
					Return key.ValueCount
				End If
			End Using
		Catch ex As Exception
			'Console.WriteLine("Error accessing registry: " & ex.Message)'
		End Try

		Return 0
	End Function

	'-----------------------------------------------------------------------------------------------
	' Delete value
	Public Function RegDeleteValue(root As RegistryKey, path As String, name As String) As Boolean
		Try
			Using key As RegistryKey = root.OpenSubKey(path, True)
				If key.GetValue(name) Is Nothing Then Return False ' Key does not exist
				key.DeleteValue(name, False) ' False to avoid exception if value does not exist
			End Using
			Return True
		Catch ex As Exception
			Return False
		End Try
	End Function


	''' <summary>
	''' Deletes all values from a specified registry key.
	''' </summary>
	''' <param name="root">The root hive (e.g., RegistryHive.CurrentUser, RegistryHive.LocalMachine).</param>
	''' <param name="path">The registry path to the key.</param>
	''' <param name="deleteDefaultValue">Boolean indicating if the default (unnamed) value should be deleted.</param>
	''' <returns>True if successful, False if an error occurs or the key cannot be opened.</returns>
	Public Function RegDeleteAllValues(root As RegistryKey, path As String, deleteDefaultValue As Boolean) As Boolean
		Try
			If root Is Nothing Then
				Return False
			End If

			' Open the subkey directly from the provided root RegistryKey with write permissions
			Using subKey As RegistryKey = root.OpenSubKey(path, writable:=True)

				If subKey Is Nothing Then
					Return False
				End If

				' Retrieve all value names under the key
				Dim valueNames As String() = subKey.GetValueNames()

				' Iterate through the values and delete them
				For Each valueName As String In valueNames
					If deleteDefaultValue Then
						' The second parameter (throwOnMissingValue) set to False prevents exceptions if the value doesn't exist
						subKey.DeleteValue(valueName, False)
					Else
						' Skip empty string name, which represents the default/unnamed value
						If Not String.IsNullOrEmpty(valueName) Then
							subKey.DeleteValue(valueName, False)
						End If
					End If
				Next

				Return True

			End Using

		Catch ex As Exception
			' Log the exception here if needed
			Return False
		End Try
	End Function

End Module
