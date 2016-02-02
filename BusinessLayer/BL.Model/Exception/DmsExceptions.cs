namespace BL.Model.Exception
{
    public class DmsExceptions :System.Exception
    {

        public DmsExceptions(string message) : base(message)
        {
        }

        public DmsExceptions(string message, System.Exception ex) : base(message, ex)
        {

        }
    }


    public class UserHasNoAccessToDocument : DmsExceptions
    {
        private const string _MESSAGE = "User could not acceess this document!";
        public UserHasNoAccessToDocument():base(_MESSAGE)
        {
        }

        public UserHasNoAccessToDocument(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
}