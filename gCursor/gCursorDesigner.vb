Imports System.ComponentModel
Imports System.ComponentModel.Design

#Region "gCursorDesigner"

Public Class GCursorDesigner
    Inherits ComponentDesigner

#Disable Warning IDE0052 ' Quitar miembros privados no leídos
    Private _gCursor As GCursor
#Enable Warning IDE0052 ' Quitar miembros privados no leídos
    Private _Lists As DesignerActionListCollection

    Public Overrides Sub Initialize(ByVal component As IComponent)
        MyBase.Initialize(component)

        ' Get control shortcut reference
        _gCursor = CType(component, GCursor)
    End Sub

#Region "ActionLists"

    Public Overrides ReadOnly Property ActionLists() As System.ComponentModel.Design.DesignerActionListCollection
        Get
            If _Lists Is Nothing Then
                _Lists = New DesignerActionListCollection From {
                    New GCursorActionList(Me.Component)
                }
            End If
            Return _Lists
        End Get
    End Property

#End Region 'ActionLists

End Class

#End Region 'gCursorDesigner

#Region "gCursorActionList"

Public Class GCursorActionList
    Inherits DesignerActionList

    Private ReadOnly _gCursorSelector As GCursor

    Public Sub New(ByVal component As IComponent)
        MyBase.New(component)

        ' Save a reference to the control we are designing.
        _gCursorSelector = DirectCast(component, GCursor)

        'Makes the Smart Tags open automatically 
        Me.AutoShow = True
    End Sub

#Region "Smart Tag Items"

#Region "Methods"

    Public Sub Edit()
        'Create a new Corners Dialog and update the controls on the form
        Dim dlg As New gCursorUIEditor()

        Try
            ' Prepare the editing dialog.
            With dlg
                .TextType = _gCursorSelector.GType
                .HotSpot = _gCursorSelector.GHotSpot
                CType(.GroupBoxHotSpot.Controls([Enum].GetName(GetType(ContentAlignment), .HotSpot)),
                    RadioButton).Checked = True

                If _gCursorSelector.GText = "" Then
                    .txbText.Text = "Example Text" & vbCrLf & "Second Line" & vbCrLf & "Third Line"
                Else
                    .txbText.Text = _gCursorSelector.GText
                End If
                .TextFont = _gCursorSelector.GFont
                CType(.GroupBoxType.Controls("rbut" & [Enum].GetName(GetType(GCursor.EType), .TextType)),
                    RadioButton).Checked = True
                .TextColor = _gCursorSelector.GTextColor
                .tbarTextTransp.Value = _gCursorSelector.GTTransp
                .txbTextTransp.Text = .tbarTextTransp.Value.ToString
                .tbarTextBoxTransp.Value = _gCursorSelector.GTBTransp
                .txbTextBoxTransp.Text = .tbarTextBoxTransp.Value.ToString
                .TextAlign = _gCursorSelector.GTextAlignment
                CType(.GroupBoxTextAlign.Controls("Align" & [Enum].GetName(GetType(ContentAlignment), .TextAlign)),
                    RadioButton).Checked = True
                .TextAutoFit = _gCursorSelector.GTextAutoFit
                CType(.GroupBoxAutoFit.Controls("rbut" & [Enum].GetName(GetType(GCursor.ETextAutoFit),
                    .TextAutoFit)), RadioButton).Checked = True
                .gCursor1.GTextBox = _gCursorSelector.GTextBox
                .TextBoxColor = _gCursorSelector.GTextBoxColor
                .TextBorderColor = _gCursorSelector.GTextBorderColor
                .TextColor = _gCursorSelector.GTextColor
                .TextFill = _gCursorSelector.GTextFade
                CType(.GroupBoxFadeStyle.Controls([Enum].GetName(GetType(GCursor.ETextFade), .TextFill)),
                    RadioButton).Checked = True

                .chkShowTextShadow.Checked = _gCursorSelector.GTextShadow
                .TextShadowColor = _gCursorSelector.GTextShadowColor
                .tbarBlur.Value = CInt(_gCursorSelector.GTextShadower.Blur / 0.1)
                .tbarOffset.Value = CInt(_gCursorSelector.GTextShadower.Offset.X / 0.1)
                .tbarShadowTransp.Value = CInt(_gCursorSelector.GTextShadower.ShadowTransp)
                .txbOffset.Text = CStr(.tbarOffset.Value * 0.1)
                .txbShadowTransp.Text = CStr(.tbarShadowTransp.Value)
                .txbBlur.Text = CStr(.tbarBlur.Value * 0.1)


                .tbarImageTransp.Value = _gCursorSelector.GITransp
                .txbImageTransp.Text = .tbarImageTransp.Value.ToString
                .tbarImageBoxTransp.Value = _gCursorSelector.GIBTransp
                .txbImageBoxTransp.Text = .tbarImageBoxTransp.Value.ToString
                .gCursor1.GImageBox = _gCursorSelector.GImageBox
                .ImageBoxColor = _gCursorSelector.GImageBoxColor
                .ImageBorderColor = _gCursorSelector.GImageBorderColor

                .chkBitBlkBack.Checked = _gCursorSelector.GBlackBitBack
                .chkShowBoxShadows.Checked = _gCursorSelector.GBoxShadow
                .chkShowImageBox.Checked = _gCursorSelector.GShowImageBox
                .chkShowTextBox.Checked = _gCursorSelector.GShowTextBox
                .chkMultiline.Checked = _gCursorSelector.GTextMultiline

                .tbarTextBoxWidth.Value = _gCursorSelector.GTextBox.Width
                .tbarTextBoxHeight.Value = _gCursorSelector.GTextBox.Height
                .tbarImageBoxWidth.Value = _gCursorSelector.GImageBox.Width
                .tbarImageBoxHeight.Value = _gCursorSelector.GImageBox.Height
                .txbTextBoxWidth.Text = CStr(.tbarTextBoxWidth.Value)
                .txbTextBoxHeight.Text = CStr(.tbarTextBoxHeight.Value)
                .txbImageBoxWidth.Text = CStr(.tbarImageBoxWidth.Value)
                .txbImageBoxHeight.Text = CStr(.tbarImageBoxHeight.Value)
                If Not IsNothing(_gCursorSelector.GImage) Then .picImage.Image = _gCursorSelector.GImage

            End With
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try


        ' Update new gCursor properties if OK button was pressed
        If dlg.ShowDialog() = DialogResult.OK Then
            Dim designerHost As IDesignerHost = CType(Me.Component.Site.GetService(
                GetType(IDesignerHost)), IDesignerHost)

            If designerHost IsNot Nothing Then
                Dim t As DesignerTransaction = designerHost.CreateTransaction()
                With dlg.gCursor1
                    Try
                        SetControlProperty("gType", .GType)
                        SetControlProperty("gHotSpot", .GHotSpot)
                        SetControlProperty("gText", .GText)
                        SetControlProperty("gFont", .GFont)
                        SetControlProperty("gTextColor", .GTextColor)
                        SetControlProperty("gTTransp", .GTTransp)
                        SetControlProperty("gTBTransp", .GTBTransp)
                        SetControlProperty("gTextAlignment", .GTextAlignment)
                        SetControlProperty("gTextAutoFit", .GTextAutoFit)
                        SetControlProperty("gTextBox", .GTextBox)
                        SetControlProperty("gTextBoxColor", .GTextBoxColor)
                        SetControlProperty("gTextBorderColor", .GTextBorderColor)
                        SetControlProperty("gTextColor", .GTextColor)
                        SetControlProperty("gTextFade", .GTextFade)
                        SetControlProperty("gTextShadow", .GTextShadow)
                        SetControlProperty("gTextShadowColor", .GTextShadowColor)
                        SetControlProperty("gTextShadower", .GTextShadower)

                        SetControlProperty("gITransp", .GITransp)
                        SetControlProperty("gIBTransp", .GIBTransp)
                        SetControlProperty("gImageBox", .GImageBox)
                        SetControlProperty("gImageBoxColor", .GImageBoxColor)
                        SetControlProperty("gImageBorderColor", .GImageBorderColor)
                        SetControlProperty("gBlackBitBack", .GBlackBitBack)

                        SetControlProperty("gBoxShadow", .GBoxShadow)
                        SetControlProperty("gShowImageBox", .GShowImageBox)
                        SetControlProperty("gShowTextBox", .GShowTextBox)
                        SetControlProperty("gTextMultiline", .GTextMultiline)

                        SetControlProperty("gTextBox", .GTextBox)
                        SetControlProperty("gImageBox", .GImageBox)

                        SetControlProperty("gImage", .GImage)

                        t.Commit()
                    Catch
                        t.Cancel()
                    End Try
                End With

            End If
        End If

    End Sub

#End Region 'Methods

    ' Set a control property. This method makes Undo/Redo
    ' work properly and marks the form as modified in the IDE.
    Private Sub SetControlProperty(ByVal property_name As String, ByVal value As Object)
        TypeDescriptor.GetProperties(_gCursorSelector) _
            (property_name).SetValue(_gCursorSelector, value)
    End Sub

#End Region ' Smart Tag Items

    ' Return the smart tag action items.
    Public Overrides Function GetSortedActionItems() As System.ComponentModel.Design.DesignerActionItemCollection
        Dim items As New DesignerActionItemCollection From {
            New DesignerActionMethodItem(
            Me,
            "Edit",
            "Edit Properties Dialog",
            "gCursor Appearance",
            "Edit gCursor's Properties",
            True)
        }

        Return items
    End Function

End Class

#End Region 'gCursorActionList

