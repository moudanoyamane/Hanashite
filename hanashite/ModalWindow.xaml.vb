
Public Class ModalWindow

    Public _libretto() As Libretto

    <Obsolete("ShowDialog(_cl As Class1())を使用してください。", True)>
    Public Overloads Function ShowDialog() As Boolean
        Return MyBase.ShowDialog()
    End Function

    Public Overloads Function ShowDialog(ByRef libretto As Libretto()) As Boolean
        _libretto = libretto
        Return MyBase.ShowDialog()
    End Function


    Private _page_num As Byte = 0
    Private _max_accent_select_button_num As Byte = 0
    Private _temp_accent_select_button As Button


    Private Sub ModalWindow_Loaded(sender As Object, e As RoutedEventArgs)

        For i As Integer = 0 To UBound(_libretto)
            _max_accent_select_button_num =
                Math.Max(_max_accent_select_button_num, _libretto(i).lyric.Length())
        Next

        For i As Byte = 0 To _max_accent_select_button_num

            Dim accent_select_button As New Button With 
             {
            .Name = "accent_select_button" + i.ToString(),
            .Content = i.ToString() + "型",
            .HorizontalAlignment = HorizontalAlignment.Stretch,
            .Height = 50,
            .Visibility = Visibility.Hidden
            }
            AddHandler accent_select_button.Click, AddressOf AccentSelectButton_Click

            select_accent_panel.Children.Add(accent_select_button)
            select_accent_panel.RegisterName(accent_select_button.Name, accent_select_button)

        Next

        SetPageLayout()

        BeforButton.IsEnabled = False

    End Sub

    Private Sub AccentSelectButton_Click(ByVal sender As Button, ByVal e As EventArgs)

        _temp_accent_select_button = select_accent_panel.FindName("accent_select_button" + _libretto(_page_num).accent_num.ToString())
        _temp_accent_select_button.Background = New SolidColorBrush(Colors.WhiteSmoke)

        For i As Integer = 0 To _max_accent_select_button_num
            If sender.Name = "accent_select_button" + i.ToString() Then
                _libretto(_page_num).accent_num = i
                _temp_accent_select_button = select_accent_panel.FindName("accent_select_button" + i.ToString())
                _temp_accent_select_button.Background = New SolidColorBrush(Colors.Aquamarine)
                Exit Sub
            End If
        Next

    End Sub


    Private Sub BeforButton_Click(sender As Object, e As RoutedEventArgs)

        _libretto(_page_num).intonation_flag = IntonationCheckBox.IsChecked
        _page_num -= 1
        IntonationCheckBox.IsChecked = _libretto(_page_num).intonation_flag

        If _page_num = 0 Then
            BeforButton.IsEnabled = False
        End If
        NextButton.Content = "次へ"

        SetPageLayout()

    End Sub


    Private Sub NextButton_Click(sender As Object, e As RoutedEventArgs)

        _libretto(_page_num).intonation_flag = IntonationCheckBox.IsChecked
        _page_num += 1

        If _page_num > UBound(_libretto) Then
            Me.Close()
            Exit Sub
        ElseIf _page_num = UBound(_libretto) Then
            NextButton.Content = "完了"
        End If

        IntonationCheckBox.IsChecked = _libretto(_page_num).intonation_flag
        BeforButton.IsEnabled = True

        SetPageLayout()

    End Sub


    Private Sub SetPageLayout()

        PhraseLabel.Content = _libretto(_page_num).phrase

        For i As Integer = 0 To (_libretto(_page_num).lyric.Length())
            _temp_accent_select_button = select_accent_panel.FindName("accent_select_button" + i.ToString())
            _temp_accent_select_button.Visibility = Visibility.Visible

            If i = _libretto(_page_num).accent_num Then
                _temp_accent_select_button.Background = New SolidColorBrush(Colors.Aquamarine)
            Else
                _temp_accent_select_button.Background = New SolidColorBrush(Colors.WhiteSmoke)
            End If

        Next
        For i As Integer = _libretto(_page_num).lyric.Length() + 1 To _max_accent_select_button_num
            _temp_accent_select_button = select_accent_panel.FindName("accent_select_button" + i.ToString())
            _temp_accent_select_button.Visibility = Visibility.Hidden

            'If i = _libretto(_page_num).accent_num Then
            ' _temp_accent_select_button.Background = New SolidColorBrush(Colors.Aquamarine)
            'Else
            '_temp_accent_select_button.Background = New SolidColorBrush(Colors.WhiteSmoke)
            'End If

        Next


    End Sub


End Class
