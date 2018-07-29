using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderNote
{
    [Serializable]
    class SavedDataClass
    {
        public string[] fileEntries;
        public string[] fileNotes;
        public string[] folderEntries;
        public string[] folderNotes;
    }
}