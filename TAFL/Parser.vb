Imports SevenZip

Public Class Parser

    Public Files = {"http://apc-cap.ic.gc.ca/datafiles/amateur.zip"}

    Public Sub New()

        DownloadFiles()

        For Each item In IO.Directory.GetFiles(IO.Path.GetTempPath(), "*.zip")
            Dim archive = New SevenZipExtractor(item)
            archive.ExtractArchive(".\Data\")

            Select Case True
                Case IO.Path.GetFileName(item.ToUpper) = "AMATEUR.ZIP"
                    'ParseFile(item)

            End Select
        Next

    End Sub

    Private Sub DownloadFiles()
        For Each file In Files
            Dim test = New Net.WebClient
            test.DownloadFile(file, IO.Path.GetTempPath() & IO.Path.GetFileName(file))
        Next
    End Sub

End Class
