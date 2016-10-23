Imports System.Drawing.Text
Imports System.Drawing.Drawing2D

Public Class TextShadower

#Region "Properties"

    Private _Text As String = "Drop Shadow"
    Public Property Text() As String
        Get
            Return _Text
        End Get
        Set(ByVal value As String)
            _Text = value
        End Set
    End Property

    Private _Font As Font = New Font("Arial", 10, FontStyle.Bold)
    Public Property Font() As Font
        Get
            Return _Font
        End Get
        Set(ByVal value As Font)
            _Font = value
        End Set
    End Property

    Private _TextBrush As Brush = New SolidBrush(Color.Blue)
    Public Property TextBrush() As Brush
        Get
            Return _TextBrush
        End Get
        Set(ByVal value As Brush)
            _TextBrush = value
        End Set
    End Property

    Private _TextColor As Color = Color.Blue
    Public Property TextColor() As Color
        Get
            Return _TextColor
        End Get
        Set(ByVal value As Color)
            _TextColor = value
            _TextBrush = New SolidBrush(value)
        End Set
    End Property

    Private _ShadowBrush As Brush = New SolidBrush(Color.Black)
    Public Property ShadowBrush() As Brush
        Get
            Return _ShadowBrush
        End Get
        Set(ByVal value As Brush)
            _ShadowBrush = value
        End Set
    End Property

    Private _ShadowColor As Color = Color.Black
    Public Property ShadowColor() As Color
        Get
            Return _ShadowColor
        End Get
        Set(ByVal value As Color)
            _ShadowColor = value
            _ShadowBrush = New SolidBrush(Color.FromArgb(_ShadowTransp, value))
        End Set
    End Property

    Private _ShadowTransp As Integer = 128
    Public Property ShadowTransp() As Integer
        Get
            Return _ShadowTransp
        End Get
        Set(ByVal value As Integer)
            _ShadowTransp = value
            _ShadowBrush = New SolidBrush(Color.FromArgb(value, _ShadowColor))
        End Set
    End Property

    Private _sf As StringFormat = New StringFormat
    Private _Alignment As ContentAlignment = ContentAlignment.MiddleCenter
    Public Property Alignment() As ContentAlignment
        Get
            Return _Alignment
        End Get
        Set(ByVal value As ContentAlignment)
            _Alignment = value
            Select Case value
                Case ContentAlignment.BottomCenter, ContentAlignment.BottomLeft, ContentAlignment.BottomRight
                    _sf.LineAlignment = StringAlignment.Far
                Case ContentAlignment.MiddleCenter, ContentAlignment.MiddleLeft, ContentAlignment.MiddleRight
                    _sf.LineAlignment = StringAlignment.Center
                Case ContentAlignment.TopCenter, ContentAlignment.TopLeft, ContentAlignment.TopRight
                    _sf.LineAlignment = StringAlignment.Near
            End Select
            Select Case value
                Case ContentAlignment.BottomRight, ContentAlignment.MiddleRight, ContentAlignment.TopRight
                    _sf.Alignment = StringAlignment.Far
                Case ContentAlignment.BottomCenter, ContentAlignment.MiddleCenter, ContentAlignment.TopCenter
                    _sf.Alignment = StringAlignment.Center
                Case ContentAlignment.BottomLeft, ContentAlignment.MiddleLeft, ContentAlignment.TopLeft
                    _sf.Alignment = StringAlignment.Near
            End Select
        End Set
    End Property

    Private _Padding As Padding
    Public Property Padding() As Padding
        Get
            Return _Padding
        End Get
        Set(ByVal value As Padding)
            _Padding = value
        End Set
    End Property

    Private _Blur As Single = 2
    Public Property Blur() As Single
        Get
            Return _Blur
        End Get
        Set(ByVal value As Single)
            _Blur = value
        End Set
    End Property

    Private _Offset As PointF = New PointF(3, 3)
    Public Property Offset() As PointF
        Get
            Return _Offset
        End Get
        Set(ByVal value As PointF)
            _Offset = value
        End Set
    End Property

    Public Sub OffsetXY(ByVal ptX As Single, ByVal ptY As Single)
        _Offset = New PointF(ptX, ptY)
    End Sub
    Public Sub OffsetXY(ByVal ptXY As Single)
        _Offset = New PointF(ptXY, ptXY)
    End Sub
#End Region 'Properties

    Sub New()
        Me.Alignment = ContentAlignment.MiddleCenter
    End Sub

    Public Sub ShadowTheText(ByVal g As Graphics, ByVal rect As Rectangle, ByVal text As String)
        Me.Text = text
        ShadowTheText(g, rect)
    End Sub

    Public Sub ShadowTheText(ByVal g As Graphics, ByVal rect As Rectangle, _
        ByVal text As String, ByVal blur As Single, ByVal offsetpt As PointF)
        Me.Text = text
        Me.Blur = blur
        Me.Offset = offsetpt
        ShadowTheText(g, rect)
    End Sub

    Public Sub ShadowTheText(ByVal g As Graphics, ByVal rect As Rectangle)

        'Make a small (Blurred) bitmap
        Using bm As Bitmap = _
          New Bitmap(CInt(rect.Width / _Blur), CInt(rect.Height / _Blur))
            'Get a graphics object for it
            Using gBlur As Graphics = Graphics.FromImage(bm)
                ' must use an antialiased rendering hint
                gBlur.TextRenderingHint = TextRenderingHint.AntiAlias
                'this matrix zooms the text and offsets it
                Dim mx As Matrix = _
                    New Matrix(1 / _Blur, 0, 0, 1 / _Blur, _Offset.X, _Offset.Y)
                gBlur.Transform = mx
                'The shadow is drawn
                gBlur.DrawString(Text, Font, _ShadowBrush, New Rectangle(0, 0, _
                   CInt(rect.Width - (_Offset.X * _Blur) - _Padding.Horizontal), _
                   CInt(rect.Height - (_Offset.Y) * _Blur) - _Padding.Vertical), _sf)
            End Using
            rect.Offset(_Padding.Left, _Padding.Top)

            'The destination Graphics uses a high quality mode
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            'and draws antialiased text for accurate fitting
            g.TextRenderingHint = TextRenderingHint.AntiAlias
            'The small image is blown up to fill the main client rectangle
            g.DrawImage(bm, rect, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel)
            'finally, the text is drawn on top
            rect.Width = CInt(rect.Width - (_Offset.X * _Blur) - _Padding.Horizontal)
            rect.Height = CInt(rect.Height - (_Offset.Y * _Blur) - _Padding.Vertical)
            g.DrawString(Text, Font, _TextBrush, rect, _sf)
        End Using

    End Sub
End Class


