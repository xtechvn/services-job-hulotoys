﻿using App_Push_Consummer.Common;
using App_Push_Consummer.Model.Address;
using App_Push_Consummer.Model.Client;
using App_Push_Consummer.Model.Comments;
using App_Push_Consummer.Model.Order;
using HuloToys_Service.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace App_Push_Consummer.Model.DB_Core
{
    public static class Repository
    {
        private static string tele_group_id = ConfigurationManager.AppSettings["tele_group_id"];
        private static string tele_token = ConfigurationManager.AppSettings["tele_token"];
        public static int saveAddressClient(AddressModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[9];
                objParam_order[0] = new SqlParameter("@ClientId", model.ClientId);
                objParam_order[1] = new SqlParameter("@ReceiverName", model.ReceiverName);
                objParam_order[2] = new SqlParameter("@Phone", model.Phone);
                objParam_order[3] = new SqlParameter("@ProvinceId", model.ProvinceId);
                objParam_order[4] = new SqlParameter("@DistrictId", model.DistrictId);
                objParam_order[5] = new SqlParameter("@WardId", model.WardId);
                objParam_order[6] = new SqlParameter("@Address", model.Address);
                objParam_order[7] = new SqlParameter("@Status", model.Status);
                objParam_order[8] = new SqlParameter("@IsActive", model.IsActive);

                var id = DBWorker.ExecuteNonQuery("sp_InsertAddressClient", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>saveAddressClient error queue = " + ex.ToString());
                return -1;
            }
        }
        public static int updateAddressClient(AddressModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[11];
                objParam_order[0] = new SqlParameter("@Id", model.Id);
                objParam_order[1] = new SqlParameter("@ClientId", model.ClientId);
                objParam_order[2] = new SqlParameter("@ReceiverName", model.ReceiverName);
                objParam_order[3] = new SqlParameter("@Phone", model.Phone);
                objParam_order[4] = new SqlParameter("@ProvinceId", model.ProvinceId);
                objParam_order[5] = new SqlParameter("@DistrictId", model.DistrictId);
                objParam_order[6] = new SqlParameter("@WardId", model.WardId);
                objParam_order[7] = new SqlParameter("@Address", model.Address);
                objParam_order[8] = new SqlParameter("@Status", model.Status);
                objParam_order[9] = new SqlParameter("@IsActive", model.IsActive);
                objParam_order[10] = new SqlParameter("@CreatedOn", model.CreatedOn);

                var id = DBWorker.ExecuteNonQuery("sp_UpdateAddressClient", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>updateAddressClient error queue = " + ex.ToString());
                return -1;
            }
        }
        public static int saveClient(AccountClientModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[20];
                objParam_order[0] = new SqlParameter("@ClientMapId", DBNull.Value);
                objParam_order[1] = new SqlParameter("@SaleMapId", DBNull.Value);
                objParam_order[2] = new SqlParameter("@ClientType", model.ClientType);
                objParam_order[3] = new SqlParameter("@ClientName", model.ClientName);
                objParam_order[4] = new SqlParameter("@Email", model.Email);
                objParam_order[5] = new SqlParameter("@Gender", DBNull.Value);
                objParam_order[6] = new SqlParameter("@Status", model.Status);
                objParam_order[7] = new SqlParameter("@Note", DBNull.Value);
                objParam_order[8] = new SqlParameter("@Avartar", DBNull.Value);
                objParam_order[9] = new SqlParameter("@JoinDate", DBNull.Value);
                objParam_order[10] = new SqlParameter("@isReceiverInfoEmail", model.isReceiverInfoEmail);
                objParam_order[11] = new SqlParameter("@Phone", model.Phone);
                objParam_order[12] = new SqlParameter("@Birthday", DBNull.Value);
                objParam_order[13] = new SqlParameter("@UpdateTime", DBNull.Value);
                objParam_order[14] = new SqlParameter("@TaxNo", DBNull.Value);
                objParam_order[15] = new SqlParameter("@AgencyType", 0);
                objParam_order[16] = new SqlParameter("@PermisionType", 0);
                objParam_order[17] = new SqlParameter("@BusinessAddress", DBNull.Value);
                objParam_order[18] = new SqlParameter("@ExportBillAddress", DBNull.Value);
                objParam_order[19] = new SqlParameter("@ClientCode", model.ClientCode);



                var id = DBWorker.ExecuteNonQuery("SP_InsertClient", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>saveAccountClient error queue = " + ex.ToString());
                return -1;
            }
        }
        public static int saveAccountClient(AccountClientModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[8];
                objParam_order[0] = new SqlParameter("@ClientId", model.ClientId);
                objParam_order[1] = new SqlParameter("@ClientType", model.ClientType);
                objParam_order[2] = new SqlParameter("@UserName", model.UserName);
                objParam_order[3] = new SqlParameter("@Password", model.Password);
                objParam_order[4] = new SqlParameter("@PasswordBackup", model.Password);
                objParam_order[5] = new SqlParameter("@ForgotPasswordToken", model.ForgotPasswordToken);
                objParam_order[6] = new SqlParameter("@Status", model.Status);
                objParam_order[7] = new SqlParameter("@GroupPermission", model.GroupPermission);


                var id = DBWorker.ExecuteNonQuery("sp_InsertAccountClient", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>saveAccountClient error queue = " + ex.ToString());
                return -1;
            }
        }
        public static int updateAccountClient(AccountClientModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[10];
                objParam_order[0] = new SqlParameter("@id", model.Id);
                objParam_order[1] = new SqlParameter("@ClientId", DBNull.Value);
                objParam_order[2] = new SqlParameter("@ClientType", DBNull.Value);
                objParam_order[3] = new SqlParameter("@UserName", DBNull.Value);
                objParam_order[4] = new SqlParameter("@Password", model.Password);
                objParam_order[5] = new SqlParameter("@PasswordBackup", model.Password);
                objParam_order[6] = new SqlParameter("@ForgotPasswordToken", model.ForgotPasswordToken);
                objParam_order[7] = new SqlParameter("@Status", DBNull.Value);
                objParam_order[8] = new SqlParameter("@GroupPermission", DBNull.Value);
                objParam_order[9] = new SqlParameter("@GoogleToken", DBNull.Value);

                var id = DBWorker.ExecuteNonQuery("sp_UpdateAccountClient", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>updateAccountClient error queue = " + ex.ToString());
                return -1;
            }
        }
        public static int saveComments(long ClientId , string Content)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[4];
                objParam_order[0] = new SqlParameter("@ClientId", ClientId);
                objParam_order[1] = new SqlParameter("@Content", Content);
                objParam_order[2] = new SqlParameter("@CreatedDate", DBNull.Value);
                objParam_order[3] = new SqlParameter("@ModifiedDate", DBNull.Value);


                var id = DBWorker.ExecuteNonQuery("sp_InsertComments", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>saveComments error queue = " + ex.ToString());
                return -1;
            }
        }
        public static ClientModel GetClientByAccountClientId(long AccountClientID)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@AccountClientID", AccountClientID);

                DataTable tb = DBWorker.GetDataTable("SP_GetClientByAccountClientId", objParam_order);
                if(tb != null && tb.Rows.Count > 0)
                {
                    var data = tb.ToList<ClientModel>();
                    return data[0];
                }
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>GetClientByAccountClientId error queue = " + ex.ToString());
            }
            return null;
        }
        public static int saveReceiverInfoEmail( string Email)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@Email", Email);


                var id = DBWorker.ExecuteNonQuery("sp_InsertReceivePromotions", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>saveComments error queue = " + ex.ToString());
                return -1;
            }
        }
        public static int InsertProductRaiting(ProductRaitingPushQueueModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[]{
                    new SqlParameter("@OrderId", model.OrderId),
                    new SqlParameter("@ProductId", model.ProductId),
                    new SqlParameter("@ProductDetailId", model.ProductDetailId),
                    new SqlParameter("@Star", model.Star),
                    new SqlParameter("@Comment", model.Comment),
                    new SqlParameter("@ImgLink", model.ImgLink),
                    new SqlParameter("@VideoLink", model.VideoLink),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@CreatedDate", model.CreatedDate),

                };

                var id = DBWorker.ExecuteNonQuery("sp_InsertRating", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>InsertProductRaiting error queue = " + ex.ToString());
                return -1;
            }
        }
        public static List<Rating> GetRaitingByProductID(string product_id)
        {
            try
            {
                int page_index = 1;
                int page_size = 4000;
                SqlParameter[] objParam_order = new SqlParameter[]
                {
                    new SqlParameter("@ProductId", product_id),
                    new SqlParameter("@UserId", DBNull.Value),
                    new SqlParameter("@PageIndex", page_index),
                    new SqlParameter("@PageSize", page_size),
                };

                DataTable tb = DBWorker.GetDataTable("SP_GetListRatingByProductId", objParam_order);
                if (tb != null && tb.Rows.Count > 0)
                {
                    var data = tb.ToList<Rating>();
                    return data;
                }
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>GetClientByAccountClientId error queue = " + ex.ToString());
            }
            return null;
        }
        public static int UpdateOrder(OrderModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[]{
                    new SqlParameter("@OrderId", model.OrderId),
                    new SqlParameter("@ClientId", model.ClientId),
                    new SqlParameter("@OrderNo", model.OrderNo),
                    new SqlParameter("@Price", model.Price),
                    new SqlParameter("@Profit", model.Profit),
                    new SqlParameter("@Discount", model.Discount),
                    new SqlParameter("@Amount", model.Amount),
                    new SqlParameter("@Status", model.OrderStatus),
                    new SqlParameter("@PaymentType", model.PaymentType),
                    new SqlParameter("@PaymentStatus", model.PaymentStatus),
                    new SqlParameter("@UtmSource", model.UtmSource),
                    new SqlParameter("@UtmMedium", model.UtmMedium),
                    new SqlParameter("@Note", model.Note),
                    new SqlParameter("@VoucherId", model.VoucherId),
                    new SqlParameter("@IsDelete", model.IsDelete),
                    new SqlParameter("@UserId", model.UserId),
                    new SqlParameter("@UserGroupIds", model.UserGroupIds),
                    new SqlParameter("@UserUpdateId", model.UserUpdateId),
                    new SqlParameter("@ProvinceId", model.ProvinceId),
                    new SqlParameter("@DistrictId", model.DistrictId),
                    new SqlParameter("@WardId", model.WardId),
                    new SqlParameter("@Address", model.Address),
                    new SqlParameter("@ShippingFee", model.ShippingFee),
                    new SqlParameter("@CarrierId", model.CarrierId),
                    new SqlParameter("@ShippingType", model.ShippingType),
                    new SqlParameter("@ShippingCode", model.ShippingCode),
                    new SqlParameter("@ShippingStatus", model.ShippingStatus),
                    new SqlParameter("@PackageWeight", model.PackageWeight),

                };

                var id = DBWorker.ExecuteNonQuery("sp_UpdateOrder", objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                ErrorWriter.InsertLogTelegramByUrl(tele_token, tele_group_id, "GetDataset =>InsertProductRaiting error queue = " + ex.ToString());
                return -1;
            }
        }
    }
}
