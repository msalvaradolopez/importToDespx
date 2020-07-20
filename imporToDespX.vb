Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Xml
Imports ImporToDespX
Imports ImporToDespX.wsSTECNO


Public Class imporToDespX
    Private Sub imporToDespX_Load(sender As Object, e As EventArgs) Handles MyBase.Load


    End Sub

    Private Sub imporToDespX_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim lsError As String = ""

        Me.WindowState = FormWindowState.Minimized

        Try

            mensajeError.Text = "INICIO DEL PROCESO"
            Dim oTrackingGPS As trackingGPS = New trackingGPS()

            oTrackingGPS.inicio(lsError)

            If Not String.IsNullOrEmpty(lsError) Then
                mensajeError.Text = "Aviso..." + lsError
            Else
                mensajeError.Text = "FIN DEL PROCEESO"
            End If

        Catch ex As Exception
            mensajeError.Text = ex.Message.ToString()

        End Try

        Me.Close()
    End Sub
End Class
