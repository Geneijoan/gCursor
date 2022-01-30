Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports gCursorLib

Public Class Form1

#Region "Declarations"

    Private Source As Control

#Region "Example Properties"
    Private HotSpot As ContentAlignment = ContentAlignment.MiddleCenter
    Private TextAlign As ContentAlignment = ContentAlignment.MiddleCenter
    Private TextAutoFit As gCursor.eTextAutoFit = gCursor.eTextAutoFit.Height
    Private TextType As gCursor.eType = gCursor.eType.Both
    Private TextFill As gCursor.eTextFade = gCursor.eTextFade.Solid
    Private TextColor As Color = Color.Blue
    Private TextShadowColor As Color = Color.Black
    Private TextBoxColor As Color = Color.Blue
    Private TextBorderColor As Color = Color.Red
    Private ImageBoxColor As Color = Color.Orange
    Private ImageBorderColor As Color = Color.Black
    Private TextFont As New Font("Arial", 12)
#End Region 'Example Properties

#Region "Quotes"
    Const Quote1 As String = "Flying is learning how to throw yourself at the ground and miss. "
    Const Quote2 As String = "Some are born to move the world, to live their fantasies " & vbCr & _
                                "But most of us just dream about the things we'd like to be " & vbCr & _
                                "Sadder still to watch it die, then never to have known it " & vbCr & _
                                "For you, the blind who once could see " & vbCr & _
                                "The bell tolls for thee... "
    Const Quote3 As String = "You don't get something for nothing " & vbCr & _
                                "You can't have freedom for free. " & vbCr & _
                                "You won't get wise " & vbCr & _
                                "With the sleep still in your eyes " & vbCr & _
                                "No matter what your dream might be. "
#End Region 'Quotes

#Region "Scroll ListView"

    Private WithEvents ScrollTimer As New Timer
    Private scrollDirection As Integer
    Private Const WM_SCROLL As Integer = &H115S

    Private Declare Function SendMessage Lib "user32" Alias "SendMessageA" _
    (ByVal hwnd As Integer, _
     ByVal wMsg As Integer, _
     ByVal wParam As Integer, _
     ByRef lParam As Object) As Integer

#End Region 'Scroll ListView

#End Region 'Declarations

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        PictureBox1.AllowDrop = True
        RichTextBox2.AllowDrop = True

        UpdateBoxExample()
        RichTextBox1.Text = Quote1 & vbCr & vbCr & Quote2 & vbCr & vbCr & Quote3

        'Dim Dir As New IO.DirectoryInfo("C:\")
        'Dim DirItem As IO.DirectoryInfo
        'Dim TestOK As String
        'For Each DirItem In Dir.GetDirectories
        '    Try
        '        TestOK = DirItem.GetDirectories.Length.ToString
        '        Dim NewNode As New TreeNode(DirItem.Name, 0, 0)
        '        NewNode.Tag = DirItem
        '        TreeView1.Nodes.Add(NewNode)
        '    Catch ex As Exception
        '    End Try
        'Next
        UpdateAdjusters(GCursorLabel)

    End Sub

#Region "RichTextBox Drop"

    Private Sub Richtextbox2_DragDrop(ByVal sender As Object, ByVal e As _
      System.Windows.Forms.DragEventArgs) Handles RichTextBox2.DragDrop

        'Identify the Source Drag Control
        Label2.Text = "Drag Source = Type: " & Source.GetType.Name & "  Name: " & Source.Name

        'Identify type of data being drug and put it in the Drop Control 
        If e.Data.GetDataPresent(DataFormats.Rtf) Then
            RichTextBox2.Rtf() = e.Data().GetData(DataFormats.Rtf).ToString()
            If (e.KeyState And 8) = 8 Then
            Else
                DeleteText(RichTextBox1, RichTextBox1.SelectionStart, RichTextBox1.SelectionLength)
            End If

        ElseIf e.Data.GetDataPresent(DataFormats.Text) Then
            RichTextBox2.Text = e.Data().GetData(DataFormats.Text).ToString()

        ElseIf e.Data.GetDataPresent(DataFormats.Bitmap) Then
            SetClipboardImage(CType(e.Data().GetData(DataFormats.Bitmap), Image), 220, 180)
            RichTextBox2.Clear()
            RichTextBox2.Paste(DataFormats.GetFormat(DataFormats.Bitmap))

        ElseIf e.Data.GetDataPresent("System.Windows.Forms.ListViewItem") Then
            Dim li As ListViewItem = CType(e.Data().GetData("System.Windows.Forms.ListViewItem"), ListViewItem)
            SetClipboardImage(CType(li.ImageList.Images(li.ImageIndex), Image), 48, 48)
            RichTextBox2.Text = " " & li.Text
            RichTextBox2.Paste(DataFormats.GetFormat(DataFormats.Bitmap))

        ElseIf e.Data.GetDataPresent("System.Windows.Forms.TreeNode") Then
            Dim tn As TreeNode = CType(e.Data().GetData("System.Windows.Forms.TreeNode"), TreeNode)
            RichTextBox2.Text = "Folder '" & tn.Text & "' Contains: " & vbCr & vbCr & _
                "   " & CType(tn.Tag, IO.DirectoryInfo).GetDirectories.Length & " Folders" & vbCr & _
                "   " & CType(tn.Tag, IO.DirectoryInfo).GetFiles.Length & " Files"
            RichTextBox2.SelectionStart = tn.Text.Length + 23
            SetClipboardImage(CType(tn.TreeView.ImageList.Images(1), Image), 32, 32)
            RichTextBox2.Paste(DataFormats.GetFormat(DataFormats.Bitmap))
            RichTextBox2.SelectionStart = RichTextBox2.Text.LastIndexOf(" Folders") + 10
            SetClipboardImage(CType(tn.TreeView.ImageList.Images(2), Image), 32, 32)
            RichTextBox2.Paste(DataFormats.GetFormat(DataFormats.Bitmap))

            If (e.KeyState And 8) = 8 Then
            Else
                tn.Remove()
            End If
        End If

    End Sub

    Sub SetClipboardImage(ByVal img As Image, ByVal bitWidth As Integer, ByVal bitHeight As Integer)
        'Because of the Blue background problem 
        'Add a solid White background behind the Image
        Dim newImage As Image = New Bitmap(bitWidth, bitHeight, _
                                Imaging.PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(newImage)
            g.Clear(Color.White)
            g.DrawImage(img, 0, 0, bitWidth, bitHeight)
        End Using

        Clipboard.SetImage(CType(newImage.Clone, Image))
        newImage.Dispose()

    End Sub

    Private Sub Richtextbox2_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles RichTextBox2.DragOver

        If e.Data.GetDataPresent(DataFormats.Rtf) Or _
               e.Data.GetDataPresent("System.Windows.Forms.TreeNode") Then
            If (e.KeyState And 8) = 8 Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.Move
            End If

        ElseIf e.Data.GetDataPresent(DataFormats.Text) Or _
               e.Data.GetDataPresent(DataFormats.Bitmap) Or _
               e.Data.GetDataPresent("System.Windows.Forms.ListViewItem") Then

            e.Effect = DragDropEffects.Copy

        End If
    End Sub

#End Region 'RichTextBox Drop

#Region "Label"

    Private Sub Label1_GiveFeedback(ByVal sender As Object, ByVal e As System.Windows.Forms.GiveFeedbackEventArgs) Handles Label1.GiveFeedback
        e.UseDefaultCursors = False
        If ((e.Effect And DragDropEffects.Copy) = DragDropEffects.Copy) Then
            GCursorLabel.gEffect = gCursor.eEffect.Copy
        Else
            GCursorLabel.gEffect = gCursor.eEffect.No
        End If
        Cursor.Current = GCursorLabel.gCursor
    End Sub

    Private Sub Label1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Label1.MouseDown
        With GCursorLabel
            .gText = Label1.Text
            .gTextAutoFit = TextAutoFit
            .gHotSpot = HotSpot
            .gTextAlignment = TextAlign
            .gTTransp = CInt(TextBox1.Text)
            .gITransp = CInt(TextBox4.Text)
            .gTBTransp = CInt(TextBox3.Text)
            .gTextColor = TextColor
            .gTextShadowColor = TextShadowColor
            .gTextShadower.Font = .gFont
            .gTextShadower.OffsetXY(CSng(TrackBar4.Value * 0.1))
            .gTextShadower.Blur = CSng(TrackBar5.Value * 0.1)
            .gTextShadower.ShadowTransp = TrackBar6.Value
            .gTextBoxColor = TextBoxColor
            .gTextBorderColor = TextBorderColor
            .gShowTextBox = CheckBox1.Checked
            .gBlackBitBack = CheckBox2.Checked
            .gBoxShadow = CheckBox3.Checked
            .gTextShadow = CheckBox5.Checked
            .gTextFade = TextFill
            '.gFont = New Font("Times New Roman", 16, CType(FontStyle.Bold + FontStyle.Italic, FontStyle))
            .MakeCursor()
        End With
        Source = CType(sender, Control)
        Label1.DoDragDrop(Label1.Text, DragDropEffects.Copy)
    End Sub

#End Region 'Label

#Region "RichTextBox"

    Private Sub RichTextBox1_GiveFeedback(ByVal sender As Object, ByVal e As System.Windows.Forms.GiveFeedbackEventArgs) _
      Handles RichTextBox1.GiveFeedback
        e.UseDefaultCursors = False
        If ((e.Effect And DragDropEffects.Copy) = DragDropEffects.Copy) Then
            GCursorRichTB.gEffect = gCursor.eEffect.Copy
        ElseIf ((e.Effect And DragDropEffects.Move) = DragDropEffects.Move) Then
            GCursorRichTB.gEffect = gCursor.eEffect.Move
        Else
            GCursorRichTB.gEffect = gCursor.eEffect.No
        End If
        Cursor.Current = GCursorRichTB.gCursor
    End Sub

    Private Sub RichTextBox1_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles RichTextBox1.DragDrop
        If e.Data.GetDataPresent(DataFormats.Rtf) Then
            RichTextBox1.Rtf.Insert(RichTextBox1.GetCharIndexFromPosition(New Point(e.X, e.Y)), e.Data().GetData(DataFormats.Rtf).ToString())
            If (e.KeyState And 8) = 8 Then
            Else
                DeleteText(RichTextBox1, selStrt, selLeng)
            End If
        End If

    End Sub

    Private OverSelection As Boolean = False

    Private Sub RichTextBox1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles RichTextBox1.MouseMove
        Dim RTB As RichTextBox = CType(sender, RichTextBox)
        Dim CaretPos As Integer = RTB.GetCharIndexFromPosition(e.Location)
        If CaretPos >= RTB.SelectionStart _
        AndAlso CaretPos - RTB.SelectionStart < RTB.SelectionLength Then
            Cursor.Current = Cursors.Arrow
            OverSelection = True
        Else
            Cursor.Current = Cursors.IBeam
            OverSelection = False
        End If
    End Sub

    Private selStrt, selLeng As Integer

    Private Sub RichTextBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles RichTextBox1.MouseDown
        Dim RTB As RichTextBox = CType(sender, RichTextBox)
        If e.Button = Windows.Forms.MouseButtons.Left AndAlso OverSelection Then
            selStrt = RichTextBox1.SelectionStart
            selLeng = RichTextBox1.SelectionLength
            'GCursorLabel = New gCursor(RTB.SelectedText, 250, 75, 1)
            With GCursorRichTB
                .gText = RTB.SelectedText
                .gTextAutoFit = TextAutoFit
                .gHotSpot = HotSpot
                .gTextAlignment = TextAlign
                .gTextMultiline = True
                .gTTransp = CInt(TextBox1.Text)
                .gITransp = CInt(TextBox4.Text)
                .gTBTransp = CInt(TextBox3.Text)
                .gTextColor = TextColor
                .gTextShadowColor = TextShadowColor
                .gTextBoxColor = TextBoxColor
                .gTextBorderColor = TextBorderColor
                .gShowTextBox = CheckBox1.Checked
                .gBlackBitBack = CheckBox2.Checked
                .gBoxShadow = CheckBox3.Checked
                .gTextShadow = CheckBox5.Checked
                .gTextFade = TextFill
                .gTextShadower.Font = .gFont
                .gTextShadower.OffsetXY(CSng(TrackBar4.Value * 0.1))
                .gTextShadower.Blur = CSng(TrackBar5.Value * 0.1)
                .gTextShadower.ShadowTransp = TrackBar6.Value

                .MakeCursor()
            End With
            Dim da As New DataObject
            da.SetText(RTB.SelectedRtf, TextDataFormat.Rtf)

            Source = CType(sender, Control)
            RTB.DoDragDrop(da, CType(DragDropEffects.Copy + DragDropEffects.Move, DragDropEffects))
        End If
    End Sub

    Private Function DeleteText(ByVal rtb As System.Windows.Forms.RichTextBox, ByVal start As Integer, ByVal length As Integer) As Boolean
        Dim currentCaret As Integer = rtb.SelectionStart ' use to reset position
        rtb.Select(start, length) ' select the text
        rtb.SelectedText = String.Empty ' set it to nothing
        Dim success As Boolean
        If rtb.SelectedText <> String.Empty Then ' if for any reason it could not be changed
            success = False
        Else
            success = True
        End If
        rtb.SelectionStart = currentCaret ' move the caret back to the current position
        Return success
    End Function

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        RichTextBox1.Text = Quote1 & vbCr & vbCr & Quote2 & vbCr & vbCr & Quote3
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        RichTextBox1.SelectAll()
    End Sub

#End Region 'RichTextBox

#Region "PictureBox"

    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        GCursorPicBox = New GCursor(CType(PictureBox1.Image.Clone, Bitmap), 120, 100, CInt(TextBox4.Text))
        With GCursorPicBox
            .GHotSpot = HotSpot
            .GBlackBitBack = CheckBox2.Checked
            .GBoxShadow = CheckBox3.Checked
            .GShowImageBox = CheckBox4.Checked
            .GImageBorderColor = ImageBorderColor
            .GImageBoxColor = ImageBoxColor
            .GIBTransp = CInt(TextBox7.Text)
            .MakeCursor()
        End With
        Dim da As New DataObject
        da.SetImage(CType(PictureBox1.Image.Clone, Image))

        Source = CType(sender, Control)
        PictureBox1.DoDragDrop(da, DragDropEffects.Copy)
    End Sub

    Private Sub PictureBox1_GiveFeedback(ByVal sender As Object, ByVal e As System.Windows.Forms.GiveFeedbackEventArgs) Handles PictureBox1.GiveFeedback
        e.UseDefaultCursors = False
        If ((e.Effect And DragDropEffects.Copy) = DragDropEffects.Copy) Then
            GCursorPicBox.gEffect = gCursor.eEffect.Copy
        Else
            GCursorPicBox.gEffect = gCursor.eEffect.No
        End If
        Cursor.Current = GCursorPicBox.gCursor

    End Sub
#End Region 'PictureBox

#Region "ListView"

    Private Sub ListView1_GiveFeedback(ByVal sender As Object, ByVal e As System.Windows.Forms.GiveFeedbackEventArgs) Handles ListView1.GiveFeedback
        e.UseDefaultCursors = False
        If ((e.Effect And DragDropEffects.Copy) = DragDropEffects.Copy) Then
            GCursorListView.gEffect = gCursor.eEffect.Copy
        ElseIf ((e.Effect And DragDropEffects.Move) = DragDropEffects.Move) Then
            GCursorListView.gEffect = gCursor.eEffect.Move
        Else
            GCursorListView.gEffect = gCursor.eEffect.No
        End If
        Cursor.Current = GCursorListView.gCursor
    End Sub

    Private Sub ListView1_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles ListView1.ItemDrag
        GCursorListView = New gCursor(ListView1.SelectedItems(0), 100, 30, CInt(TextBox4.Text), CInt(TextBox7.Text), _
                                 50, 50, CInt(TextBox1.Text), CInt(TextBox3.Text))
        With GCursorListView
            .gFont = TextFont
            .gText = ListView1.SelectedItems(0).Text
            .gTextAutoFit = TextAutoFit
            .gHotSpot = HotSpot
            .gTextMultiline = False
            .gTextAlignment = TextAlign
            '.gITransp = CInt(TextBox4.Text)
            '.gIBTransp = CInt(TextBox7.Text)
            '.gTTransp = CInt(TextBox1.Text)
            '.gTBTransp = CInt(TextBox3.Text)
            .gTextColor = TextColor
            .gTextShadower.Font = .gFont
            .gTextShadowColor = TextShadowColor
            .gTextShadower.OffsetXY(CSng(TrackBar4.Value * 0.1))
            .gTextShadower.Blur = CSng(TrackBar5.Value * 0.1)
            .gTextShadower.ShadowTransp = TrackBar6.Value
            .gTextBoxColor = TextBoxColor
            .gTextBorderColor = TextBorderColor
            .gImageBorderColor = ImageBorderColor
            .gImageBoxColor = ImageBoxColor
            .gShowTextBox = CheckBox1.Checked
            .gBlackBitBack = CheckBox2.Checked
            .gShowImageBox = CheckBox4.Checked
            .gBoxShadow = CheckBox3.Checked
            .gTextShadow = CheckBox5.Checked
            .gTextFade = TextFill
            .MakeCursor()
        End With

        Source = CType(sender, Control)
        ListView1.DoDragDrop(ListView1.SelectedItems(0), DragDropEffects.Copy)
    End Sub

#Region "Scroll ListView"

    Private Sub ListView1_DragOver(ByVal sender As Object, _
      ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView1.DragOver

        If e.Data.GetDataPresent("System.Windows.Forms.ListViewItem", False) Then
            Dim Mpt As Point = ListView1.PointToClient(New Point(e.X, e.Y))
            If Mpt.Y <= ListView1.Font.Height \ 2 Then
                'If the Cursor is close to the top,
                'set for scrolling Up and start the timer
                scrollDirection = 0
                ScrollTimer.Start()
                GCursorListView.gScrolling = gCursor.eScrolling.ScrollUp
                e.Effect = DragDropEffects.None

            ElseIf Mpt.Y >= ListView1.ClientSize.Height - ListView1.Font.Height Then
                'If the Cursor is close to the bottom,
                'set for scrolling Down and start the timer
                scrollDirection = 1
                ScrollTimer.Start()
                GCursorListView.gScrolling = gCursor.eScrolling.ScrollDn
                e.Effect = DragDropEffects.None
            Else
                ScrollTimer.Stop()
                GCursorListView.gScrolling = gCursor.eScrolling.No
            End If
        End If

    End Sub

    Private Sub ScrollTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles ScrollTimer.Tick
        Try
            'Speed up the scroll if cursor moves further from the list
            If GCursorListView.gScrolling = gCursor.eScrolling.ScrollDn Then
                ScrollTimer.Interval = 300 - (10 * _
                  (_ListView1.PointToClient(MousePosition).Y - ListView1.ClientSize.Height))
            ElseIf GCursorListView.gScrolling = gCursor.eScrolling.ScrollUp Then
                ScrollTimer.Interval = 300 + (10 * _
                  (ListView1.PointToClient(MousePosition).Y - (ListView1.Font.Height \ 2)))
            End If
        Catch ex As Exception
        End Try

        If MouseButtons <> Windows.Forms.MouseButtons.Left Or _
            ListView1.PointToClient(MousePosition).Y >= ListView1.ClientSize.Height + 30 Or _
            ListView1.PointToClient(MousePosition).Y <= (ListView1.Font.Height \ 2) - 30 Or _
            ListView1.PointToClient(MousePosition).X <= 0 Or _
            ListView1.PointToClient(MousePosition).X >= ListView1.ClientSize.Width _
        Then
            ScrollTimer.Stop()
            GCursorListView.gScrolling = gCursor.eScrolling.No
            GCursorListView.MakeCursor()
        Else
            ScrollControl(CType(ListView1, ListView), scrollDirection)
        End If

    End Sub

    Private Sub ScrollControl(ByRef objControl As Control, ByRef intDirection As Integer)
        SendMessage(objControl.Handle.ToInt32, WM_SCROLL, intDirection, VariantType.Null)
    End Sub

#End Region 'Scroll ListView

#End Region 'ListView

#Region "TreeView"

    Private Sub TreeView1_GiveFeedback(ByVal sender As Object, ByVal e As System.Windows.Forms.GiveFeedbackEventArgs) Handles TreeView1.GiveFeedback
        e.UseDefaultCursors = False
        If ((e.Effect And DragDropEffects.Copy) = DragDropEffects.Copy) Then
            GCursorTreeView.gEffect = gCursor.eEffect.Copy
        ElseIf ((e.Effect And DragDropEffects.Move) = DragDropEffects.Move) Then
            GCursorTreeView.gEffect = gCursor.eEffect.Move
        Else
            GCursorTreeView.gEffect = gCursor.eEffect.No
        End If
        Cursor.Current = GCursorTreeView.gCursor
    End Sub


    Private Sub TreeView1_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles TreeView1.ItemDrag
        Dim tNode As TreeNode = CType(e.Item, TreeNode)
        GCursorTreeView = New gCursor(tNode, 100, 24, CInt(TextBox4.Text), CInt(TextBox7.Text), 24, 24, CInt(TextBox1.Text), CInt(TextBox3.Text))
        With GCursorTreeView
            .gFont = TextFont
            .gTextAutoFit = TextAutoFit
            .gHotSpot = HotSpot
            .gTextMultiline = False
            .gTextAlignment = TextAlign
            .gTextColor = TextColor
            .gTextShadower.Font = .gFont
            .gTextShadowColor = TextShadowColor
            .gTextShadower.OffsetXY(CSng(TrackBar4.Value * 0.1))
            .gTextShadower.Blur = CSng(TrackBar5.Value * 0.1)
            .gTextShadower.ShadowTransp = TrackBar6.Value
            .gTextBoxColor = TextBoxColor
            .gTextBorderColor = TextBorderColor
            .gImageBorderColor = ImageBorderColor
            .gImageBoxColor = ImageBoxColor
            .gShowTextBox = CheckBox1.Checked
            .gBlackBitBack = CheckBox2.Checked
            .gShowImageBox = CheckBox4.Checked
            .gBoxShadow = CheckBox3.Checked
            .gTextShadow = CheckBox5.Checked
            .gTextFade = TextFill
            .MakeCursor()
        End With

        Source = CType(sender, Control)
        TreeView1.DoDragDrop(tNode, CType(DragDropEffects.Copy + DragDropEffects.Move, DragDropEffects))

    End Sub


#End Region 'TreeView

#Region "Adjust Properties"

    Private Sub RadioButton5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles RadioButton5.CheckedChanged
        Try
            'TextType = CType([Enum].Parse(GetType(gCursor.eType), Currrbut.Text), gCursor.eType)
            TextType = GCursor.EType.Text
            UpdateBoxExample()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub RadioButton6_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles RadioButton6.CheckedChanged
        Try
            'TextType = CType([Enum].Parse(GetType(gCursor.eType), Currrbut.Text), gCursor.eType)
            TextType = GCursor.EType.Picture
            UpdateBoxExample()
        Catch ex As Exception

        End Try

    End Sub


    Private Sub RadioButton7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles RadioButton7.CheckedChanged
        Try
            'TextType = CType([Enum].Parse(GetType(gCursor.eType), Currrbut.Text), gCursor.eType)
            TextType = GCursor.EType.Both
            UpdateBoxExample()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles RadioButton1.CheckedChanged, RadioButton2.CheckedChanged, RadioButton3.CheckedChanged, RadioButton4.CheckedChanged
        Try
            'TextAutoFit = CType([Enum].Parse(GetType(gCursor.eTextAutoFit), Currrbut.Text), gCursor.eTextAutoFit)
            TextAutoFit = GCursor.ETextAutoFit.None
            UpdateBoxExample()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles RadioButton2.CheckedChanged
        Try
            'TextAutoFit = CType([Enum].Parse(GetType(gCursor.eTextAutoFit), Currrbut.Text), gCursor.eTextAutoFit)
            TextAutoFit = GCursor.ETextAutoFit.All
            UpdateBoxExample()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles RadioButton3.CheckedChanged
        Try
            'TextAutoFit = CType([Enum].Parse(GetType(gCursor.eTextAutoFit), Currrbut.Text), gCursor.eTextAutoFit)
            TextAutoFit = GCursor.ETextAutoFit.Width
            UpdateBoxExample()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub RadioButton4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles RadioButton4.CheckedChanged
        Try
            'TextAutoFit = CType([Enum].Parse(GetType(gCursor.eTextAutoFit), Currrbut.Text), gCursor.eTextAutoFit)
            TextAutoFit = GCursor.ETextAutoFit.Height
            UpdateBoxExample()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub RadioButton9_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles RadioButton9.CheckedChanged
        Dim Currrbut As RadioButton = CType(sender, RadioButton)
        If Currrbut.Checked Then
            Try
                'TextFill = CType([Enum].Parse(GetType(gCursor.eTextFade), Currrbut.Text), gCursor.eTextFade)
                TextFill = gCursor.eTextFade.Linear
                UpdateBoxExample()
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub RadioButton10_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles RadioButton10.CheckedChanged
        Dim Currrbut As RadioButton = CType(sender, RadioButton)
        If Currrbut.Checked Then
            Try
                'TextFill = CType([Enum].Parse(GetType(gCursor.eTextFade), Currrbut.Text), gCursor.eTextFade)
                TextFill = gCursor.eTextFade.Path
                UpdateBoxExample()
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub RadioButton11_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles RadioButton11.CheckedChanged
        Dim Currrbut As RadioButton = CType(sender, RadioButton)
        If Currrbut.Checked Then
            Try
                'TextFill = CType([Enum].Parse(GetType(gCursor.eTextFade), Currrbut.Text), gCursor.eTextFade)
                TextFill = gCursor.eTextFade.Solid
                UpdateBoxExample()
            Catch ex As Exception
            End Try
        End If
    End Sub


    Private Sub TopLeft_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TopLeft.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                'HotSpot = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name), ContentAlignment)
                HotSpot = ContentAlignment.TopLeft
                UpdateBoxExample()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MiddleLeft_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MiddleLeft.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                'HotSpot = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name), ContentAlignment)
                HotSpot = ContentAlignment.MiddleLeft
                UpdateBoxExample()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub BottomLeft_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles BottomLeft.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                'HotSpot = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name), ContentAlignment)
                HotSpot = ContentAlignment.BottomLeft
                UpdateBoxExample()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub TopCenter_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TopCenter.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                'HotSpot = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name), ContentAlignment)
                HotSpot = ContentAlignment.TopCenter
                UpdateBoxExample()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MiddleCenter_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MiddleCenter.CheckedChanged
        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                'HotSpot = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name), ContentAlignment)
                HotSpot = ContentAlignment.MiddleCenter
                UpdateBoxExample()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub TopRight_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles TopRight.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                'HotSpot = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name), ContentAlignment)
                HotSpot = ContentAlignment.TopRight
                UpdateBoxExample()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub BottomCenter_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles BottomCenter.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                'HotSpot = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name), ContentAlignment)
                HotSpot = ContentAlignment.BottomCenter
                UpdateBoxExample()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MiddleRight_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MiddleRight.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                'HotSpot = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name), ContentAlignment)
                HotSpot = ContentAlignment.MiddleRight
                UpdateBoxExample()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub BottomRight_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles BottomRight.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                'HotSpot = CType([Enum].Parse(GetType(ContentAlignment), Currrbut.Name), ContentAlignment)
                HotSpot = ContentAlignment.BottomRight
                UpdateBoxExample()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub NearCenterFar_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles Near.CheckedChanged, Center.CheckedChanged, Far.CheckedChanged

        Try
            Dim Currrbut As RadioButton = CType(sender, RadioButton)
            If Currrbut.Checked = True Then
                Select Case Currrbut.Name
                    Case "Near"
                        TextAlign = ContentAlignment.MiddleLeft
                    Case "Center"
                        TextAlign = ContentAlignment.MiddleCenter
                    Case "Far"
                        TextAlign = ContentAlignment.MiddleRight
                End Select
                UpdateBoxExample()
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

        UpdateBoxExample()

    End Sub

    Private Sub CheckBox2_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles CheckBox2.Click, CheckBox3.Click, CheckBox1.Click, _
        CheckBox4.Click, CheckBox5.Click, CheckBox6.Click, CheckBox7.Click
        UpdateBoxExample()

    End Sub

    Private Sub TrackBar1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar1.ValueChanged
        TextBox1.Text = TrackBar1.Value.ToString
        UpdateBoxExample()
    End Sub

    Private Sub TrackBar2_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar2.ValueChanged
        TextBox3.Text = TrackBar2.Value.ToString
        UpdateBoxExample()
    End Sub

    Private Sub TrackBar3_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar3.ValueChanged
        TextBox4.Text = TrackBar3.Value.ToString
        UpdateBoxExample()
    End Sub

    Private Sub TrackBar7_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar7.ValueChanged
        TextBox7.Text = TrackBar7.Value.ToString
        UpdateBoxExample()

    End Sub

    Private Sub TrackBar4_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles TrackBar4.ValueChanged
        TextBox5.Text = CStr(TrackBar4.Value * 0.1)
        UpdateBoxExample()

    End Sub

    Private Sub TrackBar6_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar6.ValueChanged
        TextBox6.Text = CStr(TrackBar6.Value)
        UpdateBoxExample()

    End Sub

    Private Sub TrackBar5_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrackBar5.ValueChanged
        TextBox2.Text = CStr(TrackBar5.Value * 0.1)
        UpdateBoxExample()

    End Sub

#End Region 'Adjust Properties

#Region "UpdateBoxExample"

    Sub UpdateBoxExample()
        Dim bm As New Bitmap(Panel2.Width, Panel2.Height, PixelFormat.Format32bppPArgb)
        Dim cur As New GCursor
        With cur

            Select Case TextType
                Case gCursor.eType.Text
                    .gTextBox = New Size(Panel2.Width - 23, Panel2.Height - 34)
                Case gCursor.eType.Both
                    .gTextBox = New Size(Panel2.Width - 72, Panel2.Height - 34)
            End Select

            .gFont = TextFont
            .gHotSpot = HotSpot
            .gImageBox = New Size(40, 50)
            .gImage = CType(PictureBox2.Image, Bitmap)
            .gText = "Example" & vbCrLf & "With" & vbCrLf & "Multiline"

            .gType = TextType
            .gTextAutoFit = TextAutoFit
            .gTextAlignment = TextAlign
            .gTTransp = CInt(IIf(TextBox1.Text <> "", TextBox1.Text, "0"))
            .gTBTransp = CInt(IIf(TextBox3.Text <> "", TextBox3.Text, "0"))
            .gITransp = CInt(IIf(TextBox4.Text <> "", TextBox4.Text, "0"))
            .gIBTransp = CInt(IIf(TextBox7.Text <> "", TextBox7.Text, "0"))
            .gTextColor = TextColor
            .gTextShadowColor = TextShadowColor
            .gTextShadower.OffsetXY(CSng(TrackBar4.Value * 0.1))
            .gTextShadower.Blur = CSng(TrackBar5.Value * 0.1)
            .gTextShadower.ShadowTransp = TrackBar6.Value
            .gTextBoxColor = TextBoxColor
            .gTextBorderColor = TextBorderColor
            .gImageBorderColor = ImageBorderColor
            .gImageBoxColor = ImageBoxColor
            .gShowTextBox = CheckBox1.Checked
            .gBlackBitBack = CheckBox2.Checked
            .gBoxShadow = CheckBox3.Checked
            .gShowImageBox = CheckBox4.Checked
            .gTextShadow = CheckBox5.Checked
            .gTextFade = TextFill
            .gTextMultiline = CheckBox7.Checked
            If CheckBox6.Checked Then
                .gEffect = gCursor.eEffect.Move
            Else
                .MakeCursor(False)
            End If
        End With

        Using g As Graphics = Graphics.FromImage(bm)
            g.DrawImage(cur.gCursorImage, 0, 0)
        End Using
        Panel2.BackgroundImage = CType(bm.Clone, Bitmap)

        bm.Dispose()

    End Sub


    Sub UpdateAdjusters(ByRef cur As gCursor)

        With cur

            TextFont = .gFont
            Select Case .gType
                Case gCursor.eType.Text
                    RadioButton5.Checked = True
                Case gCursor.eType.Picture
                    RadioButton6.Checked = True
                Case gCursor.eType.Both
                    RadioButton7.Checked = True
            End Select

            CType(GroupBox2.Controls([Enum].GetName(GetType(ContentAlignment), .gHotSpot)), RadioButton).Checked = True

            '.gImageBox = New Size(40, 50)
            '.gImage = CType(PictureBox2.Image, Bitmap)
            '.gText = "Example" & vbCrLf & "With" & vbCrLf & "Multiline"

            Select Case .gTextAutoFit
                Case gCursor.eTextAutoFit.None
                    RadioButton1.Checked = True
                Case gCursor.eTextAutoFit.All
                    RadioButton4.Checked = True
                Case gCursor.eTextAutoFit.Width
                    RadioButton2.Checked = True
                Case gCursor.eTextAutoFit.Height
                    RadioButton3.Checked = True
            End Select

            Select Case .gTextAlignment
                Case ContentAlignment.BottomLeft, ContentAlignment.MiddleLeft, ContentAlignment.TopLeft
                    Near.Checked = True
                Case ContentAlignment.BottomCenter, ContentAlignment.MiddleCenter, ContentAlignment.TopCenter
                    Center.Checked = True
                Case ContentAlignment.BottomRight, ContentAlignment.MiddleRight, ContentAlignment.TopRight
                    Far.Checked = True
            End Select

            TrackBar1.Value = .gTTransp ' = CInt(TextBox1.Text)
            TrackBar2.Value = .gTBTransp ' = CInt(TextBox3.Text)
            TrackBar3.Value = .gITransp ' = CInt(TextBox4.Text)
            TrackBar7.Value = .gIBTransp ' = CInt(TextBox7.Text)

            TextColor = .gTextColor
            TextShadowColor = .gTextShadowColor
            TrackBar4.Value = .gTextShadower.Offset.X / 0.1
            TrackBar5.Value = .gTextShadower.Blur / 0.1
            TrackBar6.Value = .gTextShadower.ShadowTransp
            TextBoxColor = .gTextBoxColor
            TextBorderColor = .gTextBorderColor
            ImageBorderColor = .gImageBorderColor
            ImageBoxColor = .gImageBoxColor
            CheckBox1.Checked = .gShowTextBox
            CheckBox2.Checked = .gBlackBitBack
            CheckBox3.Checked = .gBoxShadow
            CheckBox4.Checked = .gShowImageBox
            CheckBox5.Checked = .gTextShadow


            Select Case .gTextFade
                Case gCursor.eTextFade.Solid
                    RadioButton11.Checked = True
                Case gCursor.eTextFade.Linear
                    RadioButton9.Checked = True
                Case gCursor.eTextFade.Path
                    RadioButton10.Checked = True
            End Select

            CheckBox7.Checked = .gTextMultiline
            .MakeCursor(CheckBox6.Checked)
            UpdateBoxExample()
        End With

        'Using g As Graphics = Graphics.FromImage(bm)
        '    g.DrawImage(cur.gCursorImage, 0, 0)
        'End Using
        'Panel2.BackgroundImage = CType(bm.Clone, Bitmap)

        'bm.Dispose()

    End Sub

#End Region 'UpdateBoxExample

#Region "ScreenShot"

    'Because the RichTextBox Drop event uses the Clpboard don't
    'actually Drop on it or you will replace the ScreenShot

    Private Function FormScreenShot() As Bitmap

        Dim pt As Point
        Using FormImage As New Bitmap(Me.Size.Width, Me.Size.Height)
            Using g As Graphics = Graphics.FromImage(FormImage)

                g.CopyFromScreen(Me.Location, New Point(0, 0), Me.Size)

                If MouseButtons = Windows.Forms.MouseButtons.Left Then
                    'Get the Custom Cursor
                    If GCursorLabel.GCursorImage IsNot Nothing Then
                        pt = PointToClient(Point.Subtract(MousePosition,
                                           CType(GCursorLabel.GCursor.HotSpot, Size)))
                        g.DrawImage(GCursorLabel.GCursorImage, pt.X + 4, pt.Y + 30)
                    End If
                Else
                    'Get the Normal Cursor
                    pt = PointToClient(Point.Subtract(MousePosition,
                                       CType(Cursor.Current.HotSpot, Size)))
                    Cursor.Current.Draw(g, New Rectangle(pt.X + 4, pt.Y + 30,
                            Cursor.Current.Size.Width, Cursor.Current.Size.Height))

                End If
            End Using
            Return CType(FormImage.Clone, Bitmap)
        End Using

    End Function

    Dim ScreenShot As Integer = 0
    Private WithEvents Timer1 As New Timer
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        If ScreenShot < 0 Then
            Button1.Visible = False
            Button4.Visible = False
            Me.Refresh()
            Clipboard.Clear()
            Clipboard.SetImage(FormScreenShot)
            Button1.Text = "Get Screen Shot"
            Button1.Visible = True
            Button4.Visible = True
            Timer1.Stop()
            ShowClipboard("This is the Screen Shot currently on the Clipboard")
        Else
            Button1.Text = ScreenShot.ToString
            ScreenShot -= 1
        End If

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ScreenShot = 5
        Timer1.Interval = 500
        Timer1.Start()
    End Sub

    'This is to get a copy of just the gCursor Image
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        GCursorLabel.gEffect = gCursor.eEffect.Move
        Using CursorImage As New Bitmap(GCursorLabel.GCursorImage.Size.Width,
            GCursorLabel.GCursorImage.Size.Height),
            g As Graphics = Graphics.FromImage(CursorImage)

            g.Clear(Color.White)
            If GCursorLabel.GCursorImage IsNot Nothing Then g.DrawImage(GCursorLabel.GCursorImage, 0, 0)
            Clipboard.SetImage(CursorImage)
        End Using
        ShowClipboard("Current Cursor Image on the Clipboard")
    End Sub

    Private Sub ShowClipboard(ByVal Text As String)
        Using frm As New Form
            frm.Text = Text
            frm.BackColor = Color.White
            frm.BackgroundImageLayout = ImageLayout.Zoom
            frm.BackgroundImage = Clipboard.GetImage
            frm.ClientSize = frm.BackgroundImage.Size
            frm.ShowDialog()
        End Using
    End Sub

#End Region 'ScreenShot

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged

        Select Case TabControl1.SelectedIndex
            Case 0
                UpdateAdjusters(GCursorLabel)
            Case 1
                UpdateAdjusters(GCursorRichTB)
            Case 2
                UpdateAdjusters(GCursorPicBox)
            Case 3
                UpdateAdjusters(GCursorListView)
            Case 4
                UpdateAdjusters(GCursorTreeView)
        End Select
    End Sub

End Class

