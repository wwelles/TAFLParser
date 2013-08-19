Imports System.Net
Imports System.Web.Http

Public Class SearchCallSignController
    Inherits ApiController

    ' GET api/searchcallsign/5
    Public Function GetValues(id As String) As IEnumerable(Of TAFL.Models.Amateur)
        Return TAFL.Services.Amateur.LoadByCallsign(id)
    End Function

End Class
