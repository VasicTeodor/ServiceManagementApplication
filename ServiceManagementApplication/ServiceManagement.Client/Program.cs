using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using static ServiceManagement.Common.ClientParameters;

namespace ServiceManagement.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            string address = "net.tcp://localhost:9999/ServiceManagement";

            using (ClientProxy proxy = new ClientProxy(binding, new EndpointAddress(new Uri(address))))
            {
                IFormatter form = new BinaryFormatter();

                string key = proxy.Connect();
                bool admin = false;

                ClientInfo client = new ClientInfo();
                client.machineName = Environment.MachineName;

                Console.WriteLine("******************************");


                foreach (IdentityReference group in WindowsIdentity.GetCurrent().Groups)
                {
                    SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                    var name = sid.Translate(typeof(NTAccount));

                    if (name.Value.Contains("Admin"))
                    {
                        admin = true;
                    }
                }
                
                int num = -1;

                do
                {

                    if (admin)
                    {
                        Console.WriteLine("1. Start service");
                        Console.WriteLine("2. Add port to blacklist");
                        Console.WriteLine("3. Add protocol to blacklist");
                        Console.WriteLine("4. Add port and protocol to blacklist");
                        Console.WriteLine("5. Exit");

                        Int32.TryParse(Console.ReadLine(), out num);

                        switch (num)
                        {
                            case 1:
                                Console.Clear();
                                Console.WriteLine("1. Enter port");
                                client.port = Console.ReadLine();
                                Console.WriteLine("2. Enter protocol");
                                client.protocol = Console.ReadLine();

                                MemoryStream ser = new MemoryStream();
                                form.Serialize(ser, (object)client);
                                byte[] iVector;
                                var chiped = EncryptData(ser, key, out iVector);

                                if (proxy.Checksum())
                                {
                                    proxy.StartService(chiped, (int)ser.Length, iVector);
                                }
                                else
                                {
                                    Console.WriteLine("Checksum is not valid.");
                                }

                                break;
                            case 2:
                                Console.Clear();
                                Console.WriteLine("1. Enter port");
                                string port = Console.ReadLine();
                                Console.WriteLine("2. Enter group (Admin, Client, Executor)");
                                string group = Console.ReadLine();

                                if (proxy.Checksum())
                                {
                                    proxy.AddPort(port, group);
                                }
                                else
                                {
                                    Console.WriteLine("Checksum is not valid.");
                                }
                                break;
                            case 3:
                                Console.Clear();
                                Console.WriteLine("1. Enter protocol");
                                string protocol = Console.ReadLine();
                                Console.WriteLine("2. Enter group (Admin, Client, Executor)");
                                string group2 = Console.ReadLine();

                                if (proxy.Checksum())
                                {
                                    proxy.AddProtocol(protocol, group2);
                                }
                                else
                                {
                                    Console.WriteLine("Checksum is not valid.");
                                }

                                break;
                            case 4:
                                Console.Clear();
                                Console.WriteLine("1. Enter port");
                                string port1 = Console.ReadLine();
                                Console.WriteLine("2. Enter protocol");
                                string protocol2 = Console.ReadLine();
                                Console.WriteLine("3. Enter group (Admin, Client, Executor)");
                                string group3 = Console.ReadLine();

                                if (proxy.Checksum())
                                {
                                    proxy.AddPortAndProtocol(port1, protocol2, group3);
                                }
                                else
                                {
                                    Console.WriteLine("Checksum is not valid.");
                                }

                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine(" Start service ");
                        Console.WriteLine("1. Enter port");
                        client.port = Console.ReadLine();
                        Console.WriteLine("2. Enter protocol");
                        client.protocol = Console.ReadLine();

                        MemoryStream ser = new MemoryStream();
                        form.Serialize(ser, (object)client);
                        byte[] iVector;
                        var chiped = EncryptData(ser, key, out iVector);
                        proxy.StartService(chiped, (int)ser.Length, iVector);
                        
                    }

                } while (num != 5);
            }
        }

        private static byte[] EncryptData(MemoryStream serialized, string key, out byte[] iv)
        {
            //encrypt serialized data
            Aes alg = Aes.Create();
            alg.GenerateIV();
            iv = alg.IV;
            byte[] aesKey = new byte[32];
            Array.Copy(Encoding.ASCII.GetBytes(key), 0, aesKey, 0, 32);
            alg.Key = aesKey;
            alg.Mode = CipherMode.CBC;
            serialized.Position = 0;

            MemoryStream enc = new MemoryStream();
            CryptoStream cw = new CryptoStream(enc, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cw.Write(serialized.ToArray(), 0, (int)serialized.Length);
            cw.FlushFinalBlock(); // Also .Close()'s

            enc.Position = 0; // rewind

            return enc.ToArray();
        }
    }
}
