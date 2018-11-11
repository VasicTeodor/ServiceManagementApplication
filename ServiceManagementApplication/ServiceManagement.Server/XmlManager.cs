using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServiceManagement.Server
{
    public class XmlManager
    {
        private static XmlManager _instance;
        private string fileName = "../../../blacklist.xml";
        public XmlManager() { }
        public static XmlManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new XmlManager();

                return _instance;
            }
        }

        public bool IsPortInBlackList(string port, string groupp)
        {
            XDocument xmlDocument = XDocument.Load(fileName);

            bool retVal = (from elemnt in xmlDocument.Root.Elements("BlackClass")
                           where (elemnt.Element("Group").Value.ToString().ToLower().Equals(groupp.ToLower()) &&
                           (elemnt.Element("Port").Value.ToString().ToLower().Equals(port.ToLower())))
                           select elemnt).Any();

            return retVal;
        }

        public bool IsProtocolInBlackList(string protocol, string groupp)
        {
            XDocument xmlDocument = XDocument.Load(fileName);

            bool retVal = (from elemnt in xmlDocument.Root.Elements("BlackClass")
                           where (elemnt.Element("Group").Value.ToString().ToLower().Equals(groupp.ToLower()) &&(
                           elemnt.Element("Protocol").Value.ToString().ToLower().Equals(protocol.ToLower())))
                           select elemnt).Any();

            return retVal;
        }

        public bool IsInBlackList(string port, string protocol, string groupp)
        {
            XDocument xmlDocument = XDocument.Load(fileName);

            bool retVal = (from elemnt in xmlDocument.Root.Elements("BlackClass")
                           where (elemnt.Element("Group").Value.ToString().ToLower().Equals(groupp.ToLower()) && 
                           (elemnt.Element("Port").Value.ToString().ToLower().Equals(port.ToLower()) ||
                           elemnt.Element("Protocol").Value.ToString().ToLower().Equals(protocol.ToLower())))
                           select elemnt).Any();

            return retVal;
        }

        public bool Write(BlackClass blackClass)
        {
            if (!File.Exists(fileName))
            {
                XDocument xmlDocument = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),

                new XElement("BlackList",
                new XElement("BlackClass",
                new XElement("Port", blackClass.Port),
                new XElement("Protocol", blackClass.Protocol),
                new XElement("Group", blackClass.Group))
                ));

                try
                {
                    xmlDocument.Save(fileName);
                    return true;
                }
                catch(Exception ex)
                {
                    Trace.WriteLine("Error: " + ex.Message);
                    return false;
                }

            }
            else
            {
                try
                {
                    FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    XDocument doc = XDocument.Load(stream);
                    XElement customers = doc.Element("BlackList");
                    customers.Add(new XElement("BlackClass",
                                  new XElement("Port", blackClass.Port),
                                  new XElement("Protocol", blackClass.Protocol),
                                  new XElement("Group", blackClass.Group)));
                    doc.Save(fileName);
                    return true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
