Imports SevenZip

Public Class Parser

    Public Files = {"http://apc-cap.ic.gc.ca/datafiles/amateur.zip"}

    Public Sub New()

        Console.WriteLine("Downloading files")

        DownloadFiles()

        Console.WriteLine("Downloading files complete")

        For Each item In IO.Directory.GetFiles(IO.Path.GetTempPath(), "*.zip")
            Console.WriteLine(String.Format("Unzipping file: {0}", item))

            Dim archive = New SevenZipExtractor(item)
            archive.ExtractArchive(IO.Path.GetTempPath())

            Console.WriteLine(String.Format("Unzipping complete: {0}", item))

            Select Case True
                Case IO.Path.GetFileName(item.ToUpper) = "AMATEUR.ZIP"
                    Console.WriteLine(String.Format("Parsing: {0}", item))
                    'ParseFile(item)

            End Select
        Next

    End Sub

    Private Sub DownloadFiles()
        For Each file In Files
            Try
                Dim test = New Net.WebClient
                test.DownloadFile(file, IO.Path.GetTempPath() & IO.Path.GetFileName(file))
                Console.WriteLine(String.Format("Download of {0} successful", file))
            Catch ex As Exception
                Console.WriteLine(String.Format("Download of {0} failed: {1}", file, ex.ToString))
            End Try
        Next
    End Sub

End Class
