using System;

namespace EmptyEvacuation.Scripts.Systems.Amaryllis.Support
{
    public interface IActionWidget
    { 
        void Show(Action<bool, string> callback, IActionWidgetData data = null);
    }
    
    public interface IActionWidgetData{}
}
