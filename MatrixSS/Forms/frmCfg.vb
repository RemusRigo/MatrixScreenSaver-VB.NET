Imports Microsoft.Win32

Public Class frmCfg

   Private Sub frmCfg_Load(sender As Object, e As EventArgs) Handles MyBase.Load
      Me.Text = "Matrix ScreenSaver by © Remus RIGO v1.0.20260927"
      ssStatusLabelInfo.Text = "Matrix ScreenSaver v1.0.20260927"
      ssStatusLabelInfo.IsLink = True

      cmbBoxColor.Items.Add("Red")
      cmbBoxColor.Items.Add("Green")
      cmbBoxColor.Items.Add("Blue")

      If RegValueExists(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowNumbers") Then
         chkBoxNumbers.Checked = RegReadDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowNumbers") <> 0
      End If
      If RegValueExists(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowSmallLetters") Then
         chkBoxSmallLetter.Checked = RegReadDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowSmallLetters") <> 0
      End If
      If RegValueExists(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowCapsLetters") Then
         chkBoxCapsLetter.Checked = RegReadDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowCapsLetters") <> 0
      End If
      If RegValueExists(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowKatakana") Then
         chkBoxKatakana.Checked = RegReadDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowKatakana") <> 0
      Else
         chkBoxKatakana.Checked = True ' default to true
      End If
      If RegValueExists(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "Color") Then
         cmbBoxColor.SelectedIndex = RegReadDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "Color")
      Else
         cmbBoxColor.SelectedIndex = 1 ' default to Green
      End If
   End Sub

   Private Sub chkBoxNumbers_CheckedChanged(sender As Object, e As EventArgs) Handles chkBoxNumbers.CheckedChanged
      RegWriteDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowNumbers", If(chkBoxNumbers.Checked, 1, 0))
   End Sub

   Private Sub chkBoxSmallLetter_CheckedChanged(sender As Object, e As EventArgs) Handles chkBoxSmallLetter.CheckedChanged
      RegWriteDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowSmallLetters", If(chkBoxSmallLetter.Checked, 1, 0))
   End Sub

   Private Sub chkBoxCapsLetter_CheckedChanged(sender As Object, e As EventArgs) Handles chkBoxCapsLetter.CheckedChanged
      RegWriteDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowCapsLetters", If(chkBoxCapsLetter.Checked, 1, 0))
   End Sub

   Private Sub chkBoxKatakana_CheckedChanged(sender As Object, e As EventArgs) Handles chkBoxKatakana.CheckedChanged
      RegWriteDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowKatakana", If(chkBoxKatakana.Checked, 1, 0))
   End Sub

   Private Sub cmbBoxColor_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBoxColor.SelectedIndexChanged
      RegWriteDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "Color", cmbBoxColor.SelectedIndex)
   End Sub

   Private Sub ssStatusLabelInfo_Click(sender As Object, e As EventArgs) Handles ssStatusLabelInfo.Click
      Process.Start(New ProcessStartInfo With {
         .FileName = "https://github.com/RemusRigo/Matrix-ScreenSaver-VB.NET",
         .UseShellExecute = True
      })
   End Sub

   Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
      Me.DialogResult = DialogResult.OK
   End Sub
End Class