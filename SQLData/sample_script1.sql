-- Schema and user defined table type

	Create Table Report_EmployeeDetails (
		Id	nvarchar(20),
		CompanyId nvarchar(20),
		Age	int,
		ReferenceId uniqueidentifier
	)
	GO

    CREATE TYPE [dbo].[EmployeeDetail] AS TABLE(  
        Id	nvarchar(20),
		CompanyId nvarchar(20),
		Age	int,
		ReferenceId uniqueidentifier   
    )  
	GO


