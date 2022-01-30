
Public Class GCursorUIEditor

#Region "Declarations"

    Public gCursor1 As New GCursor

#Region "Example Properties"
    Public TextFont As Font
    Public HotSpot As ContentAlignment = ContentAlignment.MiddleCenter
    Public TextAlign As ContentAlignment = ContentAlignment.TopCenter
    Public TextAutoFit As GCursor.ETextAutoFit = GCursor.ETextAutoFit.Height
    Public TextType As GCursor.EType = GCursor.EType.Both
    Public TextFill As GCursor.ETextFade = GCursor.ETextFade.Solid
    Public TextColor As Color = Color.Blue
    Public TextShadowColor As Color = Color.Black
    Public TextBoxColor As Color = Color.Blue
    Public TextBorderColor As Color = Color.Red
    Public ImageBoxColor As Color = Color.Orange
    Public ImageBorderColor As Color = Color.Black
    Public sf As New StringFormat
#End Region 'Example Properties

#End Region 'Declarations

#Region "Adjust Properties"

    Private Sub RadioButton7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles rbutBoth.CheckedChanged, rbutPicture.CheckedChanged, rbutText.CheckedChanged
        Dim Currrbut As RadioButton = CType(sender, RadioButton)
        Try
            TextType = CType([Enum].Parse(GetType(GCursor.EType), Currrbut.Text), GCursor.EType)
            UpdateExample()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub TextAutoFit_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles rbutNone.CheckedChanged, rbutAll.CheckedChanged, rbutWidth.CheckedChanged, rbutHeight.CheckedChanged
        Dim Currrbut As RadioButton = CType(sender, RadioButton)
        Try
            TextAutoFit = CType([Enum].Parse(GetType(GCursor.ETextAutoFit), Currrbut.Text), GCursor.ETextAutoFit)
            UpdateExample()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TextFill_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles Linear.CheckedChanged, Path.CheckedChanged, Solid.CheckedChanged
        Dim Currrbut As RadioButton = CType(sender, RadioButton)
        If Currrbut.Checked Then
            Try
                TextFill = CType([Enum].Parse(GetType(GCursor.ETextFade), Currrbut.Text), GCursor.ETextFade)
                UpdateExample()
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub HotSpot_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TopLeft.CheckedChanged, MiddleLeft.CheckedChanged, BottomLeft.CheckedChanged,
            TopCenter.CheckedChanged, MiddleCenter.CheckedChanged, TopRight.CheckedChanged,
            BottomCenter.CheckedChanged, MiddleRight.CheckedChanged, BottomRight.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                HotSpot = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name), ContentAlignment)
                UpdateExample()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ButTextColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles butTextColor.Click, butTextBoxColor.Click, butTextBorder.Click,
        butImageBoxColor.Click, butImageBorderColor.Click, butTextShadowColor.Click
        Select Case CType(sender, Button).Name
            Case "butTextColor"
                ColorDialog1.Color = TextColor
                ColorDialog1.ShowDialog()
                TextColor = ColorDialog1.Color

            Case "butTextBoxColor"
                ColorDialog1.Color = TextBoxColor
                ColorDialog1.ShowDialog()
                TextBoxColor = ColorDialog1.Color

            Case "butTextBorder"
                ColorDialog1.Color = TextBorderColor
                ColorDialog1.ShowDialog()
                TextBorderColor = ColorDialog1.Color

            Case "butImageBoxColor"
                ColorDialog1.Color = ImageBoxColor
                ColorDialog1.ShowDialog()
                ImageBoxColor = ColorDialog1.Color

            Case "butImageBorderColor"
                ColorDialog1.Color = ImageBorderColor
                ColorDialog1.ShowDialog()
                ImageBorderColor = ColorDialog1.Color

            Case "butTextShadowColor"
                ColorDialog1.Color = TextShadowColor
                ColorDialog1.ShowDialog()
                TextShadowColor = ColorDialog1.Color

        End Select

        UpdateExample()

    End Sub

    Private Sub Options_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles chkBitBlkBack.Click, chkShowBoxShadows.Click, chkShowTextBox.Click, chkShowImageBox.Click, chkMultiline.Click
        UpdateExample()

    End Sub

    Private Sub TbarTextTransp_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbarTextTransp.Scroll
        txbTextTransp.Text = tbarTextTransp.Value.ToString
        UpdateExample()
    End Sub

    Private Sub TbarTextBoxTransp_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbarTextBoxTransp.Scroll
        txbTextBoxTransp.Text = tbarTextBoxTransp.Value.ToString
        UpdateExample()
    End Sub

    Private Sub TbarImageTransp_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbarImageTransp.Scroll
        txbImageTransp.Text = tbarImageTransp.Value.ToString
        UpdateExample()
    End Sub

    Private Sub TbarImageBoxTransp_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbarImageBoxTransp.Scroll
        txbImageBoxTransp.Text = tbarImageBoxTransp.Value.ToString
        UpdateExample()

    End Sub

    Private Sub TbarOffset_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles tbarOffset.Scroll
        txbOffset.Text = CStr(tbarOffset.Value * 0.1)
        UpdateExample()

    End Sub

    Private Sub TbarShadowTransp_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbarShadowTransp.Scroll
        txbShadowTransp.Text = CStr(tbarShadowTransp.Value)
        UpdateExample()

    End Sub

    Private Sub TbarBlur_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbarBlur.Scroll
        txbBlur.Text = CStr(tbarBlur.Value * 0.1)
        UpdateExample()

    End Sub

    Private Sub GCursorUIEditor_FormClosing(ByVal sender As Object,
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub GCursorUIEditor_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        UpdateExample()
    End Sub

    Private Sub ChkShowTextShadow_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkShowTextShadow.CheckedChanged
        UpdateExample()

    End Sub

    Private Sub ButFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butFont.Click
        FontDialog1.Font = TextFont
        FontDialog1.ShowDialog()
        TextFont = FontDialog1.Font

        UpdateExample()
    End Sub

    Private Sub ButLoadImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butLoadImage.Click
        OpenFileDialog1.Multiselect = False
        OpenFileDialog1.ShowDialog()
        Try
            Using bm As New Bitmap(OpenFileDialog1.FileName)
                picImage.Image = CType(bm.Clone, Image)
            End Using
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try


        UpdateExample()
    End Sub

    Private Sub TbarTextBoxWidth_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbarTextBoxWidth.Scroll
        txbTextBoxWidth.Text = CStr(tbarTextBoxWidth.Value)
        UpdateExample()
    End Sub

    Private Sub TbarTextBoxHeight_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbarTextBoxHeight.Scroll
        txbTextBoxHeight.Text = CStr(tbarTextBoxHeight.Value)
        UpdateExample()
    End Sub

    Dim OrigSize As Size
    Dim curvalue As Integer

    Private Sub TbarImageBoxWidth_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tbarImageBoxWidth.MouseDown
        OrigSize = New Size(tbarImageBoxWidth.Value, tbarImageBoxHeight.Value)
    End Sub

    Private Sub TbarImageBoxWidth_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbarImageBoxWidth.Scroll

        If chkAspRatio.Checked Then
            If OrigSize.Width = 0 Then Exit Sub

            Dim ratio As Single = CSng(tbarImageBoxWidth.Value / OrigSize.Width)
            If (OrigSize.Height * ratio) > tbarImageBoxHeight.Maximum _
              Or (OrigSize.Height * ratio) < tbarImageBoxHeight.Minimum Then
                tbarImageBoxWidth.Value = curvalue
            Else
                txbImageBoxWidth.Text = CStr(tbarImageBoxWidth.Value)
                tbarImageBoxHeight.Value = CInt(OrigSize.Height * ratio)
                txbImageBoxHeight.Text = CStr(tbarImageBoxHeight.Value)
                curvalue = tbarImageBoxWidth.Value
                UpdateExample()
            End If
        Else
            txbImageBoxWidth.Text = CStr(tbarImageBoxWidth.Value)
            UpdateExample()
        End If


    End Sub

    Private Sub TbarImageBoxHeight_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbarImageBoxHeight.Scroll
        txbImageBoxHeight.Text = CStr(tbarImageBoxHeight.Value)
        UpdateExample()
    End Sub

    Private Sub ButSizeFromImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butSizeFromImage.Click
        If picImage.Image.Width > tbarImageBoxWidth.Maximum Then
            tbarImageBoxWidth.Value = tbarImageBoxWidth.Maximum
        Else
            tbarImageBoxWidth.Value = picImage.Image.Width
        End If
        If picImage.Image.Height > tbarImageBoxHeight.Maximum Then
            tbarImageBoxHeight.Value = tbarImageBoxHeight.Maximum
        Else
            tbarImageBoxHeight.Value = picImage.Image.Height
        End If
        txbImageBoxWidth.Text = CStr(tbarImageBoxWidth.Value)
        txbImageBoxHeight.Text = CStr(tbarImageBoxHeight.Value)
        curvalue = tbarImageBoxWidth.Value
        UpdateExample()

    End Sub

    Private Sub Panel2_GiveFeedback(ByVal sender As Object, ByVal e As System.Windows.Forms.GiveFeedbackEventArgs) Handles Panel2.GiveFeedback
        e.UseDefaultCursors = False

        gCursor1.gEffect = GCursor.EEffect.Copy

        Cursor.Current = gCursor1.GCursor
    End Sub

    Private Sub Panel2_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel2.MouseMove
        If gCursor1.gText <> txbText.Text Then UpdateExample()
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Dim HS As Point = gCursor1.GetHotSpot
            HS.X += Panel2.Location.X + Panel3.Location.X + 1
            HS.Y += Panel2.Location.Y + Panel3.Location.Y + 1
            Cursor.Position = PointToScreen(HS)

            Panel2.DoDragDrop(txbText.Text, DragDropEffects.Copy)

        End If
    End Sub

    Private Sub TextAlign_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles AlignTopRight.CheckedChanged, AlignTopCenter.CheckedChanged, AlignBottomCenter.CheckedChanged,
        AlignBottomLeft.CheckedChanged, AlignMiddleRight.CheckedChanged, AlignMiddleLeft.CheckedChanged,
        AlignBottomRight.CheckedChanged, AlignTopLeft.CheckedChanged, AlignMiddleCenter.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                TextAlign = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name.Replace("Align", "")), ContentAlignment)
                UpdateExample()
            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Sub TxbText_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txbText.LostFocus
        If gCursor1.GText <> txbText.Text Then UpdateExample()
    End Sub

    Private Sub ChkAspRatio_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAspRatio.CheckedChanged
        tbarImageBoxHeight.Enabled = Not chkAspRatio.Checked
    End Sub

    Private Sub ChkGrayBack_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkGrayBack.CheckedChanged
        If chkGrayBack.Checked Then
            Panel2.BackColor = Color.Gray
        Else
            Panel2.BackColor = Color.White
        End If
    End Sub

#End Region 'Adjust Properties

#Region "UpdateExample"

    Sub UpdateExample()
        Dim bm As New Bitmap(Panel2.Width, Panel2.Height)
        With gCursor1

            .gFont = TextFont
            .gTextBox = New Size(tbarTextBoxWidth.Value, tbarTextBoxHeight.Value)
            .gHotSpot = HotSpot
            .gImageBox = New Size(tbarImageBoxWidth.Value, tbarImageBoxHeight.Value)
            If Not IsNothing(picImage.Image) Then
                .gImage = CType(picImage.Image, Bitmap)
                lblImageWidth.Text = "Width =" & .gImage.Size.Width.ToString
                lblImageHeight.Text = "Height =" & .gImage.Size.Height.ToString
            End If
            .gText = txbText.Text
            .gType = TextType
            .gTextAutoFit = TextAutoFit
            .gTextAlignment = TextAlign
            .gTTransp = CInt(txbTextTransp.Text)
            .gTBTransp = CInt(txbTextBoxTransp.Text)
            .gITransp = CInt(txbImageTransp.Text)
            .gIBTransp = CInt(txbImageBoxTransp.Text)
            .gTextColor = TextColor
            .gTextShadowColor = TextShadowColor
            .gTextShadower.OffsetXY(CSng(tbarOffset.Value * 0.1))
            .gTextShadower.Blur = CSng(tbarBlur.Value * 0.1)
            .gTextShadower.ShadowTransp = tbarShadowTransp.Value
            .gTextBoxColor = TextBoxColor
            .gTextBorderColor = TextBorderColor
            .gImageBorderColor = ImageBorderColor
            .gImageBoxColor = ImageBoxColor
            .gShowTextBox = chkShowTextBox.Checked
            .GBlackBitBack = chkBitBlkBack.Checked
            .GBoxShadow = chkShowBoxShadows.Checked
            .gShowImageBox = chkShowImageBox.Checked
            .gTextShadow = chkShowTextShadow.Checked
            .gTextMultiline = chkMultiline.Checked
            .gTextFade = TextFill
            .gEffect = GCursor.EEffect.Move
            .MakeCursor()
        End With

        Using g As Graphics = Graphics.FromImage(bm)
            g.DrawImage(gCursor1.GCursorImage, 0, 0)
        End Using
        Panel2.BackgroundImage = CType(bm.Clone, Bitmap)
        bm.Dispose()

    End Sub

#End Region 'UpdateBoxExample

End Class

