using App_Push_Consummer.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Model.DB_Core
{
    public class DBWorker
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];

        private static string startupPath = AppDomain.CurrentDomain.BaseDirectory;

        private static string connection = ConfigurationManager.AppSettings["hulotoys_db"];

        public static void Fill(DataTable dataTable, string procedureName, SqlParameter[] parameters)
        {
            using (SqlConnection oConnection = new SqlConnection(connection))
            {
                SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
                oCommand.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    oCommand.Parameters.AddRange(parameters);
                }
                SqlDataAdapter oAdapter = new SqlDataAdapter();
                oAdapter.SelectCommand = oCommand;
                oConnection.Open();

                using (SqlTransaction oTransaction = oConnection.BeginTransaction())
                {
                    try
                    {
                        oAdapter.SelectCommand.Transaction = oTransaction;
                        oAdapter.Fill(dataTable);
                        oTransaction.Commit();
                    }
                    catch (Exception ex)
                    {                        
                        ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Fill Error = " + ex.ToString());
                        ErrorWriter.WriteLog(startupPath, "Procedure Fill Data Table with param " + ex.ToString());
                        oTransaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        if (oConnection.State == ConnectionState.Open)
                        {
                            oConnection.Close();
                        }
                        oConnection.Dispose();
                        oAdapter.Dispose();
                    }
                }
                if (oConnection.State != ConnectionState.Closed)
                    ErrorWriter.WriteLog(startupPath, "Procedure Fill Data Table with param", oConnection.State.ToString());
            }
        }
        public static void Fill(DataSet dataSet, string procedureName, int rp = -1)
        {
            using (SqlConnection oConnection = new SqlConnection((rp == -1) ? connection : connection))
            {
                SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
                oCommand.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter oAdapter = new SqlDataAdapter();
                oAdapter.SelectCommand = oCommand;
                oConnection.Open();

                using (SqlTransaction oTransaction = oConnection.BeginTransaction())
                {
                    try
                    {
                        oAdapter.SelectCommand.Transaction = oTransaction;
                        oAdapter.Fill(dataSet);
                        oTransaction.Commit();
                    }
                    catch (Exception ex)
                    {                        
                        ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Fill Error = " + ex.ToString());
                        oTransaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        if (oConnection.State == ConnectionState.Open)
                        {
                            oConnection.Close();
                        }
                        oConnection.Dispose();
                        oAdapter.Dispose();
                    }
                }
                if (oConnection.State != ConnectionState.Closed)
                {
                    ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Fill Error = oConnection.State != ConnectionState.Closed)");
                }
            }
        }

        public static int ExecuteNonQuery(string procedureName, SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection oConnection = new SqlConnection(connection))
                {
                    SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
                    oCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter OuputParam = oCommand.Parameters.Add("@Identity", SqlDbType.Int);
                    OuputParam.Direction = ParameterDirection.Output;
                    oConnection.Open();

                    using (SqlTransaction oTransaction = oConnection.BeginTransaction())
                    {
                        try
                        {
                            if (parameters != null)
                            {
                                oCommand.Parameters.AddRange(parameters);
                            }
                            oCommand.Transaction = oTransaction;


                            oCommand.ExecuteNonQuery();
                            oTransaction.Commit();
                        }
                        catch (Exception ex)
                        {                                                        
                            ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "oTransaction Error = " + ex.ToString());
                            oTransaction.Rollback();
                            oCommand.Parameters.Clear();
                            return -1;
                        }
                        finally
                        {
                            oCommand.Parameters.Clear();
                            if (oConnection.State == ConnectionState.Open)
                            {
                                oConnection.Close();
                            }
                            oConnection.Dispose();
                            oCommand.Dispose();
                        }
                    }

                    if (oConnection.State != ConnectionState.Closed)                        
                        ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "Procedure EXecute non query with param => error queue = " + oConnection.State.ToString());
                    return Convert.ToInt32(OuputParam.Value);
                }
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "ExecuteNonQuery => error queue = " + ex.ToString());
                return -1;
            }
        }

        public static DataSet ExecuteQuery(string procedureName, int rp = -1)
        {
            using (SqlConnection oConnection = new SqlConnection((rp == -1) ? connection : connection))
            {
                SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
                oCommand.CommandType = CommandType.StoredProcedure;
                DataSet oReturnValue = new DataSet();
                oConnection.Open();

                using (SqlTransaction oTransaction = oConnection.BeginTransaction())
                {
                    try
                    {
                        oCommand.Transaction = oTransaction;
                        Fill(oReturnValue, procedureName, rp);
                        oTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "ExecuteQuery => error queue = " + ex.ToString());
                        oTransaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        if (oConnection.State == ConnectionState.Open)
                        {
                            oConnection.Close();
                        }
                        oConnection.Dispose();
                        oCommand.Dispose();

                    }
                }
                if (oConnection.State != ConnectionState.Closed)
                {

                }
                return oReturnValue;
            }
        }
        public static DataSet GetDataSet(string procedureName, SqlParameter[] parameters = null)
        {
            DataSet _dataSet = new DataSet();
            try
            {
                using (SqlConnection oConnection = new SqlConnection(connection))
                {
                    SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
                    oCommand.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        oCommand.Parameters.AddRange(parameters);
                    }
                    SqlDataAdapter oAdapter = new SqlDataAdapter();
                    oAdapter.SelectCommand = oCommand;
                    oConnection.Open();

                    using (SqlTransaction oTransaction = oConnection.BeginTransaction())
                    {
                        try
                        {
                            oAdapter.SelectCommand.Transaction = oTransaction;
                            oAdapter.Fill(_dataSet);
                            oTransaction.Commit();
                            oCommand.Parameters.Clear();
                        }
                        catch
                        {
                            oTransaction.Rollback();
                        }
                        finally
                        {
                            if (oConnection.State == ConnectionState.Open)
                            {
                                oConnection.Close();
                            }
                            oConnection.Dispose();
                            oAdapter.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {                
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset => error queue = " + ex.ToString());
            }
            return _dataSet;
        }
        public static DataTable GetDataTable(string procedureName, SqlParameter[] parameters = null)
        {
            DataTable _dataTable = new DataTable();
            try
            {
                using (SqlConnection oConnection = new SqlConnection(connection))
                {
                    SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
                    oCommand.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        oCommand.Parameters.AddRange(parameters);
                    }

                    SqlDataAdapter oAdapter = new SqlDataAdapter();
                    oAdapter.SelectCommand = oCommand;
                    oConnection.Open();

                    using (SqlTransaction oTransaction = oConnection.BeginTransaction())
                    {
                        try
                        {
                            oAdapter.SelectCommand.Transaction = oTransaction;
                            oAdapter.Fill(_dataTable);
                            oTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            oTransaction.Rollback();
                            string data_log = "";
                            if (parameters != null && parameters.Length > 0)
                            {
                                data_log = string.Join(",", parameters.Select(x => x.ParameterName)) + ":" + string.Join(",", parameters.Select(x => x.Value == null ? "NULL" : x.Value.ToString()));

                            }
                            ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "SP Name: " + procedureName + "\n" + "Params: " + data_log + "\nGetDataTable - Transaction Rollback - DbWorker: " + ex);
                            throw;
                        }
                        finally
                        {
                            if (oConnection.State == ConnectionState.Open)
                            {
                                oConnection.Close();
                            }
                            oConnection.Dispose();
                            oAdapter.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataTable - DbWorker: " + ex);
            }
            return _dataTable;
        }
    }
}
