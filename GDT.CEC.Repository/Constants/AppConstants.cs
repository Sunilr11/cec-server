
namespace GDT.CEC.Repository.Constants
{
    public static class AppConstants
    {
        #region General
        public const string MODEL_VALIDATION_FAILED = "Model validation failed";
        public const string GENERIC_EXCEPTION_MESSAGE = "An internal error occurred in the system. Please contact the technical support team!";
        public const string FILE_IMPORTED_SUCCESSFULLY = "File imported and data processed successfully";
        public const string NO_FILE_UPLOADED = "No file uploaded";
        public const string INVALID_INPUT_REQUEST = "Invalid input request!";

        public const string NO_USER_PERMISSION = "User does not have permission to view/edit this.";
        public const string USER_INACTIVE = "User is Disabled.Please contact the Administrator.";
        #endregion

        #region ROLES
        public const int ROLE_ID_ADMIN = 1;
        public const int ROLE_ID_USER = 2;
        public const int ROLE_ID_SUPERADMIN = 3;

        #endregion

        #region 
        public const string EMAILTEMPLATE_OTP = @"wwwroot/Templates/otpTemplate.html";
        public const string EMAILTEMPLATE_CRED = @"wwwroot/Templates/credTemplate.html";
        public const string EMAILTEMPLATE_REJECT = @"wwwroot/Templates/rejectTemplate.html";
        #endregion

        #region Users
        public const string USERS_ADDED_SUCCESSFULLY = "Users added successfully!";
        public const string INVALID_OBJECTID_FORMAT = "Invalid ObjectId format!";
        public const string USERS_DELETED_SUCCESSFULLY = "Users deleted successfully!";
        public const string USERS_UPDATED_SUCCESSFULLY = "Users updated successfully!";
        public const string USERS_FETCHED_SUCCESSFULLY = "Users fetched successfully!";
        public const string USER_ALREADY_EXIST = "User with this email already exist!";
        public const string USER_OTP_FAILED = "Otp verification failed!";
        public const string USER_OTP_SUCCESS = "Otp verification success!";
        public const string USER_REGISTRATION_REJECTED = "Registration Sucessfully Rejected";
        public const string USER_REGISTRATION_APPROVED = "Registration Sucessfully Approved";
        public const string IMPORT_USER_STARTED = "Import user Started!";
        public const string EMAIL_CRED_SUBJECT = "GDT account credentials";
        public const string EMAIL_REJECT_SUBJECT = "GDT account registration REJECTED";

        public const string INVALID_EMAIL = "Please enter a valid email!";
        #endregion

        #region Mongo Collections
        public const string COLLECTION_USER = "users";
        public const string COLLECTION_MENU = "menus";
        public const string COLLECTION_LABS = "labs";
        public const string COLLECTION_CATEGORY = "categories";
        public const string COLLECTION_HOME = "hometemplate";
        public const string COLLECTION_NETWORK = "networktemplate";
        public const string COLLECTION_MOBILITY = "mobilitytemplate";
        public const string COLLECTION_DIGITALWORKSPACE = "digitalworkspacetemplate";
        public const string COLLECTION_SECURITY = "securitytemplate";
        public const string COLLECTION_CLOUD = "cloudtemplate";
        public const string COLLECTION_ABOUT = "abouttemplate";
        public const string COLLECTION_AREAOFINTEREST = "areaofinterest";
        public const string COLLECTION_LABACCESSLOG = "useraccesslogs";
        public const string COLLECTION_MENUDETAILS = "menudetails";
        public const string COLLECTION_ROLE = "role";
        #endregion

        #region Cache Key

        public const string ALL_USER_DOC_KEYNAME = "AllUsers";

        #endregion
    }
}
