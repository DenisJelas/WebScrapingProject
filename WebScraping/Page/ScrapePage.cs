using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using Microsoft.VisualBasic.FileIO;
using WebScraping.DTO;
using WebScraping.Data;
using System.Collections.ObjectModel;

namespace WebScraping.Page
{
    public class ScrapePage
    {
        Dictionary<int,string> categories = new Dictionary<int, string>
        {
            [1] = "Sve",
            [2] = "Punopravna",
            [3] = "Punopravna iz školstva",
            [4] = "Akademska",
            [5] = "Pridružena",
            [6] = "Privremena",

        };
        private string ModifyXpathforSelectedCategory(string xpath,int CategoryNumber)
        {
            xpath = xpath + "/option[" + CategoryNumber.ToString() + "]";
            return xpath;
        }

        public void ScrapeCarnetWebPage(int CategoryNumber)
        {
            List<int> CategoriesToCollect = new List<int>();
            if(CategoryNumber==1)
            {
                for (int catnum = 2; catnum <= categories.Count; catnum++)
                {
                    CategoriesToCollect.Add(catnum);
                }
            }
            else
            {
                CategoriesToCollect.Add(CategoryNumber);
            }

            //data initialization of page items
            string strUrl = "https://www.carnet.hr/popis-ustanova-clanica/";
            string xPathPrihvatiCookie = "//*[text()='Prihvaćam']";
            string xPathZupanije = "//select[@class='c-zupanije']";
            string xPathZupanijeOptions = "//select[@class='c-zupanije']/option";
            string xPathSveKategorije = "//select[@class='c-kategorije']"; //sve kategorije
            string xPathPrazneKategorije = "//span[@class='c-kat__title t-s20 is-disabled']"; //prazna kategorija, nema članica
            string xPathBrojStranica = "//section[@class='c-kat__pagination c-pagination m-bg--s-teal col-12']/div[last()]"; //ukupan broj stranica
            string xPathPrvaStranica = "//div[(@class='c-kat__page page-numbers' or @class='c-kat__page page-numbers ' or @class='c-kat__page page-numbers current') and text()='1']"; //prva stranica
            string xPathOdabranaStranica = "//div[@class='c-kat__page page-numbers current']/following-sibling::div"; //odabrana stranica
            string xPathPodaciClanice = "//div[@class='c-kat__rezultati row']/div[@class='c-kat__rez col-12 col-md-6']"; //podaci o clanicama

            ChromeScraper cs = new ChromeScraper();
            cs.CreateDriver();

            cs.Navigate(strUrl);
            
            Thread.Sleep(3000);

            cs.Click(xPathPrihvatiCookie); //prihvati Cookie

            Thread.Sleep(2000);

            //driver.ExecuteJavaScript<string>("document.body.style.zoom='70%'");

            cs.WaitForElement(xPathZupanije,5); //pričekaj učitavanje elementa Županije

            Thread.Sleep(3000);

            var allOptions = cs.SelectElements(xPathZupanijeOptions);

            Clanice podaci_Clanice = new Clanice();
            podaci_Clanice.vrijeme_Prikupljanja = DateTime.Today;

            for (int i = 1; i < allOptions.Count; i++)
            {
                allOptions[i].Click();  //odaberi županiju
                Thread.Sleep(2000);

                int NumOfClanica = 0;

                foreach (int catnum in CategoriesToCollect)
                {
                    int NumOfClanicaPerCategory = 0;

                    string xPathOdabranaKategorije = ModifyXpathforSelectedCategory(xPathSveKategorije, catnum);
                    cs.Click(xPathOdabranaKategorije); //odaberi kategoriju
                    Thread.Sleep(2000);

                    if (!cs.ElementExists(xPathPrazneKategorije))
                    {
                        if (cs.ElementExists(xPathPrvaStranica)) //paging/više od jedne stranice
                        {
                            cs.Click(xPathPrvaStranica); //paging/prva stranica
                            Thread.Sleep(2000);

                            var maxNumOfPages = cs.SelectElement(xPathBrojStranica); // paging/ ukupan broj stranica
                            int numOfPages = int.Parse(maxNumOfPages.Text);

                            for (int j = 1; j <= numOfPages; j++)
                            {
                                //scrape here
                                ReadOnlyCollection<IWebElement> sveClanice = cs.SelectElements(xPathPodaciClanice);

                                for (int z = 0; z < sveClanice.Count; z++)
                                {
                                    Clanica cl = DataProcessing.BuildDataItem(sveClanice[z]);
                                    podaci_Clanice.Lista_Clanica.Add(cl);
                                }
                                NumOfClanicaPerCategory = NumOfClanicaPerCategory + sveClanice.Count;
                                podaci_Clanice.odabrana_Kategorija = categories[catnum];

                                if (j < numOfPages)
                                    cs.Click(xPathOdabranaStranica);
                                else
                                    cs.Click(xPathPrvaStranica);

                                Thread.Sleep(2000);
                            }
                        }
                        else
                        {
                            //scrape here
                            ReadOnlyCollection<IWebElement> sveClanice = cs.SelectElements(xPathPodaciClanice);

                            for (int z = 0; z < sveClanice.Count; z++)
                            {
                                Clanica cl = DataProcessing.BuildDataItem(sveClanice[z]);
                                podaci_Clanice.Lista_Clanica.Add(cl);
                            }
                            NumOfClanicaPerCategory = NumOfClanicaPerCategory + sveClanice.Count;
                            podaci_Clanice.odabrana_Kategorija = categories[catnum];
                        }
                    }
                    Console.WriteLine("  Županija: {0} Kategorija: {1}, broj članica: {2}", allOptions[i].Text, categories[catnum], NumOfClanicaPerCategory.ToString());
                    NumOfClanica = NumOfClanica + NumOfClanicaPerCategory;
                }
                Console.WriteLine("Županija: {0}, broj članica ukupno: {1}", allOptions[i].Text, NumOfClanica.ToString());
            }
            podaci_Clanice.ukupan_Broj = podaci_Clanice.Lista_Clanica.Count;
            string strJson = DataProcessing.BuildJsonfromObject(podaci_Clanice);

            DataProcessing.SaveJsonInFile(strJson,"Clanice_Carneta_kategorija_" + categories[CategoryNumber] + "_" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt");

            cs.CloseDriver();
        }
    }
}
