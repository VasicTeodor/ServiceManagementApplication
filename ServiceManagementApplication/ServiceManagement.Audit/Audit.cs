using ServiceManagement.CertificatesManager;
using ServiceManagement.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceManagement.Audit
{
    public class Audit : IAudit
    {
        public static bool shutDown = false;
        public string Port { get; set; }
        public string Protocol { get; set; }
        public string MachineName { get; set; }
        public int Counter { get; set; }
        private bool _canResetCnt;

        public Audit()
        {
            Port = "";
            Protocol = "";
            MachineName = "";
            shutDown = true;

            new Thread(() =>
            {
                Timer();
            }).Start();
            

        }
        public void DenialOfService()
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("ServiceManagement application detected DOS.", EventLogEntryType.Warning, 1, 101);
            }

            Console.WriteLine("DenialOfService() has been detected!");
        }

        public void AuthorizationFailure(string port, string protocol, string machineName)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("ServiceManagement application authorization failed.", EventLogEntryType.Warning);
            }
            Console.WriteLine("AuthorizationFailure() has been detected!");

            DetectDenialOfService(port, protocol, machineName);

            Port = port;
            Protocol = protocol;
            MachineName = machineName;
        }

        private void DetectDenialOfService(string port, string protocol, string machineName)
        {
            _canResetCnt = true;

            if (!string.IsNullOrEmpty(Port) && !string.IsNullOrEmpty(Protocol))
            {
                if (!port.ToLower().Equals(Port.ToLower()) && !protocol.ToLower().Equals(Protocol.ToLower()) && !machineName.ToLower().Equals(MachineName.ToLower()))
                {
                    Counter = 0;
                }
            }
            else if (!string.IsNullOrEmpty(Port))
            {
                if (!port.ToLower().Equals(Port.ToLower()) || !machineName.ToLower().Equals(MachineName.ToLower()))
                {
                    Counter = 0;
                }
            }
            else if (!string.IsNullOrEmpty(Protocol))
            {
                if (!protocol.ToLower().Equals(Protocol.ToLower()) || !machineName.ToLower().Equals(MachineName.ToLower()))
                {
                    Counter = 0;
                }
            }

            Counter += 1;

            if (Counter == 3)
            {
                DenialOfService();
            }
        }

        private void Timer()
        {
            do
            {
                if (Counter != 0)
                {
                    Stopwatch s = new Stopwatch();
                    s.Start();
                    Console.WriteLine("Timer is started.");
                    while (s.Elapsed < TimeSpan.FromSeconds(10))
                    {
                        if (Counter == 0)
                        {
                            Console.WriteLine("Counter is back to 0.");
                            _canResetCnt = false;
                            break;
                        }
                    }
                    s.Stop();
                    if(_canResetCnt)
                    {
                        Counter = 0;
                        Console.WriteLine("Timeout for DoS! Counter is set to 0.");
                    }
                }

            } while (shutDown);
        }

        public void ChecksumFailed()
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("ServiceManagement application detected Checksum failure.", EventLogEntryType.Error, 1, 102);
            }

            Console.WriteLine("Checksum() has failed!");
        }
    }
}
