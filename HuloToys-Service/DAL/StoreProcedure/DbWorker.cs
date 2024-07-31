using HuloToys_Service.Utilities.Lib;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL.StoreProcedure
{
    public class DbWorker
    {
        private static string _connection;
        public IConfiguration configuration;
        public DbWorker(string connection, IConfiguration _configuration)
        {
            _connection = connection;
            configuration = _configuration;
        }

        /// <summary>
        /// Get DataTable
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string procedureName, SqlParameter[] parameters = null)
        {
            DataTable _dataTable = new DataTable();
            try
            {
                using (SqlConnection oConnection = new SqlConnection(_connection))
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
                            LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "SP Name: " + procedureName + "\n" + "Params: " + data_log + "\nGetDataTable - Transaction Rollback - DbWorker: " + ex);

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
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetDataTable - DbWorker: " + ex);
             
            }
            return _dataTable;
        }

        /// <summary>
        /// GET DataSet
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string procedureName, SqlParameter[] parameters = null)
        {
            DataSet _dataSet = new DataSet();
            try
            {
                using (SqlConnection oConnection = new SqlConnection(_connection))
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
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "GetDataSet - DbWorker: " + ex);

            }
            return _dataSet;
        }

        public object ExecuteScalar(String procedureName, SqlParameter[] parameters = null)
        {
            object oReturnValue = null;
            try
            {
                using (SqlConnection oConnection = new SqlConnection(_connection))
                {
                    SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
                    oCommand.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        oCommand.Parameters.AddRange(parameters);
                    }

                    oConnection.Open();

                    using (SqlTransaction oTransaction = oConnection.BeginTransaction())
                    {
                        try
                        {
                            oCommand.Transaction = oTransaction;
                            oReturnValue = oCommand.ExecuteScalar();
                            oTransaction.Commit();
                        }
                        catch
                        {
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
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "ExecuteScalar - DbWorker: " + ex);
            }
            return oReturnValue;
        }

        public int ExecuteNonQuery(string procedureName, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection oConnection = new SqlConnection(_connection))
                {
                    SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
                    oCommand.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        oCommand.Parameters.AddRange(parameters);
                    }
                    SqlParameter OuputParam = oCommand.Parameters.Add("@Identity", SqlDbType.Int);
                    OuputParam.Direction = ParameterDirection.Output;
                    oConnection.Open();
                    using (SqlTransaction oTransaction = oConnection.BeginTransaction())
                    {
                        try
                        {

                            oCommand.Transaction = oTransaction;
                            oCommand.ExecuteNonQuery();
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
                            LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "Dbworker: " + _connection.Split("Persist")[0] + " SP Name: " + procedureName + "\n" + "Params: " + data_log + "\nExecuteNonQuery - Transaction Rollback - DbWorker: " + ex);

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
                    return Convert.ToInt32(OuputParam.Value);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "ExecuteNonQuery - DbWorker: " + ex);

                return -1;
            }
        }

        public void ExecuteNonQueryNoIdentity(string procedureName, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection oConnection = new SqlConnection(_connection))
                {
                    SqlCommand oCommand = new SqlCommand(procedureName, oConnection);
                    oCommand.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        oCommand.Parameters.AddRange(parameters);
                    }

                    oConnection.Open();
                    using (SqlTransaction oTransaction = oConnection.BeginTransaction())
                    {
                        try
                        {
                            oCommand.Transaction = oTransaction;
                            oCommand.ExecuteNonQuery();
                            oTransaction.Commit();
                        }
                        catch
                        {
                            oTransaction.Rollback();
                            oCommand.Parameters.Clear();
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
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "ExecuteNonQueryNoIdentity - DbWorker: " + ex);

            }
        }

        public DataSet ExecuteSqlString(string SqlQuery)
        {
            DataSet _dataSet = new DataSet();
            try
            {
                using (SqlConnection oConnection = new SqlConnection(_connection))
                {
                    SqlCommand oCommand = new SqlCommand("execute_all_data", oConnection);
                    oCommand.CommandType = CommandType.StoredProcedure;
                    if (!string.IsNullOrEmpty(SqlQuery))
                    {
                        oCommand.Parameters.AddWithValue("@SqlCommand", SqlQuery);
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
                        }
                        catch(Exception ex)
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
                // LogHelper.InsertLogTelegram("ExecuteScalar - DbWorker: " + ex);
            }
            return _dataSet;
        }
        public void Fill(DataTable dataTable, string procedureName, SqlParameter[] parameters)
        {
            using (SqlConnection oConnection = new SqlConnection(_connection))
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
                        LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "Fill - DbWorker: " + ex.ToString());

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
                    LogHelper.InsertLogTelegramByUrl(configuration["telegram:log_try_catch:bot_token"], configuration["telegram:log_try_catch:group_id"], "Fill - DbWorker Connection State: " + oConnection.State);

            }
        }
    }
}