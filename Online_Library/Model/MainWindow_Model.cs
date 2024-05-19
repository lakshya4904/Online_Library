using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Library.Model
{
    internal class MainWindow_Model: ObservableObject
    {

        public string? Name {  get; set; }
        public int? Age {  get; set; }

        public void Clear()
        {
            Name = null;
            Age = 0;
        }
    }
}
