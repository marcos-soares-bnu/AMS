Imports System.Configuration
Imports System.Diagnostics
Imports System.IO
Imports System.Timers
Imports System.Text
Imports Topshelf
Imports MPSfwk

Module Module1

    Sub Main()
        HostFactory.Run(Sub(configurator)
                            configurator.Service(Of TimedNotifier)(Sub(service)
                                                                       service.ConstructUsing(Function(factory)
                                                                                                  Return New TimedNotifier
                                                                                              End Function)

                                                                       service.WhenStarted(Function(notifier, hostControl) As Boolean
                                                                                               Return notifier.Start(hostControl)
                                                                                           End Function)

                                                                       service.WhenStopped(Function(notifier, hostControl) As Boolean
                                                                                               Return notifier.Stop(hostControl)
                                                                                           End Function)
                                                                   End Sub)

                            configurator.RunAsLocalSystem()

                            configurator.SetDisplayName("DS_AuditXML Service: DS_AuditXML_cmd")
                            configurator.SetDescription("DS_AuditXML leitura de tarefas (arquivos em wait) periodicamente (config) e execucao com logs de eventos.")
                            configurator.SetServiceName("DS_AuditXML_cmd")
                        End Sub)
    End Sub

End Module

Class TimedNotifier

    Implements ServiceControl
    Private _timer As System.Timers.Timer
    Public WAIT_MINUTO

    Public Sub New()

        WAIT_MINUTO = ConfigurationManager.AppSettings("WAIT_MINUTO")

        '---------------------------------------------------------------
        ' Testa o Modo de Compilação...
        '
#If DEBUG Then

#Else
        _timer = New Timer(WAIT_MINUTO * 60000) With {.AutoReset = True}
        AddHandler _timer.Elapsed, AddressOf TimerOnElapsed
#End If
        '
        '---------------------------------------------------------------

    End Sub

    Private Sub TimerOnElapsed()

        '======================================================================
        ' Varre a pasta abaixo e busca todos os arquivos e exec DS_AuditXML_cmd
        ' passando parametros no proprio nome do(s) arquivo(s) encontrado(s)
        ' --- caso ocorra algum erro, move o arquivo para a pasta erro...
        '-----------------------------------------------------------------
        If (LeuSrvConn()) Then

            ListaSrvDir("X:\#Msoares\")

        Else

            ProcessDirectory(ConfigurationManager.AppSettings("DS_agwait"))

        End If
        '======================================================================

    End Sub

    Public Function Start(ByVal hostControl As HostControl) As Boolean Implements ServiceControl.Start
        _timer.Start()
        Return True
    End Function

    Public Function [Stop](ByVal hostControl As HostControl) As Boolean Implements ServiceControl.[Stop]
        _timer.Stop()
        Return True
    End Function

    Public Sub ProcessDirectory(ByVal targetDirectory As String)

        Dim fileEntries As String() = Directory.GetFiles(targetDirectory)
        ' Process the list of files found in the directory. 
        Dim fileName As String
        For Each fileName In fileEntries
            ProcessFile(fileName)
        Next fileName
        '-------------------------------------------------------------------------------
        'Dim subdirectoryEntries As String() = Directory.GetDirectories(targetDirectory)
        ' Recurse into subdirectories of this directory. 
        'Dim subdirectory As String
        'For Each subdirectory In subdirectoryEntries
        'ProcessDirectory(subdirectory)
        'Next subdirectory
        '-------------------------------------------------------------------------------

    End Sub

    Public Sub ListaSrvDir(ByVal path As String)

    End Sub

    Private Function LeuSrvConn() As Boolean

        ' Encode the validationKey string. 
        '
        Dim enc As Encoding = Encoding.ASCII
        Dim s As String = ConfigurationManager.AppSettings("validationKey").Substring(0, 8)
        Dim bytes As Byte() = enc.GetBytes(s)

        ' Le a string criptografada no arquivo de entrada...
        '
        Dim sr As StreamReader = New StreamReader(ConfigurationManager.AppSettings("DS_agroot") + ConfigurationManager.AppSettings("ArqcoMvServer"))
        Dim args_enc As String = ""
        Dim args_desc As String = ""
        args_enc = sr.ReadLine()

        ' Descripta args_enc e separa os parametros para os comandos
        '
        args_desc = MPSfwk.Crypt.Decrypt(args_enc, bytes)
        Dim param() As String = args_desc.Replace("_", "\\").Split(" ")

        ' Faz as conexoes ao Servidor Remoto (move)...
        '
        Return MPSfwk.WMI.netToServer(param(0), param(1), ConfigurationManager.AppSettings("PathUNCServer"), 8000)

    End Function

    Public Sub ProcessFile(ByVal path As String)

        Dim dthrProc As DateTime = DateTime.Now
        Dim path_at_root = ConfigurationManager.AppSettings("DS_ATroot")
        Dim path_ag_root = ConfigurationManager.AppSettings("DS_agroot")
        Dim path_ag_wait = ConfigurationManager.AppSettings("DS_agwait")
        Dim path_ag_erro = ConfigurationManager.AppSettings("DS_agerro")
        Dim path_ag_move = ConfigurationManager.AppSettings("DS_agmove")
        Dim path_ag_exec = ConfigurationManager.AppSettings("DS_agexec")
        Dim dsexec = path_at_root + "DS_AuditXML_cmd.exe"
        Dim dest As String = ""

        'Armazena o caminho e o nome do arquivo como params...
        Dim params As String() = path.Split(New Char() {"\"c})
        Dim contParams = params.Length - 1
        Dim fileParams As String = params(contParams)
        '
        Dim flgCiclo As String = ""
        Dim sparamDate As String = ""
        Dim paramDate As DateTime
        Dim ehDate As Boolean
        Dim straux As String = "Invalido"

        Try
            'Verifica os params do nome do arquivo... e caso o horario seja igual ao atual...executa!
            If (fileParams.Length < 13) Then
                ehDate = False
            Else
                flgCiclo = fileParams.Substring(0, 1)
                sparamDate = fileParams.Substring(1, 12)
                ehDate = DateTime.TryParseExact(sparamDate, "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, paramDate)
            End If

            'Testa se o arquivo possui a nomenclatura valida...
            If ehDate Then

                'Executa quando a dif. de min estiver entre 0 e 1...
                If (((dthrProc - paramDate).TotalMinutes >= 0) And _
                    ((dthrProc - paramDate).TotalMinutes < WAIT_MINUTO)) Then

                    ' Escreve no Log de Evento conforme condicoes abaixo..
                    WriteToEventLog(path, ExecProcess(path_at_root, dsexec, fileParams.Substring(13).Replace("&", " ")))

                    ' Testa a flgCiclo se for "C" copia, senao move os arquivos...
                    If (flgCiclo = "C") Then

                        ' Copia para a pasta de exec...
                        dest = path_ag_exec + fileParams + "_exec_" + DateTime.Now.ToString("yyyyMMddHHmm")
                        File.Copy(path, dest)
                        WriteToEventLog(dest, "|Info| Copiou de: - " + path + " para: " + dest)

                        ' Add 1 dia...
                        dest = path.Replace((flgCiclo + sparamDate), (flgCiclo + (paramDate.AddDays(1).ToString("yyyyMMddHHmm"))))
                        File.Move(path, dest)
                        WriteToEventLog(dest, "|Info| Add 1 dia: - " + path + " para: " + dest)

                    Else

                        ' Move os arquivos com a flgCiclo = M...
                        dest = path_ag_exec + fileParams + "_exec_" + DateTime.Now.ToString("yyyyMMddHHmm")
                        If File.Exists(path) Then
                            File.Move(path, dest)
                            WriteToEventLog(dest, "|Info| Moveu de: - " + path + " para: " + dest)
                        End If

                    End If

                ElseIf ((dthrProc - paramDate).TotalMinutes > 1) Then

                    ' Arquivos na pasta wait, porem a data e hr já passaram, testa e renomeia...
                    ' Adiciona 1 dia e renomeia o arquivo listado até que seja executado novamente...
                    If File.Exists(path) Then

                        ' Testa o flgCiclo, se alterado apenas move o arquivo para a pasta exec...
                        If (flgCiclo = "C") Then
                            dest = path.Replace((flgCiclo + sparamDate), (flgCiclo + (paramDate.AddDays(1).ToString("yyyyMMddHHmm"))))
                            WriteToEventLog(path, "|Info| Add 1 dia: - " + path + " para: " + dest)
                        Else
                            dest = path_ag_exec + fileParams
                            WriteToEventLog(path, "|Info| Moveu de: - " + path + " para: " + dest)
                        End If
                        File.Move(path, dest)

                    End If

                End If

            ElseIf (path.IndexOf(".xml") > 0) Then

                dest = path_ag_move + fileParams
                straux = "XML"
                WriteToEventLog("", "|Info| Arq. " + straux + " \ Movido de: - " + path + " para: " + dest)

            Else

                ' Move para a pasta de erro, os arquivos invalidos ou XML de dados...
                If File.Exists(path) Then
                    dest = path_ag_erro + fileParams
                    File.Move(path, dest)
                    '
                    WriteToEventLog(dest, "|Info| Arq. " + straux + " \ Movido de: - " + path + " para: " + dest)
                End If

            End If

        Catch Ex As Exception
            WriteToEventLog(path, "|Info| Ocorreu um erro no processamento do arquivo - " + path + " dest = " + dest + " - Erro: " + Ex.Message)
        End Try

    End Sub

    Private Function ExecProcess(ByVal wrkdir As String, ByVal pgm As String, ByVal param As String) As String

        Dim proc = New Process() With
                    { _
                    .StartInfo = New ProcessStartInfo() With
                                { _
                                    .FileName = pgm, _
                                    .Arguments = param, _
                                    .UseShellExecute = False, _
                                    .RedirectStandardOutput = True, _
                                    .CreateNoWindow = True, _
                                    .WorkingDirectory = wrkdir _
                                } _
                    }

        proc.Start()
        Dim line As String = ""
        'After process starts, you read the output
        While Not proc.StandardOutput.EndOfStream
            ' do something with line (append a stringbuilder for example)
            line = line + proc.StandardOutput.ReadLine() + vbCrLf
        End While

        Return line

    End Function
    '*************************************************************
    'NAME:          WriteToEventLog
    'PURPOSE:       Write to Event Log
    'PARAMETERS:    Entry - Value to Write
    '               AppName - Name of Client Application. Needed 
    '               because before writing to event log, you must 
    '               have a named EventLog source. 
    '               EventType - Entry Type, from EventLogEntryType 
    '               Structure e.g., EventLogEntryType.Warning, 
    '               EventLogEntryType.Error
    '               LogName: Name of Log (System, Application; 
    '               Security is read-only) If you 
    '               specify a non-existent log, the log will be
    '               created
    'RETURNS:       True if successful
    '*************************************************************
    Public Function WriteToEventLog(ByVal pfile As String, _
                    ByVal entry As String, _
                    Optional ByVal appName As String = "DS_AuditXML", _
                    Optional ByVal eventType As  _
                    EventLogEntryType = EventLogEntryType.Information, _
                    Optional ByVal logName As String = "DS_AuditXML_cmd") As Boolean

        Dim objEventLog As New EventLog

        Try

            'Testa se for xml não escreve nada no arquivo...
            'write in the pfile...
            If (pfile <> "") Then
                File.AppendAllText(pfile, (vbCrLf + entry))
            End If

            'Register the Application as an Event Source
            If Not EventLog.SourceExists(appName) Then
                EventLog.CreateEventSource(appName, logName)
            End If

            'log the entry
            objEventLog.Source = appName
            objEventLog.WriteEntry(entry, eventType)

            Return True

        Catch Ex As Exception

            Return False

        End Try

    End Function

End Class