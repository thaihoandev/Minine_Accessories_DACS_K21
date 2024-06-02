namespace Accessories_Store.Helpers
{
	public class MySetting
	{
		
	}
	public static class PaymentType
	{
		public static string COD = "COD";
		public static int StatusCOD = 1;

		public static string VNPAY = "VnPay";
		public static int StatusVNPAY = 2;

		public static int StatusMOMO = 3;
		public static string MOMO = "Momo";

		public static int StatusPAYPAL = 4;
		public static string PAYPAL = "Paypal";

	}
    public static class Status
    {
        public static int StatusDelete = -1;
        public static int StatusCancel = 0;
        public static int StatusOk = 1;
        public static int StatusNotConfirmed = 2;

        public static int StatusTransporting = 3;

    }

    public static class CategoryTypeStatus
    {
        public static int CategoryStatus = 1;
        public static int ObjectStatus = 2;
        public static int CollectionStatus = 3;

    }
}
