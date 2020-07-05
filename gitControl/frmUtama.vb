Imports System.IO
Imports System.Net
Imports System.Web.Script.Serialization

Public Class frmUtama
    Private psi As ProcessStartInfo
    Private cmd As Process
    Private Delegate Sub InvokeWithString(ByVal text As String)

    Private hasil As String = ""

    Private Sub execute(ByVal arg As String)
        Try
            cmd.Kill()
        Catch ex As Exception
        End Try
        hasil = ""
        TimerGagal.Stop()
        TimerSukses.Stop()
        txtPesan.Text = ""
        txtPesan.BackColor = Color.Gainsboro
        richTxt.Clear()

        psi = New ProcessStartInfo("git", arg)

        Dim sysEncoding As System.Text.Encoding
        System.Text.Encoding.GetEncoding(Globalization.CultureInfo.CurrentUICulture.TextInfo.OEMCodePage)
        With psi
            .UseShellExecute = False
            .RedirectStandardError = True
            .RedirectStandardOutput = True
            .RedirectStandardInput = True
            .CreateNoWindow = True
            .StandardOutputEncoding = sysEncoding
            .StandardErrorEncoding = sysEncoding
        End With
        cmd = New Process With {.StartInfo = psi, .EnableRaisingEvents = True}
        AddHandler cmd.ErrorDataReceived, AddressOf AsyncDataReceived
        AddHandler cmd.OutputDataReceived, AddressOf AsyncDataReceived
        cmd.Start()
        cmd.BeginOutputReadLine()
        cmd.BeginErrorReadLine()
    End Sub

    Private Sub AsyncDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        Me.Invoke(New InvokeWithString(AddressOf SyncOutput), e.Data)
    End Sub

    Private Sub SyncOutput(ByVal text As String)
        Try
            If text.Contains("https://github.com") Then
                'jangan tampilkan
            Else
                richTxt.AppendText(text & Environment.NewLine)
                richTxt.ScrollToCaret()
            End If
        Catch ex As Exception

        End Try
        
        hasil += text & Environment.NewLine

        If hasil.Contains("file changed") Or hasil.Contains("insertion(+)") Then
            txtPesan.Text = "Update Berhasil"
            txtPesan.BackColor = Color.Lime
            TimerGagal.Stop()
            TimerSukses.Start()
        ElseIf hasil.Contains("Already up to date.") Then
            txtPesan.Text = "Belum ada update terbaru"
            txtPesan.BackColor = Color.Blue
        ElseIf hasil.Contains("HEAD is now at") Then
            txtPesan.Text = "Reset Berhasil, Tekan UPDATE!"
            txtPesan.BackColor = Color.Blue
        Else 'If hasil.Contains("error:") Then
            txtPesan.Text = "Update Gagal, Tekan Reset sebelum diupdate!"
            txtPesan.BackColor = Color.Red
            TimerGagal.Start()
            TimerSukses.Stop()
        End If
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        execute("pull")
    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        execute("reset --hard")
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles TimerGagal.Tick
        If txtPesan.BackColor = Color.Red Then
            txtPesan.BackColor = Color.Crimson
        ElseIf txtPesan.BackColor = Color.Crimson Then
            txtPesan.BackColor = Color.Red
        End If
    End Sub

    Private Sub TimerSukses_Tick(sender As Object, e As EventArgs) Handles TimerSukses.Tick
        If txtPesan.BackColor = Color.Lime Then
            txtPesan.BackColor = Color.LimeGreen
        ElseIf txtPesan.BackColor = Color.LimeGreen Then
            txtPesan.BackColor = Color.Lime
        End If
    End Sub
End Class

