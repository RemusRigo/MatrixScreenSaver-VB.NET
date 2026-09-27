'--------------------------------------------------------------------------------------------------
' Matrix ScreenSaver: MainProgram.vb: Main program
'    © 2026 Remus Rigo
'       v1.0.20260927
'--------------------------------------------------------------------------------------------------

Module MainProgram
   <STAThread>
   Sub Main(args As String())
      'Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)
      Application.EnableVisualStyles()
      Application.SetCompatibleTextRenderingDefault(False)

      ' Windows screensaver arguments:
      '    /s          run the screensaver
      '    /c[:hwnd]   show settings (no args = settings too, e.g. right-click > Configure)
      '    /p hwnd     preview in the Screen Saver Settings dialog
      Dim mode As String = "/c"
      Dim hwndArg As String = ""
      If args.Length > 0 Then
         Dim first As String = args(0).Trim().ToLowerInvariant().Replace("-"c, "/"c)
         mode = If(first.Length >= 2, first.Substring(0, 2), first)
         If first.Length > 3 AndAlso first(2) = ":"c Then
            hwndArg = first.Substring(3)            ' /c:1234
         ElseIf args.Length > 1 Then
            hwndArg = args(1)                       ' /p 1234
         End If
      End If

      Select Case mode
         Case "/s"
            RunScreenSaver()
         Case "/p"
            ShowPreview(hwndArg)
         Case Else
            ShowConfig(hwndArg)
      End Select
   End Sub

   '----------------------------------------------------------------------------------------------
   ' Fill all monitors
   Private Sub RunScreenSaver()
      Dim forms As New List(Of frmMatrix)
      For Each screen As Screen In Screen.AllScreens
         Dim frm As New frmMatrix(screen.Bounds)
         forms.Add(frm)
         frm.Show()
      Next

      Application.Run()
   End Sub

   '----------------------------------------------------------------------------------------------
   ' Run inside the little monitor of the Screen Saver Settings dialog
   Private Sub ShowPreview(hwndArg As String)
      Dim hwnd As Long
      If Not Long.TryParse(hwndArg, hwnd) OrElse hwnd = 0 Then Return

      Application.Run(New frmMatrix(New IntPtr(hwnd)))
   End Sub

   '----------------------------------------------------------------------------------------------
   ' Show config window, owned by the Screen Saver Settings dialog when its handle is passed
   Private Sub ShowConfig(hwndArg As String)
      Using frm As New frmCfg()
         Dim hwnd As Long
         If Long.TryParse(hwndArg, hwnd) AndAlso hwnd <> 0 Then
            Dim owner As New NativeWindow()
            owner.AssignHandle(New IntPtr(hwnd))
            Try
               frm.StartPosition = FormStartPosition.CenterParent
               frm.ShowDialog(owner)
            Finally
               owner.ReleaseHandle()
            End Try
         Else
            frm.ShowDialog()
         End If
      End Using
   End Sub

End Module
