<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmCfg
   Inherits System.Windows.Forms.Form

   'Form overrides dispose to clean up the component list.
   <System.Diagnostics.DebuggerNonUserCode()>
   Protected Overrides Sub Dispose(ByVal disposing As Boolean)
      Try
         If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
         End If
      Finally
         MyBase.Dispose(disposing)
      End Try
   End Sub

   'Required by the Windows Form Designer
   Private components As System.ComponentModel.IContainer

   'NOTE: The following procedure is required by the Windows Form Designer
   'It can be modified using the Windows Form Designer.  
   'Do not modify it using the code editor.
   <System.Diagnostics.DebuggerStepThrough()>
   Private Sub InitializeComponent()
      grpCharSet = New GroupBox()
      chkBoxKatakana = New CheckBox()
      chkBoxCapsLetter = New CheckBox()
      chkBoxSmallLetter = New CheckBox()
      chkBoxNumbers = New CheckBox()
      cmbBoxColor = New ComboBox()
      lblColor = New Label()
      StatusStrip = New StatusStrip()
      ssStatusLabelInfo = New ToolStripStatusLabel()
      btnOk = New Button()
      grpCharSet.SuspendLayout()
      StatusStrip.SuspendLayout()
      SuspendLayout()
      ' 
      ' grpCharSet
      ' 
      grpCharSet.Controls.Add(chkBoxKatakana)
      grpCharSet.Controls.Add(chkBoxCapsLetter)
      grpCharSet.Controls.Add(chkBoxSmallLetter)
      grpCharSet.Controls.Add(chkBoxNumbers)
      grpCharSet.Location = New Point(12, 12)
      grpCharSet.Name = "grpCharSet"
      grpCharSet.Size = New Size(93, 123)
      grpCharSet.TabIndex = 1
      grpCharSet.TabStop = False
      grpCharSet.Text = "Char set"
      ' 
      ' chkBoxKatakana
      ' 
      chkBoxKatakana.AutoSize = True
      chkBoxKatakana.Location = New Point(17, 97)
      chkBoxKatakana.Name = "chkBoxKatakana"
      chkBoxKatakana.Size = New Size(74, 19)
      chkBoxKatakana.TabIndex = 4
      chkBoxKatakana.Text = "Katakana"
      chkBoxKatakana.UseVisualStyleBackColor = True
      ' 
      ' chkBoxCapsLetter
      ' 
      chkBoxCapsLetter.AutoSize = True
      chkBoxCapsLetter.Location = New Point(17, 72)
      chkBoxCapsLetter.Name = "chkBoxCapsLetter"
      chkBoxCapsLetter.Size = New Size(46, 19)
      chkBoxCapsLetter.TabIndex = 3
      chkBoxCapsLetter.Text = "A-Z"
      chkBoxCapsLetter.UseVisualStyleBackColor = True
      ' 
      ' chkBoxSmallLetter
      ' 
      chkBoxSmallLetter.AutoSize = True
      chkBoxSmallLetter.Location = New Point(17, 47)
      chkBoxSmallLetter.Name = "chkBoxSmallLetter"
      chkBoxSmallLetter.Size = New Size(42, 19)
      chkBoxSmallLetter.TabIndex = 2
      chkBoxSmallLetter.Text = "a-z"
      chkBoxSmallLetter.UseVisualStyleBackColor = True
      ' 
      ' chkBoxNumbers
      ' 
      chkBoxNumbers.AutoSize = True
      chkBoxNumbers.Location = New Point(17, 22)
      chkBoxNumbers.Name = "chkBoxNumbers"
      chkBoxNumbers.Size = New Size(43, 19)
      chkBoxNumbers.TabIndex = 1
      chkBoxNumbers.Text = "0-9"
      chkBoxNumbers.UseVisualStyleBackColor = True
      ' 
      ' cmbBoxColor
      ' 
      cmbBoxColor.FormattingEnabled = True
      cmbBoxColor.Location = New Point(169, 30)
      cmbBoxColor.Name = "cmbBoxColor"
      cmbBoxColor.Size = New Size(121, 23)
      cmbBoxColor.TabIndex = 2
      ' 
      ' lblColor
      ' 
      lblColor.AutoSize = True
      lblColor.Location = New Point(111, 35)
      lblColor.Name = "lblColor"
      lblColor.Size = New Size(39, 15)
      lblColor.TabIndex = 3
      lblColor.Text = "Color:"
      ' 
      ' StatusStrip
      ' 
      StatusStrip.Items.AddRange(New ToolStripItem() {ssStatusLabelInfo})
      StatusStrip.Location = New Point(0, 189)
      StatusStrip.Name = "StatusStrip"
      StatusStrip.Size = New Size(359, 22)
      StatusStrip.TabIndex = 4
      StatusStrip.Text = "StatusStrip1"
      ' 
      ' ssStatusLabelInfo
      ' 
      ssStatusLabelInfo.Name = "ssStatusLabelInfo"
      ssStatusLabelInfo.Size = New Size(106, 17)
      ssStatusLabelInfo.Text = "Matrix ScreenSaver"
      ' 
      ' btnOk
      ' 
      btnOk.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
      btnOk.Location = New Point(314, 163)
      btnOk.Name = "btnOk"
      btnOk.Size = New Size(40, 23)
      btnOk.TabIndex = 5
      btnOk.Text = "O&k"
      btnOk.UseVisualStyleBackColor = True
      ' 
      ' frmCfg
      ' 
      AutoScaleDimensions = New SizeF(7F, 15F)
      AutoScaleMode = AutoScaleMode.Font
      ClientSize = New Size(359, 211)
      Controls.Add(btnOk)
      Controls.Add(StatusStrip)
      Controls.Add(lblColor)
      Controls.Add(cmbBoxColor)
      Controls.Add(grpCharSet)
      FormBorderStyle = FormBorderStyle.FixedDialog
      MaximizeBox = False
      MinimizeBox = False
      Name = "frmCfg"
      ShowInTaskbar = False
      StartPosition = FormStartPosition.CenterScreen
      Text = "Matrix ScreenSaver Settings"
      grpCharSet.ResumeLayout(False)
      grpCharSet.PerformLayout()
      StatusStrip.ResumeLayout(False)
      StatusStrip.PerformLayout()
      ResumeLayout(False)
      PerformLayout()
   End Sub

   Friend WithEvents grpCharSet As GroupBox
   Friend WithEvents chkBoxNumbers As CheckBox
   Friend WithEvents chkBoxSmallLetter As CheckBox
   Friend WithEvents chkBoxCapsLetter As CheckBox
   Friend WithEvents chkBoxKatakana As CheckBox
   Friend WithEvents cmbBoxColor As ComboBox
   Friend WithEvents lblColor As Label
   Friend WithEvents StatusStrip As StatusStrip
   Friend WithEvents ssStatusLabelInfo As ToolStripStatusLabel
   Friend WithEvents btnOk As Button
End Class
