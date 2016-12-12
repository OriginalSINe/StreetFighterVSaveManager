using System;
using System.IO;

namespace SFVSaveUnFer
{
    class Program
    {
        //The location fo the save files
        private const string File_Progress = "GameProgressSave.sav";
        private const string File_System = "GameSystemSave.sav";

        //The locations of other files
        private const string Folder_Backups = "SaveGames_Backup"; //The folder to store backups to
        private const string File_User = "currentuser.txt"; //The text file which stores the username

        private static string User_New; //The new user
        private static string User_Current; //The current user;

        //the paths where the current save files will be backed up to;
        private static string Path_Current_Backup_Progress;
        private static string Path_Current_Backup_System;

        //The paths to the backup save files to restore to the current
        private static string Path_New_Restore_Progress;
        private static string Path_New_Restore_System;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the SFV Save UnF***er!");
            Console.WriteLine();

            Console.WriteLine("Backup Process begin.");
            CheckExists_BackupFolder();
            CheckExists_SaveFile();

            User_Current_Read();

            Current_Backup_Remove(); //Removes the backups for the current user
            Current_Backup_Create(); //Creates the backups for the current user

            Console.WriteLine("Backup Process complete!");

            Console.WriteLine();
            Console.WriteLine("Press Enter to begin Restore process, or close window to exit."); Console.ReadLine();
            Console.WriteLine("Restore Process begin.");

            User_New_Read(); //Gets the current user name; Ensures that those files exist;
            Current_Files_Remove(); //Remove the saves for the current user;

            New_Backup_Restore(); //Restore the new user

            User_New_Save();

            Console.WriteLine("Restore process complete!");
            Console.WriteLine();
            Console.WriteLine("Thanks for using the SFV Save UnF***er!");
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        /// <summary>
        /// Removes the current save files;
        /// </summary>
        private static void Current_Files_Remove()
        {
            RemoveFile(File_Progress);
            RemoveFile(File_System);
        }

        /// <summary>
        /// Checks if the save files exist;
        /// If they don't, then just ends the application;
        /// </summary>
        private static void CheckExists_SaveFile()
        {
            if (!File.Exists(File_Progress) || !File.Exists(File_System))
            {
                Console.WriteLine("FATAL ERROR: Save files are missing! Exiting application...");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Removes OLD BACKUPS Backups for the CURRENT user
        /// </summary>
        private static void Current_Backup_Remove()
        {
            Console.WriteLine("Removing old backups for user '" + User_Current + "'.");
            RemoveFile(Path_Current_Backup_Progress);
            RemoveFile(Path_Current_Backup_System);
        }

        /// <summary>
        /// Creatse NEW BACKUPS files for the CURRENT USER in the backup folder;
        /// Copies from the current to the backups folder;
        /// </summary>
        private static void Current_Backup_Create()
        {
            Console.WriteLine("Creating new backups for user '" + User_Current + "'.");
            File.Copy(File_Progress, Path_Current_Backup_Progress);
            File.Copy(File_System, Path_Current_Backup_System);
        }

        /// <summary>
        /// Restoring saves for NEW USER
        /// </summary>
        private static void New_Backup_Restore()
        {
            Console.WriteLine("Restoring backups for user '" + User_New + "'.");

            //Moves the files to restore to the current one
            File.Copy(Path_New_Restore_Progress, File_Progress);
            File.Copy(Path_New_Restore_System, File_System);
        }

        /// <summary>
        /// Tries to remove the file;
        /// </summary>
        /// <param name="fileName"></param>
        private static void RemoveFile(string fileName)
        {
            //Remove the Progress file if it exists
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                //Console.WriteLine("Removed file " + fileName);
            }
            else
            {
                Console.WriteLine(fileName + " does not exist. Did not remove file.");
            }
        }

        /// <summary>
        /// Writes the current 
        /// </summary>
        /// <returns></returns>
        private static void User_New_Save()
        {
            RemoveFile(File_User);

            File.Create(File_User).Close();

            StreamWriter sw = new StreamWriter(File_User);
            sw.Write(User_New);
            sw.Close();
        }

        /// <summary>
        /// Gets the username from the user;
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static void User_New_Read()
        {
            Console.Write("Input User name to restore: ");
            User_New = Console.ReadLine();

            if (User_New == "")
            {
                Console.WriteLine("ERROR: User name is blank! Please enter a valid user name.");
                User_New_Read();
                return;
            }

            //Builds the file names of the backups for the NEW USER to restore
            Path_New_Restore_Progress = Folder_Backups + Path.DirectorySeparatorChar + File_Progress + "_" + User_New;
            Path_New_Restore_System = Folder_Backups + Path.DirectorySeparatorChar + File_System + "_" + User_New;

            //Checks if the files exist
            if (!File.Exists(Path_New_Restore_Progress) || !File.Exists(Path_New_Restore_System))
            {
                Console.WriteLine("ERROR: Could not find backups of user '" + User_New + "'.");
                User_New_Read();
                return;
            }

            Console.WriteLine("New user set as '" + User_New + "'.");
        }

        /// <summary>
        /// Checks if the backups folder exists;
        /// If it does not exist, then creates it;
        /// </summary>
        private static void CheckExists_BackupFolder()
        {
            if (!Directory.Exists(Folder_Backups))
            {
                Console.WriteLine("WARN: Backups folder does not exist. Creating it...");
                Directory.CreateDirectory(Folder_Backups);
            }
        }

        /// <summary>
        /// Gets the last username from the text file;
        /// </summary>
        /// <returns></returns>
        private static void User_Current_Read()
        {
            if (File.Exists(File_User))
            {
                StreamReader sr = new StreamReader(File_User);
                User_Current = sr.ReadLine();
                sr.Close();

                if (User_Current == null)
                {
                    Console.WriteLine("WARN: Current user not setup in currentuser.txt!");
                    Console.Write("Please input current user: ");
                    User_Current = Console.ReadLine();

                    StreamWriter sw = new StreamWriter(File_User);
                    sw.Write(User_Current);
                    sw.Close();
                }

                //Creates backup for the CURRENT USER file names
                Path_Current_Backup_Progress = Folder_Backups + Path.DirectorySeparatorChar + File_Progress + "_" + User_Current;
                Path_Current_Backup_System = Folder_Backups + Path.DirectorySeparatorChar + File_System + "_" + User_Current;

                Console.WriteLine("Current user detected as '" + User_Current + "'.");
            }
            else
            {
                Console.WriteLine("WARN: currentuser.txt does not exist! Creating it...");
                File.Create(File_User).Close();
                User_Current_Read();
            }
        }
    }
}