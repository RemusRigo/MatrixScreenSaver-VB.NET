'--------------------------------------------------------------------------------------------------
' Matrix ScreenSaver: frmMatrix.vb: Matrix form
'    © 2026 Remus Rigo
'       v1.0.20260927
'--------------------------------------------------------------------------------------------------

Imports Microsoft.Win32

Public Class frmMatrix
   Inherits Form

   Private ReadOnly startMousePos As Point

   Private ReadOnly rand As New Random()
   Private ReadOnly timer As New Timer()

   Private columns As List(Of MatrixColumn)
   Private charWidth As Integer
   Private charHeight As Integer
   Private numCols As Integer
   Private numRows As Integer
   Private matrixFont As Font
   Private firstMove As Boolean = True
   Private startPos As Point

   ' Theme tint: set once from registry (0=red, 1=green, 2=blue)
   Private tintR As Single, tintG As Single, tintB As Single

   ' Preview mode: hosted inside the Screen Saver Settings dialog's little monitor
   Private ReadOnly isPreview As Boolean = False
   Private ReadOnly previewHwnd As IntPtr = IntPtr.Zero

   '----------------------------------------------------------------------------------------------
   ' Win32 API
   Private Const GWL_STYLE As Integer = -16
   Private Const WS_CHILD As Integer = &H40000000

   Private Structure RECT
      Public Left As Integer
      Public Top As Integer
      Public Right As Integer
      Public Bottom As Integer
   End Structure

   Private Declare Function SetParent Lib "user32" (hWndChild As IntPtr, hWndNewParent As IntPtr) As IntPtr
   Private Declare Function GetWindowLong Lib "user32" Alias "GetWindowLongW" (hWnd As IntPtr, nIndex As Integer) As Integer
   Private Declare Function SetWindowLong Lib "user32" Alias "SetWindowLongW" (hWnd As IntPtr, nIndex As Integer, dwNewLong As Integer) As Integer
   Private Declare Function GetClientRect Lib "user32" (hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
   Private Declare Function IsWindowVisible Lib "user32" (hWnd As IntPtr) As Boolean

   ' Character set: mix of ASCII + Katakana-like range
   Private ReadOnly chars As Char() = BuildCharSet()

   '----------------------------------------------------------------------------------------------
   ' Column data structure
   Private Class MatrixColumn
      Public Property ColumnIndex As Integer
      Public Property HeadRow As Single   ' in "row units"
      Public Property Speed As Single     ' rows per tick
      Public Property Length As Integer   ' trail length in rows
      Public Property Characters As Char()
      Public Property IsMessageColumn As Boolean = False
   End Class

   '----------------------------------------------------------------------------------------------
   ' Generate chars
   Private Shared Function BuildCharSet() As Char()
      Dim list As New List(Of Char)()

      ' 0-9
      If RegValueExists(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowNumbers") AndAlso
         RegReadDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowNumbers") = 1 Then
         For c As Integer = Asc("0"c) To Asc("9"c)
            list.Add(ChrW(c))
         Next
      End If

      ' a-z
      If RegValueExists(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowSmallLetters") AndAlso
         RegReadDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowSmallLetters") = 1 Then
         For c As Integer = Asc("a"c) To Asc("z"c)
            list.Add(ChrW(c))
         Next
      End If

      ' A-Z
      If RegValueExists(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowCapsLetters") AndAlso
         RegReadDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowCapsLetters") = 1 Then
         For c As Integer = Asc("A"c) To Asc("Z"c)
            list.Add(ChrW(c))
         Next
      End If

      ' Katakana: check defaults (create registry value if it doesn't exist)
      If Not RegValueExists(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowKatakana") Then
         RegWriteDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowKatakana", 1)
      End If

      ' Katakana / Unicode range: U+30A0 to U+30FF
      ' If nothing is selected, default to Katakana
      If (RegReadDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "ShowKatakana") = 1) OrElse (list.Count = 0) Then
         For code As Integer = &H30A0 To &H30FF
            list.Add(ChrW(code))
         Next
      End If

      Return list.ToArray()
   End Function

   '----------------------------------------------------------------------------------------------
   ' Full screen mode (/s): one form per monitor
   Public Sub New(bounds As Rectangle)
      Me.StartPosition = FormStartPosition.Manual
      Me.Bounds = bounds

      Me.BackColor = Color.Black
      Me.FormBorderStyle = FormBorderStyle.None
      Me.WindowState = FormWindowState.Maximized
      Me.TopMost = True
      Me.DoubleBuffered = True
      Cursor.Hide()

      ' Font: monospaced for clean columns
      matrixFont = New Font("Consolas", 16, FontStyle.Bold, GraphicsUnit.Pixel)

      StartTimer()

      startMousePos = Cursor.Position
   End Sub

   '----------------------------------------------------------------------------------------------
   ' Preview mode (/p hwnd): child window of the Screen Saver Settings preview monitor
   Public Sub New(previewHwnd As IntPtr)
      Me.isPreview = True
      Me.previewHwnd = previewHwnd

      ' set these before the handle is created (changing them later recreates the handle)
      Me.BackColor = Color.Black
      Me.FormBorderStyle = FormBorderStyle.None
      Me.ShowInTaskbar = False
      Me.StartPosition = FormStartPosition.Manual
      Me.DoubleBuffered = True

      ' smaller font so the tiny preview still shows plenty of columns
      matrixFont = New Font("Consolas", 8, FontStyle.Bold, GraphicsUnit.Pixel)

      ' make this form a child of the preview window & fill its client area
      SetParent(Me.Handle, previewHwnd)
      SetWindowLong(Me.Handle, GWL_STYLE, GetWindowLong(Me.Handle, GWL_STYLE) Or WS_CHILD)

      Dim rc As RECT
      GetClientRect(previewHwnd, rc)
      Me.Location = Point.Empty
      Me.Size = New Size(rc.Right - rc.Left, rc.Bottom - rc.Top)

      StartTimer()
   End Sub

   '----------------------------------------------------------------------------------------------
   ' Timer ~ 30 FPS
   Private Sub StartTimer()
      timer.Interval = 30
      AddHandler timer.Tick, AddressOf OnTick
      timer.Start()
   End Sub

   '----------------------------------------------------------------------------------------------
   ' Create a new column with random properties
   Private Function CreateColumn(colIndex As Integer) As MatrixColumn
      Dim c As New MatrixColumn()
      c.ColumnIndex = colIndex

      ' Start head at random row above screen for staggered entry
      c.HeadRow = -rand.Next(0, numRows)

      ' Speed in rows per tick (float)
      c.Speed = 0.1F + CSng(rand.NextDouble() * 0.3) ' 0.1 – 0.4 rows/tick
      'c.Speed = 0.2F + CSng(rand.NextDouble() * 0.8) '0.2 – 1.0 rows/tick
      'c.Speed = 0.5F + CSng(rand.NextDouble() * 1.5) '0.5 – 2.0 rows/tick

      ' Trail length (rows | chars)
      c.Length = rand.Next(7, 49)

      ' Randomize initial characters
      c.Characters = New Char(c.Length - 1) {}

      ' populate with chars
      For i As Integer = 0 To c.Length - 1
         c.Characters(i) = RandomChar()
      Next
      Return c
   End Function

   '----------------------------------------------------------------------------------------------
   ' Generate a random character from the character set
   Private Function RandomChar() As Char
      Return chars(rand.Next(chars.Length))
   End Function

   '----------------------------------------------------------------------------------------------
   ' Load color theme from registry
   Private Sub LoadTint()
      Dim selectedColor As Integer = 1 ' default to green
      If RegValueExists(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "Color") Then
         selectedColor = RegReadDWord(Registry.CurrentUser, "Software\Remus RIGO\MatrixSS", "Color")
      End If

      Select Case selectedColor
         Case 0 : tintR = 1.0F : tintG = 0.0F : tintB = 0.0F   ' red
         Case 2 : tintR = 0.0F : tintG = 0.45F : tintB = 1.0F  ' blue (a bit of green keeps it bright)
         Case Else : tintR = 0.0F : tintG = 1.0F : tintB = 0.0F ' green
      End Select
   End Sub

   '----------------------------------------------------------------------------------------------
   ' Scale the tint by intensity (0..1); "white" lifts the other channels toward white for heads
   Private Function Tint(alpha As Integer, intensity As Single, Optional white As Integer = 0) As Color
      Dim r As Integer = Math.Min(255, white + CInt((255 - white) * tintR * intensity))
      Dim g As Integer = Math.Min(255, white + CInt((255 - white) * tintG * intensity))
      Dim b As Integer = Math.Min(255, white + CInt((255 - white) * tintB * intensity))
      Return Color.FromArgb(alpha, r, g, b)
   End Function

   '----------------------------------------------------------------------------------------------
   ' Setup a message in the center of the screen
   Private Sub SetupMessage(msg As String)
      Dim col As MatrixColumn = columns(numCols \ 2)   ' center column

      col.Length = msg.Length
      col.Characters = New Char(col.Length - 1) {}

      ' normal message:  col.Characters(i) = msg(i)
      ' reverse message: col.Characters(msg.Length - 1 - i) = msg(i)

      For i As Integer = 0 To msg.Length - 1
         col.Characters(msg.Length - 1 - i) = msg(i)
      Next

      ' head enters screen immediately from row 0 downward
      col.HeadRow = col.Length

      ' low speed for message column
      col.Speed = 0.1F

      col.IsMessageColumn = True
   End Sub

   '----------------------------------------------------------------------------------------------
   ' OnLoad
   Protected Overrides Sub OnLoad(e As EventArgs)
      MyBase.OnLoad(e)

      ' Read color theme once
      LoadTint()

      ' Calculate character size
      Using g As Graphics = Me.CreateGraphics()
         Dim size As SizeF = g.MeasureString("W", matrixFont)
         charWidth = CInt(Math.Ceiling(size.Width))
         charHeight = CInt(Math.Ceiling(size.Height))
      End Using

      ' Calculate rows & cols
      numCols = CInt(Math.Ceiling(Me.ClientSize.Width / charWidth))
      numRows = CInt(Math.Ceiling(Me.ClientSize.Height / charHeight))

      ' Initialize columns
      columns = New List(Of MatrixColumn)()
      For colIndex As Integer = 0 To numCols - 1
         columns.Add(CreateColumn(colIndex))
      Next

      startPos = Cursor.Position

      ' Display message in center column & reverse it for proper reading direction
      SetupMessage(StrReverse("© 2026 Remus Rigo"))
   End Sub

   '----------------------------------------------------------------------------------------------
   ' OnTick
   Private Sub OnTick(sender As Object, e As EventArgs)
      ' Preview: quit when the Settings dialog closes or another screensaver is selected
      If isPreview AndAlso Not IsWindowVisible(previewHwnd) Then
         Application.Exit()
         Return
      End If

      For Each col In columns
         col.HeadRow += col.Speed

         ' Skip mutation for message column
         If rand.NextDouble() < 0.1 AndAlso Not col.IsMessageColumn Then
            Dim idx As Integer = rand.Next(0, col.Length)
            col.Characters(idx) = RandomChar()
         End If

         ' If head is far beyond bottom, reset column
         If col.HeadRow - col.Length > numRows Then
            col.IsMessageColumn = False
            Dim colIndex As Integer = col.ColumnIndex
            Dim newCol As MatrixColumn = CreateColumn(colIndex)
            col.HeadRow = newCol.HeadRow
            col.Speed = newCol.Speed
            col.Length = newCol.Length
            col.Characters = newCol.Characters
         End If
      Next

      Me.Invalidate()
   End Sub

   '----------------------------------------------------------------------------------------------
   ' OnPaint
   Protected Overrides Sub OnPaint(e As PaintEventArgs)
      MyBase.OnPaint(e)

      Dim g As Graphics = e.Graphics
      g.Clear(Color.Black)
      g.SmoothingMode = Drawing2D.SmoothingMode.None
      g.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit

      For Each col In columns
         Dim x As Integer = col.ColumnIndex * charWidth

         For i As Integer = 0 To col.Length - 1
            Dim rowPos As Single = col.HeadRow - (col.Length - 1 - i)
            Dim y As Integer = CInt(rowPos * charHeight)

            If y < -charHeight OrElse y > Me.ClientSize.Height Then
               Continue For
            End If

            Dim ch As Char = col.Characters(i)
            Dim t As Single = CSng(i) / CSng(Math.Max(1, col.Length - 1))

            Dim color As Color
            If col.IsMessageColumn Then
               If i = col.Length - 1 Then
                  color = Tint(255, 1.0F, 180)                                   ' near-white head
               Else
                  color = Tint(CInt(255 * (0.3F + 0.7F * t)), 0.86F)             ' solid bright trail
               End If
            ElseIf i = col.Length - 1 Then
               color = Tint(255, 1.0F, 220)                                      ' near-white head
            Else
               color = Tint(CInt(255 * (0.15F + 0.85F * t)), 0.4F + 0.6F * t)    ' fading trail
            End If

            Using brush As New SolidBrush(color)
               g.DrawString(ch.ToString(), matrixFont, brush, x, y)
            End Using
         Next
      Next
   End Sub

   '----------------------------------------------------------------------------------------------
   ' OnKeyDown
   Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
      MyBase.OnKeyDown(e)
      If isPreview Then Return
      ' Any key exits
      Application.Exit()
   End Sub

   '----------------------------------------------------------------------------------------------
   ' OnMouseClick
   Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
      MyBase.OnMouseClick(e)
      If isPreview Then Return
      Application.Exit()
   End Sub

   '----------------------------------------------------------------------------------------------
   ' OnMouseMove
   Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
      MyBase.OnMouseMove(e)
      If isPreview Then Return

      ' Ignore the very first event — Windows fires one on startup
      If firstMove Then
         firstMove = False
         Return
      End If

      ' Only exit if the cursor actually moved from its original position
      If Cursor.Position <> startPos Then
         Application.Exit()
      End If
   End Sub

End Class
