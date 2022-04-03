using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TraidingBotGUI.Utilitys
{
    /// <summary>
    /// Class to log all errors and trading history
    /// </summary>
    static class Logger
    {
        private static readonly string Datapath = ApplicationData.Current.LocalFolder.Path;

        //Possible States, that could be logged
        public enum log { Data = 1, Error, State }


        private static TextWriter writer;

        /// <summary>
        /// Open a log File, depending on the state of the Logger
        /// </summary>
        /// <param name="State">Data or Error log possible</param>
        /// <returns>returns the corresponding file</returns>
        private static TextWriter FileLog(log State)
        {
            switch(State)
            {
                case log.Data:
                    return (File.AppendText(Datapath + "\\DataLog.log"));                  
                case log.Error:
                    return (File.AppendText(Datapath + "\\ErrorLog.log"));
                case log.State:
                    return (File.AppendText(Datapath + "\\StateLog.log"));
                default:
                    throw (new ArgumentException("Couldnt find a fitting logging state"));
            }
        }

        /// <summary>
        /// Writes a string in a Log-File, depending on the state of the App
        /// </summary>
        /// <param name="logMessage">Content of the Message</param>
        /// <param name="State">Error or Data log possible</param>
        /// <returns>returns true for a succesfull write</returns>
        public static bool WriteLog(string logMessage, log State)
        {
            try
            {
                using (writer = FileLog(State))
                {
                    writer.Write("\r\nLog Entry : ");
                    writer.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                        DateTime.Now.ToLongDateString());
                    writer.WriteLine("  :");
                    writer.WriteLine("  :{0}", logMessage);
                    writer.WriteLine("-------------------------------");

                    return true;
                }
            }
            catch(ArgumentException e)
            {
                Logger.WriteLog(e.Message, log.Error);
                return false;
            }
            catch(Exception e)
            {
                //Logger.WriteLog(e.Message, log.Error);
                return false;
            }
            
        }

    }
}
