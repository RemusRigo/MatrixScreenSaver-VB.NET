Imports System
Imports System.Windows.Forms

Module Main
   <STAThread>
   Sub Main(args As String())
      'Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)
      Application.EnableVisualStyles()
      Application.SetCompatibleTextRenderingDefault(False)


      Dim forms As New List(Of MatrixForm)

      For Each screen As Screen In Screen.AllScreens
         Dim frm As New MatrixForm(screen.Bounds)
         forms.Add(frm)
         frm.Show()
      Next

      Application.Run()
   End Sub

End Module
