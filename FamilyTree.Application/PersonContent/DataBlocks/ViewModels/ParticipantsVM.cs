﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyTree.Application.PersonContent.DataBlocks.ViewModels
{
    public class ParticipantVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}