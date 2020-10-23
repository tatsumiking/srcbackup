using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace umail
{
    public class LibUsbProtect
    {
        [DllImport("UsbPrt.dll")]
        static extern int Check(int key);

        public int ProtectMail()
        {
            int ret;

#if DEBUG
            ret = Constants.USBPROTECTNO;
#else
            ret = Check(Constants.USBPROTECTNO);
#endif
            return (ret);
        }
    }
}
