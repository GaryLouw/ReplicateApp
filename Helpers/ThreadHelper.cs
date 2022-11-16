using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicateApp.Helpers
{
    public static class ThreadHelper
    {
        delegate void SetTextCallback(Form f, ListView ctrl, string text);

        public static void AddViewItem(Form form, ListView ctrl, string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AddViewItem);
                form.Invoke(d, new object[] { form, ctrl, text });
            }
            else
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = text;
                ctrl.Items.Add(lvi);
            }
        }
    }
}
