﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickrBrowser.Common
{

    /// <summary>
    /// View model describing one of the filters available for viewing search results.
    /// </summary>
    public class SearchFilter : FlickrBrowser.Common.BindableBase
    {
        private String _name;
        private int _count;
        private bool _active;

        public SearchFilter(String name, int count, bool active = false)
        {
            this.Name = name;
            this.Count = count;
            this.Active = active;
        }

        public override String ToString()
        {
            return Description;
        }

        public String Name
        {
            get { return _name; }
            set { if (this.SetProperty(ref _name, value)) this.OnPropertyChanged("Description"); }
        }

        public int Count
        {
            get { return _count; }
            set { if (this.SetProperty(ref _count, value)) this.OnPropertyChanged("Description"); }
        }

        public bool Active
        {
            get { return _active; }
            set { this.SetProperty(ref _active, value); }
        }

        public String Description
        {
            get { return String.Format("{0} ({1})", _name, _count); }
        }
    }
}
