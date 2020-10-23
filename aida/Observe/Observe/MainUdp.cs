using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Observe
{
    public partial class MainWindow : Window
    {
        public void udpReceiveGPSData()
        {
            int nIdx;
            int max, i, j;
            string sGpsPos;
            ClsGpsPos clsGpsPos;
            UdpClient udp;
            byte[] rcvBytes;

            IPAddress selfAddress = IPAddress.Parse(m_sSelfIP);

            //UdpClientを作成し、ローカルエンドポイントにバインドする
            IPEndPoint selfEP = new IPEndPoint(selfAddress, m_nPort);
            udp = new UdpClient(selfEP);
            m_rcvUDPClient = udp;
            IPEndPoint remoteEP = null;

            try
            {
                rcvBytes = udp.Receive(ref remoteEP);
            }
            catch (Exception ex)
            {
                udp.Close();
                return;
            }

            m_bUDPFlag = false;

            max = rcvBytes.Length - 5;
            byte[] strBytes = new byte[max];
            byte[] skey = System.Text.Encoding.UTF8.GetBytes("{{[[");
            byte[] ekey = System.Text.Encoding.UTF8.GetBytes("]]}}");
            for (i = 0; i < max; i++)
            {
                if ((skey[0] == rcvBytes[i])
                 && (skey[1] == rcvBytes[i+1])
                 && (skey[2] == rcvBytes[i+2])
                 && (skey[3] == rcvBytes[i+3]))
                {
                    break;
                }
            }
            if (i == max)
            {
                udp.Close();
                return;
            }
            for (i = i + 4, j = 0; i < max; i++, j++)
            {
                if ((ekey[0] == rcvBytes[i])
                 && (ekey[1] == rcvBytes[i + 1])
                 && (ekey[2] == rcvBytes[i + 2])
                 && (ekey[3] == rcvBytes[i + 3]))
                {
                    break;
                }
                strBytes[j] = rcvBytes[i];
            }
            for (; j < max; j++)
            {
                strBytes[j] = 0;
            }
            sGpsPos = Encoding.UTF8.GetString(strBytes);
            string[] ary = sGpsPos.Split(',');
            //受信した送信者の情報
            // remoteEP.Address, remoteEP.Port;
            // if (m_clsCardCrt.checkIP(remoteEP.Address) == true)
            if (checkRemoteIP(remoteEP.Address) == true)
            {
                clsGpsPos = new ClsGpsPos();
                nIdx = searchCardIdx(ary[0]);
                if (nIdx != -1)
                {
                    clsGpsPos.m_sDate = ary[1];
                    clsGpsPos.m_dLat = m_libCmn.StrToDouble(ary[2]);
                    clsGpsPos.m_dLnd = m_libCmn.StrToDouble(ary[3]);
                    m_clsObserve.m_lstClsCard[nIdx].m_lstGpsPos.Add(clsGpsPos);
                }
            }
            //UdpClientを閉じる
            udp.Close();
        }
    }
}
