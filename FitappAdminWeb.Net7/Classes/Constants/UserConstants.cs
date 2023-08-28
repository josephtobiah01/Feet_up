namespace FitappAdminWeb.Net7.Classes.Constants
{
    public static class UserConstants
    {
        public static readonly int ADMIN_USER_USERLEVEL_BASE = 1000;
        public static readonly int AUTOCREATED_USERLEVEL = 1;

        public static readonly int EXTYPE_UPLOADIMAGE_MAX_WIDTH = 163;
        public static readonly int EXTYPE_UPLOADIMAGE_MAX_HEIGHT = 121;
        public static readonly int EXTYPE_UPLOADIMAGE_SIZELIMIT = 4194304;
    }

    public enum SignupStatus
    {
        INCOMPLETE = 0,
        REVIEW_PENDING = 1,
        COMPLETE = 2
    }
}
