using System;
using System.Collections.Generic;

namespace Genesis.UI
{
    public interface IZone
    {
        event EventHandler ViewCollectionChanged;
        string ZoneName { get; set; }
        IEnumerable<object> Views { get; }
        void Add(object view);
        void Add(string viewName, object view);
        bool Contains(object view);
        bool Contains(string viewName);
        object GetView(string viewName);
        void Remove(object view);
        void Remove(string viewName);
        void RemoveAll();
    }
}
