<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class imporToDespX
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.mensajeError = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'mensajeError
        '
        Me.mensajeError.Location = New System.Drawing.Point(12, 59)
        Me.mensajeError.Name = "mensajeError"
        Me.mensajeError.Size = New System.Drawing.Size(288, 20)
        Me.mensajeError.TabIndex = 0
        Me.mensajeError.Text = "Iniciando..."
        '
        'imporToDespX
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(322, 147)
        Me.Controls.Add(Me.mensajeError)
        Me.MaximizeBox = False
        Me.Name = "imporToDespX"
        Me.Text = "trackingGPS"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents mensajeError As TextBox
End Class
