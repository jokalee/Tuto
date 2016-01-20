﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Navigator.ViewModels;

namespace Tuto.Navigator.ViewModels
{
    public class MainModel : NotifierModel
    {
        public BatchWorkQueueViewModel Queue { get; private set; }
        public VideothequeModel VideothequeModel { get; private set; }

        public MainModel(Videotheque videotheque)
        {
            Queue = new BatchWorkQueueViewModel(Program.WorkQueue);
            VideothequeModel = new VideothequeModel(videotheque);
                
        }


    }
}
