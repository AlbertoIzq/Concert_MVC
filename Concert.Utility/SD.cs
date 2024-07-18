using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concert.Utility
{
    public static class SD
    {
        public const string DATABASE_USED_LOCAL = "Local";
        public const string DATABASE_USED_AZURE = "Azure";
        public const string DATABASE_USED_LOCAL_ENV = "DataBase_ConnectionString_Local";
        public const string DATABASE_USED_AZURE_ENV = "DataBase_ConnectionString_Azure";

        public const string ROLE_CUSTOMER = "Customer";
        public const string ROLE_COMPANY = "Company";
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_EMPLOYEE = "Employee";

		public const string STATUS_PENDING = "Pending";
		public const string STATUS_APPROVED = "Approved";
		public const string STATUS_IN_PROCESS = "Processing";
		public const string STATUS_CONFIRMED = "Confirmed";
		public const string STATUS_CANCELLED = "Cancelled";
		public const string STATUS_REFUNDED = "Refunded";

		public const string PAYMENT_STATUS_PENDING = "Pending";
		public const string PAYMENT_STATUS_APPROVED = "Approved";
		public const string PAYMENT_STATUS_DELAYED_PAYMENT = "ApprovedForDelayedPayment";
        public const string PAYMENT_STATUS_REFUNDED = "Refunded";
        public const string PAYMENT_STATUS_CANCELLED = "Cancelled";
        public const string PAYMENT_STATUS_REJECTED = "Rejected";

        public const string SESSION_SETLIST = "SessionSetList";

        public const int PAYMENT_DELAYED_DAYS = 30;
	}
}
