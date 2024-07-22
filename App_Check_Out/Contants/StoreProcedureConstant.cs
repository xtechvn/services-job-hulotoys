using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.Contants
{
    public static class StoreProcedureConstant
    {
        public static string GetClientByID = "SP_GetClientByID";
        public static string GetClientByAccountClientID = "SP_GetClientByAccountClientID";
        public static string GetContactClientByID = "SP_GetContactClientByID";
        public static string GetContractByID = "SP_GetContractByID";
        public static string GetFlyBookingDetailByOrderID = "SP_GetFlyBookingDetailByOrderID";
        public static string GetOrderByID = "SP_GetOrderByID";
        public static string GetPassengerByContactClientID = "SP_GetPassengerByContactClientID";
        public static string CreateContactClients = "SP_CreateContactClients";
        public static string CreateFlyBookingDetail = "SP_CreateFlyBookingDetail";
        public static string CreateOrder = "SP_CreateOrder";
        public static string CreatePassengers = "SP_CreatePassengers";
        public static string CreateBaggage = "SP_CreateBaggage";
        public static string InsertFlyBookingExtraPackages = "SP_InsertFlyBookingExtraPackages";
        public static string CheckIfNewOrderValid = "SP_CheckIfNewOrderValid";
        public static string CreateFlySegment = "SP_CreateFlySegment";
        public static string CreateHotelBooking = "SP_CreateHotelBooking";
        public static string CreateHotelBookingRoomRates = "SP_CreateHotelBookingRoomRates";
        public static string CreateHotelBookingRooms = "SP_CreateHotelBookingRooms";
        public static string CreateHotelGuest = "SP_CreateHotelGuest";
        public static string InsertVinWonderBooking = "SP_InsertVinWonderBooking";
        public static string InsertVinWonderBookingTicket = "sp_InsertVinWonderBookingTicket";
        public static string InsertVinWonderBookingTicketDetail = "sp_InsertVinWonderBookingTicketDetail";
        public static string InsertVinWonderBookingTicketCustomer = "sp_InsertVinWonderBookingTicketCustomer";
        public static string SP_GetVinWondeCustomerByBookingId = "SP_GetVinWondeCustomerByBookingId";
        public static string SP_GetVinWonderBookingByOrderID = "SP_GetVinWonderBookingByOrderID";
        public static string SP_GetVinWonderBookingTicketByBookingID = "SP_GetVinWonderBookingTicketByBookingID";
        public static string SP_UpdateOperatorByOrderid = "SP_UpdateOperatorByOrderid";
        public static string InsertTour = "SP_InsertTour";
        public static string InsertTourPackages = "SP_InsertTourPackages";
        public static string SP_GetDetailTourProductByID = "SP_GetDetailTourProductByID";
        public static string sp_updateVinWonderBooking = "sp_updateVinWonderBooking";
        public static string InsertHotelBookingRoomsOptional = "sp_InsertHotelBookingRoomsOptional";
        public static string UpdateHotelBookingRoomsOptional = "sp_UpdateHotelBookingRoomsOptional";
        public static string InsertHotelBookingRoomRatesOptional = "sp_InsertHotelBookingRoomRatesOptional";

    }
}
