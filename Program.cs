using System;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace SqlBulkInsert
{
    class Program
    {

        static string conString = "Data Source=MSI;Initial Catalog=Test;Integrated Security=true";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            SqlBulkInsertData(10000);
        }


        static void SqlBulkInsertData(int numberOfRecords)
        {
            var referenceId = Guid.NewGuid();
            var batchSize = 1000;
            var random = new Random();

            int count = 0;
            int id = 0;

            using TransactionScope scope = new(TransactionScopeOption.RequiresNew);
            using SqlConnection sqlcon = new(conString);

            try
            {
                DataTable dt = new DataTable();
                //Add columns  
                dt.Columns.Add(new DataColumn("Id", typeof(string)));
                dt.Columns.Add(new DataColumn("CompanyId", typeof(string)));
                dt.Columns.Add(new DataColumn("Age", typeof(string)));
                dt.Columns.Add(new DataColumn("ReferenceId", typeof(Guid)));
                
                SqlCommand sqlcom = new("InsertEmployeeDetails", sqlcon);
                sqlcom.CommandType = CommandType.StoredProcedure;
                sqlcon.Open();

                while (count < numberOfRecords)
                {
                    for (int x = 0; x < batchSize; x++)
                    {
                        var age = random.Next(20, 60);
                        dt.Rows.Add((++id).ToString(), "100", age.ToString(), referenceId);
                        count++;
                    }

                    sqlcom.Parameters.AddWithValue("@EmployeeDetail", dt);
                    sqlcom.ExecuteNonQuery();

                    sqlcom.Parameters.Clear();
                    dt.Clear();
                }

                scope.Complete();
                Console.WriteLine("Test Bulk Insert completed successfully.");
            }
            catch (Exception ex)
            {
                // Do something
                Console.WriteLine("Exception in saving data : " + ex);
            }
            finally
            {
                sqlcon.Close();
                sqlcon.Dispose();
                
                scope.Dispose();
            }
            
        }
    }
}
