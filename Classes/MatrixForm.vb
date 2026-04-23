Imports System.Drawing
Imports System.Windows.Forms

Public Class MatrixForm
   Inherits Form

   Private ReadOnly rand As New Random()
   Private ReadOnly timer As New Timer()
   Private ReadOnly startMousePos As Point
   Private columns As List(Of MatrixColumn)
   Private charWidth As Integer
   Private charHeight As Integer
   Private numCols As Integer
   Private numRows As Integer
   Private matrixFont As Font
   Private firstMove As Boolean = True
   Private startPos As Point

   ' Character set: mix of ASCII + Katakana-like range
   Private ReadOnly chars As Char() = BuildCharSet()

   ' Column data structure
   Private Class MatrixColumn
      Public Property ColumnIndex As Integer
      Public Property HeadRow As Single   ' in "row units"
      Public Property Speed As Single     ' rows per tick
      Public Property Length As Integer   ' trail length in rows
      Public Property Characters As Char()
   End Class

   Public Sub New(bounds As Rectangle)
      '      InitializeComponent()
      Me.StartPosition = FormStartPosition.Manual
      Me.Bounds = bounds

      Me.BackColor = Color.Black
      Me.FormBorderStyle = FormBorderStyle.None
      'Me.WindowState = FormWindowState.Maximized
      Me.TopMost = True
      Me.DoubleBuffered = True
      Cursor.Hide()

      ' Font: monospaced for clean columns
      matrixFont = New Font("Consolas", 16, FontStyle.Bold, GraphicsUnit.Pixel)

      ' Timer ~ 30 FPS
      timer.Interval = 33
      AddHandler timer.Tick, AddressOf OnTick
      timer.Start()

      startMousePos = Cursor.Position
   End Sub


   Private Function CreateColumn(colIndex As Integer) As MatrixColumn
      Dim c As New MatrixColumn()
      c.ColumnIndex = colIndex
      c.HeadRow = -rand.Next(0, numRows)             ' Start head at random row above screen for staggered entry

      ' Speed in rows per tick (float)
      c.Speed = 0.1F + CSng(rand.NextDouble() * 0.3) ' 0.1 – 0.4 rows/tick (adjust for faster/slower rain)
      'c.Speed = 0.2F + CSng(rand.NextDouble() * 0.8) '0.2 – 1.0 rows/tick
      'c.Speed = 0.5F + CSng(rand.NextDouble() * 1.5) '0.5 – 2.0 rows/tick

      c.Length = rand.Next(7, 49)                    ' Trail length (rows | chars)
      c.Characters = New Char(c.Length - 1) {}       ' Randomize initial characters
      For i As Integer = 0 To c.Length - 1           ' populate with chars
         c.Characters(i) = RandomChar()
      Next
      Return c
   End Function

   '----------------------------------------------------------------------------------------------
   ' Generate chars
   Private Shared Function BuildCharSet() As Char()
      Dim list As New List(Of Char)()

      ' ASCII letters + digits
      'For c As Integer = Asc("0"c) To Asc("9"c)
      '    list.Add(ChrW(c))
      'Next
      'For c As Integer = Asc("A"c) To Asc("Z"c)
      '    list.Add(ChrW(c))
      'Next
      'For c As Integer = Asc("a"c) To Asc("z"c)
      '    list.Add(ChrW(c))
      'Next

      ' Katakana-like block (just for vibe)
      For code As Integer = &H30A0 To &H30FF
         list.Add(ChrW(code))
      Next

      Return list.ToArray()
   End Function

   Private Function RandomChar() As Char
      Return chars(rand.Next(chars.Length))
   End Function

   '----------------------------------------------------------------------------------------------
   ' OnLoad
   Protected Overrides Sub OnLoad(e As EventArgs)
      MyBase.OnLoad(e)

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
   End Sub

   '----------------------------------------------------------------------------------------------
   ' OnTick
   Private Sub OnTick(sender As Object, e As EventArgs)
      ' Update simulation
      For Each col In columns
         ' Move head in "rows" space
         col.HeadRow += col.Speed

         ' Occasionally mutate characters for flicker
         If rand.NextDouble() < 0.1 Then
            Dim idx As Integer = rand.Next(0, col.Length)
            col.Characters(idx) = RandomChar()
         End If

         ' If head is far beyond bottom, reset column
         If col.HeadRow - col.Length > numRows Then
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

         ' Draw from tail to head
         For i As Integer = 0 To col.Length - 1
            Dim rowPos As Single = col.HeadRow - (col.Length - 1 - i)
            Dim y As Integer = CInt(rowPos * charHeight)

            ' Skip if off-screen
            If y < -charHeight OrElse y > Me.ClientSize.Height Then
               Continue For
            End If

            Dim ch As Char = col.Characters(i)

            ' Compute fade factor: 0..1 along trail
            Dim t As Single = CSng(i) / CSng(Math.Max(1, col.Length - 1))

            ' Head is brightest, tail darker
            Dim color As Color
            If i = col.Length - 1 Then
               ' Head: bright white with slight green tint
               color = Color.FromArgb(255, 220, 255, 220)
            Else
               ' Tail: green with alpha based on t
               Dim alpha As Integer = CInt(255 * (0.15F + 0.85F * t))
               Dim green As Integer = CInt(255 * (0.4F + 0.6F * t))
               color = Color.FromArgb(alpha, 0, green, 0)
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
      If e.KeyCode = Keys.Escape Then
         Application.Exit()
      End If
   End Sub

   '----------------------------------------------------------------------------------------------
   ' OnMouseClick
   Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
      MyBase.OnMouseClick(e)
      Application.Exit()
   End Sub

   '----------------------------------------------------------------------------------------------
   ' OnMouseMove
   Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
      MyBase.OnMouseMove(e)
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
