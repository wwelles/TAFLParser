Namespace TAFL.Services

Public Class Amateur

        Public Const amateurSQL = _
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

        Public Shared Function LoadByCallsign(search As String) As List(Of TAFL.Models.Amateur)
            Dim dbConn As New DB
            Dim values As New List(Of TAFL.Models.Amateur)

            Dim results = dbConn.Query(String.Format("SELECT * FROM amateur WHERE Callsign LIKE N'%{0}%'", search))

            While results.Read()
                values.Add(fillModel(CType(results, IDataRecord)))
            End While

            Return values

        End Function

        Public Shared Function LoadByName(search As String) As List(Of TAFL.Models.Amateur)
            Dim dbConn As New DB
            Dim values As New List(Of TAFL.Models.Amateur)

            Dim results = dbConn.Query(String.Format("SELECT * FROM amateur WHERE Names LIKE N'%{0}%' OR Surname LIKE N'%{0}%'", search))

            While results.Read()
                values.Add(fillModel(CType(results, IDataRecord)))
            End While

            Return values

        End Function

        Private Shared Function fillModel(result As IDataRecord) As TAFL.Models.Amateur

            Dim record As New TAFL.Models.Amateur

            record.Callsign = result("Callsign")
            record.Names = result("Names")
            record.Surname = result("Surname")
            record.Address = result("Address")
            record.City = result("City")
            record.Province = result("Province")
            record.Postal = result("Postal")
            record.Qualification_BASIC = result("BASIC")
            record.Qualification_5WPM = result("5WPM")
            record.Qualification_12WPM = result("12WPM")
            record.Qualification_ADVANCED = result("ADVANCED")
            record.Qualification_BasicHonours = result("BasicHonours")
            record.ClubName1 = result("ClubName1")
            record.ClubName2 = result("ClubName2")
            record.ClubAddress = result("ClubAddress")
            record.ClubCity = result("ClubCity")
            record.ClubProvince = result("ClubProvince")
            record.ClubPostal = result("ClubPostal")

            Return record

        End Function

    End Class

End Namespace
