using System;
using System.Windows.Forms;

namespace Bindery.Tests.TestClasses
{
    public class TestControl : UserControl
    {
        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        public object ObjectValue { get; set; }
    }
}