﻿using Angon.common.config;
using System;
using System.Collections.Generic;

namespace Angon.common.comprotocols.requests
{
    internal class OrderPost : Request
    {

        public string SavedZipTo { get; set; }

        public OrderPost(byte[] data) : base(data)
        {

            
        }
    }
}
