﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angon.common.storage.data
{
    class Slave
    {
        string ip;
        int port;
        bool availableForWork;

        public string Ip { get => ip; set => ip = value; }
        public int Port { get => port; set => port = value; }
        public bool AvailableForWork { get => availableForWork; set => availableForWork = value; }
    }
}