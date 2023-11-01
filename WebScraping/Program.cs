

using WebScraping.Page;

namespace WebScraping
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ScrapePage sp = new ScrapePage();

            string CategoryName = GetCategoryName(args);
            int CategoryNumber = GetCategoryNumber(args);

            Console.WriteLine("Prikupljanje podataka o Carnet članicama:");
            Console.WriteLine("Kategorija: {0}", CategoryName);

            sp.ScrapeCarnetWebPage(CategoryNumber);
        }
        private static int GetCategoryNumber(string[] args)
        {
            //Kategorije Carnet ustanova
            //1-Sve
            //2-Punopravna
            //3-Punopravna iz školstva
            //4-Akademska
            //5-Pridružena
            //6-Privremena

            int Category = 1;
            if (args.Length > 0)
                Category = int.Parse(args[1]);
            return Category;

        }
        private static string GetCategoryName(string[] args)
        {
            //Kategorije Carnet ustanova
            //1-Sve
            //2-Punopravna
            //3-Punopravna iz školstva
            //4-Akademska
            //5-Pridružena
            //6-Privremena          
            string CaregoryName="";
            switch(GetCategoryNumber(args))
            {
                case 1:
                    CaregoryName = "Sve"; break;
                case 2:
                    CaregoryName = "Punopravna"; break;
                case 3:
                    CaregoryName = "Punopravna iz školstva"; break;
                case 4:
                    CaregoryName = "Akademska"; break;
                case 5:
                    CaregoryName = "Pridružena"; break;
                case 6:
                    CaregoryName = "Privremena"; break;
            }
            return CaregoryName;
        }
    }
}