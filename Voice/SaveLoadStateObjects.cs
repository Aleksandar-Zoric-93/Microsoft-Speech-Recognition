﻿using System;
using System.IO;


namespace Voice
{
    [Serializable]
    class SaveLoadStateObjects
    {
        MainWindow mw;


        public SaveLoadStateObjects(MainWindow mainWindow)
        {
            mw = mainWindow;
        }

        public void save()
        {
            String thisName;
            thisName = mw.myName;
            BinaryWriter bw;

            //create the file
            try
            {
                //Set Dir and binary file name
                string path;
                path = System.IO.Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.MyDoc‌​uments), "Ella", "Config");

                //if Dir exists, write to file
                if (Directory.Exists(path))
                {
                    bw = new BinaryWriter(new FileStream(path, FileMode.Create));
                    //writing into the file
                    try
                    {
                        bw.Write(thisName);
                    }

                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message + "\n Cannot write to file.");
                        return;
                    }
                    bw.Close();
                }
                //if Dir does not exist, create it and then write to file
                else
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Ella"));
                    bw = new BinaryWriter(new FileStream(path, FileMode.Create));
                    //writing into the file
                    try
                    {
                        bw.Write(thisName);
                    }

                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message + "\n Cannot write to file.");
                        return;
                    }
                    bw.Close();
                }

                //bw = new BinaryWriter(new FileStream(path, FileMode.Create));            
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot create file.");
                return;
            }

        }

        public void load()
        {
            BinaryReader br;
            string path;
            path = System.IO.Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.MyDoc‌​uments), "Ella", "Config");
            //reading from the file
            try
            {
                br = new BinaryReader(new FileStream(path, FileMode.Open));
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot open file.");
                return;
            }
            try
            {
                mw.myName = br.ReadString();
                Console.WriteLine("Current saved name: {0}", mw.myName);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot read from file.");
                return;
            }
            br.Close();


        }

        public void delete()
        {
            string path;
            path = System.IO.Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.MyDoc‌​uments), "Ella");

            System.IO.DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
