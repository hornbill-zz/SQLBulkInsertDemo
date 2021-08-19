create procedure [dbo].[InsertEmployeeDetails]
	@EmployeeDetail EmployeeDetail readonly
as  
begin  
   insert into Report_EmployeeDetails 
	select Id
		, Companyid
		, age
		, ReferenceId 
	from @EmployeeDetail  
end 