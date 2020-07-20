Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Public Class consultas


    Public tipopaquete As Integer
    Public IdMensaje As Integer
    Public Mensaje As String
    Public zipcode As String
    Public recvdate As String
    Public recvtime As String
    Public msgdate As String
    Public msgtime As String
    Public PosDate As String
    Public postime As String
    Public Prioridad As Integer
    Public NumAntena As String
    Public NombreAntena As String
    Public Posicion As String
    Public PosLatitud As Decimal
    Public PosLongitud As Decimal
    Public Direccion As String
    Public StatusQt As String
    Public timezone As String
    Public Ignition As String
    Public sistemaorigen As String
    Public ClaveMacro As String
    Public tipomacro As Integer
    Public DetMacro As String
    Public AcuseRecibo As String
    Public Desttype As Integer
    Public Status As Integer
    Public Velocidad As Integer
    Public Angulo As Integer
    Public fechaeta As Date
    Public dist_faltante As Integer



    Public Function getProveedoresGPS(ByRef _error As String) As DataTable
        _error = "Begin get PRo"
        Return Nothing
        Dim dtProveedoresGPS As New DataTable

        ' conexión string
        Dim conn As String = ConfigurationManager.AppSettings("connDB")
        Dim DBConn As New SqlConnection(conn)
        Dim DBAdaptador As SqlDataAdapter
        Dim DBcomando As SqlCommand

        Try

            DBConn.Open()
            DBcomando = New SqlCommand("select * from desp_gpsproveedores order by 1 ", DBConn)

            DBAdaptador = New SqlDataAdapter(DBcomando)

            DBAdaptador.Fill(dtProveedoresGPS)

            Return dtProveedoresGPS
        Catch ex As Exception
            _error = ex.Message.ToString()
            Return Nothing
        Finally
            DBConn.Close()
        End Try

    End Function

    Public Function setImportToDesp(ByRef _error As String) As Boolean

        Dim lsMensajeError As String = ""

        ' conexión string
        Dim conn As String = ConfigurationManager.AppSettings("connDB")
        Dim DBConn As New SqlConnection(conn)
        Dim DBcomando As SqlCommand


        DBConn.Open()
        Dim myTrans As SqlTransaction = DBConn.BeginTransaction()

        Try


            ' busca la unidad asociada a la antena
            Dim lsIdUnidad As String = ""


            DBcomando = New SqlCommand("select id_unidad from desp_antena where mctnumber = @NumAntena ", DBConn)
            DBcomando.Parameters.AddWithValue("@NumAntena", Me.NumAntena)
            DBcomando.Transaction = myTrans
            lsIdUnidad = DBcomando.ExecuteScalar()


            If String.IsNullOrEmpty(lsIdUnidad) Then
                lsMensajeError = "No existe relación entre la antena : " + Me.NumAntena + " y unidad."

                DBcomando = New SqlCommand(" insert into desp_importtodesperrors(tipopaquete,idmensaje,mensaje,zipcode,recvdate,recvtime,msgdate,msgtime,posdate,
                                                                                    postime,prioridad,num_antena,nombreantena,posicion,poslat,poslon,status_qt,
                                                                                    timezone,ignition,sistemaorigen,clave_macro,tipomacro,detmacro,
                                                                                    status_registro,descripcion_error)
                                                      values (@tipopaquete,@idmensaje,@mensaje,@zipcode,@recvdate,@recvtime,@msgdate,@msgtime,@posdate,@postime,
                                                                @prioridad,@num_antena,@nombreantena,@posicion,@poslat,@poslon,@status_qt,@timezone,@ignition,
                                                                @sistemaorigen,@clave_macro,@tipomacro,@detmacro,@status_registro,@descripcion_error)  ", DBConn)

                DBcomando.Parameters.AddWithValue("@descripcion_error", lsMensajeError)

            Else
                DBcomando = New SqlCommand(" insert into desp_importtodesp(tipopaquete,idmensaje,mensaje,zipcode,recvdate,recvtime,msgdate,msgtime,posdate,
                                                                                    postime,prioridad,num_antena,nombreantena,posicion,poslat,poslon,status_qt,
                                                                                    timezone,ignition,sistemaorigen,clave_macro,tipomacro,detmacro,
                                                                                    status_registro,acuserecibo,desttype,velocidad,angulo,direccion,
                                                                                    fechaeta, dist_faltante)
                                                      values (@tipopaquete, @idmensaje, @mensaje, @zipcode, @recvdate, @recvtime, @msgdate, @msgtime, @posdate,
                                                                                    @postime, @prioridad, @num_antena, @nombreantena, @posicion, @poslat, @poslon, @status_qt,
                                                                                    @timezone, @ignition, @sistemaorigen, @clave_macro, @tipomacro, @detmacro,
                                                                                    @status_registro, @acuserecibo, @desttype, @velocidad, @angulo, @direccion,
                                                                                    @fechaeta, @dist_faltante)  ", DBConn)

                DBcomando.Parameters.AddWithValue("@acuserecibo", Me.AcuseRecibo)
                DBcomando.Parameters.AddWithValue("@desttype", Me.Desttype)
                DBcomando.Parameters.AddWithValue("@velocidad", Me.Velocidad)
                DBcomando.Parameters.AddWithValue("@angulo", Me.Angulo)
                DBcomando.Parameters.AddWithValue("@direccion", Me.Direccion)
                DBcomando.Parameters.AddWithValue("@fechaeta", Me.fechaeta)
                DBcomando.Parameters.AddWithValue("@dist_faltante", Me.dist_faltante)

                Me.NombreAntena = lsIdUnidad

            End If

            DBcomando.Parameters.AddWithValue("@tipopaquete", Me.tipopaquete)
            DBcomando.Parameters.AddWithValue("@IdMensaje", Me.IdMensaje)
            DBcomando.Parameters.AddWithValue("@Mensaje", Me.Mensaje)
            DBcomando.Parameters.AddWithValue("@zipcode", Me.zipcode)
            DBcomando.Parameters.AddWithValue("@recvdate", Me.recvdate)
            DBcomando.Parameters.AddWithValue("@recvtime", Me.recvtime)
            DBcomando.Parameters.AddWithValue("@msgdate", Me.msgdate)
            DBcomando.Parameters.AddWithValue("@msgtime", Me.msgtime)
            DBcomando.Parameters.AddWithValue("@posdate", Me.PosDate)
            DBcomando.Parameters.AddWithValue("@postime", Me.postime)
            DBcomando.Parameters.AddWithValue("@prioridad", Me.Prioridad)
            DBcomando.Parameters.AddWithValue("@num_antena", Me.NumAntena)
            DBcomando.Parameters.AddWithValue("@nombreantena", Me.NombreAntena)
            DBcomando.Parameters.AddWithValue("@posicion", Me.Posicion)
            DBcomando.Parameters.AddWithValue("@poslat", Me.PosLatitud)
            DBcomando.Parameters.AddWithValue("@poslon", Me.PosLongitud)
            DBcomando.Parameters.AddWithValue("@status_qt", Me.StatusQt)
            DBcomando.Parameters.AddWithValue("@timezone", Me.timezone)
            DBcomando.Parameters.AddWithValue("@ignition", Me.Ignition)
            DBcomando.Parameters.AddWithValue("@sistemaorigen", Me.sistemaorigen)
            DBcomando.Parameters.AddWithValue("@clave_macro", Me.ClaveMacro)
            DBcomando.Parameters.AddWithValue("@tipomacro", Me.tipomacro)
            DBcomando.Parameters.AddWithValue("@detmacro", Me.DetMacro)
            DBcomando.Parameters.AddWithValue("@status_registro", Me.Status)

            DBcomando.Transaction = myTrans
            DBcomando.ExecuteNonQuery()
            myTrans.Commit()

        Catch ex As Exception
            _error = ex.Message.ToString()
            myTrans.Rollback()
        Finally
            DBConn.Close()
        End Try

        Return True

    End Function
End Class
