using MetroFtpClient.Ftp.Contracts.Interfaces;
using MetroFtpClient.Infrastructure.Enums;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroFtpClient.Infrastructure.Notifications
{
    public class AddOrUpdateConnectionNotification : Confirmation
    {
        public AddOrUpdateConnectionNotification(DataOperation dataOperation = DataOperation.Insert, IConnectionSettings connectionSettings = null)
        {
            this.DataOperation = dataOperation;
            this.ConnetionSettings = connectionSettings;
        }

        public DataOperation DataOperation { get; private set; }
        public IConnectionSettings ConnetionSettings { get; set; }
    }
}
