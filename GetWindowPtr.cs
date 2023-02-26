using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace CPUTempOverlay
{
    public class GetWindowPtr
    {

        public int[] iProcId = new int[9999];

        public string[] GetListWindows()
        {
            string[] listWindows = new string[9999];
            int itr = 0;

            //全てのプロセスを列挙する
            foreach (System.Diagnostics.Process p in
                System.Diagnostics.Process.GetProcesses())
            {
                //メインウィンドウのタイトルがある時だけ列挙する
                if (p.MainWindowTitle.Length != 0)
                {
                    listWindows[itr] = "プロセス名:" + p.ProcessName + ", ";
                    listWindows[itr] += "タイトル名:" + p.MainWindowTitle;
                    iProcId[itr] = p.Id;
                    itr++;
                }
            }
            return listWindows;
        }
        public IntPtr GetPtrWindow( int pid)
        {
            IntPtr iptr = IntPtr.Zero;

            //全てのプロセスを列挙する
            foreach (System.Diagnostics.Process p in
                System.Diagnostics.Process.GetProcesses())
            {
                //メインウィンドウのタイトルがある時だけ列挙する
                if (p.MainWindowTitle.Length != 0)
                {
                    if(p.Id == pid)
                    {
                        // プロセスidが一致するもののハンドルポインタを取得
                        iptr = p.Handle;
                        break;
                    }

                }
            }
            return iptr;
        }
    }
}

