using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
namespace anheiSave
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //equipment
        string equipmentSave;
        string charFile;
        string lastEquipmentFile;
        //task
        string diabloSaveFolder;
        string taskSaveFolder;
        string ext = ".d2s";
      
        //back
        string backSave;
        //atuo
        DispatcherTimer timer;
        string autoSourceFile;
        string autoTargetFile;
        DateTime autoSourceFileLMTime;

        public MainWindow()
        {
            InitializeComponent();
            //path
            string gamesave = AppDomain.CurrentDomain.BaseDirectory + "save\\";
            equipmentSave = gamesave + "equipment\\";
            lastEquipmentFile = gamesave + "equipment.txt";
            taskSaveFolder = gamesave + "task\\";
            backSave = gamesave + "back\\";

            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            diabloSaveFolder = userFolder + "\\Saved Games\\Diablo II\\";
            charFile = diabloSaveFolder + "Peter.d2s";

            //equipment list
            DirectoryInfo dirInfo = new DirectoryInfo(equipmentSave);
            FileInfo[] fileInfos = dirInfo.GetFiles();
            foreach(FileInfo fileInfo in fileInfos)
            {
                listEquipment.Items.Add(fileInfo.Name);
            }
            //task list
            dirInfo = new DirectoryInfo(taskSaveFolder);
            fileInfos = dirInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                string name = fileInfo.Name;
                string[] name_s = name.Split('_');
                ListBoxItem item = new ListBoxItem();
                item.Content = name_s[0];
                item.Tag = name_s[1];
                listTask.Items.Add(item);
            }
            //char list
            dirInfo = new DirectoryInfo(diabloSaveFolder);
            fileInfos = dirInfo.GetFiles("*" + ext);
            foreach (FileInfo fileInfo in fileInfos)
            {
                string name = fileInfo.Name;
                string[] name_s = name.Split('_');
                listChar.Items.Add(name.Replace(ext,""));
            }
            //sound
            me1.Source = new Uri(gamesave+ "录音.mp3");
            log.AppendText("welcome\n");
        }

        private void ListEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listEquipment.SelectedItem != null)
            {
                //backup last file
                string lastEquipment = File.ReadAllText(lastEquipmentFile);
                if(lastEquipment != "")
                {
                    File.Copy(charFile, equipmentSave + lastEquipment, true);
                    log.AppendText("save " + lastEquipment + "\n");
                }
                string equipment = (string)listEquipment.SelectedItem;
                File.Copy(equipmentSave + equipment, charFile, true);
                File.WriteAllText(lastEquipmentFile, equipment);
                log.AppendText(equipment + " is ready\n");
            }
        }

        private void ListTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recover();
        }

        private void BtnRecove_Click(object sender, RoutedEventArgs e)
        {
            recover();
        }

        private void recover()
        {
            if (listTask.SelectedItem != null)
            {
                ListBoxItem a = (ListBoxItem)listTask.SelectedItem;
                string tag = a.Tag.ToString();
                string content = a.Content.ToString();
                File.Copy(taskSaveFolder + content + '_' + tag, diabloSaveFolder + tag, true);
                log.AppendText(content + " is ready\n");
            }
        }


     
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (listChar.SelectedItem != null)
            {
                string nick = (string)listChar.SelectedItem;
                string full = nick + ext;
                File.Copy(diabloSaveFolder + full, backSave + full, true);
                log.AppendText(nick + " is back up\n");
            }
        }

        private void BtnRecoverC_Click(object sender, RoutedEventArgs e)
        {
            if (listChar.SelectedItem != null)
            {
                string nick = (string)listChar.SelectedItem;
                string full = nick + ext;
                File.Copy(backSave + full, diabloSaveFolder + full, true);
                log.AppendText(nick + " is recover\n");
            }
        }

        private void BtnAutoRecoverX_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string bname = b.Name;
            string btext = b.Content.ToString();
            Boolean isSelect = false;
            //clear timer
            if (timer != null)
            {
                timer.IsEnabled = false;
                timer.Stop();
                timer = null;
                BtnAutoRecover.Content = "自动恢复";
                BtnAutoRecoverC.Content = "自动恢复";
                log.AppendText("stop auto recover\n");
            }
           // set timer
            if (btext == "自动恢复")
            {
                if(bname == "BtnAutoRecover" && listTask.SelectedItem != null)
                {
                    isSelect = true;
                    ListBoxItem a = (ListBoxItem)listTask.SelectedItem;
                    string tag = a.Tag.ToString();
                    string content = a.Content.ToString();
                    autoSourceFile = taskSaveFolder + content + '_' + tag;
                    autoTargetFile = diabloSaveFolder + tag;               
                    BtnAutoRecover.Content = "自动恢复中";
                    log.AppendText(content + " start auto recover\n");                    
                }
                else if (bname == "BtnAutoRecoverC" && listChar.SelectedItem != null)
                {
                    isSelect = true;
                    string nick = (string)listChar.SelectedItem;
                    string full = nick + ext;
                    autoSourceFile = backSave + full;
                    autoTargetFile = diabloSaveFolder + full;
                    BtnAutoRecoverC.Content = "自动恢复中";
                    log.AppendText(nick + " start auto recover\n");
                }

                // set time
                if (isSelect)
                {
                    autoSourceFileLMTime = File.GetLastWriteTime(autoSourceFile);
                    //timer
                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(10);
                    timer.Tick += autoRecover;
                    timer.IsEnabled = true;
                    timer.Start();
                }
            }
        }
        private void autoRecover(object sender, EventArgs e)
        {
            DateTime saveTime = File.GetLastWriteTime(autoTargetFile);
            if (autoSourceFileLMTime.CompareTo(saveTime) < 0)
            {
                File.Copy(autoSourceFile, autoTargetFile, true);
                log.AppendText(autoTargetFile.Replace(diabloSaveFolder,"") + " recover\n");
                me1.Stop();
                me1.Play();               
            }
        }

    }
}
