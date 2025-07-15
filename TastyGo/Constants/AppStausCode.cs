namespace TastyGo.Helpers
{
    public enum AppStatusCode
    {
        // General (1000–1099)
        SUCCESS = 1000,
        CREATED = 1001,
        NOT_FOUND = 1002,
        UNAUTHORIZED = 1003,
        FORBIDDEN = 1004,
        CONFLICT = 1005,
        VALIDATION_ERROR = 1006,

        INVALID_PASSWORD_EMAIL = 1007,

        // Authentication & Account (1100–1199)
        AUTH_SUCCESS = 1100,
        AUTH_FAILED = 1101,

        EMAIL_ALREADY_EXISTS = 1102,
        EMAIL_NOT_VERIFIED = 1103,
        ACCOUNT_LOCKED = 1104,
        ACCOUNT_NOT_FOUND = 1105,
        PASSWORD_RESET_SENT = 1106,
        PASSWORD_RESET_FAILED = 1107,
        TOKEN_EXPIRED = 1108,
        TOKEN_INVALID = 1109,

        // Validation & Input (1200–1299)
        FIELD_REQUIRED = 1200,
        INVALID_PHONE_FORMAT = 1201,
        INVALID_DATE_FORMAT = 1202,
        DUPLICATE_ENTRY = 1203,

        // Order & Cart (1300–1399)
        ORDER_CREATED = 1300,
        ORDER_NOT_FOUND = 1301,
        ORDER_ALREADY_DELIVERED = 1302,
        ORDER_CANCELLED = 1303,
        CART_EMPTY = 1304,
        ITEM_NOT_AVAILABLE = 1305,

        // Restaurant & Menu (1400–1499)
        RESTAURANT_NOT_FOUND = 1400,
        ITEM_ALREADY_EXISTS = 1401,
        ITEM_REMOVED = 1402,
        MENU_UPDATE_FAILED = 1403,

        // Wallet & Payments (1500–1599)
        WALLET_TOPUP_SUCCESS = 1500,
        WALLET_TOPUP_FAILED = 1501,
        INSUFFICIENT_FUNDS = 1502,
        TXN_NOT_FOUND = 1503,
        PAYMENT_SUCCESS = 1504,
        PAYMENT_FAILED = 1505,
        REFUND_ISSUED = 1506,

        // Driver & Delivery (1600–1699)
        DRIVER_NOT_FOUND = 1600,
        DRIVER_NOT_VERIFIED = 1601,
        DRIVER_ALREADY_ASSIGNED = 1602,
        DELIVERY_STARTED = 1603,
        DELIVERY_COMPLETED = 1604,

        // Promo (1700–1799)
        PROMO_APPLIED = 1700,
        PROMO_INVALID = 1701,
        PROMO_USAGE_LIMIT = 1702,

        // System (1800–1899)
        SERVER_ERROR = 1800,
        DB_ERROR = 1801,
        SERVICE_UNAVAILABLE = 1802,
        TIMEOUT = 1803
    }
}
