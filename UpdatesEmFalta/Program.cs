using WUApiLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdatesEmFalta
{
    class Program
    {
        static void Main(string[] args)
        {
            bool addthisupdate, rebootMayBeRequired;
            UpdateSession uSession = new UpdateSession();
            IUpdateSearcher uSearcher = uSession.CreateUpdateSearcher();
            uSession.ClientApplicationID = "ABS WUA";
            uSearcher.Online = true;


            try
            {
                //processamento de updates - Scan
                ISearchResult sResult = uSearcher.Search("(IsInstalled=0 and Type='Software' and CategoryIDs contains '0FA1201D-4330-4FA8-8AE9-B877473B6441') or (IsInstalled=0 and Type='Software' and CategoryIDs contains 'E6CF1350-C01B-414D-A61F-263D14D133B4')");
                Console.Write(sResult.Updates.Count + Environment.NewLine);
                foreach (IUpdate update in sResult.Updates)
                {
                    Console.WriteLine(update.Title);
                }
                UpdateCollection updatesToDownload = new UpdateCollection();
                foreach (IUpdate update in sResult.Updates)
                {
                    addthisupdate = false;
                    if (update.InstallationBehavior.CanRequestUserInput == true)
                    {
                        Console.WriteLine("skipping interactive update: " + update.Title);
                    }
                    else
                    {
                        if (update.EulaAccepted == false)
                        {
                            update.AcceptEula();
                            addthisupdate = true;
                        }
                        else
                        {
                            addthisupdate = true;
                        }
                    }
                    if (addthisupdate == true)
                    {
                        updatesToDownload.Add(update);
                    }
                }
                if (updatesToDownload.Count == 0)
                {
                    Console.Write("All applicable updates were skipped.");
                }
                else
                {

                    //processamento de updates - Download
                    UpdateDownloader downloader = uSession.CreateUpdateDownloader();
                    downloader.Updates = updatesToDownload;
                    downloader.Download();
                    /*
                    //processamento de updates - Install
                    UpdateInstaller updatesToInstall = uSession.CreateUpdateInstaller();
                    UpdateCollection updatesToInstall = new UpdateCollection();
                    rebootMayBeRequired = false;
                    foreach(IUpdate update in sResult.Updates)
                    {
                        if (update.IsDownloaded==true)
                        {
                            updatesToInstall.Add(update);
                            if (update.InstallationBehavior.RebootBehavior > 0)
                            {
                                rebootMayBeRequired = true;
                            }
                        }
                    }
                    if (updatesToInstall.Updates.Count == 0) Console.Write("No updates were successfully downloaded.");
                    //UpdateInstaller installer = 
                    //installer.CreateUpdateInstaller();
             */
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong: " + ex.Message);
            }
        }
    }
}
