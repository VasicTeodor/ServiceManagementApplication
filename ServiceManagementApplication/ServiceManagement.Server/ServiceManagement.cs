using ServiceManagement.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ServiceManagement.Common.ClientParameters;

namespace ServiceManagement.Server
{
    public class ServiceManagement : IServiceManagement
    {
        private string Key { get; set; }
        private byte[] _checksum;
        
        public void AddPort(string port, string group)
        {
            try
            {
                MyPrincipal mp = Thread.CurrentPrincipal as MyPrincipal;

                if (mp.IsInRole("ModifyBlackList"))
                {
                    XmlManager.Instance.Write(new BlackClass(port , "Null", group));

                    Console.WriteLine("AddPort() executed!");

                    _checksum = GetCheckSum();
                }
            }
            catch (Exception ex)
            {
                throw new SecurityException("Authorization failed" + ex.Message);
            }
        }

        public void AddPortAndProtocol(string port, string protocol, string group)
        {
            try
            {
                MyPrincipal mp = Thread.CurrentPrincipal as MyPrincipal;

                if (mp.IsInRole("ModifyBlackList"))
                {
                    XmlManager.Instance.Write(new BlackClass(port , protocol, group));

                    Console.WriteLine("AddPortAndProtocol() executed!");

                    _checksum = GetCheckSum();
                }
            }
            catch (Exception ex)
            {
                throw new SecurityException("Authorization failed" + ex.Message);
            }
        }

        public void AddProtocol(string protocol, string group)
        {
            
            try
            {
                MyPrincipal mp = Thread.CurrentPrincipal as MyPrincipal;

                if (mp.IsInRole("ModifyBlackList"))
                {
                    XmlManager.Instance.Write(new BlackClass("-1", protocol, group));

                    Console.WriteLine("AddProtocol() executed!");

                    _checksum = GetCheckSum();
                }
            }
            catch (Exception ex)
            {
                throw new SecurityException("Authorization failed" + ex.Message);
            }
        }

        public bool Checksum()
        {
            bool retval = GetCheckSum().SequenceEqual(_checksum);

            if (retval)
            {
                return retval;
            }
            else
            {
                AuditClient.Instance.Proxy.ChecksumFailed();

                try
                {
                    Process.GetCurrentProcess().Kill();
                }
                catch(Exception ex)
                {
                    Trace.WriteLine("Error: " + ex.Message);
                }
                return retval;
            }
        }

        public string Connect()
        {
            try
            {
                MyPrincipal mp = Thread.CurrentPrincipal as MyPrincipal;

                if (mp.IsInRole("ExchangeSessionKey"))
                {
                    //string sessionId = OperationContext.Current.SessionId;
                    Console.WriteLine("Connect() executed!");
                    Key = RandomString(32);
                    //Key = "5ome5ecRetK3y123L45Q67u89asadfjiiweoavmpdpbgyjmpsiogujm98reubopgifpujgaebpgokpiogpgpodfkggkpi90485698um9849bobai3jnygv78tbaf874";
                    _checksum = GetCheckSum();
                    return Key;
                }
                return "";
            }
            catch(Exception ex)
            {
                throw new SecurityException("Authorization failed" + ex.Message);
            }
        }

        public void StartService(byte[] array, int size, byte[] iVector)
        {
            try
            {
                MyPrincipal mp = Thread.CurrentPrincipal as MyPrincipal;

                if (mp.IsInRole("RunService"))
                {
                    IFormatter form = new BinaryFormatter();
                    Aes alg = Aes.Create();
                    alg.IV = iVector;
                    byte[] aesKey = new byte[32];
                    Array.Copy(Encoding.ASCII.GetBytes(Key), 0, aesKey, 0, 32);
                    alg.Key = aesKey;
                    alg.Mode = CipherMode.CBC;
                    Console.WriteLine("Decrypt test..");
                    MemoryStream bencr = new MemoryStream(array);

                    CryptoStream cr = new CryptoStream(bencr, alg.CreateDecryptor(), CryptoStreamMode.Read);

                    byte[] buf = new byte[size];
                    cr.Read(buf, 0, size);

                    MemoryStream unenc = new MemoryStream(buf);
                    unenc.Position = 0;

                    Console.WriteLine("Deserialize test..");
                    ClientInfo resd = (ClientInfo)form.Deserialize(unenc);
                    Console.WriteLine("Got: " + resd.port);
                    Console.WriteLine("Got: " + resd.protocol);
                    Console.WriteLine("Got: " + resd.machineName);

                    if (CheckBlackList(resd, mp.Groups))
                    {
                        return;
                    }
                    else
                    {
                        Process p = new Process();                     
                        p.StartInfo.FileName = @"..\..\..\ServiceManagement.Service\bin\Debug\ServiceManagement.Service.exe"; 
                        p.StartInfo.Arguments = resd.port + " " + resd.protocol;
                        p.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SecurityException("Authorization failed" + ex.Message);
            }
        }

        private bool CheckBlackList(ClientInfo client, List<String> groups)
        {

            foreach (String s in groups)
            {
                bool retVal = XmlManager.Instance.IsInBlackList(client.port, client.protocol, s);

                if (retVal)
                {
                    if (XmlManager.Instance.IsPortInBlackList(client.port, s) && XmlManager.Instance.IsProtocolInBlackList(client.protocol, s))
                    {
                        AuditClient.Instance.Proxy.AuthorizationFailure(client.port, client.protocol, client.machineName);
                    }
                    else if(XmlManager.Instance.IsPortInBlackList(client.port, s))
                    {
                        AuditClient.Instance.Proxy.AuthorizationFailure(client.port, "", client.machineName);
                    }
                    else if(XmlManager.Instance.IsProtocolInBlackList(client.protocol, s))
                    {
                        AuditClient.Instance.Proxy.AuthorizationFailure("", client.protocol, client.machineName);
                    }
                    return true;
                }
            }
            return false;
        }

        private byte[] GetCheckSum()
        {
            try
            {
                MyPrincipal mp = Thread.CurrentPrincipal as MyPrincipal;

                if (mp.IsInRole("ModifyBlackList"))
                {
                    string fileName = "../../../blacklist.xml";

                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(fileName))
                        {
                            return md5.ComputeHash(stream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SecurityException("Authorization failed" + ex.Message);
            }
            return null;
        }

        private string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
