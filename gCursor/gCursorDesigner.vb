Imports System.ComponentModel
Imports System.ComponentModel.Design

#Region "gCursorDesigner"

Public Class gCursorDesigner
    Inherits ComponentDesigner

    Private _gCursor As gCursor
    Private _Lists As DesignerActionListCollection

    Public Overrides Sub Initialize(ByVal component As IComponent)
        MyBase.Initialize(component)

        ' Get control shortcut reference
        _gCursor = CType(component, gCursor)
    End Sub

#Region "ActionLists"

    Public Overrides ReadOnly Property ActionLists() As System.ComponentModel.Design.DesignerActionListCollection
        Get
            If _Lists Is Nothing Then
                _Lists = New DesignerActionListCollection
                _Lists.Add(New gCursorActionList(Me.Component))
            End If
            Return _Lists
        End Get
    End Property

#End Region 'ActionLists

End Class

#End Region 'gCursorDesigner

#Region "gCursorActionList"

Public Class gCursorActionList
    Inherits DesignerActionList

    Private _gCursorSelector As gCursor

    Public Sub New(ByVal component As IComponent)
        MyBase.New(component)

        ' Save a reference to the control we are designing.
        _gCursorSelector = DirectCast(component, gCursor)

        'Makes the Smart Tags open automatically 
        Me.AutoShow = True
    End Sub

#Region "Smart Tag Items"

#Region "Methods"

    Public Sub Edit()
        'Create a new Corners Dialog and update the controls on the form
        Dim dlg As gCursorUIEditor = New gCursorUIEditor()

        Try
            ' Prepare the editing dialog.
            With dlg
                .TextType = _gCursorSelector.gType
                .HotSpot = _gCursorSelector.gHotSpot
                CType(.GroupBoxHotSpot.Controls([Enum].GetName(GetType(ContentAlignment), .HotSpot)), _
                    RadioButton).Checked = True

                If _gCursorSelector.gText = "" Then
                    .txbText.Text = "Example Text" & vbCrLf & "Second Line" & vbCrLf & "Third Line"
                Else
                    .txbText.Text = _gCursorSelector.gText
                End If
                .TextFont = _gCursorSelector.gFont
                CType(.GroupBoxType.Controls("rbut" & [Enum].GetName(GetType(gCursor.eType), .TextType)), _
                    RadioButton).Checked = True
                .TextColor = _gCursorSelector.gTextColor
                .tbarTextTransp.Value = _gCursorSelector.gTTransp
                .txbTextTransp.Text = .tbarTextTransp.Value.ToString
                .tbarTextBoxTransp.Value = _gCursorSelector.gTBTransp
                .txbTextBoxTransp.Text = .tbarTextBoxTransp.Value.ToString
                .TextAlign = _gCursorSelector.gTextAlignment
                CType(.GroupBoxTextAlign.Controls("Align" & [Enum].GetName(GetType(ContentAlignment), .TextAlign)), _
                    RadioButton).Checked = True
                .TextAutoFit = _gCursorSelector.gTextAutoFit
                CType(.GroupBoxAutoFit.Controls("rbut" & [Enum].GetName(GetType(gCursor.eTextAutoFit), _
                    .TextAutoFit)), RadioButton).Checked = True
                .gCursor1.gTextBox = _gCursorSelector.gTextBox
                .TextBoxColor = _gCursorSelector.gTextBoxColor
                .TextBorderColor = _gCursorSelector.gTextBorderColor
                .TextColor = _gCursorSelector.gTextColor
                .TextFill = _gCursorSelector.gTextFade
                CType(.GroupBoxFadeStyle.Controls([Enum].GetName(GetType(gCursor.eTextFade), .TextFill)), _
                    RadioButton).Checked = True

                .chkShowTextShadow.Checked = _gCursorSelector.gTextShadow
                .TextShadowColor = _gCursorSelector.gTextShadowColor
                .tbarBlur.Value = CInt(_gCursorSelector.gTextShadower.Blur / 0.1)
                .tbarOffset.Value = CInt(_gCursorSelector.gTextShadower.Offset.X / 0.1)
                .tbarShadowTransp.Value = CInt(_gCursorSelector.gTextShadower.ShadowTransp)
                .txbOffset.Text = CStr(.tbarOffset.Value * 0.1)
                .txbShadowTransp.Text = CStr(.tbarShadowTransp.Value)
                .txbBlur.Text = CStr(.tbarBlur.Value * 0.1)


                .tbarImageTransp.Value = _gCursorSelector.gITransp
                .txbImageTransp.Text = .tbarImageTransp.Value.ToString
                .tbarImageBoxTransp.Value = _gCursorSelector.gIBTransp
                .txbImageBoxTransp.Text = .tbarImageBoxTransp.Value.ToString
                .gCursor1.gImageBox = _gCursorSelector.gImageBox
                .ImageBoxColor = _gCursorSelector.gImageBoxColor
                .ImageBorderColor = _gCursorSelector.gImageBorderColor

                .chkBitBlkBack.Checked = _gCursorSelector.gBlackBitBack
                .chkShowBoxShadows.Checked = _gCursorSelector.gBoxShadow
                .chkShowImageBox.Checked = _gCursorSelector.gShowImageBox
                .chkShowTextBox.Checked = _gCursorSelector.gShowTextBox
                .chkMultiline.Checked = _gCursorSelector.gTextMultiline

                .tbarTextBoxWidth.Value = _gCursorSelector.gTextBox.Width
                .tbarTextBoxHeight.Value = _gCursorSelector.gTextBox.Height
                .tbarImageBoxWidth.Value = _gCursorSelector.gImageBox.Width
                .tbarImageBoxHeight.Value = _gCursorSelector.gImageBox.Height
                .txbTextBoxWidth.Text = CStr(.tbarTextBoxWidth.Value)
                .txbTextBoxHeight.Text = CStr(.tbarTextBoxHeight.Value)
                .txbImageBoxWidth.Text = CStr(.tbarImageBoxWidth.Value)
                .txbImageBoxHeight.Text = CStr(.tbarImageBoxHeight.Value)
                If Not IsNothing(_gCursorSelector.gImage) Then .picImage.Image = _gCursorSelector.gImage

            End With
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try


        ' Update new gCursor properties if OK button was pressed
        If dlg.ShowDialog() = DialogResult.OK Then
            Dim designerHost As IDesignerHost = CType(Me.Component.Site.GetService( _
                GetType(IDesignerHost)), IDesignerHost)

            If designerHost IsNot Nothing Then
                Dim t As DesignerTransaction = designerHost.CreateTransaction()
                With dlg.gCursor1
                    Try
                        SetControlProperty("gType", .gType)
                        SetControlProperty("gHotSpot", .gHotSpot)
                        SetControlProperty("gText", .gText)
                        SetControlProperty("gFont", .gFont)
                        SetControlProperty("gTextColor", .gTextColor)
                        SetControlProperty("gTTransp", .gTTransp)
                        SetControlProperty("gTBTransp", .gTBTransp)
                        SetControlProperty("gTextAlignment", .gTextAlignment)
                        SetControlProperty("gTextAutoFit", .gTextAutoFit)
                        SetControlProperty("gTextBox", .gTextBox)
                        SetControlProperty("gTextBoxColor", .gTextBoxColor)
                        SetControlProperty("gTextBorderColor", .gTextBorderColor)
                        SetControlProperty("gTextColor", .gTextColor)
                        SetControlProperty("gTextFade", .gTextFade)
                        SetControlProperty("gTextShadow", .gTextShadow)
                        SetControlProperty("gTextShadowColor", .gTextShadowColor)
                        SetControlProperty("gTextShadower", .gTextShadower)

                        SetControlProperty("gITransp", .gITransp)
                        SetControlProperty("gIBTransp", .gIBTransp)
                        SetControlProperty("gImageBox", .gImageBox)
                        SetControlProperty("gImageBoxColor", .gImageBoxColor)
                        SetControlProperty("gImageBorderColor", .gImageBorderColor)
                        SetControlProperty("gBlackBitBack", .gBlackBitBack)

                        SetControlProperty("gBoxShadow", .gBoxShadow)
                        SetControlProperty("gShowImageBox", .gShowImageBox)
                        SetControlProperty("gShowTextBox", .gShowTextBox)
                        SetControlProperty("gTextMultiline", .gTextMultiline)

                        SetControlProperty("gTextBox", .gTextBox)
                        SetControlProperty("gImageBox", .gImageBox)

                        SetControlProperty("gImage", .gImage)

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
        Dim items As New DesignerActionItemCollection()

        items.Add(New DesignerActionMethodItem( _
            Me, _
            "Edit", _
            "Edit Properties Dialog", _
            "gCursor Appearance", _
            "Edit gCursor's Properties", _
            True))

        Return items
    End Function

End Class

#End Region 'gCursorActionList

