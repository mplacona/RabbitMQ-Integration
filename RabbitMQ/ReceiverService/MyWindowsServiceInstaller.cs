using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ReceiverService
{
    [RunInstaller(true)]
    public class MyWindowsServiceInstaller : Installer
    {
        public MyWindowsServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            // set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.ServiceName = "Q-Adwords-CreateCampaign Receiver";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "Q-Adwords-CreateCampaign Receiver";
            serviceInstaller.Description = "A receiver for Q-Adwords-CreateCampaign";

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
