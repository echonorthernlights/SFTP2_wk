using SFTP2.Data.Entities;

namespace SFTP2.Services
{
    public class GlobalDataService
    {
        private object _dataLock = new object();
        private InFlow _data;

        public InFlow Data
        {
            get
            {
                lock (_dataLock)
                {
                    return _data;
                }
            }
            set
            {
                lock (_dataLock)
                {
                    _data = value;
                }
            }
        }
    }

}
