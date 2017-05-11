namespace BL.Model.Context
{
    /// <summary>
    /// класс сотрудника
    /// </summary>
    public class Client
    {
        public int Id
        {
            get
            {
                if (_Id < 0) return 0;
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        private int _Id;
        public string Code { get; set; }
    }
}