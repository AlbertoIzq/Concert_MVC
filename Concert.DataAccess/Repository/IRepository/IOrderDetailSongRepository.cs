﻿using Concert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concert.DataAccess.Repository.IRepository
{
    public interface IOrderDetailSongRepository : IRepository<OrderDetailSong>
    {
        void Update(OrderDetailSong orderDetailSong);
    }
}