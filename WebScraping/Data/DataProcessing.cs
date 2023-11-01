using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebScraping.DTO;
using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace WebScraping.Data
{
    public class DataProcessing
    {

        public static Clanica BuildDataItem(IWebElement we)
        {
            Clanica cl = new Clanica();
            cl.Ime = GetHTMLElementText(we, "//span[1]");
            cl.Adresa = GetHTMLElementText(we, "//span[2]").Replace("Adresa: ", "");
            cl.Grad = GetHTMLElementText(we, "//span[3]").Replace("Grad: ", "");
            cl.Kategorija = GetHTMLElementText(we, "//span[4]").Replace("Kategorija: ", "");
            cl.Zupanija = GetHTMLElementText(we, "//span[5]").Replace("Županija: ", "");
            cl.Link = GetHTMLElementAttributeText(we, "//span[1]/a", "href");
            return cl;
        }
        private static string GetHTMLElementText(IWebElement we, string xpath)
        {
            string strText = "";

            string strHTML = we.GetAttribute("innerHTML");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(strHTML);
            var htmlnode = htmlDoc.DocumentNode.SelectSingleNode(xpath);
            strText = htmlnode.InnerText;
            return strText;

        }
        private static string GetHTMLElementAttributeText(IWebElement we, string xpath, string attribute)
        {
            string strText = "";

            string strHTML = we.GetAttribute("innerHTML");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(strHTML);
            var htmlnode = htmlDoc.DocumentNode.SelectSingleNode(xpath);
            if(htmlnode != null)
                strText = htmlnode.Attributes[attribute].Value;
            return strText;
        }
        public static string BuildJsonfromObject(Object cl)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(cl, options);
            return jsonString;
        }
        public static void SaveJsonInFile(string json, string filename)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string strPath = Path.Combine(baseDir, filename);

            File.WriteAllText(strPath, json);

            Console.WriteLine("Prikupljeni podaci su spremljeni u datoteku {0}", strPath);

        }

    }
}
