Imports SevenZip
Imports System.Data.SqlClient
Imports System.Text.RegularExpressions

Public Class Parser

    Private Files As String() = {"http://apc-cap.ic.gc.ca/datafiles/amateur.zip"}
    Private Const ConnString As String = "Server=55172d93-4e48-49c7-ba01-a21c001fa435.sqlserver.sequelizer.com;Database=db55172d934e4849c7ba01a21c001fa435;User ID=kfehbnzhsdtnrgcu;Password=KAZmbpavvGsZeo5MozoBCGUJTZX2AwR53VkH6ZiinV5DofoDkhrqbKucPpzizh38;"
    Private DataPath As String = IO.Path.GetTempPath()

    Private amateurSQL = _
        "CREATE TABLE amateur " & _
        "(" & _
        "  Callsign varchar(6) PRIMARY KEY," & _
        "  Names varchar(35) NULL," & _
        "  Surname varchar(35) NULL," & _
        "  Address varchar(70) NULL," & _
        "  City varchar(35) NULL," & _
        "  Province varchar(2) NULL," & _
        "  Postal varchar(10) NULL," & _
        "  BASIC varchar(1) NULL," & _
        "  [5WPM] varchar(1) NULL," & _
        "  [12WPM] varchar(1) NULL," & _
        "  ADVANCED varchar(1) NULL," & _
        "  BasicHonours varchar(1) NULL," & _
        "  ClubName1 varchar(70) NULL," & _
        "  ClubName2 varchar(70) NULL," & _
        "  ClubAddress varchar(70) NULL," & _
        "  ClubCity varchar(35) NULL," & _
        "  ClubProvince varchar(2) NULL," & _
        "  ClubPostal varchar(7) NULL" & _
        ");"

    Public Sub New()

        If Environment.MachineName.ToUpper.StartsWith("W7WELLES") Then DataPath = ".\"

        Console.WriteLine("Downloading files")
        DownloadFiles()
        Console.WriteLine("Downloading files complete")

        Using conn As New SqlConnection(ConnString)
            conn.Open()

            For Each item In IO.Directory.GetFiles(DataPath, "*.zip")
                Console.WriteLine(String.Format("Unzipping file: {0}", item))
                Dim archive = New SevenZipExtractor(item)
                archive.ExtractArchive(DataPath)
                Console.WriteLine(String.Format("Unzipping complete: {0}", item))

                Console.WriteLine(String.Format("Parsing: {0}", item))
                ParseFile(item, conn)
                Console.WriteLine(String.Format("Parsing complete: {0}", item))

            Next

        End Using

    End Sub

    Private Sub DownloadFiles()
        For Each file In Files
            Try
                Dim test = New Net.WebClient
                test.DownloadFile(file, DataPath & IO.Path.GetFileName(file))
                Console.WriteLine(String.Format("Download of {0} successful", file))
            Catch ex As Exception
                Console.WriteLine(String.Format("Download of {0} failed: {1}", file, ex.ToString))
            End Try
        Next
    End Sub

    Private Sub ParseFile(file As String, ByRef conn As SqlConnection)

        Select Case True
            Case IO.Path.GetFileName(file).ToUpper = "AMATEUR.ZIP"

                Console.WriteLine("Creating Amateur Table")
                'SQLNonQuery(conn, amateurSQL)
                SQLNonQuery(conn, "DELETE FROM amateur")

                Dim fileContents = ""
                Using sr As New IO.StreamReader(DataPath & IO.Path.GetFileNameWithoutExtension(file) & ".txt")
                    fileContents = sr.ReadToEnd
                End Using

                Dim pattern = "^(.{6})(.{35})?(.{35})?(.{70})?(.{35})?(.{2})?(.{10})?(.{1})?(.{1})?(.{1})?(.{1})?(.{1})?(.{70})?(.{70})?(.{70})?(.{35})?(.{2})?(.{7})?"
                Dim matches As MatchCollection = Regex.Matches(fileContents, pattern, RegexOptions.Multiline Or RegexOptions.IgnoreCase)
                Dim burnCount = 0
                Dim insertSQL As String = ""

                For Each match As Match In matches
                    If burnCount >= 2 Then
                        If burnCount >= 3 Then insertSQL &= ControlChars.NewLine
                        insertSQL &= _
                            String.Format("INSERT INTO amateur VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}');", _
                                            match.Groups(1).Value.Trim.Replace("'", "''"), _
                                            match.Groups(2).Value.Trim.Replace("'", "''"), _
                                            match.Groups(3).Value.Trim.Replace("'", "''"), _
                                            match.Groups(4).Value.Trim.Replace("'", "''"), _
                                            match.Groups(5).Value.Trim.Replace("'", "''"), _
                                            match.Groups(6).Value.Trim.Replace("'", "''"), _
                                            match.Groups(7).Value.Trim.Replace("'", "''"), _
                                            match.Groups(8).Value.Trim.Replace("'", "''"), _
                                            match.Groups(9).Value.Trim.Replace("'", "''"), _
                                            match.Groups(10).Value.Trim.Replace("'", "''"), _
                                            match.Groups(11).Value.Trim.Replace("'", "''"), _
                                            match.Groups(12).Value.Trim.Replace("'", "''"), _
                                            match.Groups(13).Value.Trim.Replace("'", "''"), _
                                            match.Groups(14).Value.Trim.Replace("'", "''"), _
                                            match.Groups(15).Value.Trim.Replace("'", "''"), _
                                            match.Groups(16).Value.Trim.Replace("'", "''"), _
                                            match.Groups(17).Value.Trim.Replace("'", "''"), _
                                            match.Groups(18).Value.Trim.Replace("'", "''"))
                        Console.WriteLine(String.Format("Created insert record #{0}", burnCount - 2))
                    End If

                    burnCount += 1
                Next

                SQLNonQuery(conn, insertSQL)

        End Select

    End Sub

    Private Sub SQLNonQuery(ByRef conn As SqlConnection, sql As String)
        Dim createCmd = New SqlCommand(sql, conn)
        createCmd.ExecuteNonQuery()
    End Sub

End Class
