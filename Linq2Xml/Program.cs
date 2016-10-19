using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
                //var result = (from l in course.Elements("lesson")
                //    where l.Attribute("id").Value.Equals("1")
                //    select l).First().Attributes();

                //foreach (var l in result)
                //{
                //    Console.WriteLine(l);
                //}

                var result =
                    course.Descendants().Elements("description").Ancestors("lesson").Distinct().Attributes("title");
                foreach (var l in result)
                {
                    Console.WriteLine(l);
                }

                var lesson = course.Elements("lesson").Where(l => l.Attribute("id").Value == "2").First();
                var newSubLesson = XElement.Parse(@"<sublesson id=""2.6"" title=""A bonus point"" />");
                lesson.Add(newSubLesson);
                var newLesson = new XElement("lesson", 
                    new XAttribute("id", "3"),
                    new XAttribute("title", "Standard Query Operators Part 1"));
                lesson.AddAfterSelf(newLesson);
                Console.WriteLine(course.ToString());

                var course2 = new XElement("course", from l in course.Elements("lesson")
                                                     select new XElement("lesson", l.Attributes(),
                                                     from sl in l.Elements("sublesson")
                                                     select new XElement("sublesson", 
                                                     from a in sl.Attributes()
                                                     select new XElement(a.Name, a.Value))));
                Console.WriteLine(course2);

                var course3 = new XElement("course", from l in course2.Elements("lesson")
                                                     select new XElement("lesson", l.Attributes(),
                                                     from sl in l.Elements("sublesson")
                                                     select new XElement("sublesson", 
                                                     from a in sl.Descendants()
                                                     select new XAttribute(a.Name, a.Value))));
                Console.WriteLine(course3);
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

    public class LessonNode
    {
        public string id { get; set; }
        public string title { get; set; }
    }
}
