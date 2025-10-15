using System;
using System.Windows.Forms;

namespace loginApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // تمكين المظهر الحديث للنوافذ
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // تشغيل النموذج الرئيسي
            Application.Run(new Form1());
        }
    }
}
