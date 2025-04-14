namespace TastyGo.Interfaces.Other
{
    public interface IConstants
    {

        // Order Settings
        int MAX_ITEMS_PER_ORDER { get; }
        int MAX_DELIVERY_DISTANCE_KM { get; }

        // Fees & Charges
        double DELIVERY_FEE_PERCENTAGE { get; }
        double DELIVERY_FEE_CAP { get; }


        // System User (for system-generated actions)
        string SYSTEM_USER_EMAIL { get; }
        string SYSTEM_USER_FIRST_NAME { get; }
        string SYSTEM_USER_LAST_NAME { get; }

        // Token Configs
        int PASSWORD_RESET_TOKEN_EXPIRATION_MINUTES { get; }
        int PASSWORD_RESET_TOKEN_LENGTH { get; }
        int EMAIL_VERIFICATION_TOKEN_LENGTH { get; }
        int EMAIL_VERIFICATION_TOKEN_EXPIRATION_MINUTES { get; }

    }
}

