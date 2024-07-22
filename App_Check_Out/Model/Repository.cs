using APP.CHECKOUT_SERVICE.Common;
using APP.CHECKOUT_SERVICE.Contants;
using APP.CHECKOUT_SERVICE.ViewModel.Mail;
using APP.CHECKOUT_SERVICE.ViewModel.Order;
using APP.CHECKOUT_SERVICE.ViewModel.Order.HotelRent;
using APP.CHECKOUT_SERVICE.ViewModel.Order.VinWonder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utilities.Contants;

namespace APP.CHECKOUT_SERVICE.Model
{
    public static class Repository
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];

        /// <summary>
        /// example: get data
        /// </summary>
        /// <param name="order_id"></param>      
        /// <returns></returns>
        public static OrderInfo getOrderDetail(int order_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@order_id", order_id);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "sp_getOrderDetail", objParam);

                return tb.ToList<OrderInfo>().SingleOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getOrderDetail" + ex.ToString());
                return null;
            }
        }

        public static OrderInfo getOrderDetailBigint(long order_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@order_id", order_id);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "sp_getOrderDetail", objParam);

                return tb.ToList<OrderInfo>().SingleOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getOrderDetail" + ex.ToString());
                return null;
            }
        }

        public static Client getClientDetail(long clientId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ClientID", clientId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetClientByID", objParam);

                return tb.ToList<Client>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getClientDetail" + ex.ToString());
                return null;
            }

        }

        public static Client getClientDetailByAccountClientId(long account_client_id)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@AccountClientID", account_client_id);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetClientByAccountClientId", objParam);

                return tb.ToList<Client>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getClientDetail" + ex.ToString());
                return null;
            }

        }

        public static FlightSegment getFlightSegment(long flyBookingDetailId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@flyBookingDetailId", flyBookingDetailId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetFlightSegment", objParam);

                return tb.ToList<FlightSegment>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getFlightSegment" + ex);
                return null;
            }

        }

        public static List<FlightSegment> getFlightSegmentList(List<long> flyBookingDetailIds)
        {
            try
            {
                List<FlightSegment> flightSegments = new List<FlightSegment>();
                foreach (var item in flyBookingDetailIds)
                {
                    SqlParameter[] objParam = new SqlParameter[1];
                    objParam[0] = new SqlParameter("@flyBookingDetailIds", item);

                    DataTable tb = new DataTable();
                    DBWorker.Fill(tb, "SP_GetFlightSegmentList", objParam);

                    var result = tb.ToList<FlightSegment>().FirstOrDefault();
                    if (result != null)
                        flightSegments.Add(result);

                }
                return flightSegments;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getFlightSegmentList" + ex);
                return null;
            }

        }

        public static ContactClient getContactClient(int contact_client_id)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@Id", contact_client_id);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetContactClient", objParam);

                return tb.ToList<ContactClient>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getContactClient" + ex);
                return null;
            }

        }

        public static FlyBookingDetail getFlyBookingDetail(long orderId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@orderId", orderId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetFlyBookingDetail", objParam);

                return tb.ToList<FlyBookingDetail>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getFlyBookingDetail" + ex);
                return null;
            }

        }

        public static List<FlyBookingDetail> getFlyBookingDetailList(long orderId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@orderId", orderId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetFlyBookingDetail", objParam);

                return tb.ToList<FlyBookingDetail>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getFlyBookingDetailList" + ex);
                return null;
            }

        }

        public static List<AirPortCode> getAllAirportCode()
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[0];
                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetAllAirportCode", objParam);
                return tb.ToList<AirPortCode>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getAllAirportCode" + ex);
                return null;
            }
        }

        public static List<Airlines> getAirlinesCodes(List<String> lstCode)
        {
            try
            {
                var listAirline = new List<Airlines>();
                foreach (var item in lstCode)
                {
                    SqlParameter[] objParam = new SqlParameter[1];
                    objParam[0] = new SqlParameter("@airlinesCodes", string.Join(",", lstCode));
                    DataTable tb = new DataTable();
                    DBWorker.Fill(tb, "SP_GetAirlinesCodes", objParam);
                    var result = tb.ToList<Airlines>().FirstOrDefault();
                    if (result != null)
                        listAirline.Add(result);
                }
                return listAirline;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getAirlinesCodes" + ex);
                return null;
            }
        }

        public static List<Airlines> getAllAirlines()
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[0];
                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetAllAirlines", objParam);
                return tb.ToList<Airlines>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getAirlinesCodes" + ex);
                return null;
            }
        }

        public static List<Passenger> getListPassenger(int orderId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@orderId", orderId);
                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetListPassenger", objParam);
                return tb.ToList<Passenger>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getListPassenger" + ex);
                return null;
            }
        }

        public static List<Baggage> getListBaggage(List<int> listPassengerId)
        {
            try
            {
                List<Baggage> baggages = new List<Baggage>();
                foreach (var item in listPassengerId)
                {
                    SqlParameter[] objParam = new SqlParameter[1];
                    objParam[0] = new SqlParameter("@listPassengerId", item);
                    DataTable tb = new DataTable();
                    DBWorker.Fill(tb, "SP_GetListBaggage", objParam);
                    var result = tb.ToList<Baggage>().ToList();
                    foreach (var baggage in result)
                    {
                        baggages.Add(baggage);
                    }
                }
                return baggages;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getListBaggage" + ex);
                return null;
            }
        }

        public static GroupClassAirlines getGroupClassAirlines(string classCode, string airline, string fairtype)
        {
            try
            {
                if (classCode.Contains("_ECO")) classCode = "_ECO";
                if (classCode.Contains("_DLX")) classCode = "_DLX";
                if (classCode.Contains("_BOSS")) classCode = "_BOSS";
                if (classCode.Contains("_SBOSS")) classCode = "_SBOSS";
                if (classCode.Contains("_Combo")) classCode = "_Combo";
                if (airline.ToLower().Equals("vu")) classCode = "";
                SqlParameter[] objParam = new SqlParameter[3];
                objParam[0] = new SqlParameter("@classCode", classCode);
                objParam[1] = new SqlParameter("@airline", airline);
                objParam[2] = new SqlParameter("@fairtype", fairtype);
                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetGroupClassAirlines", objParam);
                return tb.ToList<GroupClassAirlines>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getAllAirportCode" + ex);
                return null;
            }
        }

        public static DataSet CheckIfNewOrderValid(OrderViewModel obj_order)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[3];
                if (obj_order.ContractId == null)
                {
                    objParam[0] = new SqlParameter("@ContractID", DBNull.Value);
                }
                else
                {
                    objParam[0] = new SqlParameter("@ContractID", obj_order.ContractId);
                }
                objParam[1] = new SqlParameter("@OrderNo", obj_order.OrderNo);
                objParam[2] = new SqlParameter("@AccountClientID", obj_order.AccountClientId);
                return DBWorker.GetDataSet(StoreProcedureConstant.CheckIfNewOrderValid, objParam);
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CheckIfNewOrderValid" + ex.ToString());
                return null;
            }
        }

        public static Client GetClientByAccountClientId(long account_client_id)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@AccountClientID", account_client_id);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, StoreProcedureConstant.GetClientByAccountClientID, objParam);

                return tb.ToList<Client>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository GetClientByAccountClientId" + ex.ToString());
                return null;
            }
        }

        public static int CreateContactClients(ContactClientViewModel obj_contact_client, long account_client_id)
        {
            try
            {

                SqlParameter[] objParam_cclient = new SqlParameter[6];
                objParam_cclient[0] = new SqlParameter("@Name", obj_contact_client.Name);
                objParam_cclient[1] = new SqlParameter("@Mobile", obj_contact_client.Mobile);
                objParam_cclient[2] = new SqlParameter("@Email", obj_contact_client.Email);
                objParam_cclient[3] = new SqlParameter("@CreateDate", obj_contact_client.CreateDate);
                objParam_cclient[4] = new SqlParameter("@AccountClientId", account_client_id);
                objParam_cclient[5] = new SqlParameter("@OrderId", obj_contact_client.OrderId);
                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateContactClients, objParam_cclient);
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CheckIfNewOrderValid" + ex.ToString());
                return -1;
            }
        }

        public static int CreateOrder(FlyBookingOrderSummit order_info)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[34];
                objParam_order[0] = new SqlParameter("@OrderNo", order_info.obj_order.OrderNo);
                objParam_order[1] = new SqlParameter("@ServiceType", order_info.obj_order.ServiceType);
                objParam_order[2] = new SqlParameter("@Amount", order_info.obj_order.Amount);
                objParam_order[3] = new SqlParameter("@CreateTime", order_info.obj_order.CreateTime);
                objParam_order[4] = new SqlParameter("@ClientId", order_info.obj_order.ClientId);
                objParam_order[5] = new SqlParameter("@ContactClientId", order_info.obj_contact_client.Id);
                objParam_order[6] = new SqlParameter("@OrderStatus", order_info.obj_order.OrderStatus);
                if (order_info.obj_order.ContractId != null)
                {
                    objParam_order[7] = new SqlParameter("@ContractId", order_info.obj_order.ContractId);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@ContractId", DBNull.Value);
                }
                if (order_info.obj_order.PaymentType == null)
                {
                    order_info.obj_order.PaymentType = 0;
                }
                objParam_order[8] = new SqlParameter("@PaymentType", order_info.obj_order.PaymentType);
                objParam_order[9] = new SqlParameter("@BankCode", order_info.obj_order.BankCode);
                if (order_info.obj_order.PaymentDate != null)
                {
                    objParam_order[10] = new SqlParameter("@PaymentDate", order_info.obj_order.PaymentDate);
                }
                else
                {
                    objParam_order[10] = new SqlParameter("@PaymentDate", DBNull.Value);
                }

                if (order_info.obj_order.PaymentNo != null)
                {
                    objParam_order[11] = new SqlParameter("@PaymentNo", order_info.obj_order.PaymentNo);
                }
                else
                {
                    objParam_order[11] = new SqlParameter("@PaymentNo", DBNull.Value);
                }
                objParam_order[12] = new SqlParameter("@Profit", order_info.obj_order.Profit);
                objParam_order[13] = new SqlParameter("@Discount", order_info.obj_order.Discount);
                objParam_order[14] = new SqlParameter("@PaymentStatus", order_info.obj_order.PaymentStatus);
                objParam_order[15] = new SqlParameter("@OrderId", order_info.obj_order.Id);
                objParam_order[16] = new SqlParameter("@ExpriryDate", order_info.obj_order.ExpriryDate);
                objParam_order[17] = new SqlParameter("@ProductService", order_info.obj_order.ProductService.ToString());
                objParam_order[18] = new SqlParameter("@AccountClientId", order_info.obj_order.AccountClientId);
                objParam_order[19] = new SqlParameter("@StartDate", order_info.obj_order.StartDate);
                objParam_order[20] = new SqlParameter("@EndDate", order_info.obj_order.EndDate);
                objParam_order[21] = new SqlParameter("@SystemType", order_info.obj_order.SystemType);
                objParam_order[22] = new SqlParameter("@SalerId", order_info.obj_order.SalerId);
                if (order_info.obj_order.SalerId != null)
                {
                    objParam_order[22] = new SqlParameter("@SalerId", order_info.obj_order.SalerId);
                }
                else
                {
                    objParam_order[22] = new SqlParameter("@SalerId", DBNull.Value);
                }
                objParam_order[23] = new SqlParameter("@SalerGroupId", order_info.obj_order.SalerGroupId);
                objParam_order[24] = new SqlParameter("@UserUpdateId", order_info.obj_order.UserUpdateId);

                if (order_info.obj_order.PercentDecrease != null)
                {
                    objParam_order[25] = new SqlParameter("@PercentDecrease", order_info.obj_order.PercentDecrease);
                }
                else
                {
                    objParam_order[25] = new SqlParameter("@PercentDecrease", DBNull.Value);
                }

                if (order_info.obj_order.Price != null)
                {
                    objParam_order[26] = new SqlParameter("@Price", order_info.obj_order.Price);
                }
                else
                {
                    objParam_order[26] = new SqlParameter("@Price", DBNull.Value);
                }

                if (order_info.obj_order.Label != null)
                {
                    objParam_order[27] = new SqlParameter("@Label", order_info.obj_order.Label);
                }
                else
                {
                    objParam_order[27] = new SqlParameter("@Label", DBNull.Value);
                }

                if (order_info.obj_order.VoucherId != null)
                {
                    objParam_order[28] = new SqlParameter("@VoucherId", order_info.obj_order.VoucherId);
                }
                else
                {
                    objParam_order[28] = new SqlParameter("@VoucherId", DBNull.Value);
                }
                objParam_order[29] = new SqlParameter("@CreatedBy", order_info.obj_order.CreatedBy);
                order_info.obj_order.SupplierId = 0;
                objParam_order[30] = new SqlParameter("@SupplierId", order_info.obj_order.SupplierId);
                objParam_order[31] = new SqlParameter("@Note", order_info.obj_order.Note);
                objParam_order[32] = new SqlParameter("@UtmMedium", order_info.obj_order.UtmMedium);
                if (order_info.obj_order.UtmSource != null)
                {
                    objParam_order[33] = new SqlParameter("@UtmSource", order_info.obj_order.UtmSource);
                }
                else
                {
                    objParam_order[33] = new SqlParameter("@UtmSource", DBNull.Value);
                }


                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateOrder, objParam_order);
                order_info.obj_order.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CreateOrder" + ex.ToString());
                return -1;
            }
        }

        public static int CreateOrder(HotelRentOrderSummit order_info)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[34];
                objParam_order[0] = new SqlParameter("@OrderNo", order_info.obj_order.OrderNo);
                objParam_order[1] = new SqlParameter("@ServiceType", order_info.obj_order.ServiceType);
                objParam_order[2] = new SqlParameter("@Amount", order_info.obj_order.Amount);
                objParam_order[3] = new SqlParameter("@CreateTime", order_info.obj_order.CreateTime);
                objParam_order[4] = new SqlParameter("@ClientId", order_info.obj_order.ClientId);
                objParam_order[5] = new SqlParameter("@ContactClientId", order_info.obj_contact_client.Id);
                objParam_order[6] = new SqlParameter("@OrderStatus", order_info.obj_order.OrderStatus);
                if (order_info.obj_order.ContractId != null)
                {
                    objParam_order[7] = new SqlParameter("@ContractId", order_info.obj_order.ContractId);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@ContractId", DBNull.Value);
                }
                objParam_order[8] = new SqlParameter("@PaymentType", order_info.obj_order.PaymentType);
                objParam_order[9] = new SqlParameter("@BankCode", order_info.obj_order.BankCode);
                if (order_info.obj_order.PaymentDate != null)
                {
                    objParam_order[10] = new SqlParameter("@PaymentDate", order_info.obj_order.PaymentDate);
                }
                else
                {
                    objParam_order[10] = new SqlParameter("@PaymentDate", DBNull.Value);
                }

                if (order_info.obj_order.PaymentNo != null)
                {
                    objParam_order[11] = new SqlParameter("@PaymentNo", order_info.obj_order.PaymentNo);
                }
                else
                {
                    objParam_order[11] = new SqlParameter("@PaymentNo", DBNull.Value);
                }
                objParam_order[12] = new SqlParameter("@Profit", order_info.obj_order.Profit);
                objParam_order[13] = new SqlParameter("@Discount", order_info.obj_order.Discount);
                objParam_order[14] = new SqlParameter("@PaymentStatus", order_info.obj_order.PaymentStatus);
                objParam_order[15] = new SqlParameter("@OrderId", order_info.obj_order.Id);
                objParam_order[16] = new SqlParameter("@ExpriryDate", order_info.obj_order.ExpriryDate);
                objParam_order[17] = new SqlParameter("@ProductService", order_info.obj_order.ProductService.ToString());
                objParam_order[18] = new SqlParameter("@AccountClientId", order_info.obj_order.AccountClientId);
                objParam_order[19] = new SqlParameter("@StartDate", order_info.obj_order.StartDate);
                objParam_order[20] = new SqlParameter("@EndDate", order_info.obj_order.EndDate);
                objParam_order[21] = new SqlParameter("@SystemType", order_info.obj_order.SystemType);

                if (order_info.obj_order.SalerId != null)
                {
                    objParam_order[22] = new SqlParameter("@SalerId", order_info.obj_order.SalerId);
                }
                else
                {
                    objParam_order[22] = new SqlParameter("@SalerId", DBNull.Value);
                }
                objParam_order[23] = new SqlParameter("@SalerGroupId", order_info.obj_order.SalerGroupId);
                objParam_order[24] = new SqlParameter("@UserUpdateId", order_info.obj_order.UserUpdateId);
                if (order_info.obj_order.PercentDecrease != null)
                {
                    objParam_order[25] = new SqlParameter("@PercentDecrease", order_info.obj_order.PercentDecrease);
                }
                else
                {
                    objParam_order[25] = new SqlParameter("@PercentDecrease", DBNull.Value);
                }

                if (order_info.obj_order.Price != null)
                {
                    objParam_order[26] = new SqlParameter("@Price", order_info.obj_order.Price);
                }
                else
                {
                    objParam_order[26] = new SqlParameter("@Price", DBNull.Value);
                }

                if (order_info.obj_order.Label != null)
                {
                    objParam_order[27] = new SqlParameter("@Label", order_info.obj_order.Label);
                }
                else
                {
                    objParam_order[27] = new SqlParameter("@Label", DBNull.Value);
                }

                if (order_info.obj_order.VoucherId != null)
                {
                    objParam_order[28] = new SqlParameter("@VoucherId", order_info.obj_order.VoucherId);
                }
                else
                {
                    objParam_order[28] = new SqlParameter("@VoucherId", DBNull.Value);
                }
                objParam_order[29] = new SqlParameter("@CreatedBy", order_info.obj_order.CreatedBy);
                order_info.obj_order.SupplierId = 0;
                objParam_order[30] = new SqlParameter("@SupplierId", order_info.obj_order.SupplierId);
                objParam_order[31] = new SqlParameter("@Note", order_info.obj_order.Note);
                if (order_info.obj_order.UtmMedium != null)
                {
                    objParam_order[32] = new SqlParameter("@UtmMedium", order_info.obj_order.UtmMedium );
                }
                else
                {
                    objParam_order[32] = new SqlParameter("@UtmMedium", DBNull.Value);
                }
                if (order_info.obj_order.UtmSource != null)
                {
                    objParam_order[33] = new SqlParameter("@UtmSource", order_info.obj_order.UtmSource);
                }
                else
                {
                    objParam_order[33] = new SqlParameter("@UtmSource", DBNull.Value);
                }

                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateOrder, objParam_order);
                order_info.obj_order.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CreateOrder" + ex.ToString());
                return -1;
            }
        }
        public static int CreateOrder(OrderViewModel order_info, int? contact_client_id)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[34];
               // SqlParameter[] objParam_order = new SqlParameter[31];
                objParam_order[0] = new SqlParameter("@OrderNo", order_info.OrderNo);
                objParam_order[1] = new SqlParameter("@ServiceType", order_info.ServiceType);
                objParam_order[2] = new SqlParameter("@Amount", order_info.Amount);
                objParam_order[3] = new SqlParameter("@CreateTime", order_info.CreateTime);
                objParam_order[4] = new SqlParameter("@ClientId", order_info.ClientId);
                objParam_order[5] = new SqlParameter("@ContactClientId", contact_client_id);
                objParam_order[6] = new SqlParameter("@OrderStatus", order_info.OrderStatus);
                if (order_info.ContractId != null)
                {
                    objParam_order[7] = new SqlParameter("@ContractId", order_info.ContractId);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@ContractId", DBNull.Value);
                }
                if (order_info.PaymentType == null)
                {
                    order_info.PaymentType = 0;
                }
                objParam_order[8] = new SqlParameter("@PaymentType", order_info.PaymentType);
                objParam_order[9] = new SqlParameter("@BankCode", order_info.BankCode);
                if (order_info.PaymentDate != null)
                {
                    objParam_order[10] = new SqlParameter("@PaymentDate", order_info.PaymentDate);
                }
                else
                {
                    objParam_order[10] = new SqlParameter("@PaymentDate", DBNull.Value);
                }

                if (order_info.PaymentNo != null)
                {
                    objParam_order[11] = new SqlParameter("@PaymentNo", order_info.PaymentNo);
                }
                else
                {
                    objParam_order[11] = new SqlParameter("@PaymentNo", DBNull.Value);
                }
                objParam_order[12] = new SqlParameter("@Profit", order_info.Profit);
                objParam_order[13] = new SqlParameter("@Discount", order_info.Discount);
                objParam_order[14] = new SqlParameter("@PaymentStatus", order_info.PaymentStatus);
                objParam_order[15] = new SqlParameter("@OrderId", order_info.Id);
                objParam_order[16] = new SqlParameter("@ExpriryDate", order_info.ExpriryDate);
                objParam_order[17] = new SqlParameter("@ProductService", order_info.ProductService.ToString());
                objParam_order[18] = new SqlParameter("@AccountClientId", order_info.AccountClientId);
                objParam_order[19] = new SqlParameter("@StartDate", order_info.StartDate);
                objParam_order[20] = new SqlParameter("@EndDate", order_info.EndDate);
                objParam_order[21] = new SqlParameter("@SystemType", order_info.SystemType);
                objParam_order[22] = new SqlParameter("@SalerId", order_info.SalerId);
                if (order_info.SalerId != null)
                {
                    objParam_order[22] = new SqlParameter("@SalerId", order_info.SalerId);
                }
                else
                {
                    objParam_order[22] = new SqlParameter("@SalerId", DBNull.Value);
                }
                objParam_order[23] = new SqlParameter("@SalerGroupId", order_info.SalerGroupId);
                objParam_order[24] = new SqlParameter("@UserUpdateId", order_info.UserUpdateId);

                if (order_info.PercentDecrease != null)
                {
                    objParam_order[25] = new SqlParameter("@PercentDecrease", order_info.PercentDecrease);
                }
                else
                {
                    objParam_order[25] = new SqlParameter("@PercentDecrease", DBNull.Value);
                }

                if (order_info.Price != null)
                {
                    objParam_order[26] = new SqlParameter("@Price", order_info.Price);
                }
                else
                {
                    objParam_order[26] = new SqlParameter("@Price", DBNull.Value);
                }

                if (order_info.Label != null)
                {
                    objParam_order[27] = new SqlParameter("@Label", order_info.Label);
                }
                else
                {
                    objParam_order[27] = new SqlParameter("@Label", DBNull.Value);
                }

                if (order_info.VoucherId != null)
                {
                    objParam_order[28] = new SqlParameter("@VoucherId", order_info.VoucherId);
                }
                else
                {
                    objParam_order[28] = new SqlParameter("@VoucherId", DBNull.Value);
                }
                objParam_order[29] = new SqlParameter("@CreatedBy", order_info.CreatedBy);
                order_info.SupplierId = 0;
                objParam_order[30] = new SqlParameter("@SupplierId", order_info.SupplierId);
                objParam_order[31] = new SqlParameter("@Note", order_info.Note);
                if (order_info.UtmMedium != null)
                {
                    objParam_order[32] = new SqlParameter("@UtmMedium", order_info.UtmMedium);
                }
                else
                {
                    objParam_order[32] = new SqlParameter("@UtmMedium", DBNull.Value);
                }
                if (order_info.UtmSource != null)
                {
                    objParam_order[33] = new SqlParameter("@UtmSource", order_info.UtmSource);
                }
                else
                {
                    objParam_order[33] = new SqlParameter("@UtmSource", DBNull.Value);
                }


                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateOrder, objParam_order);
                order_info.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CreateOrder" + ex.ToString());
                return -1;
            }
        }
        public static int CreatePassengers(PassengerViewModel passenger)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[8];
                objParam_order[0] = new SqlParameter("@Name", passenger.Name);
                if (passenger.MembershipCard != null)
                {
                    objParam_order[1] = new SqlParameter("@MembershipCard", passenger.MembershipCard);
                }
                else
                {
                    objParam_order[1] = new SqlParameter("@MembershipCard", DBNull.Value);
                }
                objParam_order[2] = new SqlParameter("@PersonType", passenger.PersonType);
                if (passenger.Birthday != null)
                {
                    objParam_order[3] = new SqlParameter("@Birthday", passenger.Birthday);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@Birthday", DBNull.Value);
                }
                objParam_order[4] = new SqlParameter("@Gender", passenger.Gender);
                objParam_order[5] = new SqlParameter("@OrderId", passenger.OrderId);
                if (passenger.Note != null && passenger.Note.Trim()!="")
                {
                    objParam_order[6] = new SqlParameter("@Note", passenger.Note);
                }
                else
                {
                    objParam_order[6] = new SqlParameter("@Note", DBNull.Value);
                }
                objParam_order[7] = new SqlParameter("@GroupBookingId", passenger.GroupBookingId);

                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreatePassengers, objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CheckIfNewOrderValid" + ex.ToString());
                return -1;
            }
        }
        public static int CreateHotelGuest(HotelGuestViewModel guest)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[6];
                objParam_order[0] = new SqlParameter("@Name", guest.Name);
                if (guest.Birthday != null)
                {
                    objParam_order[1] = new SqlParameter("@Birthday", guest.Birthday);
                }
                else
                {
                    objParam_order[1] = new SqlParameter("@Birthday", DBNull.Value);
                }
                objParam_order[2] = new SqlParameter("@HotelBookingRoomsID", guest.HotelBookingRoomsID);
                objParam_order[3] = new SqlParameter("@HotelBookingId", guest.HotelBookingId);
                objParam_order[4] = new SqlParameter("@Note", guest.Note==null?"": guest.Note);
                objParam_order[5] = new SqlParameter("@Type", guest.Type == null ? 0 : guest.Type);


                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateHotelGuest, objParam_order);
                return id;

            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CreateHotelGuest" + ex.ToString());
                return -1;
            }
        }

        public static OrderInfo UpdateOrderPayment(long order_id, int payment_type)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[2];
                objParam_order[0] = new SqlParameter("@OrderID", order_id);
                objParam_order[1] = new SqlParameter("@PaymentType", payment_type);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_UpdateOrderPaymentMethod", objParam_order);
                return tb.ToList<OrderInfo>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CheckIfNewOrderValid" + ex.ToString());
                return null;
            }
        }

        public static int CreateBaggage(BaggageViewModel baggage)
        {
            try
            {

                SqlParameter[] objParam_baggage = new SqlParameter[14];
                objParam_baggage[0] = new SqlParameter("@Airline", baggage.Airline);
                objParam_baggage[1] = new SqlParameter("@Code", baggage.Code);
                objParam_baggage[2] = new SqlParameter("@Confirmed", baggage.Confirmed);
                objParam_baggage[3] = new SqlParameter("@Currency", baggage.Currency);
                if (baggage.EndPoint != null)
                {
                    objParam_baggage[4] = new SqlParameter("@EndPoint", baggage.EndPoint);
                }
                else
                {
                    objParam_baggage[4] = new SqlParameter("@EndPoint", DBNull.Value);
                }
                objParam_baggage[5] = new SqlParameter("@FlightId", baggage.FlightId);
                objParam_baggage[6] = new SqlParameter("@Leg", baggage.Leg);
                objParam_baggage[7] = new SqlParameter("@Name", baggage.Name);
                objParam_baggage[8] = new SqlParameter("@PassengerId", baggage.PassengerId);
                objParam_baggage[9] = new SqlParameter("@Price", baggage.Price);
                if (baggage.StartPoint != null)
                {
                    objParam_baggage[10] = new SqlParameter("@StartPoint", baggage.StartPoint);
                }
                else
                {
                    objParam_baggage[10] = new SqlParameter("@StartPoint", DBNull.Value);
                }
                if (baggage.StatusCode != null)
                {
                    objParam_baggage[11] = new SqlParameter("@StatusCode", baggage.StatusCode);
                }
                else
                {
                    objParam_baggage[11] = new SqlParameter("@StatusCode", DBNull.Value);
                }
                objParam_baggage[12] = new SqlParameter("@Value", baggage.Value);
                objParam_baggage[13] = new SqlParameter("@WeightValue", baggage.WeightValue);

                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateBaggage, objParam_baggage);
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CheckIfNewOrderValid" + ex.ToString());
                return -1;
            }
        }
        public static int CreateFlyBookingExtraPackages(BaggageViewModel baggage, PassengerViewModel passenger,string group_fly_id,int created_by)
        {
            try
            {
                string PackageCode = "Hành lý ";
                string PackageId = "baggage-";
                int baggage_count = 1;
                if (baggage.Leg == 0)
                {
                    PackageCode += "Chiều đi ";
                    PackageId += "0-";
                }
                if (baggage.Leg == 1)
                {
                    PackageCode += "Chiều về ";
                    PackageId += "1-";

                }
                int profit = 0;
                PackageId += CommonHelper.RemoveUnicode(baggage.Code.Replace(" ",""));
                PackageCode +="- "+ baggage.Name + " (Hành Khách: "+passenger.Name+")";
                DateTime now = DateTime.Now;
                SqlParameter[] objParam_baggage = new SqlParameter[12];
                objParam_baggage[0] = new SqlParameter("@PackageId", PackageId);
                objParam_baggage[1] = new SqlParameter("@PackageCode", PackageCode);
                objParam_baggage[2] = new SqlParameter("@GroupFlyBookingId", group_fly_id);
                objParam_baggage[3] = new SqlParameter("@Amount", baggage.Price);
                objParam_baggage[4] = new SqlParameter("@BasePrice", baggage.Price);
                objParam_baggage[5] = new SqlParameter("@Quantity", baggage_count);
                objParam_baggage[6] = new SqlParameter("@Price", baggage.Price);
                objParam_baggage[7] = new SqlParameter("@CreatedBy", created_by);
                objParam_baggage[8] = new SqlParameter("@CreatedDate", now);
                objParam_baggage[9] = new SqlParameter("@UpdatedBy", created_by);
                objParam_baggage[10] = new SqlParameter("@UpdatedDate", now);
                objParam_baggage[11] = new SqlParameter("@Profit", profit);


                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.InsertFlyBookingExtraPackages, objParam_baggage);
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CheckIfNewOrderValid" + ex.ToString());
                return -1;
            }
        }
        public static int CreateFlyBookingDetail(FlyBookingDetailViewModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[53];
                objParam_order[0] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[1] = new SqlParameter("@PriceId", model.PriceId);
                objParam_order[2] = new SqlParameter("@BookingCode", model.BookingCode);
                objParam_order[3] = new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = new SqlParameter("@Difference", model.Difference);
                objParam_order[5] = new SqlParameter("@Currency", model.Currency);
                objParam_order[6] = new SqlParameter("@Flight", model.Flight);
                objParam_order[7] = new SqlParameter("@ExpiryDate", model.ExpiryDate);
                objParam_order[8] = new SqlParameter("@Session", model.Session);
                objParam_order[9] = new SqlParameter("@Airline", model.Airline);
                objParam_order[10] = new SqlParameter("@StartPoint", model.StartPoint);
                objParam_order[11] = new SqlParameter("@EndPoint", model.EndPoint);
                objParam_order[12] = new SqlParameter("@GroupClass", model.GroupClass);
                objParam_order[13] = new SqlParameter("@Leg", model.Leg);
                objParam_order[14] = new SqlParameter("@AdultNumber", model.AdultNumber);
                objParam_order[15] = new SqlParameter("@ChildNumber", model.ChildNumber);
                objParam_order[16] = new SqlParameter("@InfantNumber", model.InfantNumber);
                objParam_order[17] = new SqlParameter("@FareAdt", model.FareAdt);
                objParam_order[18] = new SqlParameter("@FareChd", model.FareChd);
                objParam_order[19] = new SqlParameter("@FareInf", model.FareInf);
                objParam_order[20] = new SqlParameter("@TaxAdt", model.TaxAdt);
                objParam_order[21] = new SqlParameter("@TaxChd", model.TaxChd);
                objParam_order[22] = new SqlParameter("@TaxInf", model.TaxInf);
                objParam_order[23] = new SqlParameter("@FeeAdt", model.FeeAdt);
                objParam_order[24] = new SqlParameter("@FeeChd", model.FeeChd);
                objParam_order[25] = new SqlParameter("@FeeInf", model.FeeInf);
                objParam_order[26] = new SqlParameter("@ServiceFeeAdt", model.ServiceFeeAdt);
                objParam_order[27] = new SqlParameter("@ServiceFeeChd", model.ServiceFeeChd);
                objParam_order[28] = new SqlParameter("@ServiceFeeInf", model.ServiceFeeInf);
                objParam_order[29] = new SqlParameter("@AmountAdt", model.AmountAdt);
                objParam_order[30] = new SqlParameter("@AmountChd", model.AmountChd);
                objParam_order[31] = new SqlParameter("@AmountInf", model.AmountInf);
                objParam_order[32] = new SqlParameter("@TotalNetPrice", model.TotalNetPrice);
                objParam_order[33] = new SqlParameter("@TotalDiscount", model.TotalDiscount);
                objParam_order[34] = new SqlParameter("@TotalCommission", model.TotalCommission);
                objParam_order[35] = new SqlParameter("@TotalBaggageFee", model.TotalBaggageFee);
                objParam_order[36] = new SqlParameter("@StartDate", model.StartDate);
                objParam_order[37] = new SqlParameter("@EndDate", model.EndDate);
                objParam_order[38] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[39] = new SqlParameter("@Status", model.Status);
                objParam_order[40] = new SqlParameter("@Profit", model.Profit);
                if (model.ServiceCode != null)
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", DBNull.Value);

                }
                objParam_order[42] = new SqlParameter("@Price", model.Price);
                objParam_order[43] = new SqlParameter("@PriceAdt", model.PriceAdt);
                objParam_order[44] = new SqlParameter("@PriceChd", model.PriceChd);
                objParam_order[45] = new SqlParameter("@PriceInf", model.PriceInf);
                if (model.SupplierId != null)
                {
                    objParam_order[46] = new SqlParameter("@SupplierId", model.SupplierId);
                }
                else
                {
                    objParam_order[46] = new SqlParameter("@SupplierId", DBNull.Value);

                }
                if (model.Note != null)
                {
                    objParam_order[47] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[47] = new SqlParameter("@Note", DBNull.Value);

                }
                objParam_order[48] = new SqlParameter("@ProfitAdt", model.ProfitAdt);
                objParam_order[49] = new SqlParameter("@ProfitChd", model.ProfitChd);
                objParam_order[50] = new SqlParameter("@ProfitInf", model.ProfitInf);  
                objParam_order[51] = new SqlParameter("@AdgCommission", model.Adgcommission);
                objParam_order[52] = new SqlParameter("@OthersAmount", model.OthersAmount);
                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateFlyBookingDetail, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CreateFlyBookingDetail" + ex.ToString());
                return -1;
            }
        }

        public static int CreateFlySegment(FlyingSegmentViewModel fly_segment)
        {
            try
            {
                SqlParameter[] dt_fly_segment = new SqlParameter[22];
                dt_fly_segment[0] = new SqlParameter("@FlyBookingId", fly_segment.FlyBookingId);
                dt_fly_segment[1] = new SqlParameter("@OperatingAirline", fly_segment.OperatingAirline);
                dt_fly_segment[2] = new SqlParameter("@StartPoint", fly_segment.StartPoint);
                dt_fly_segment[3] = new SqlParameter("@EndPoint", fly_segment.EndPoint);
                dt_fly_segment[4] = new SqlParameter("@StartTime", fly_segment.StartTime.LocalDateTime);
                dt_fly_segment[5] = new SqlParameter("@EndTime", fly_segment.EndTime.LocalDateTime);
                dt_fly_segment[6] = new SqlParameter("@FlightNumber", fly_segment.FlightNumber);
                dt_fly_segment[7] = new SqlParameter("@Duration", fly_segment.Duration);
                dt_fly_segment[8] = new SqlParameter("@Class", fly_segment.Class);
                dt_fly_segment[9] = new SqlParameter("@Plane", fly_segment.Plane);
                if (fly_segment.StartTerminal != null)
                {
                    dt_fly_segment[10] = new SqlParameter("@StartTerminal", fly_segment.StartTerminal);
                }
                else
                {
                    dt_fly_segment[10] = new SqlParameter("@StartTerminal", DBNull.Value);
                }
                if (fly_segment.EndTerminal != null)
                {
                    dt_fly_segment[11] = new SqlParameter("@EndTerminal", fly_segment.EndTerminal);
                }
                else
                {
                    dt_fly_segment[11] = new SqlParameter("@EndTerminal", DBNull.Value);
                }
                if (fly_segment.StopPoint != null)
                {
                    dt_fly_segment[12] = new SqlParameter("@StopPoint", fly_segment.StopPoint);
                }
                else
                {
                    dt_fly_segment[12] = new SqlParameter("@StopPoint", DBNull.Value);
                }
                dt_fly_segment[13] = new SqlParameter("@StopTime", fly_segment.StopTime);

                if (fly_segment.AllowanceBaggage != null)
                {
                    dt_fly_segment[14] = new SqlParameter("@AllowanceBaggage", fly_segment.AllowanceBaggage);
                }
                else
                {
                    dt_fly_segment[14] = new SqlParameter("@AllowanceBaggage", DBNull.Value);
                }

                if (fly_segment.HandBaggage != null)
                {
                    dt_fly_segment[15] = new SqlParameter("@HandBaggage", fly_segment.HandBaggage);
                }
                else
                {
                    dt_fly_segment[15] = new SqlParameter("@HandBaggage", DBNull.Value);
                }
                dt_fly_segment[16] = new SqlParameter("@HasStop", fly_segment.HasStop);
                dt_fly_segment[17] = new SqlParameter("@ChangeStation", fly_segment.ChangeStation);
                dt_fly_segment[18] = new SqlParameter("@ChangeAirport", fly_segment.ChangeAirport);
                dt_fly_segment[19] = new SqlParameter("@StopOvernight", fly_segment.StopOvernight);
                dt_fly_segment[20] = new SqlParameter("@AllowanceBaggageValue", fly_segment.AllowanceBaggageValue);
                dt_fly_segment[21] = new SqlParameter("@HandBaggageValue", fly_segment.HandBaggageValue);

                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateFlySegment, dt_fly_segment);
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CheckIfNewOrderValid" + ex.ToString());
                return -1;
            }
        }

        public static int CreateHotelBooking(HotelBooking booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[31];
                objParam_order[0] = new SqlParameter("@OrderId", booking.OrderId);
                objParam_order[1] = new SqlParameter("@BookingId", booking.BookingId);
                objParam_order[2] = new SqlParameter("@PropertyId", booking.PropertyId);
                if (booking.HotelType != null)
                {
                    objParam_order[3] = new SqlParameter("@HotelType", booking.HotelType);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@HotelType", (int)ServicesType.VINHotelRent);
                }
                objParam_order[4] = new SqlParameter("@ArrivalDate", booking.ArrivalDate);
                objParam_order[5] = new SqlParameter("@DepartureDate", booking.DepartureDate);
                objParam_order[6] = new SqlParameter("@numberOfRoom", booking.NumberOfRoom);
                objParam_order[7] = new SqlParameter("@numberOfAdult", booking.NumberOfAdult);
                objParam_order[8] = new SqlParameter("@numberOfChild", booking.NumberOfChild);
                objParam_order[9] = new SqlParameter("@numberOfInfant", booking.NumberOfInfant);
                objParam_order[10] = new SqlParameter("@totalPrice", booking.TotalPrice);
                objParam_order[11] = new SqlParameter("@totalProfit", booking.TotalProfit);
                objParam_order[12] = new SqlParameter("@totalAmount", booking.TotalAmount);
                objParam_order[13] = new SqlParameter("@Status", booking.Status);
                objParam_order[14] = new SqlParameter("@HotelName", booking.HotelName);
                if (booking.Telephone != null)
                {
                    objParam_order[15] = new SqlParameter("@Telephone", booking.Telephone);
                }
                else
                {
                    objParam_order[15] = new SqlParameter("@Telephone", DBNull.Value);
                }
                if (booking.Email != null)
                {
                    objParam_order[16] = new SqlParameter("@Email", booking.Email);
                }
                else
                {
                    objParam_order[16] = new SqlParameter("@Email", DBNull.Value);
                }
                if (booking.Address != null)
                {
                    objParam_order[17] = new SqlParameter("@Address", booking.Address);
                }
                else
                {
                    objParam_order[17] = new SqlParameter("@Address", DBNull.Value);
                }
                if (booking.ImageThumb != null)
                {
                    objParam_order[18] = new SqlParameter("@ImageThumb", booking.ImageThumb);
                }
                else
                {
                    objParam_order[18] = new SqlParameter("@ImageThumb", DBNull.Value);
                }
                objParam_order[19] = new SqlParameter("@CheckinTime", booking.CheckinTime);
                objParam_order[20] = new SqlParameter("@CheckoutTime", booking.CheckoutTime);
                objParam_order[21] = new SqlParameter("@ExtraPackageAmount", booking.ExtraPackageAmount);
                if (booking.SalerId != null)
                {
                    objParam_order[22] = new SqlParameter("@SalerId", booking.SalerId);
                }
                else
                {
                    objParam_order[22] = new SqlParameter("@SalerId", DBNull.Value);
                }
                objParam_order[23] = new SqlParameter("@CreatedBy", booking.CreatedBy);
                objParam_order[24] = new SqlParameter("@CreatedDate", DateTime.Now);
                if (booking.ServiceCode != null && booking.ServiceCode.Trim() != "")
                {
                    objParam_order[25] = new SqlParameter("@ServiceCode", booking.ServiceCode);
                }
                else
                {
                    objParam_order[25] = new SqlParameter("@ServiceCode", DBNull.Value);
                }
                objParam_order[26] = new SqlParameter("@Price", booking.TotalPrice);
                if (booking.SupplierId != null)
                {
                    objParam_order[27] = new SqlParameter("@SupplierId", booking.SupplierId);
                }
                else
                {
                    objParam_order[27] = new SqlParameter("@SupplierId", DBNull.Value);

                }
                if (booking.Note != null)
                {
                    objParam_order[28] = new SqlParameter("@Note", booking.Note);
                }
                else
                {
                    objParam_order[28] = new SqlParameter("@Note", DBNull.Value);
                }
                objParam_order[29] = new SqlParameter("@TotalDiscount", booking.TotalDiscount);
                objParam_order[30] = new SqlParameter("@TotalOthersAmount", booking.TotalOthersAmount);
                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateHotelBooking, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CheckIfNewOrderValid" + ex.ToString());
                return -1;
            }
        }

        public static int CreateHotelBookingRooms(HotelBookingRooms booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[18];
                objParam_order[0] = new SqlParameter("@HotelBookingId", booking.HotelBookingId);
                objParam_order[1] = new SqlParameter("@RoomTypeID", booking.RoomTypeId);
                objParam_order[2] = new SqlParameter("@Price", booking.Price);
                objParam_order[3] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[4] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                objParam_order[5] = new SqlParameter("@RoomTypeCode", booking.RoomTypeCode);
                objParam_order[6] = new SqlParameter("@RoomTypeName", booking.RoomTypeName);
                objParam_order[7] = new SqlParameter("@numberOfAdult", booking.NumberOfAdult);
                objParam_order[8] = new SqlParameter("@numberOfChild", booking.NumberOfChild);
                objParam_order[9] = new SqlParameter("@numberOfInfant", booking.NumberOfInfant);
                objParam_order[10] = new SqlParameter("@PackageIncludes", booking.PackageIncludes);
                objParam_order[11] = new SqlParameter("@ExtraPackageAmount", booking.ExtraPackageAmount);
                objParam_order[12] = new SqlParameter("@Status", booking.Status);
                objParam_order[13] = new SqlParameter("@TotalUnitPrice", booking.TotalUnitPrice);
                objParam_order[14] = new SqlParameter("@CreateBy", booking.CreatedBy);
                objParam_order[15] = new SqlParameter("@CreatedDate", booking.CreatedDate);
                objParam_order[16] = new SqlParameter("@NumberOfRooms", booking.NumberOfRooms);
                if (booking.SupplierId != null)
                {
                    objParam_order[17] = new SqlParameter("@SupplierId", booking.SupplierId);
                }
                else
                {
                    objParam_order[17] = new SqlParameter("@SupplierId", DBNull.Value);
                }
                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateHotelBookingRooms, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CheckIfNewOrderValid" + ex.ToString());
                return -1;
            }
        }

        public static int CreateHotelBookingRoomRates(HotelBookingRoomRates booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[15];
                objParam_order[0] = new SqlParameter("@HotelBookingRoomId", booking.HotelBookingRoomId);
                objParam_order[1] = new SqlParameter("@RatePlanId", booking.RatePlanId);
                objParam_order[2] = new SqlParameter("@StayDate", booking.StayDate);
                objParam_order[3] = new SqlParameter("@Price", booking.Price);
                objParam_order[4] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[5] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                if (booking.AllotmentId != null)
                {
                    objParam_order[6] = new SqlParameter("@AllotmentId", booking.AllotmentId);
                }
                else
                {
                    objParam_order[6] = new SqlParameter("@AllotmentId", DBNull.Value);
                }
                if (booking.RatePlanCode != null)
                {
                    objParam_order[7] = new SqlParameter("@RatePlanCode", booking.RatePlanCode);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@RatePlanCode", DBNull.Value);
                }
                if (booking.PackagesInclude != null)
                {
                    objParam_order[8] = new SqlParameter("@PackagesInclude", booking.PackagesInclude);
                }
                else
                {
                    objParam_order[8] = new SqlParameter("@PackagesInclude", DBNull.Value);
                }
                objParam_order[9] = new SqlParameter("@Nights", booking.Nights);
                objParam_order[10] = new SqlParameter("@StartDate", booking.StartDate);
                objParam_order[11] = new SqlParameter("@EndDate", booking.EndDate);
                objParam_order[12] = new SqlParameter("@OperatorPrice", booking.OperatorPrice);
                objParam_order[13] = new SqlParameter("@SalePrice", booking.SalePrice);
                objParam_order[14] = new SqlParameter("@CreatedBy", booking.CreatedBy);

                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.CreateHotelBookingRoomRates, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CheckIfNewOrderValid" + ex.ToString());
                return -1;
            }
        }

        public static List<HotelBooking> getHotelBookings(int orderID)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderID ", orderID);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetHotelBookingByOrderID", objParam);

                return tb.ToList<HotelBooking>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getHotelBookings" + ex);
                return null;
            }

        }

        public static List<HotelBookingRooms> getHotelBookingRoom(long hotelBookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingID ", hotelBookingId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetHotelBookingRoomByHotelBookingID", objParam);

                return tb.ToList<HotelBookingRooms>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getHotelBookingRoom" + ex);
                return null;
            }

        }

        public static List<HotelBookingRoomRates> getHotelBookingRoomRate(long hotelBookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingRoomID ", hotelBookingId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetHotelBookingRateByHotelBookingRoomID", objParam);

                return tb.ToList<HotelBookingRoomRates>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getFlyBookingDetailList" + ex);
                return null;
            }

        }

        public static List<HotelGuest> getHotelGuest(long hotelBookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId ", hotelBookingId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetHotelGuestByOrderId", objParam);

                return tb.ToList<HotelGuest>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getHotelGuest" + ex);
                return new List<HotelGuest>();
            }

        }
        public static int UpdateFlyBookingDetail(FlyBookingDetailViewModel model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[57];
                objParam_order[0] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[1] = new SqlParameter("@PriceId", model.PriceId);
                objParam_order[2] = new SqlParameter("@BookingCode", model.BookingCode);
                objParam_order[3] = new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = new SqlParameter("@Difference", model.Difference);
                objParam_order[5] = new SqlParameter("@Currency", model.Currency);
                objParam_order[6] = new SqlParameter("@Flight", model.Flight);
                objParam_order[7] = new SqlParameter("@ExpiryDate", model.ExpiryDate);
                objParam_order[8] = new SqlParameter("@Session", model.Session);
                objParam_order[9] = new SqlParameter("@Airline", model.Airline);
                objParam_order[10] = new SqlParameter("@StartPoint", model.StartPoint);
                objParam_order[11] = new SqlParameter("@EndPoint", model.EndPoint);
                objParam_order[12] = new SqlParameter("@GroupClass", model.GroupClass);
                objParam_order[13] = new SqlParameter("@Leg", model.Leg);
                objParam_order[14] = new SqlParameter("@AdultNumber", model.AdultNumber);
                objParam_order[15] = new SqlParameter("@ChildNumber", model.ChildNumber);
                objParam_order[16] = new SqlParameter("@InfantNumber", model.InfantNumber);
                objParam_order[17] = new SqlParameter("@FareAdt", model.FareAdt);
                objParam_order[18] = new SqlParameter("@FareChd", model.FareChd);
                objParam_order[19] = new SqlParameter("@FareInf", model.FareInf);
                objParam_order[20] = new SqlParameter("@TaxAdt", model.TaxAdt);
                objParam_order[21] = new SqlParameter("@TaxChd", model.TaxChd);
                objParam_order[22] = new SqlParameter("@TaxInf", model.TaxInf);
                objParam_order[23] = new SqlParameter("@FeeAdt", model.FeeAdt);
                objParam_order[24] = new SqlParameter("@FeeChd", model.FeeChd);
                objParam_order[25] = new SqlParameter("@FeeInf", model.FeeInf);
                objParam_order[26] = new SqlParameter("@ServiceFeeAdt", model.ServiceFeeAdt);
                objParam_order[27] = new SqlParameter("@ServiceFeeChd", model.ServiceFeeChd);
                objParam_order[28] = new SqlParameter("@ServiceFeeInf", model.ServiceFeeInf);
                objParam_order[29] = new SqlParameter("@AmountAdt", model.AmountAdt);
                objParam_order[30] = new SqlParameter("@AmountChd", model.AmountChd);
                objParam_order[31] = new SqlParameter("@AmountInf", model.AmountInf);
                objParam_order[32] = new SqlParameter("@TotalNetPrice", model.TotalNetPrice);
                objParam_order[33] = new SqlParameter("@TotalDiscount", model.TotalDiscount);
                objParam_order[34] = new SqlParameter("@TotalCommission", model.TotalCommission);
                objParam_order[35] = new SqlParameter("@TotalBaggageFee", model.TotalBaggageFee);
                objParam_order[36] = new SqlParameter("@StartDate", model.StartDate);
                objParam_order[37] = new SqlParameter("@EndDate", model.EndDate);
                objParam_order[38] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[39] = new SqlParameter("@Status", model.Status);
                objParam_order[40] = new SqlParameter("@Profit", model.Profit);
                if (model.ServiceCode != null)
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", DBNull.Value);

                }
                objParam_order[42] = new SqlParameter("@Id", model.Id);
                objParam_order[43] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                if (model.SalerId != null)
                {
                    objParam_order[44] = new SqlParameter("@SalerId", model.SalerId);
                }
                else
                {
                    objParam_order[44] = new SqlParameter("@SalerId", DBNull.Value);

                }
                if (model.GroupBookingId != null)
                {
                    objParam_order[45] = new SqlParameter("@GroupBookingId", model.GroupBookingId);
                }
                else
                {
                    objParam_order[45] = new SqlParameter("@GroupBookingId", DBNull.Value);

                }
                objParam_order[46] = new SqlParameter("@Price", model.Price);
                objParam_order[47] = new SqlParameter("@PriceAdt", model.PriceAdt);
                if (model.PriceChd != null)
                {
                    objParam_order[48] = new SqlParameter("@PriceChd", model.PriceChd);
                }
                else
                {
                    objParam_order[48] = new SqlParameter("@PriceChd", DBNull.Value);

                }
                if (model.PriceInf != null)
                {
                    objParam_order[49] = new SqlParameter("@PriceInf", model.PriceInf);
                }
                else
                {
                    objParam_order[49] = new SqlParameter("@PriceInf", DBNull.Value);

                }
                if (model.SupplierId != null)
                {
                    objParam_order[50] = new SqlParameter("@SupplierId", model.SupplierId);
                }
                else
                {
                    objParam_order[50] = new SqlParameter("@SupplierId", DBNull.Value);

                }
                if (model.Note != null)
                {
                    objParam_order[51] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[51] = new SqlParameter("@Note", DBNull.Value);

                }
                objParam_order[52] = new SqlParameter("@ProfitAdt", model.ProfitAdt);
                objParam_order[53] = new SqlParameter("@ProfitChd", model.ProfitChd);
                objParam_order[54] = new SqlParameter("@ProfitInf", model.ProfitInf);
                objParam_order[55] = new SqlParameter("@AdgCommission", model.Adgcommission);
                objParam_order[56] = new SqlParameter("@OthersAmount", model.OthersAmount);
                var id = DBWorker.ExecuteNonQuery("SP_UpdateFlyBookingDetail", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("UpdateFlyBookingDetail - Repository: " + ex);
                return -1;
            }
        }
        public static List<Hotel> GetHotelByHotelId(string hotel_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelID", hotel_id);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetHotelByHotelId", objParam);

                return tb.ToList<Hotel>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository GetHotelByHotelId" + ex);
                return new List<Hotel>();
            }

        }
        public static int CreateVinWonderBooking(VinWonderBooking model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[17];
                objParam_order[0] = new SqlParameter("@SiteName", model.SiteName);
                objParam_order[1] = new SqlParameter("@SiteCode", model.SiteCode);
                objParam_order[2] = new SqlParameter("@Status", model.Status);
                objParam_order[3] = new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = new SqlParameter("@TotalPrice", model.TotalPrice);
                objParam_order[5] = new SqlParameter("@TotalProfit", model.TotalProfit);
                objParam_order[6] = new SqlParameter("@SupplierId", model.SupplierId);
                objParam_order[7] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[8] = new SqlParameter("@SalerId", model.SalerId);
                objParam_order[9] = new SqlParameter("@Note", model.Note);
                objParam_order[10] = new SqlParameter("@TotalUnitPrice", model.TotalUnitPrice);
                objParam_order[11] = new SqlParameter("@AdavigoBookingId", model.AdavigoBookingId);
                if (model.ServiceCode != null)
                {
                    objParam_order[12] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[12] = new SqlParameter("@ServiceCode", DBNull.Value);
                }
                objParam_order[13] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[14] = new SqlParameter("@CreatedDate", model.CreatedDate);
                objParam_order[15] = new SqlParameter("@Commission", model.Commission);
                objParam_order[16] = new SqlParameter("@OthersAmount", model.OthersAmount);

                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.InsertVinWonderBooking, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("CreateVinWonder - Repository. " + ex);
                return -1;
            }
        }
        public static int UpdateVinWonderBooking(VinWonderBooking model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[17];
                objParam_order[0] = new SqlParameter("@SiteName", model.SiteName);
                objParam_order[1] = new SqlParameter("@SiteCode", model.SiteCode);
                objParam_order[2] = new SqlParameter("@Status", model.Status);
                objParam_order[3] = new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = new SqlParameter("@TotalPrice", model.TotalPrice);
                objParam_order[5] = new SqlParameter("@TotalProfit", model.TotalProfit);
                objParam_order[6] = new SqlParameter("@SupplierId", model.SupplierId);
                objParam_order[7] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[8] = new SqlParameter("@SalerId", model.SalerId);
                objParam_order[9] = new SqlParameter("@Note", model.Note);
                objParam_order[10] = new SqlParameter("@TotalUnitPrice", model.TotalUnitPrice);
                objParam_order[11] = new SqlParameter("@AdavigoBookingId", model.AdavigoBookingId);
                if (model.ServiceCode != null)
                {
                    objParam_order[12] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[12] = new SqlParameter("@ServiceCode", DBNull.Value);
                }
                objParam_order[13] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam_order[14] = new SqlParameter("@Id", model.Id);
                objParam_order[15] = new SqlParameter("@Commission", model.Commission);
                objParam_order[16] = new SqlParameter("@OthersAmount", model.OthersAmount);
                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.sp_updateVinWonderBooking, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("UpdateVinWonderBooking - Repository. " + ex);
                return -1;
            }
        }
        public static int CreateVinWonderBookingTicket(VinWonderBookingTicket model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[15];
                objParam_order[0] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[1] = new SqlParameter("@RateCode", model.RateCode);

                objParam_order[2] = new SqlParameter("@Name", model.Name);
                objParam_order[3] = new SqlParameter("@Quantity", model.Quantity);
                objParam_order[4] = new SqlParameter("@Amount", model.Amount);
                objParam_order[5] = new SqlParameter("@BasePrice", model.BasePrice);
                objParam_order[6] = new SqlParameter("@Profit", model.Profit);

                objParam_order[7] = new SqlParameter("@DateUsed", model.DateUsed);

                objParam_order[8] = new SqlParameter("@adt", model.Adt);
                objParam_order[9] = new SqlParameter("@child", model.Child);
                objParam_order[10] = new SqlParameter("@old", model.Old);
                objParam_order[11] = new SqlParameter("@totalPrice", model.TotalPrice);
                objParam_order[12] = new SqlParameter("@UnitPrice", model.UnitPrice);
                objParam_order[13] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[14] = new SqlParameter("@CreatedDate", model.CreatedDate);


                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.InsertVinWonderBookingTicket, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CreateVinWonderBookingTicket" + ex.ToString());
                return -1;
            }
        }
        public static int CreateVinWonderTicketDetail(VinWonderBookingTicketDetail booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[25];
                objParam_order[0] = new SqlParameter("@BookingTicketId", booking.BookingTicketId);
                objParam_order[1] = new SqlParameter("@Code", booking.@Code);
                objParam_order[2] = new SqlParameter("@ServiceKey", booking.ServiceKey);
                objParam_order[3] = new SqlParameter("@ShortName", booking.ShortName);
                objParam_order[4] = new SqlParameter("@Name", booking.Name);
                objParam_order[5] = new SqlParameter("@GroupName", booking.GroupName);
                objParam_order[6] = new SqlParameter("@TypeName", booking.TypeName);
                objParam_order[7] = new SqlParameter("@BasePrice", booking.BasePrice);
                objParam_order[8] = new SqlParameter("@Price", booking.Price);
                objParam_order[9] = new SqlParameter("@TotalPrice", booking.TotalPrice);
                objParam_order[10] = new SqlParameter("@RateDiscountPercent", booking.RateDiscountPercent);
                objParam_order[11] = new SqlParameter("@RateDiscountPrice", booking.RateDiscountPrice);
                objParam_order[12] = new SqlParameter("@PromotionDiscountPercent", booking.PromotionDiscountPercent);
                objParam_order[13] = new SqlParameter("@PromotionDiscountPrice", booking.PromotionDiscountPrice);
                objParam_order[14] = new SqlParameter("@VATPercent", booking.Vatpercent);
                objParam_order[15] = new SqlParameter("@Availability", booking.Availability);
                objParam_order[16] = new SqlParameter("@NumberOfUses", booking.NumberOfUses);
                objParam_order[17] = new SqlParameter("@DateFrom", booking.DateFrom);
                objParam_order[18] = new SqlParameter("@DateTo", booking.DateTo);
                objParam_order[19] = new SqlParameter("@WeekDays", booking.WeekDays);
                objParam_order[20] = new SqlParameter("@GateCode", booking.GateCode);
                objParam_order[21] = new SqlParameter("@GateName", booking.GateName);
                objParam_order[22] = new SqlParameter("@CreatedBy", booking.CreatedBy);
                objParam_order[23] = new SqlParameter("@CreatedDate", booking.CreatedDate);
                objParam_order[24] = new SqlParameter("@TypeCode", booking.TypeCode);


                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.InsertVinWonderBookingTicketDetail, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CreateVinWonderTicketDetail" + ex.ToString());
                return -1;
            }
        }
        public static int CreateVinWonderBookingTicketCustomer(VinWonderBookingTicketCustomer model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[9];
                objParam_order[0] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[1] = new SqlParameter("@FullName", model.FullName);
                if (model.Birthday != null)
                {
                    objParam_order[2] = new SqlParameter("@Birthday", model.Birthday);
                }
                else
                {
                    objParam_order[2] = new SqlParameter("@Birthday", DBNull.Value);

                }
                if (model.Phone != null && model.Phone.Trim() != "")
                {
                    objParam_order[3] = new SqlParameter("@Phone", model.Phone);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@Phone", DBNull.Value);

                }

                if (model.Note != null)
                {
                    objParam_order[4] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[4] = new SqlParameter("@Note", DBNull.Value);

                }


                objParam_order[5] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[6] = new SqlParameter("@CreatedDate", model.CreatedDate);
                if (model.Genre != null)
                {
                    objParam_order[7] = new SqlParameter("@Genre", model.Genre);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@Genre", DBNull.Value);

                }
                objParam_order[8] = new SqlParameter("@Email", model.Email);


                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.InsertVinWonderBookingTicketCustomer, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository CreateVinWonderBookingTicket" + ex.ToString());
                return -1;
            }
        }

        public static List<VinWonderBooking> getVinWonderBookings(int orderID)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderID ", orderID);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, StoreProcedureConstant.SP_GetVinWonderBookingByOrderID, objParam);

                return tb.ToList<VinWonderBooking>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getVinWonderBookings" + ex);
                return null;
            }

        }

        public static List<VinWonderBookingTicket> getVinWonderBookingTickets(long bookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@BookingId", bookingId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, StoreProcedureConstant.SP_GetVinWonderBookingTicketByBookingID, objParam);

                return tb.ToList<VinWonderBookingTicket>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getVinWonderBookingTickets" + ex);
                return null;
            }

        }

        public static List<VinWonderBookingTicketCustomer> getVinWonderBookingTicketCustomers(long bookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@BookingId ", bookingId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, StoreProcedureConstant.SP_GetVinWondeCustomerByBookingId, objParam);

                return tb.ToList<VinWonderBookingTicketCustomer>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getVinWonderBookingTicketCustomers" + ex);
                return null;
            }

        }
        public static int UpdateOrderOperator(long order_id)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@Orderid", order_id);
                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateOperatorByOrderid, objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository UpdateOrderOperator" + ex);
                return -1;
            }
        }
        public static List<int> GetListSalerByClientId(long client_id)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ClientId", client_id);

                var tb= DBWorker.GetDataTable( "sp_GetListSalerByClientId", objParam);
                List<int> data = (from row in tb.AsEnumerable()
                            select new
                            { SalerId = Convert.ToInt32(!row["SalerId"].Equals(DBNull.Value) ? row["SalerId"] : 0) 
                            }).Select(x=>x.SalerId).ToList();
                return data;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository GetListSalerByClientId" + ex);
                return null;
            }

        }
        public static long? GetUserAgentByClientId(long clientId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ClientId", clientId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetUserAgentByClientId", objParam);

                var s= tb.ToList<UserAgentSPModel>().FirstOrDefault();
                if (s != null && s.UserId > 0) return s.UserId;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getClientDetail" + ex.ToString());
            }
            return null;

        }
        public static User? GetHeadOfDepartmentByRoleID(long role_id)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@RoleId", role_id);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetHeadOfDepartmentByRoleID", objParam);

                var s = tb.ToList<User>().FirstOrDefault();
                if (s != null && s.Id > 0) return s;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository GetHeadOfDepartmentByRoleID" + ex.ToString());
            }
            return null;

        }
        public static DataTable? GetTourProduct(long tour_product_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@TourProductId", tour_product_id)
                };

                return DBWorker.GetDataTable("SP_GetDetailTourProductByID", objParam);

            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository GetTourProduct" + ex.ToString());
            }
            return null;
        }
        public static int CreateTour(ViewModel.Tour.TourViewModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[17];
                objParam_order[0] = new SqlParameter("@TourType", model.TourType);
                objParam_order[1] = new SqlParameter("@OrganizingType", model.OrganizingType);

                objParam_order[2] = new SqlParameter("@StartDate", model.StartDate);
                objParam_order[3] = new SqlParameter("@EndDate", model.EndDate);
                objParam_order[4] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[5] = new SqlParameter("@SalerId", model.SalerId);
                objParam_order[6] = new SqlParameter("@Amount", model.Amount);
                if (model.ServiceCode != null)
                {
                    objParam_order[7] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@ServiceCode", DBNull.Value);
                }
                objParam_order[8] = new SqlParameter("@SupplierId", model.SupplierId);
                objParam_order[9] = new SqlParameter("@Status", model.Status);
                objParam_order[10] = new SqlParameter("@TourProductId", model.TourProductId);

                objParam_order[11] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[12] = new SqlParameter("@CreatedDate", model.CreatedDate);
                objParam_order[13] = new SqlParameter("@Profit", model.Profit);
                objParam_order[14] = new SqlParameter("@Note", model.Note);

                objParam_order[15] = new SqlParameter("@Commission", model.Commission);
                objParam_order[16] = new SqlParameter("@OthersAmount", model.OthersAmount);

                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.InsertTour, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("CreateTour - Repository. " + ex);
                return -1;
            }
        }

        public static int CreateTourPackages(ViewModel.Tour.TourPackages model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[12];
                objParam_order[0] = new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = new SqlParameter("@PackageName", model.PackageName);
                objParam_order[2] = new SqlParameter("@BasePrice", model.BasePrice);
                objParam_order[3] = new SqlParameter("@Quantity", model.Quantity);

                objParam_order[4] = new SqlParameter("@AmountBeforeVat", model.AmountBeforeVat);
                objParam_order[5] = new SqlParameter("@AmountVat", model.AmountVat);
                objParam_order[6] = new SqlParameter("@Amount", model.Amount);
                objParam_order[7] = new SqlParameter("@VAT", model.Vat);

                objParam_order[8] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[9] = new SqlParameter("@CreatedDate", model.CreatedDate);
                objParam_order[10] = new SqlParameter("@PackageCode", model.PackageCode);
                objParam_order[11] = new SqlParameter("@Profit", model.Profit);


                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.InsertTourPackages, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("CreateTourPackages - Repository. " + ex);
                return -1;
            }
        }
        public static int CreateHotelBookingRoomsOptional(HotelBookingRoomsOptional booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[11];
                objParam_order[0] = new SqlParameter("@HotelBookingId", booking.HotelBookingId);
                objParam_order[1] = new SqlParameter("@HotelBookingRoomId", booking.HotelBookingRoomId);
                objParam_order[2] = new SqlParameter("@Price", booking.Price);
                objParam_order[3] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[4] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                objParam_order[5] = new SqlParameter("@CreatedBy", booking.CreatedBy);
                objParam_order[6] = new SqlParameter("@CreatedDate", booking.CreatedDate);
                objParam_order[7] = new SqlParameter("@NumberOfRooms", booking.NumberOfRooms);
                objParam_order[8] = new SqlParameter("@SupplierId", booking.SupplierId);
                objParam_order[9] = new SqlParameter("@PackageName", booking.PackageName);
                objParam_order[9] = new SqlParameter("@PackageName", booking.PackageName);
                if (booking.IsRoomFund != null)
                {
                    objParam_order[10] = new SqlParameter("@IsRoomFund", booking.IsRoomFund);
                }
                else
                {
                    objParam_order[10] = new SqlParameter("@IsRoomFund", DBNull.Value);

                }
                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.InsertHotelBookingRoomsOptional, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("CreateHotelBookingRoomsOptional - Repository. " + ex);
                return -1;
            }
        }
        public static int CreateHotelBookingRoomsRateOptional(HotelBookingRoomRatesOptional booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[8];
                objParam_order[0] = new SqlParameter("@HotelBookingRoomRatesId", booking.HotelBookingRoomRatesId);
                objParam_order[1] = new SqlParameter("@HotelBookingRoomOptionalId", booking.HotelBookingRoomOptionalId);
                objParam_order[2] = new SqlParameter("@Price", booking.Price);
                objParam_order[3] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[4] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                objParam_order[5] = new SqlParameter("@CreatedBy", booking.CreatedBy);
                objParam_order[6] = new SqlParameter("@CreatedDate", booking.CreatedDate);
                objParam_order[7] = new SqlParameter("@OperatorPrice", booking.OperatorPrice);
                var id = DBWorker.ExecuteNonQuery(StoreProcedureConstant.InsertHotelBookingRoomRatesOptional, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                Telegram.pushLog("CreateHotelBookingRoomsRateOptional - Repository. " + ex);
                return -1;
            }
        }
        public static List<HotelBookingViewModel> GetDetailHotelBookingByID(long HotelBookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId ", HotelBookingId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetDetailHotelBookingByID", objParam);

                return tb.ToList<HotelBookingViewModel>();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository GetDetailHotelBookingByID" + ex);
                return null;
            }

        }
        public static User getUserDetail(long user_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@user_id", user_id);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "sp_getUserDetail", objParam);

                return tb.ToList<User>().SingleOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository getUserDetail" + ex.ToString());
                return null;
            }
        }
        public static List<HotelBookingsRoomOptionalViewModel> GetHotelBookingRoomsOptionalByBookingId(long hotelbookingid)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", hotelbookingid);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetListHotelBookingRoomsOptionalByBookingId", objParam);

                return tb.ToList<HotelBookingsRoomOptionalViewModel>();
               
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository GetHotelBookingRoomsOptionalByBookingId" + ex.ToString());
            }
            return null;
        }
        public static List<HotelBookingRoomRatesOptionalViewModel> GetHotelBookingRoomRatesOptionalByBookingId(long hotelbookingid)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", hotelbookingid);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetListHotelBookingRoomRatesOptionalByBookingId", objParam);

                return tb.ToList<HotelBookingRoomRatesOptionalViewModel>();

            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository GetHotelBookingRoomsOptionalByBookingId" + ex.ToString());
            }
            return null;
        }
        public static AccountClient GetDetailAccountClientByClientId(long ClientId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ClientId", ClientId);

                DataTable tb = new DataTable();
                DBWorker.Fill(tb, "SP_GetDetailAccountClientByClientId", objParam);

                return tb.ToList<AccountClient>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Telegram.pushLog("Repository GetDetailAccountClientByClientId" + ex.ToString());
                return null;
            }

        }
    }
}
