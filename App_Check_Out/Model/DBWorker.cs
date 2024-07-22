﻿using APP.CHECKOUT_SERVICE.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace APP.CHECKOUT_SERVICE.Model
{
    public class DBWorker
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];

        private static string startupPath = AppDomain.CurrentDomain.BaseDirectory;

        private static string connection = ConfigurationManager.AppSettings["CPMS_DFP_REPORT"];

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
                        Telegram.pushLog("Fill " + ex.ToString());
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
                        Telegram.pushLog("Fill " + ex.ToString());
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
                if (oConnection.State != ConnectionState.Closed) ;

            }
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
                            Telegram.pushLog("Error " + ex.ToString());
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
                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Error " + ex.ToString());

            }
            return _dataTable;
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
                            //  String er = "<========|| storedName: " + procedureName + " ||" + string.Join("=#=", parameters.Select(x => x.SqlValue).ToList()) + "||=======>";
                            // ErrorWriter.WriteLog(startupPath, "Error", ex.ToString());
                            //  ErrorWriter.WriteLog(startupPath, "Error", ex.ToString());
                           Telegram.pushLog("Error " + ex.ToString());
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
                        Telegram.pushLog("Procedure EXecute non query with param" + oConnection.State.ToString());

                    return Convert.ToInt32(OuputParam.Value);
                }
            }
            catch (Exception ex)
            {
                Telegram.pushLog("ExecuteNonQuery " + ex.ToString());
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
                        Telegram.pushLog("ExecuteQuery " + ex.ToString());
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
                Telegram.pushLog("GetDataset " + ex.ToString());
            }
            return _dataSet;
        }
    }
}
