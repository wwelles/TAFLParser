Imports SevenZip

Public Class Parser

    Public Files = {"http://apc-cap.ic.gc.ca/datafiles/amateur.zip"}

    Public Sub New()

        DownloadFiles()

        For Each item In IO.Directory.GetFiles(IO.Path.GetTempPath(), "*.zip")
            Debug.WriteLine(String.Format("Unzipping file: {0}", item))

            Dim archive = New SevenZipExtractor(item)
            archive.ExtractArchive(IO.Path.GetTempPath())

            Debug.WriteLine(String.Format("Unzipping complete: {0}", item))

            Select Case True
                Case IO.Path.GetFileName(item.ToUpper) = "AMATEUR.ZIP"
                    Debug.WriteLine(String.Format("Parsing: {0}", item))
                    'ParseFile(item)

            End Select
        Next

    End Sub

    Private Sub DownloadFiles()
        For Each file In Files
            Try
                Dim test = New Net.WebClient
                test.DownloadFile(file, IO.Path.GetTempPath() & IO.Path.GetFileName(file))
                Debug.WriteLine(String.Format("Download of {0} successful", file))
            Catch ex As Exception
                Debug.WriteLine(String.Format("Download of {0} failed: {1}", file, ex.ToString))
            End Try
        Next
    End Sub

End Class
