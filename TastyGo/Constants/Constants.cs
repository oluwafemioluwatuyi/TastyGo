using TastyGo.Interfaces.Other;

namespace TastyGo.Constants
{
    public class Constants : IConstants
    {
        public int MAX_ITEMS_PER_ORDER => 9;

        public int MAX_DELIVERY_DISTANCE_KM => 25; // Max distance allowed for delivery in kilometers

        public double DELIVERY_FEE_PERCENTAGE => 5.5; // % of order subtotal (e.g., 5.5%)

        public double DELIVERY_FEE_CAP => 1000.0; // Max delivery fee in Naira or your local currency

        public string SYSTEM_USER_EMAIL => "system@tastygo.com";

        public string SYSTEM_USER_FIRST_NAME => "System";

        public string SYSTEM_USER_LAST_NAME => "Bot";

        public int PASSWORD_RESET_TOKEN_EXPIRATION_MINUTES => 30; // Valid for 30 minutes

        public int PASSWORD_RESET_TOKEN_LENGTH => 6; // 6-digit numeric token

        public int EMAIL_VERIFICATION_TOKEN_LENGTH => 6; // Also 6-digit, consistent with reset

        public int EMAIL_VERIFICATION_TOKEN_EXPIRATION_MINUTES => 60; // 1 hour
        public int REFRESH_TOKEN_LENGTH => 32; // Length of the refresh token
        public int REFRESH_TOKEN_EXPIRATION_DAYS => 7; // Refresh token valid for 7 days
    }
}
