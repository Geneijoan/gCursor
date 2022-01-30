Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.ComponentModel

#Region "Version"

'Version 1.0 February 2009
'Version 1.1 March 2009
'   Added TextShadower
'Version 1.2 March 2009
'   Fixed some layout problems
'Version 1.3 March 2009
'   Fixed some Text Alignment problem
'   Added separate Transparency for Image Box
'   Changed the Property Font to gFont
'Version 1.4 March 2009
'   Turned the Class into a Component
'   Added a Property Editor in the design environment

#End Region 'Version 1.4

<ToolboxItem(True), ToolboxBitmap(GetType(gCursor), "gCursor.gCursor.bmp")>
<Designer(GetType(gCursorDesigner))>
Public Class GCursor

    Private ReadOnly CurNo As New Cursor(New System.IO.MemoryStream(My.Resources.No))
    Private ReadOnly CurMove As New Cursor(New System.IO.MemoryStream(My.Resources.Move))
    Private ReadOnly CurCopy As New Cursor(New System.IO.MemoryStream(My.Resources.Copy))
    Private ReadOnly sf As New StringFormat

#Region "CreateIconIndirect"

    Private Structure IconInfo
        Public fIcon As Boolean
        Public xHotspot As Int32
        Public yHotspot As Int32
        Public hbmMask As IntPtr
        Public hbmColor As IntPtr
    End Structure

    <DllImport("user32.dll", EntryPoint:="CreateIconIndirect")>
    Private Shared Function CreateIconIndirect(ByVal iconInfo As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Shared Function DestroyIcon(ByVal handle As IntPtr) As Boolean
    End Function

    <DllImport("gdi32.dll")>
    Public Shared Function DeleteObject(ByVal hObject As IntPtr) As Boolean
    End Function

    Public Function CreateCursor(ByVal bmp As Bitmap) As Cursor

        'Setup the Cursors IconInfo
        Dim tmp As New IconInfo With {
            .xHotspot = _gHotSpotPt.X,
            .yHotspot = _gHotSpotPt.Y,
            .fIcon = False
        }
        If _gBlackBitBack Then
            tmp.hbmMask = bmp.GetHbitmap(Color.FromArgb(0, 0, 0, 0))
            tmp.hbmColor = bmp.GetHbitmap(Color.FromArgb(0, 0, 0, 0))
        Else
            tmp.hbmMask = bmp.GetHbitmap()
            tmp.hbmColor = bmp.GetHbitmap()
        End If

        'Create the Pointer for the Cursor Icon
        Dim pnt As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(tmp))
        Marshal.StructureToPtr(tmp, pnt, True)
        Dim curPtr As IntPtr = CreateIconIndirect(pnt)

        'Save the image of the cursor with the _gBlackBitBack effect
        'Not really needed for normal use.
        'I use it to create a screen shot with the gCursor included
        _gCursorImage = Icon.FromHandle(curPtr).ToBitmap



        'Clean Up
        DestroyIcon(pnt)
        DeleteObject(tmp.hbmMask)
        DeleteObject(tmp.hbmColor)

        Return New Cursor(curPtr)
    End Function

#End Region 'CreateIconIndirect

#Region "New"

    Public Sub New()
        MakeCursor()
    End Sub

    Public Sub New(ByVal Image As Bitmap, ByVal width As Integer, ByVal height As Integer,
        Optional ByVal ImageTransparency As Integer = 0, Optional ByVal ImageBoxTransparency As Integer = 80)
        _gImage = Image
        _gType = EType.Picture
        _gImageBox.Height = height
        _gImageBox.Width = width
        GITransp = ImageTransparency
        GIBTransp = ImageBoxTransparency
        MakeCursor()
    End Sub

    Public Sub New(ByVal Text As String, ByVal width As Integer, ByVal height As Integer, Optional ByVal TextTransparency As Integer = 0)
        _gText = Text
        _gType = EType.Text
        _gTextBox.Height = height
        _gTextBox.Width = width
        GTTransp = TextTransparency
        MakeCursor()
    End Sub

    Public Sub New(ByVal Image As Bitmap, ByVal imgwidth As Integer, ByVal imgheight As Integer, ByVal Text As String, ByVal txtwidth As Integer, ByVal txtheight As Integer, Optional ByVal ImageTransparency As Integer = 0, Optional ByVal TextTransparency As Integer = 0)
        _gImage = Image
        _gText = Text
        _gType = EType.Both
        _gImageBox.Height = imgheight
        _gImageBox.Width = imgwidth
        _gTextBox.Height = txtheight
        _gTextBox.Width = txtwidth
        GITransp = ImageTransparency
        GTTransp = TextTransparency
        MakeCursor()
    End Sub

    Public Sub New(ByVal lstItm As ListViewItem, ByVal txtwidth As Integer, ByVal txtheight As Integer, Optional ByVal ImageTransparency As Integer = 0, Optional ByVal ImageBoxTransparency As Integer = 80, Optional ByVal imgwidth As Integer = 50, Optional ByVal imgheight As Integer = 50, Optional ByVal TextTransparency As Integer = 0, Optional ByVal TextBoxTransparency As Integer = 80)
        If lstItm.ImageList IsNot Nothing Then
            _gImage = CType(lstItm.ImageList.Images(lstItm.ImageIndex), Bitmap)
            _gType = EType.Both
            _gImageBox.Height = imgheight
            _gImageBox.Width = imgwidth
            GITransp = ImageTransparency
            GIBTransp = ImageBoxTransparency
        Else
            _gType = EType.Text
        End If
        _gTextBox.Height = txtheight
        _gTextBox.Width = txtwidth
        GTTransp = TextTransparency
        GTBTransp = TextBoxTransparency
        _gText = lstItm.Text
        MakeCursor()
    End Sub

    Public Sub New(ByVal trvItm As TreeNode, ByVal txtwidth As Integer, ByVal txtheight As Integer, Optional ByVal ImageTransparency As Integer = 0, Optional ByVal ImageBoxTransparency As Integer = 80, Optional ByVal imgwidth As Integer = 50, Optional ByVal imgheight As Integer = 50, Optional ByVal TextTransparency As Integer = 100, Optional ByVal TextBoxTransparency As Integer = 100)
        If trvItm.TreeView.ImageList IsNot Nothing Then
            _gImage = CType(trvItm.TreeView.ImageList.Images(trvItm.SelectedImageIndex), Bitmap)
            _gType = EType.Both
            _gImageBox.Height = imgheight
            _gImageBox.Width = imgwidth
            GITransp = ImageTransparency
            GIBTransp = ImageBoxTransparency
        Else
            _gType = EType.Text
        End If
        _gTextBox.Height = txtheight
        _gTextBox.Width = txtwidth
        GTTransp = TextTransparency
        GTBTransp = TextBoxTransparency
        _gText = trvItm.Text
        MakeCursor()
    End Sub

#End Region 'New

#Region "Properties"

#Region "Cursor Props"

    Private _gCursor As Cursor = Cursors.Default
    <Browsable(False)>
    <System.ComponentModel.DesignerSerializationVisibility(
    System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property GCursor() As Cursor
        Get
            Return _gCursor
        End Get
        Set(ByVal Value As Cursor)
            _gCursor = Value
        End Set
    End Property

    Private _gCursorImage As Bitmap
    'The True Image of the Displayed Cursor
    <Browsable(False)>
    Public Property GCursorImage() As Bitmap
        Get
            Return _gCursorImage
        End Get
        Set(ByVal Value As Bitmap)
            _gCursorImage = Value
        End Set
    End Property

    Enum EEffect
        No
        Move
        Copy
    End Enum
    Private _gEffect As EEffect = EEffect.No
    'What Drag Effect to display
    <Browsable(False)>
    Public Property GEffect() As EEffect
        Get
            Return _gEffect
        End Get
        Set(ByVal Value As EEffect)
            If _gEffect <> Value Then
                _gEffect = Value
                MakeCursor()
            End If
        End Set
    End Property

    Enum EScrolling
        No
        ScrollUp
        ScrollDn
        ScrollLeft
        ScrollRight
    End Enum

    Private _gScrolling As EScrolling = EScrolling.No
    '
    <Description("What Drag Effect to display")>
    <Category("Appearance gCursor")>
    Public Property GScrolling() As EScrolling
        Get
            Return _gScrolling
        End Get
        Set(ByVal Value As EScrolling)
            _gScrolling = Value
        End Set
    End Property

    Enum EType
        Text
        Picture
        Both
    End Enum
    Private _gType As EType = EType.Text
    '
    <Description("What kind of gCursor Text Only, Picture Only, or Both")>
    <Category("Appearance gCursor")>
    Public Property GType() As EType
        Get
            Return _gType
        End Get
        Set(ByVal Value As EType)
            _gType = Value
        End Set
    End Property

    Private _gBlackBitBack As Boolean = False
    '
    <Description("The pesky Background ghost when using transparency >0 and <255. True gives a Black Tint and False gives a Blue Tint")>
    <Category("Appearance gCursor")>
    Public Property GBlackBitBack() As Boolean
        Get
            Return _gBlackBitBack
        End Get
        Set(ByVal Value As Boolean)
            _gBlackBitBack = Value
        End Set
    End Property

    Private _gBoxShadow As Boolean = True
    '
    <Description("Show Shadow behind Boxes")>
    <Category("Appearance gCursor")>
    Public Property GBoxShadow() As Boolean
        Get
            Return _gBoxShadow
        End Get
        Set(ByVal Value As Boolean)
            _gBoxShadow = Value
        End Set
    End Property

    Private _gHotSpotPt As New Point(0, 0)
    Private _gHotSpot As ContentAlignment = ContentAlignment.MiddleCenter
    '
    <Description("HotSpot location on the gCursor")>
    <Category("Appearance gCursor")>
    Public Property GHotSpot() As ContentAlignment
        Get
            Return _gHotSpot
        End Get
        Set(ByVal Value As ContentAlignment)
            _gHotSpot = Value
        End Set
    End Property

#End Region 'Cursor Props

#Region "Image"

    Private _gImage As Bitmap = Nothing
    '
    <Description("Picture to use in the gCursor")>
    <Category("Appearance gCursor Image")>
    Public Property GImage() As Bitmap
        Get
            Return _gImage
        End Get
        Set(ByVal Value As Bitmap)
            If Not IsNothing(Value) Then _gImage = Value
        End Set
    End Property

    Private _gImageBox As New Size(75, 56)
    '
    <Description("Size of the Box to display around the Picture")>
    <Category("Appearance gCursor Image")>
    Public Property GImageBox() As Size
        Get
            Return _gImageBox
        End Get
        Set(ByVal Value As Size)
            _gImageBox = Value
        End Set
    End Property

    Private _gShowImageBox As Boolean = False
    '
    <Description("Show or Not Show the Box around the Picture")>
    <Category("Appearance gCursor Image")>
    Public Property GShowImageBox() As Boolean
        Get
            Return _gShowImageBox
        End Get
        Set(ByVal Value As Boolean)
            _gShowImageBox = Value
        End Set
    End Property

    Private _gImageBoxColor As Color = Color.White
    '
    <Description("Background color for the Image Box")>
    <Category("Appearance gCursor Image")>
    Public Property GImageBoxColor() As Color
        Get
            Return _gImageBoxColor
        End Get
        Set(ByVal Value As Color)
            _gImageBoxColor = Value
        End Set
    End Property

    Private _gImageBorderColor As Color = Color.Black
    '
    <Description("Color for the Border around the Image Box")>
    <Category("Appearance gCursor Image")>
    Public Property GImageBorderColor() As Color
        Get
            Return _gImageBorderColor
        End Get
        Set(ByVal Value As Color)
            _gImageBorderColor = Value
        End Set
    End Property

    Private _gImageTransp As Integer = 255
    Private _gITransp As Integer = 0
    '
    <Description("Transparency Percentage value for the Picture. Converts and puts value in _gImageTransp to 0-255 value")>
    <Category("Appearance gCursor Image")>
    Public Property GITransp() As Integer
        Get
            Return _gITransp
        End Get
        Set(ByVal Value As Integer)
            If Value > 100 Then Value = 100
            If Value < 0 Then Value = 0
            _gITransp = Value
            _gImageTransp = CInt(0.01F * (100 - Value) * 255)
        End Set
    End Property

    Private _gImageBoxTransp As Integer = 255
    Private _gIBTransp As Integer = 80
    '
    <Description("Transparency Percentage value for the Picture Box. Converts and puts value in _gImageBoxTransp to 0-255 value")>
    <Category("Appearance gCursor Image")>
    Public Property GIBTransp() As Integer
        Get
            Return _gIBTransp
        End Get
        Set(ByVal Value As Integer)
            If Value > 100 Then Value = 100
            If Value < 0 Then Value = 0
            _gIBTransp = Value
            _gImageBoxTransp = CInt(0.01F * (100 - Value) * 255)
        End Set
    End Property

#End Region 'Image

#Region "Text"
    Private _gTextBoxArea As New Size(100, 30)
    Private _gTextBox As New Size(100, 30)
    '
    <Description("Size of box around Text")>
    <Category("Appearance gCursor Text")>
    Public Property GTextBox() As Size
        Get
            Return _gTextBox
        End Get
        Set(ByVal Value As Size)
            _gTextBox = Value
        End Set
    End Property

    Private _gTextTransp As Integer = 255
    Private _gTTransp As Integer = 0
    '
    <Description("Transparency Percentage value for the Text. Converts and puts value in _gTextTransp to 0-255 value")>
    <Category("Appearance gCursor Text")>
    Public Property GTTransp() As Integer
        Get
            Return _gTTransp
        End Get
        Set(ByVal Value As Integer)
            If Value > 100 Then Value = 100
            If Value < 0 Then Value = 0
            _gTTransp = Value
            _gTextTransp = CInt(0.01F * (100 - Value) * 255)
        End Set
    End Property

    Private _gTextBoxTransp As Integer = 255
    Private _gTBTransp As Integer = 80
    '
    <Description("Transparency Percentage value for the Text Box. Converts and puts value in _gTextBoxTransp to 0-255 value")>
    <Category("Appearance gCursor Text")>
    Public Property GTBTransp() As Integer
        Get
            Return _gTBTransp
        End Get
        Set(ByVal Value As Integer)
            If Value > 100 Then Value = 100
            If Value < 0 Then Value = 0
            _gTBTransp = Value
            _gTextBoxTransp = CInt(0.01F * (100 - Value) * 255)
        End Set
    End Property

    Private _gShowTextBox As Boolean = False
    '
    <Description("Show or Not Show the Box around the Text")>
    <Category("Appearance gCursor Text")>
    Public Property GShowTextBox() As Boolean
        Get
            Return _gShowTextBox
        End Get
        Set(ByVal Value As Boolean)
            _gShowTextBox = Value
        End Set
    End Property

    Private _gTextMultiline As Boolean = False
    '
    <Description("Allow Multiline Text")>
    <Category("Appearance gCursor Text")>
    Public Property GTextMultiline() As Boolean
        Get
            Return _gTextMultiline
        End Get
        Set(ByVal Value As Boolean)
            _gTextMultiline = Value
        End Set
    End Property

    Enum ETextAutoFit
        None
        Width
        Height
        All
    End Enum
    Private _gTextAutoFit As ETextAutoFit = ETextAutoFit.None
    '
    <Description("Auto Fit the text to the chosen parameter")>
    <Category("Appearance gCursor Text")>
    Public Property GTextAutoFit() As ETextAutoFit
        Get
            Return _gTextAutoFit
        End Get
        Set(ByVal Value As ETextAutoFit)
            _gTextAutoFit = Value
        End Set
    End Property

    Private _gText As String = ""
    '
    <Description("Text String Value")>
    <Category("Appearance gCursor Text")>
    <Editor(GetType(Design.MultilineStringEditor), GetType(Drawing.Design.UITypeEditor))>
    Public Property GText() As String
        Get
            Return _gText
        End Get
        Set(ByVal Value As String)
            _gText = Value
        End Set
    End Property

    Private _gTextColor As Color = Color.Blue
    '
    <Description("Color of the Text")>
    <Category("Appearance gCursor Text")>
    Public Property GTextColor() As Color
        Get
            Return _gTextColor
        End Get
        Set(ByVal Value As Color)
            _gTextColor = Value
        End Set
    End Property

    Private _gTextShadowColor As Color = Color.Black
    '
    <Description("Color of the Text")>
    <Category("Appearance gCursor Text")>
    Public Property GTextShadowColor() As Color
        Get
            Return _gTextShadowColor
        End Get
        Set(ByVal Value As Color)
            _gTextShadowColor = Value
        End Set
    End Property

    Private _gTextShadower As New TextShadower
    '
    <Description("Show or Not Show the Text Shadow")>
    <Category("Appearance gCursor Text")>
    <Browsable(False)>
    Public Property GTextShadower() As TextShadower
        Get
            Return _gTextShadower
        End Get
        Set(ByVal Value As TextShadower)
            _gTextShadower = Value
        End Set
    End Property

    Private _gTextShadow As Boolean = False
    '
    <Description("Show or Not Show the Text Shadow")>
    <Category("Appearance gCursor Text")>
    Public Property GTextShadow() As Boolean
        Get
            Return _gTextShadow
        End Get
        Set(ByVal Value As Boolean)
            _gTextShadow = Value
        End Set
    End Property

    Private _gTextBoxColor As Color = Color.Blue
    '
    <Description("Background Color of the Text Box")>
    <Category("Appearance gCursor Text")>
    Public Property GTextBoxColor() As Color
        Get
            Return _gTextBoxColor
        End Get
        Set(ByVal Value As Color)
            _gTextBoxColor = Value
        End Set
    End Property

    Private _gTextBorderColor As Color = Color.Red
    '
    <Description("Color of the Border around the Text Box")>
    <Category("Appearance gCursor Text")>
    Public Property GTextBorderColor() As Color
        Get
            Return _gTextBorderColor
        End Get
        Set(ByVal Value As Color)
            _gTextBorderColor = Value
        End Set
    End Property

    Private _gTextAlignment As ContentAlignment = ContentAlignment.TopCenter
    '
    <Description("Horizontal Text Alignment in the Text Box")>
    <Category("Appearance gCursor Text")>
    Public Property GTextAlignment() As ContentAlignment
        Get
            Return _gTextAlignment
        End Get
        Set(ByVal Value As ContentAlignment)
            _gTextAlignment = Value
            Select Case Value
                Case ContentAlignment.BottomCenter, ContentAlignment.BottomLeft, ContentAlignment.BottomRight
                    sf.LineAlignment = StringAlignment.Far
                Case ContentAlignment.MiddleCenter, ContentAlignment.MiddleLeft, ContentAlignment.MiddleRight
                    sf.LineAlignment = StringAlignment.Center
                Case ContentAlignment.TopCenter, ContentAlignment.TopLeft, ContentAlignment.TopRight
                    sf.LineAlignment = StringAlignment.Near
            End Select
            Select Case Value
                Case ContentAlignment.BottomRight, ContentAlignment.MiddleRight, ContentAlignment.TopRight
                    sf.Alignment = StringAlignment.Far
                Case ContentAlignment.BottomCenter, ContentAlignment.MiddleCenter, ContentAlignment.TopCenter
                    sf.Alignment = StringAlignment.Center
                Case ContentAlignment.BottomLeft, ContentAlignment.MiddleLeft, ContentAlignment.TopLeft
                    sf.Alignment = StringAlignment.Near
            End Select
        End Set
    End Property

    Enum ETextFade
        Solid
        Linear
        Path
    End Enum
    Private _gTextFade As ETextFade = ETextFade.Solid
    '
    <Description("Brush type to fade Text")>
    <Category("Appearance gCursor Text")>
    Public Property GTextFade() As ETextFade
        Get
            Return _gTextFade
        End Get
        Set(ByVal Value As ETextFade)
            _gTextFade = Value
        End Set
    End Property

    Private _gFont As New Font("Arial", 10, FontStyle.Bold)
    '
    <Description("Font for the Text")>
    <Category("Appearance gCursor Text")>
    Public Property GFont() As Font
        Get
            Return _gFont
        End Get
        Set(ByVal Value As Font)
            _gFont = Value
            _gTextShadower.Font = Value
        End Set
    End Property

#End Region 'Text

#End Region 'Properties

#Region "Cursor"

#Region "Building Cursor"

    Public Sub MakeCursor(Optional ByVal addEffect As Boolean = True)
        'Set the TextBox Size
        SetTextBox()

        'Set the size of the gCursor
        Dim cWidth As Integer
        Dim cHeight As Integer
        Select Case _gType
            Case EType.Picture
                cWidth = CInt(_gImageBox.Width + 5)
                cHeight = CInt(_gImageBox.Height + 5)

            Case EType.Text
                cWidth = CInt(_gTextBoxArea.Width + 6)
                cHeight = CInt(_gTextBoxArea.Height + 6)

            Case EType.Both
                cWidth = CInt(_gImageBox.Width + _gTextBoxArea.Width + 16)
                cHeight = CInt(Math.Max(_gImageBox.Height + 6, _gTextBoxArea.Height + 6))

        End Select

        'Set the Position of the gCursor HotSpot
        SetHotSpot(cWidth, cHeight)

        'Draw the gCursor
        Dim bm As New Bitmap(cWidth + 32, cHeight + 32, PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(bm)

            Select Case _gType
                Case EType.Picture

                    'Draw the Image Box Shadow 
                    If _gBoxShadow And _gShowImageBox Then AddShadow(g, New Point(0, 0), New Size(cWidth, cHeight), False)

                    'Draw the Picture
                    DrawImage(g, CInt(_gImageBox.Width), CInt(_gImageBox.Height))

                Case EType.Text

                    'Draw the Text Box
                    If _gShowTextBox Then

                        'Draw the Text Box Shadow 
                        If _gBoxShadow Then AddShadow(g, New Point(0, 0), _gTextBoxArea)

                        'Draw the Text Box
                        DrawTextBox(g)
                    End If

                    'Draw the Text String
                    DrawText(g)

                Case EType.Both

                    'Draw the Image Box Shadow 
                    If _gBoxShadow And _gShowImageBox Then
                        AddShadow(g, New Point(0, 0), New Size(CInt(_gImageBox.Width + 5), CInt(_gImageBox.Height + 4)), False)
                    End If

                    'Draw the Picture
                    DrawImage(g, CInt(_gImageBox.Width), CInt(_gImageBox.Height))

                    'Draw the Text Box
                    If _gShowTextBox Then

                        'Draw the Text Box Shadow 
                        If _gBoxShadow Then AddShadow(g, New Point(_gImageBox.Width + 10, 0), _gTextBoxArea)

                        'Draw the Text Box
                        DrawTextBox(g, CInt(_gImageBox.Width + 10), 0)
                    End If

                    'Draw the Text String
                    DrawText(g, CInt(_gImageBox.Width + 10), 0)

            End Select

            'Draw the whole thing to the gCursor
            g.DrawImage(bm, 0, 0)

            'Add the image of the Effect Cursor to the gCursor Image
            If addEffect Then
                Dim EffectCursor As Cursor = Cursors.Default
                Select Case GScrolling
                    Case EScrolling.No
                        Select Case _gEffect
                            Case EEffect.No
                                EffectCursor = CurNo
                            Case EEffect.Move
                                EffectCursor = CurMove
                            Case EEffect.Copy
                                EffectCursor = CurCopy
                        End Select
                    Case EScrolling.ScrollDn
                        EffectCursor = Cursors.PanSouth
                    Case EScrolling.ScrollUp
                        EffectCursor = Cursors.PanNorth
                    Case EScrolling.ScrollLeft
                        EffectCursor = Cursors.PanWest
                    Case EScrolling.ScrollRight
                        EffectCursor = Cursors.PanEast

                End Select

                EffectCursor.Draw(g, New Rectangle(_gHotSpotPt.X, _gHotSpotPt.Y,
                    EffectCursor.Size.Width, EffectCursor.Size.Height))

            End If

            'Create the New Cursor
            GCursor = CreateCursor(bm)
        End Using

        bm.Dispose()
    End Sub

    Private Sub SetTextBox()
        Dim bm As New Bitmap(1000, 1000)
        sf.Trimming = StringTrimming.EllipsisCharacter
        If Not _gTextMultiline Then
            sf.FormatFlags = StringFormatFlags.NoWrap
        Else
            sf.FormatFlags = Nothing

        End If

        Using g As Graphics = Graphics.FromImage(bm)
            Dim gHeight As Integer
            If _gTextMultiline Then
                If _gTextAutoFit = ETextAutoFit.Height Then
                    gHeight = CInt(g.MeasureString(_gText, GFont, CInt(_gTextBox.Width)).Height)
                Else
                    gHeight = CInt(g.MeasureString(_gText, GFont).Height)
                End If
            Else
                gHeight = GFont.Height
            End If
            Select Case _gTextAutoFit
                Case ETextAutoFit.Height
                    _gTextBoxArea = New Size(_gTextBox.Width, gHeight)
                Case ETextAutoFit.Width
                    _gTextBoxArea = New Size(CInt(g.MeasureString(_gText, GFont).Width + 1), _gTextBox.Height)
                Case ETextAutoFit.All
                    _gTextBoxArea = New Size(CInt(g.MeasureString(_gText, GFont).Width + 1), gHeight)
                Case ETextAutoFit.None
                    _gTextBoxArea = New Size(_gTextBox.Width, _gTextBox.Height)
            End Select

            If _gTextShadow Then
                _gTextBoxArea.Width = CInt(_gTextBoxArea.Width + (_gTextShadower.Offset.X * _gTextShadower.Blur))
                _gTextBoxArea.Height = CInt(_gTextBoxArea.Height + (_gTextShadower.Offset.Y * _gTextShadower.Blur))
            End If
        End Using
        bm.Dispose()
    End Sub

    Public Function GetHotSpot() As Point
        Return _gHotSpotPt
    End Function

    Public Sub SetHotSpot(ByVal cWidth As Integer, ByVal cHeight As Integer)

        Select Case _gHotSpot
            Case ContentAlignment.BottomCenter
                _gHotSpotPt = New Point(CInt(cWidth / 2), cHeight)
            Case ContentAlignment.BottomLeft
                _gHotSpotPt = New Point(0, cHeight)
            Case ContentAlignment.BottomRight
                _gHotSpotPt = New Point(cWidth, cHeight)
            Case ContentAlignment.MiddleCenter
                _gHotSpotPt = New Point(CInt(cWidth / 2), CInt(cHeight / 2))
            Case ContentAlignment.MiddleLeft
                _gHotSpotPt = New Point(0, CInt(cHeight / 2))
            Case ContentAlignment.MiddleRight
                _gHotSpotPt = New Point(cWidth, CInt(cHeight / 2))
            Case ContentAlignment.TopCenter
                _gHotSpotPt = New Point(CInt(cWidth / 2), 0)
            Case ContentAlignment.TopLeft
                _gHotSpotPt = New Point(0, 0)
            Case ContentAlignment.TopRight
                _gHotSpotPt = New Point(cWidth, 0)
        End Select

    End Sub

#End Region 'Building Cursor

#Region "Drawing"

    Private Function ImageTransp() As Bitmap

        'Use a ColorMatrix to create a Transparent Image
        Dim bm As New Bitmap(_gImage.Width, _gImage.Height)
        Dim ia As New ImageAttributes()
        Dim cm As New ColorMatrix With {
            .Matrix33 = CSng(_gImageTransp / 255)
        }
        ia.SetColorMatrix(cm)
        Using g As Graphics = Graphics.FromImage(bm)
            g.DrawImage(_gImage,
              New Rectangle(0, 0, _gImage.Width, _gImage.Height), 0, 0,
              _gImage.Width, _gImage.Height, GraphicsUnit.Pixel, ia)
        End Using
        Return bm

    End Function

    Private Sub DrawImage(ByRef g As Graphics, ByVal cWidth As Integer, ByVal cHeight As Integer)
        If _gShowImageBox Then
            g.FillRectangle(New SolidBrush(Color.FromArgb(_gImageBoxTransp, _gImageBoxColor)), 0, 0, cWidth + 4, cHeight + 4)
        End If
        If _gImage IsNot Nothing Then
            g.DrawImage(ImageTransp, 2, 2, cWidth, cHeight)
        End If
        If _gShowImageBox Then g.DrawRectangle(New Pen(_gImageBorderColor), 0, 0, cWidth + 4, cHeight + 4)
    End Sub

    Private Sub DrawTextBox(ByRef g As Graphics, Optional ByVal ptX As Integer = 0, Optional ByVal ptY As Integer = 0)

        Using pn As New Pen(_gTextBorderColor)
            pn.Alignment = PenAlignment.Inset
            g.FillRectangle(New SolidBrush(Color.FromArgb(CInt(_gTextBoxTransp), _gTextBoxColor)),
                New Rectangle(ptX, ptY, CInt(_gTextBoxArea.Width + 6), CInt(_gTextBoxArea.Height + 6)))
            g.DrawRectangle(New Pen(_gTextBorderColor),
                New Rectangle(ptX, ptY, CInt(_gTextBoxArea.Width + 6), CInt(_gTextBoxArea.Height + 6)))
        End Using
    End Sub

    Private Sub DrawText(ByRef g As Graphics, Optional ByVal ptX As Integer = 0, Optional ByVal ptY As Integer = 0)

        'Setup Text Brushes
        Dim br As Brush = New SolidBrush(Color.FromArgb(CInt(_gTextTransp), _gTextColor))
        Dim brs As Brush = New SolidBrush(Color.FromArgb(_gTextShadower.ShadowTransp, _gTextShadowColor))
        If _gTextFade = ETextFade.Linear Then
            br = New LinearGradientBrush(
                New Rectangle(ptX + 3, ptY + 3, CInt(_gTextBoxArea.Width), CInt(_gTextBoxArea.Height + 3)),
                _gTextColor, Color.Transparent, LinearGradientMode.Horizontal)
            brs = New LinearGradientBrush(
               New Rectangle(0, 0, CInt(_gTextBoxArea.Width), CInt(_gTextBoxArea.Height + 3)),
               Color.FromArgb(_gTextShadower.ShadowTransp, _gTextShadowColor),
                Color.Transparent, LinearGradientMode.Horizontal)

        ElseIf _gTextFade = ETextFade.Path Then
            Dim gp As New GraphicsPath
            gp.AddRectangle(New Rectangle(ptX + 3, ptY + 3, CInt(_gTextBoxArea.Width), CInt(_gTextBoxArea.Height + 3)))
            br = New PathGradientBrush(gp)
            CType(br, PathGradientBrush).CenterColor = _gTextColor
            CType(br, PathGradientBrush).SurroundColors = New Color() {Color.Transparent}

            gp.Reset()
            gp.AddRectangle(New Rectangle(0, 0, CInt(_gTextBoxArea.Width), CInt(_gTextBoxArea.Height + 3)))
            brs = New PathGradientBrush(gp)
            CType(brs, PathGradientBrush).CenterColor = Color.FromArgb(_gTextShadower.ShadowTransp, _gTextShadowColor)
            CType(brs, PathGradientBrush).SurroundColors = New Color() {Color.Transparent}
            gp.Dispose()
        End If

        If _gTextShadow Then
            'If shadow is requested setup and use the TextShadower
            With _gTextShadower
                .Font = _gFont
                .TextBrush = br
                .ShadowBrush = brs
                .Alignment = _gTextAlignment
                .ShadowTheText(g, New Rectangle(ptX + 3, ptY + 3, CInt(_gTextBoxArea.Width), CInt(_gTextBoxArea.Height + 3)), _gText)
            End With

        Else
            g.TextRenderingHint = TextRenderingHint.AntiAlias
            g.DrawString(_gText, _gFont, br,
                New Rectangle(ptX + 3, ptY + 3, CInt(_gTextBoxArea.Width), CInt(_gTextBoxArea.Height + 3)), sf)
        End If

        br.Dispose()
        brs.Dispose()
    End Sub

    Private Sub AddShadow(ByRef g As Graphics, ByVal ShadowPt As Point, ByVal BoxToShadow As Size, Optional ByVal UseTextBuffer As Boolean = True)
        Dim br As LinearGradientBrush
        Dim gp As New GraphicsPath
        Dim pts() As Point
        Dim shadowsz As Size
        If UseTextBuffer Then
            shadowsz = Size.Add(BoxToShadow, New Size(7, 7))
        Else
            shadowsz = BoxToShadow
        End If
        Dim ShadowColor As Color = Color.Black
        pts = New Point() {
            New Point(ShadowPt.X + shadowsz.Width, ShadowPt.Y + 5),
            New Point(ShadowPt.X + shadowsz.Width + 5, ShadowPt.Y + 5),
            New Point(ShadowPt.X + shadowsz.Width + 5, ShadowPt.Y + shadowsz.Height + 5),
            New Point(ShadowPt.X + shadowsz.Width, ShadowPt.Y + shadowsz.Height)}
        gp.AddLines(pts)
        br = New LinearGradientBrush(New RectangleF((ShadowPt.X + shadowsz.Width - 1), (ShadowPt.Y + 5),
            6, (shadowsz.Height)),
              ShadowColor, Color.Transparent, LinearGradientMode.Horizontal)
        g.FillPath(br, gp)
        gp.Reset()
        pts = New Point() {
            New Point(ShadowPt.X + 5, ShadowPt.Y + shadowsz.Height),
            New Point(ShadowPt.X + shadowsz.Width, ShadowPt.Y + shadowsz.Height),
            New Point(ShadowPt.X + shadowsz.Width + 5, ShadowPt.Y + shadowsz.Height + 5),
            New Point(ShadowPt.X + 5, ShadowPt.Y + shadowsz.Height + 5)}
        gp.AddLines(pts)
        br = New LinearGradientBrush(New RectangleF((ShadowPt.X + 5), (ShadowPt.Y + shadowsz.Height + 5),
            (shadowsz.Width), 6),
              ShadowColor, Color.Transparent, LinearGradientMode.Vertical)
        g.FillPath(br, gp)
        br.Dispose()
        gp.Dispose()

    End Sub


#End Region 'Drawing

#End Region 'Cursor

End Class
