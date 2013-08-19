Imports SevenZip
Imports System.Data.SqlClient
Imports System.Text.RegularExpressions

Namespace TAFL

    Public Class Parser

        Private Files As String() = {"http://apc-cap.ic.gc.ca/datafiles/amateur.zip"}
        Private DataPath As String = IO.Path.GetTempPath()
        Private dbConn As DB


        Public Sub New()

            Dim result = ""

            Try

                If Environment.MachineName.ToUpper.StartsWith("W7WELLES") Then DataPath = ".\"

                Console.WriteLine("Downloading files")
                DownloadFiles()
                Console.WriteLine("Downloading files complete")

                dbConn = New DB()

                For Each item In IO.Directory.GetFiles(DataPath, "*.zip")

                    Console.WriteLine(String.Format("Unzipping file: {0}", item))
                    Dim archive = New SevenZipExtractor(item)
                    archive.ExtractArchive(DataPath)
                    Console.WriteLine(String.Format("Unzipping complete: {0}", item))

                    Console.WriteLine(String.Format("Parsing: {0}", item))
                    ParseFile(item)
                    Console.WriteLine(String.Format("Parsing complete: {0}", item))

                Next

                dbConn.Conn.Dispose()

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

        Private Sub ParseFile(file As String)

            Select Case True
                Case IO.Path.GetFileName(file).ToUpper = "AMATEUR.ZIP"

                    Dim exists = dbConn.TableExists("amateur")
                    If Not exists Then
                        Console.WriteLine("Creating Amateur Table")
                        dbConn.NonQuery(Services.Amateur.amateurSQL)
                    End If

                    Dim fileContents = ""
                    Using sr As New IO.StreamReader(DataPath & IO.Path.GetFileNameWithoutExtension(file) & ".txt")
                        fileContents = sr.ReadToEnd
                    End Using

                    Dim pattern = "^(.{7})(.{36})?(.{36})?(.{71})?(.{36})?(.{3})?(.{11})?(.{2})?(.{2})?(.{2})?(.{2})?(.{2})?(.{71})?(.{71})?(.{71})?(.{36})?(.{3})?(.{7})?"
                    Dim matches As MatchCollection = Regex.Matches(fileContents, pattern, RegexOptions.Multiline Or RegexOptions.IgnoreCase)
                    Dim burnCount = 0
                    Dim insertSQL As String = ""

                    Dim bulkCopy = New SqlBulkCopy(dbConn.Conn)
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
                        dbConn.NonQuery("DELETE FROM amateur")
                    End If

                    bulkCopy.WriteToServer(rows.ToArray)

            End Select

        End Sub

    End Class

End Namespace
