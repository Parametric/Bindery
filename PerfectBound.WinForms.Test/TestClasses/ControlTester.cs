using System;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Test.TestClasses
{
    public class ControlTester : IDisposable
    {
        private readonly Form _form = new Form();

        public ControlTester(Control control)
        {
            _form.Controls.Add(control);
            _form.Show();
        }

        public void Dispose()
        {
            _form.Close();
        }
    }
}