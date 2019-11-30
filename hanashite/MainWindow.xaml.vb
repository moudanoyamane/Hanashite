Class MainWindow

    Private argc As String()

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs)

        argc = System.Environment.GetCommandLineArgs()
        If argc.Length < 2 Then
            Me.Close()
            Environment.Exit(-1)
        End If

        SpeedComboBox.Items.Add("速い")
        SpeedComboBox.Items.Add("中くらい")
        SpeedComboBox.Items.Add("遅い")
        SpeedComboBox.Text = "中くらい"

        AccentComboBox.Items.Add("大きい")
        AccentComboBox.Items.Add("中くらい")
        AccentComboBox.Items.Add("小さい")
        AccentComboBox.Text = "中くらい"

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)

        Dim temp_lyric() As String = Split(LyricBox.Text, vbCrLf)
        If temp_lyric(0) = "" AndAlso UBound(temp_lyric) = 0 Then
            Me.Close()
            Environment.Exit(-1)
        End If

        Dim tempo As String
        Select Case SpeedComboBox.Text
            Case "速い"
                tempo = "230"
            Case "中くらい"
                tempo = "215"
            Case "遅い"
                tempo = "200"
            Case Else
                tempo = "215"
        End Select

        Dim note_step As Byte
        Select Case AccentComboBox.Text
            Case "大きい"
                note_step = 8
            Case "中くらい"
                note_step = 4
            Case "小さい"
                note_step = 2
            Case Else
                note_step = 4
        End Select

        Dim libretto() As Libretto
        ReDim libretto(UBound(temp_lyric))

        For i As Integer = 0 To UBound(libretto)
            libretto(i) = New Libretto With
            {
                .phrase = temp_lyric(i)
            }
            Dim temp_counter As Byte = 0

            For j As Integer = 0 To temp_lyric(i).Length() - 1

                Select Case temp_lyric(i)(j)
                    Case "ぁ", "ぃ", "ぅ", "ぇ", "ぉ", "ゃ", "ゅ", "ょ"
                        libretto(i).lyric(temp_counter - 1) += temp_lyric(i)(j)
                    Case Else
                        ReDim Preserve libretto(i).lyric(temp_counter)
                        libretto(i).lyric(temp_counter) = temp_lyric(i)(j)
                        temp_counter += 1
                End Select
            Next j

            libretto(i).accent_num = libretto(i).lyric.Length - 1

        Next i


        Dim modal_window As New ModalWindow

        Me.Visibility = Visibility.Hidden

        modal_window.ShowDialog(libretto)

        Dim output As New Text.StringBuilder
        Dim note_num As Integer = 60 + (note_step / 4)

        For i As Integer = 0 To UBound(libretto)

            '-----------------------------------------------------------------
            '前処理
            If libretto(i).accent_num = 0 Then
                libretto(i).accent_num = libretto(i).phrase.Length()
            End If

            If libretto(i).intonation_flag = True Then
                note_num += (note_step / 2)
            End If

            '-----------------------------------------------------------------
            'lyric(0)

            If libretto(i).accent_num <> 1 Then
                note_num -= (note_step / 4)
            End If

            output.Append(MakeNewNote(libretto(i).lyric(0), note_num, tempo))

            '-----------------------------------------------------------------
            'lyric(1 to accent_num-1)
            If libretto(i).accent_num <> 1 Then
                note_num += note_step

                For j As Integer = 1 To (libretto(i).accent_num - 1)

                    If ((j - 1) Mod (8 / note_step)) = 0 _
                    AndAlso ((j - 1) \ (8 / note_step)) <> 0 Then

                        note_num -= 1
                    End If
                    output.Append(MakeNewNote(libretto(i).lyric(j), note_num, tempo))
                Next j
            End If

            '-----------------------------------------------------------------
            'lyric(accent_num to N)

            If (libretto(i).lyric.Length() - libretto(i).accent_num) <> 0 Then

                note_num -= (note_step / 2)
                output.Append(MakeNewNote(libretto(i).lyric(libretto(i).accent_num), note_num, tempo))

                note_num -= (note_step / 2)

                For j As Integer = libretto(i).accent_num + 1 To (libretto(i).lyric.Length() - 1)

                    If ((j - libretto(i).accent_num) Mod (8 / note_step)) = 0 _
                        AndAlso ((j - libretto(i).accent_num) \ (8 / note_step)) <> 0 Then

                        note_num -= 1
                    End If
                    output.Append(MakeNewNote(libretto(i).lyric(j), note_num, tempo))
                Next j

            End If

        Next i

        IO.File.WriteAllText(argc(1), output.ToString.TrimEnd, Text.Encoding.GetEncoding("shift_jis"))

        Me.Close()

    End Sub

    Private Function MakeNewNote(ByRef lyric As String, ByRef note_num As Integer, ByRef tempo As String) As String

        Return _
        (
            "[#INSERT]" & vbCrLf &
            "Length=240" & vbCrLf &
            "Lyric=" & lyric & vbCrLf &
            "NoteNum=" & (note_num).ToString() & vbCrLf &
            "Tempo=" & tempo & vbCrLf
         )

    End Function


End Class
