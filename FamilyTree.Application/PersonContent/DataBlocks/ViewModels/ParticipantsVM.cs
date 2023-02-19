using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyTree.Application.PersonContent.DataBlocks.ViewModels
{
    public class ParticipantVM
    {
        public int Id { get; set; }

        public bool IsSelected { get; set; }

        public ICollection<ParticipantDataHolderVM> DataHolders { get; set; }
    }

    public class ParticipantDataHolderVM
    {
        public string DataHolderType { get; set; }
        public string Data { get; set; }
        public bool IsDeletable { get; set; }
        public int DataBlockId { get; set; }
    }

}
