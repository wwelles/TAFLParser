Imports System.Net
Imports System.Web.Http

Public Class SearchNameController
    Inherits ApiController

    ' GET api/searchname/Test
    Public Function GetValues(id As String) As IEnumerable(Of TAFL.Models.Amateur)
        Return TAFL.Services.Amateur.LoadByName(id)
    End Function

End Class
