using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Linq2Xml
{
    class Program
    {
        static void Main(string[] args)
        {
            XNamespace ns1 = "http://pearson.com/course";
            XNamespace ns2 = "http://pearson.com/livelessons";

            var xml = new XElement(ns1 + "course", 
                new XAttribute(XNamespace.Xmlns + "ll", "http://pearson.com/livelessons"),
                new XElement(ns2 + "lesson", new XAttribute("title", "Introducting LINQ"),
                    new XElement("sublesson", new XAttribute("title", "Get started with LINQ"))),
                new XElement(ns2 + "lesson", new XAttribute("title", "Building LINQ Queries...")));

            Console.WriteLine(xml);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            using (XmlReader reader = XmlReader.Create(@"../../data/Course.xml", settings))
            {
                var course = XElement.Load(reader);
                var result = (from l in course.Elements("lesson")
                    where l.Attribute("id").Value.Equals("1")
                    select l).First().Attributes();

                foreach (var c in result)
                {
                    Console.WriteLine(c);
                }
            } 

            string rawXmlData = System.IO.File.ReadAllText(@"../../data/PurchaseOrder.xml");
            XElement purchaseOrders = XElement.Parse(rawXmlData);
            IEnumerable<string> partNos = from item in purchaseOrders.Descendants("Item")
                select (string) item.Attribute("PartNumber");

            foreach (var pno in partNos)
            {
                Console.WriteLine(pno);
            }

            Console.ReadKey();
        }
    }
}
