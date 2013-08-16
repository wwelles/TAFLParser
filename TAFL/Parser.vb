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
        "  Callsign varchar(10) PRIMARY KEY," & _
        "  Names varchar(40) NULL," & _
        "  Surname varchar(40) NULL," & _
        "  Address varchar(75) NULL," & _
        "  City varchar(40) NULL," & _
        "  Province varchar(3) NULL," & _
        "  Postal varchar(10) NULL," & _
        "  BASIC varchar(2) NULL," & _
        "  [5WPM] varchar(2) NULL," & _
        "  [12WPM] varchar(2) NULL," & _
        "  ADVANCED varchar(2) NULL," & _
        "  BasicHonours varchar(2) NULL," & _
        "  ClubName1 varchar(75) NULL," & _
        "  ClubName2 varchar(75) NULL," & _
        "  ClubAddress varchar(75) NULL," & _
        "  ClubCity varchar(40) NULL," & _
        "  ClubProvince varchar(3) NULL," & _
        "  ClubPostal varchar(10) NULL" & _
        ");"

    Public Sub New()

        Dim result = ""

        Try

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

            result = "Success"

        Catch ex As Exception
            result = ex.ToString
        End Try

        Console.WriteLine(String.Format("Exiting, result: {0}", result))

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

                Dim exists = SQLTableExists(conn, "amateur")
                If Not exists Then
                    Console.WriteLine("Creating Amateur Table")
                    SQLNonQuery(conn, amateurSQL)
                End If

                Dim fileContents = ""
                Using sr As New IO.StreamReader(DataPath & IO.Path.GetFileNameWithoutExtension(file) & ".txt")
                    fileContents = sr.ReadToEnd
                End Using

                Dim pattern = "^(.{7})(.{36})?(.{36})?(.{71})?(.{36})?(.{3})?(.{11})?(.{2})?(.{2})?(.{2})?(.{2})?(.{2})?(.{71})?(.{71})?(.{71})?(.{36})?(.{3})?(.{7})?"
                Dim matches As MatchCollection = Regex.Matches(fileContents, pattern, RegexOptions.Multiline Or RegexOptions.IgnoreCase)
                Dim burnCount = 0
                Dim insertSQL As String = ""

                Dim bulkCopy = New SqlBulkCopy(conn)
                bulkCopy.DestinationTableName = "amateur"
                For i As Integer = 0 To 17
                    bulkCopy.ColumnMappings.Add(i, i)
                Next

                Dim rows As New List(Of DataRow)
                Dim dt As New DataTable
                For i As Integer = 0 To 17
                    dt.Columns.Add()
                Next

                For Each match As Match In matches
                    If burnCount >= 2 Then
                        Dim row As DataRow = dt.NewRow
                        For i As Integer = 0 To 17
                            row(i) = match.Groups(i + 1).Value.Trim.Replace("'", "''")
                        Next

                        rows.Add(row)
                        Console.WriteLine(String.Format("Created insert record #{0}", burnCount - 2))
                    End If

                    burnCount += 1
                Next

                If exists Then
                    Console.WriteLine("Deleting from Amateur Table")
                    SQLNonQuery(conn, "DELETE FROM amateur")
                End If

                bulkCopy.WriteToServer(rows.ToArray)

        End Select

    End Sub

    Private Sub SQLNonQuery(ByRef conn As SqlConnection, sql As String)
        Dim createCmd = New SqlCommand(sql, conn)
        createCmd.ExecuteNonQuery()
    End Sub

    Private Function SQLTableExists(ByRef conn As SqlConnection, name As String) As Boolean
        Dim cmd = New SqlCommand("select case when exists((select * from information_schema.tables where table_name = '" + name + "')) then 1 else 0 end", conn)
        Return Integer.Parse(cmd.ExecuteScalar)
    End Function

End Class
