using System.Diagnostics;

namespace MujiStore.BLL
{
    /// <summary>
    /// Thread Processing for Batch file run
    /// </summary>
    public class ThreadProcess
    {
        public void ThreadTask(string strMasterBatchFile)
        {
            //Intialize the class and assing the batch file
            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(strMasterBatchFile);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.UseShellExecute = false;
            //No Window
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo = p;
            //command windows visible is false
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //Start the batch
            proc.Start();
            //Wait for the execution
            proc.WaitForExit();
        }
    }
}