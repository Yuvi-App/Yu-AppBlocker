Imports System.IO

Public Class Form1
    Dim envpath = Environment.ExpandEnvironmentVariables("%APPDATA%")
    Dim configFile = "config.ini"
    Dim setPassword As String = ""
    Dim base64encoded As String = vbNull
    Dim passwordset As Boolean = False
    Dim p() As Process
    Dim BlockedAppsList As New List(Of String)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Computer.FileSystem.FileExists(configFile) = False Then
            MessageBox.Show("First run deteceted, please input a Parental Password")
            Dim InputParentalPassword = InputBox("Enter a Password", "Parental Password")
            Dim result As DialogResult = MessageBox.Show("Do you want to use " + InputParentalPassword + " as the password?", "Verify Password", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                'Base64 encode password
                Dim data As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(InputParentalPassword)
                base64encoded = System.Convert.ToBase64String(data)

                'SetPassword
                setPassword = InputParentalPassword
                passwordset = True

                'Write to file
                Dim file As System.IO.StreamWriter
                System.IO.File.WriteAllText(configFile, "")
                file = My.Computer.FileSystem.OpenTextFileWriter(configFile, True)
                file.WriteLine("[ParentalPassword]")
                file.WriteLine(base64encoded)
                file.WriteLine("[Blocked Apps]")
                file.Close()
                WatcherTimer.Enabled = True
                IO.File.SetAttributes(configFile, IO.FileAttributes.Hidden)
            ElseIf result = DialogResult.No Then
                MessageBox.Show("Please Restart App")
                Me.Close()
            End If

        ElseIf My.Computer.FileSystem.FileExists(configFile) = True Then
            'Read Config
            Dim configallLines As String() = File.ReadAllLines(configFile)
            base64encoded = configallLines(1)
            Dim lineCount = File.ReadAllLines(configFile).Length
            Dim count = 3
            While Not lineCount = count
                BlockedAppsList.Add(configallLines(count))
                count = count + 1
            End While

            'Decode & Set Password
            Dim data1 As Byte() = System.Convert.FromBase64String(base64encoded)
            setPassword = System.Text.ASCIIEncoding.ASCII.GetString(data1)
            passwordset = True

            'Start Timer
            WatcherTimer.Enabled = True

            'BlockAppList
            For Each item In BlockedAppsList
                lbxBlockedApps.Items.Add(item)
            Next
        End If
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Me.WindowState = FormWindowState.Minimized
        Me.Hide()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        BlockApp()
    End Sub


    ' Functions
    Private Sub BlockApp()
        Dim AppToBlock As String = txtEXEAdd.Text
        If Not AppToBlock = "" Then
            If passwordset = True Then
                Dim InputPassword As String = InputBox("Enter Password", "Parental Password")
                If InputPassword = setPassword Then
                    Using sw As StreamWriter = File.AppendText(configFile)
                        sw.WriteLine(AppToBlock)
                    End Using
                    lbxBlockedApps.Items.Add(AppToBlock)
                    BlockedAppsList.Add(AppToBlock)
                    txtEXEAdd.Clear()
                Else
                    MessageBox.Show("Password Incorrect")
                End If
            ElseIf passwordset = False Then
                MessageBox.Show("Password not set, Please rerun app")
            End If
        Else
            MessageBox.Show("Please input a exe")
        End If
    End Sub

    Private Sub WatcherTimer_Tick(sender As Object, e As EventArgs) Handles WatcherTimer.Tick
        Select Case Weekday(Now())
            Case vbMonday
                Dim curr As Date = Date.Now
                Dim startTime As New Date(curr.Year, curr.Month, curr.Day, 4, 0, 0)
                Dim endTime As New Date(curr.Year, curr.Month, curr.Day, 15, 0, 0)
                If (curr >= startTime) And (curr <= endTime) Then
                    BlockAppTimer.Enabled = True
                Else
                    BlockAppTimer.Enabled = False
                End If

            Case vbTuesday
                Dim curr As Date = Date.Now
                Dim startTime As New Date(curr.Year, curr.Month, curr.Day, 4, 0, 0)
                Dim endTime As New Date(curr.Year, curr.Month, curr.Day, 15, 0, 0)
                If (curr >= startTime) And (curr <= endTime) Then
                    BlockAppTimer.Enabled = True
                Else
                    BlockAppTimer.Enabled = False
                End If

            Case vbWednesday
                Dim curr As Date = Date.Now
                Dim startTime As New Date(curr.Year, curr.Month, curr.Day, 4, 0, 0)
                Dim endTime As New Date(curr.Year, curr.Month, curr.Day, 15, 0, 0)
                If (curr >= startTime) And (curr <= endTime) Then
                    BlockAppTimer.Enabled = True
                Else
                    BlockAppTimer.Enabled = False
                End If

            Case vbThursday
                Dim curr As Date = Date.Now
                Dim startTime As New Date(curr.Year, curr.Month, curr.Day, 4, 0, 0)
                Dim endTime As New Date(curr.Year, curr.Month, curr.Day, 15, 0, 0)
                If (curr >= startTime) And (curr <= endTime) Then
                    BlockAppTimer.Enabled = True
                Else
                    BlockAppTimer.Enabled = False
                End If

            Case vbFriday
                Dim curr As Date = Date.Now
                Dim startTime As New Date(curr.Year, curr.Month, curr.Day, 4, 0, 0)
                Dim endTime As New Date(curr.Year, curr.Month, curr.Day, 15, 0, 0)
                If (curr >= startTime) And (curr <= endTime) Then
                    BlockAppTimer.Enabled = True
                Else
                    BlockAppTimer.Enabled = False
                End If

            Case vbSaturday
                BlockAppTimer.Enabled = False

            Case vbSunday
                BlockAppTimer.Enabled = False

            Case Else
                MessageBox.Show("Couldnt get day of week")
                Me.Close()
        End Select
    End Sub

    Private Sub BlockApp_Tick(sender As Object, e As EventArgs) Handles BlockAppTimer.Tick
        'Timer Checks every 5 secs if App is running
        Try
            For Each proc1 As Process In Process.GetProcessesByName("Discord")
                proc1.Kill()
                Watcher.BalloonTipText = "Stay focused"
                Watcher.ShowBalloonTip(5)
            Next

            For Each item In BlockedAppsList
                For Each proc As Process In Process.GetProcessesByName(item)
                    proc.Kill()
                    Watcher.BalloonTipText = "Stay focused"
                    Watcher.ShowBalloonTip(5)
                Next
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Watcher.Visible = True
            Me.Hide()
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Me.WindowState = FormWindowState.Minimized
        Watcher.Visible = True
        Me.Hide()
        e.Cancel = True
    End Sub

    Private Sub Watcher_MouseClick(sender As Object, e As MouseEventArgs) Handles Watcher.MouseClick
        ContextMenuStrip1.Show(MousePosition.X, MousePosition.Y)
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        If passwordset = True Then
            Dim InputPassword As String = InputBox("Enter Password", "Parental Password")
            If InputPassword = setPassword Then
                Me.Show()
                Me.WindowState = FormWindowState.Normal
            Else
                MessageBox.Show("Password Incorrect")
            End If
        End If
    End Sub

    Private Sub QuitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QuitToolStripMenuItem.Click
        If passwordset = True Then
            Dim InputPassword As String = InputBox("Enter Password", "Parental Password")
            If InputPassword = setPassword Then
                Try
                    For Each proc As Process In Process.GetProcessesByName("Yu-AppBlocker")
                        proc.Kill()
                    Next
                    For Each proc As Process In Process.GetProcessesByName("AppBro")
                        proc.Kill()
                    Next
                Catch ex As Exception

                End Try
            Else
                MessageBox.Show("Password Incorrect")
            End If
        End If
    End Sub
End Class
