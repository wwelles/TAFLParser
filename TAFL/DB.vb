Imports System.Data.SqlClient

Public Class DB

    Public Const ConnString As String = "Server=55172d93-4e48-49c7-ba01-a21c001fa435.sqlserver.sequelizer.com;" & _
                                        "Database=db55172d934e4849c7ba01a21c001fa435;" & _
                                        "User ID=kfehbnzhsdtnrgcu;" & _
                                        "Password=KAZmbpavvGsZeo5MozoBCGUJTZX2AwR53VkH6ZiinV5DofoDkhrqbKucPpzizh38;"

    Public Sub NonQuery(sql As String)
        Dim createCmd = New SqlCommand(sql, conn)
        createCmd.ExecuteNonQuery()
    End Sub

    Public Function TableExists(name As String) As Boolean
        Dim cmd = New SqlCommand("select case when exists((select * from information_schema.tables where table_name = '" + name + "')) then 1 else 0 end", Conn)
        Return Integer.Parse(cmd.ExecuteScalar)
    End Function

    Public Function Query(sql As String) As SqlDataReader
        Dim cmd = New SqlCommand(sql, Conn)
        Return cmd.ExecuteReader
    End Function

    Public Conn As New SqlConnection(DB.ConnString)

    Public Sub New()
        Conn.Open()
    End Sub

End Class
