using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concert.Models.ViewModels
{
    public class SetListVM
    {
        public IEnumerable<SetListSong> SongList { get; set; }
        public IEnumerable<SetListService> ServiceList { get; set; }

        public double OrderTotal { get; set; }
    }


}
