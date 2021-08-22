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

            //SQL bulk Copy data example, using batch size 1000
            SqlBulkCopyData(10000);

            //SQL bulk insert using Stored Proc with batchsize of 1000
            SqlBulkInsertData(10000);
        }


        static void SqlBulkCopyData(int numberOfRecords)
        {
            var referenceId = Guid.NewGuid();
            var random = new Random();

            int count = 0;
            int id = 0;

            //using TransactionScope scope = new(TransactionScopeOption.RequiresNew);
            using SqlConnection sqlcon = new(conString);
            sqlcon.Open();
            var transaction = sqlcon.BeginTransaction();

            try
            {
                DataTable dt = new DataTable();
                //Add columns  
                dt.Columns.Add(new DataColumn("Id", typeof(string)));
                dt.Columns.Add(new DataColumn("CompanyId", typeof(string)));
                dt.Columns.Add(new DataColumn("Age", typeof(string)));
                dt.Columns.Add(new DataColumn("ReferenceId", typeof(Guid)));

                while (count < numberOfRecords)
                {
                    var age = random.Next(20, 60);
                    dt.Rows.Add((++id).ToString(), "100", age.ToString(), referenceId);
                    count++;
                }

                using (var sqlBulk = new SqlBulkCopy(sqlcon, SqlBulkCopyOptions.Default, transaction))
                {
                    sqlBulk.BatchSize = 1000;
                    sqlBulk.DestinationTableName = "Report_EmployeeDetails";
                    sqlBulk.WriteToServer(dt);
                }

                transaction.Commit();
                Console.WriteLine("Test Bulk Insert completed successfully.");
            }
            catch (Exception ex)
            {
                // Do something
                Console.WriteLine("Exception in saving data : " + ex);

                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    // This catch block will handle any errors that may have occurred
                    // on the server that would cause the rollback to fail, such as
                    // a closed connection.
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                }
            }
            finally
            {
                sqlcon.Close();
                sqlcon.Dispose();
            }

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