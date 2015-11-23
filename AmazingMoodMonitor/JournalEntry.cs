using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazingMoodMonitor
{
    class JournalEntry
    {


        public string mood {get;set;}
        public string activity { get; set; }
        public DateTime dateTime { get; set; }



        //method

       public void setDateNow()
        {
            dateTime = DateTime.Now;

        }

        //write out in json


    }
}
