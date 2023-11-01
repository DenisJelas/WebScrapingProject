using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping.DTO
{
    public class Clanice
    {
        public Clanice()
        {
            Lista_Clanica = new List<Clanica>();
        }
        public DateTime vrijeme_Prikupljanja { get; set; }

        public int ukupan_Broj { get; set; }
        public string odabrana_Kategorija { get; set; }
        public List<Clanica> Lista_Clanica { get; set; }

    }
}
