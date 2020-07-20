Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.Xml
Imports ImporToDespX.wsSTECNO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Public Class trackingGPS

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


    Public Sub inicio(ByRef _error As String)

        Dim dtProveedoresGPS As New DataTable

        Try


            dtProveedoresGPS = getProveedoresGPS(_error)

            If String.IsNullOrEmpty(_error) Then
                For Each ren As DataRow In dtProveedoresGPS.Rows
                    Select Case ren("Proveedor_GPS")
                        Case "STECNO" ' PROYECTO AFLEMS
                            getPosicionesSTECNO(ren, _error)
                    End Select
                Next
            End If


        Catch ex As Exception
            _error = ex.Message.ToString()
        End Try


    End Sub

    Public Function getPosicionesSTECNO(datosProveedor As DataRow, ByRef _error As String) As Boolean

        Dim lsUsuario As String = datosProveedor("login_auten")
        Dim lsPassw As String = datosProveedor("password_auten")

        Try

            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                                                                             Function(se As Object,
                                                                             cert As System.Security.Cryptography.X509Certificates.X509Certificate,
                                                                             chain As System.Security.Cryptography.X509Certificates.X509Chain,
                                                                             sslerror As System.Net.Security.SslPolicyErrors) True

            ' Dim client As GpsWebServicesClient = New GpsWebServicesClient()
            Dim client As GpsWebServicesClient = New GpsWebServicesClient

            ' Use the 'client' variable to call operations on the service.

            ' Always close the client.

            ' Dim WSResponse As ReportLastMessage = client.GetReportLastMessage(lsUsuario, lsPassw)
            Dim WSResponse As ReportLastMessage_v2 = client.GetReportLastMessage_v2(lsUsuario, lsPassw)

            For Each renPosicion As ReportLastMessageRow_v2 In WSResponse.rows

                Dim fechaWS As DateTime = renPosicion.dateTime
                Dim fechaLocal As DateTime = fechaWS.ToLocalTime

                Me.tipopaquete = 2
                Me.IdMensaje = 0
                Me.Mensaje = ""
                Me.zipcode = ""
                Me.recvdate = Format(Convert.ToDateTime(fechaLocal), "dd/MM/yyyy")
                Me.recvtime = Format(Convert.ToDateTime(fechaLocal), "HH:mm:ss")
                Me.msgdate = Format(Convert.ToDateTime(fechaLocal), "dd/MM/yyyy")
                Me.msgtime = Format(Convert.ToDateTime(fechaLocal), "HH:mm:ss")
                Me.PosDate = Format(Convert.ToDateTime(fechaLocal), "dd/MM/yyyy")
                Me.postime = Format(Convert.ToDateTime(fechaLocal), "HH:mm:ss")
                Me.Prioridad = 0
                Me.NumAntena = renPosicion.imei
                Me.NombreAntena = renPosicion.name  'strIdUnidad
                Dim _posicion As String = renPosicion.address
                If _posicion Is Nothing Then
                    Me.Posicion = ""
                Else
                    Me.Posicion = _posicion
                End If
                Me.PosLatitud = renPosicion.latitude
                Me.PosLongitud = renPosicion.longitude
                Dim _Direccion As String = renPosicion.addressReference
                If _Direccion Is Nothing Then
                    Me.Direccion = ""
                Else
                    Me.Direccion = _Direccion.Substring(0, 15)
                End If
                Me.StatusQt = ""
                Me.timezone = "CST"
                If renPosicion.ignition Then
                    Me.Ignition = "E"
                Else
                    Me.Ignition = "A"
                End If

                Me.sistemaorigen = "NA*"
                Me.ClaveMacro = "0"
                Me.tipomacro = 0
                Me.DetMacro = ""
                Me.AcuseRecibo = "N"
                Me.Desttype = 1
                Me.Status = 0
                Me.Velocidad = renPosicion.speed
                Me.AcuseRecibo = "N"
                Me.Desttype = 1
                Me.Angulo = 0
                Me.fechaeta = Convert.ToDateTime(fechaLocal)
                Me.dist_faltante = 0
                Me.setImportToDesp(_error)

            Next

            client.Close()

        Catch ex As Exception
            _error = ex.Message.ToString()
        End Try


        Return True
    End Function


    Public Function getProveedoresGPS(ByRef _error As String) As DataTable

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

        Dim liConsecutivo As Integer = 0

        Dim lsMensajeError As String = ""

        ' valores del viaje
        Dim id_statusviaje As String = ""
        Dim id_area As Integer = 0
        Dim no_viaje As Integer = 0
        Dim id_areapedido As Integer = 0
        Dim id_pedido As Integer = 0
        Dim id_pedidopk As Integer = 0

        ' conexión string
        Dim conn As String = ConfigurationManager.AppSettings("connDB")
        Dim DBConn As New SqlConnection(conn)
        Dim DBcomando As SqlCommand
        Dim DBAdaptador As SqlDataAdapter
        Dim dtAntena As New DataTable
        Dim dtViaje As New DataTable


        DBConn.Open()
        Dim myTrans As SqlTransaction = DBConn.BeginTransaction()

        Try


            ' busca la unidad asociada a la antena
            Dim lsIdUnidad As String = ""
            Dim lsSistemaOrigen As String = "NA*"
            Dim lbAntenaSiNo As Boolean = True


            DBcomando = New SqlCommand("select t1.id_unidad, t1.id_localizador, t2.nombre
                                        from desp_antena t1 inner join desp_localizador t2 on t2.id_localizador = t1.id_localizador 
                                        where mctnumber = @NumAntena ", DBConn)
            DBcomando.Parameters.AddWithValue("@NumAntena", Me.NumAntena)
            DBcomando.Transaction = myTrans
            DBAdaptador = New SqlDataAdapter(DBcomando)
            DBAdaptador.Fill(dtAntena)

            For Each renAntena As DataRow In dtAntena.Rows
                lsIdUnidad = renAntena("id_unidad")
                lsSistemaOrigen = renAntena("nombre")
            Next

            lsIdUnidad = DBcomando.ExecuteScalar()


            If String.IsNullOrEmpty(lsIdUnidad) Then

                DBcomando = New SqlCommand("select max(id_consecutivo) from desp_importtodesp ", DBConn)
                DBcomando.Transaction = myTrans
                liConsecutivo = Convert.ToInt32(DBcomando.ExecuteScalar())

                liConsecutivo = liConsecutivo + 1

                lbAntenaSiNo = False
                lsMensajeError = "No existe relación entre la antena : " + Me.NumAntena + " y unidad."

                DBcomando = New SqlCommand(" insert into desp_importtodesperrors(id_consecutivo, tipopaquete,idmensaje,mensaje,zipcode,recvdate,recvtime,msgdate,msgtime,posdate,
                                                                                    postime,prioridad,num_antena,nombreantena,posicion,poslat,poslon,status_qt,
                                                                                    timezone,ignition,sistemaorigen,clave_macro,tipomacro,detmacro,
                                                                                    status_registro,descripcion_error)
                                                      values (@id_consecutivo, @tipopaquete,@idmensaje,@mensaje,@zipcode,@recvdate,@recvtime,@msgdate,@msgtime,@posdate,@postime,
                                                                @prioridad,@num_antena,@nombreantena,@posicion,@poslat,@poslon,@status_qt,@timezone,@ignition,
                                                                @sistemaorigen,@clave_macro,@tipomacro,@detmacro,@status_registro,@descripcion_error)  ", DBConn)

                DBcomando.Parameters.AddWithValue("@descripcion_error", lsMensajeError)
                DBcomando.Parameters.AddWithValue("@id_consecutivo", liConsecutivo)

            Else

                lbAntenaSiNo = True
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
            DBcomando.Parameters.AddWithValue("@sistemaorigen", lsSistemaOrigen)
            DBcomando.Parameters.AddWithValue("@clave_macro", Me.ClaveMacro)
            DBcomando.Parameters.AddWithValue("@tipomacro", Me.tipomacro)
            DBcomando.Parameters.AddWithValue("@detmacro", Me.DetMacro)
            DBcomando.Parameters.AddWithValue("@status_registro", Me.Status)

            DBcomando.Transaction = myTrans
            DBcomando.ExecuteNonQuery()


            If lbAntenaSiNo Then

                ' consulta el viaje de la unidad.
                DBcomando = New SqlCommand("select top 1 id_statusviaje, t1.id_area, t1.no_viaje, viajeactual, t2.id_areapedido, t2.id_pedido, t2.id_pedidopk
                                            from trafico_viaje  t1 left outer join trafico_viaje_pedidos t2 on t2.id_area = t1.id_area and t2.no_viaje = t1.no_viaje
                                            where t1.id_unidad = @unidad 
                                                  and viajeactual = 'S' ", DBConn)
                DBcomando.Parameters.AddWithValue("@unidad", lsIdUnidad)
                DBcomando.Transaction = myTrans

                DBAdaptador = New SqlDataAdapter(DBcomando)
                DBAdaptador.Fill(dtViaje)

                id_statusviaje = "0"
                id_area = 0
                no_viaje = 0
                id_areapedido = 0
                id_pedido = 0
                id_pedidopk = 0

                For Each drViaje As DataRow In dtViaje.Rows

                    If drViaje("id_statusviaje") Is Nothing Then id_statusviaje = "0" Else id_statusviaje = drViaje("id_statusviaje")
                    If drViaje("id_area") Is Nothing Then id_area = 0 Else id_area = drViaje("id_area")
                    If drViaje("no_viaje") Is Nothing Then no_viaje = 0 Else no_viaje = drViaje("no_viaje")
                    If drViaje("id_areapedido") Is DBNull.Value Then id_areapedido = 0 Else id_areapedido = drViaje("id_areapedido")
                    If drViaje("id_pedido") Is DBNull.Value Then id_pedido = 0 Else id_pedido = drViaje("id_pedido")
                    If drViaje("id_pedidopk") Is DBNull.Value Then id_pedidopk = 0 Else id_pedidopk = drViaje("id_pedidopk")
                Next

                DBcomando = New SqlCommand("insert into desp_posicion_unidad(id_unidad,posdate,mctnumber,mctname,id_area,posicion,zipcode,poslat,poslon,
                                                                                timezone,ignition,id_status,id_area_viaje,id_viaje,id_pedido,id_pedidopk,velocidad,
                                                                                angulo,direccion, fechaeta, dist_faltante) 
                                                                         values(@id_unidad, @posdate, @mctnumber, @mctname, @id_area, @posicion, @zipcode, @poslat, @poslon,
                                                                                @timezone, @ignition, @id_status, @id_area_viaje, @id_viaje, @id_pedido, @id_pedidopk, @velocidad,
                                                                                @angulo, @direccion, @fechaeta, @dist_faltante) ", DBConn)
                DBcomando.Parameters.AddWithValue("@id_unidad", lsIdUnidad)
                DBcomando.Parameters.AddWithValue("@posdate", Me.fechaeta)
                DBcomando.Parameters.AddWithValue("@mctnumber", Me.NumAntena)
                DBcomando.Parameters.AddWithValue("@mctname", Me.NumAntena)
                DBcomando.Parameters.AddWithValue("@id_area", id_area)
                DBcomando.Parameters.AddWithValue("@posicion", Me.Posicion)
                DBcomando.Parameters.AddWithValue("@zipcode", Me.zipcode)
                DBcomando.Parameters.AddWithValue("@poslat", Me.PosLatitud)
                DBcomando.Parameters.AddWithValue("@poslon", Me.PosLongitud)
                DBcomando.Parameters.AddWithValue("@timezone", Me.timezone)
                DBcomando.Parameters.AddWithValue("@ignition", Me.Ignition)
                DBcomando.Parameters.AddWithValue("@id_status", id_statusviaje)
                DBcomando.Parameters.AddWithValue("@id_area_viaje", id_area)
                DBcomando.Parameters.AddWithValue("@id_viaje", no_viaje)
                DBcomando.Parameters.AddWithValue("@id_pedido", id_pedido)
                DBcomando.Parameters.AddWithValue("@id_pedidopk", id_pedidopk)
                DBcomando.Parameters.AddWithValue("@velocidad", Me.Velocidad)
                DBcomando.Parameters.AddWithValue("@angulo", 0)
                DBcomando.Parameters.AddWithValue("@direccion", "")
                DBcomando.Parameters.AddWithValue("@fechaeta", Me.fechaeta)
                DBcomando.Parameters.AddWithValue("@dist_faltante", Me.dist_faltante)

                DBcomando.Transaction = myTrans
                DBcomando.ExecuteNonQuery()


            End If


            myTrans.Commit()

            Catch ex As Exception
            _error = ex.Message.ToString() + " StackTrace : " + ex.StackTrace.ToString()
            myTrans.Rollback()
            Finally
                DBConn.Close()
        End Try

        Return True

    End Function

End Class
