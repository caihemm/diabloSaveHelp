using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace anheiSave
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //equipment
        string equipmentSave = "D:\\game\\save\\equipment\\";
        string charFile;
        string lastEquipmentFile = "D:\\game\\save\\equipment.txt";
        //task
        string diabloSave;
        string taskSave = "D:\\game\\save\\task\\";
        string ext = ".d2s";
        //back
        string backSave = "D:\\game\\save\\back\\";

        public MainWindow()
        {
            InitializeComponent();
            log.AppendText("welcome\n");
            diabloSave = "C:\\Users\\" + Environment.UserName + "\\Saved Games\\Diablo II\\";
            charFile = diabloSave + "Peter.d2s";

            //equipment list
            DirectoryInfo dirInfo = new DirectoryInfo(equipmentSave);
            FileInfo[] fileInfos = dirInfo.GetFiles();
            foreach(FileInfo fileInfo in fileInfos)
            {
                listEquipment.Items.Add(fileInfo.Name);
            }
            //task list
            dirInfo = new DirectoryInfo(taskSave);
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
            dirInfo = new DirectoryInfo(diabloSave);
            fileInfos = dirInfo.GetFiles("*" + ext);
            foreach (FileInfo fileInfo in fileInfos)
            {
                string name = fileInfo.Name;
                string[] name_s = name.Split('_');
                listChar.Items.Add(name.Replace(ext,""));
            }
        }


        private void ListEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listEquipment.SelectedItem != null)
            {
                //backup last file
                string lastEquipment = File.ReadAllText(lastEquipmentFile);
                File.Copy(charFile, equipmentSave + lastEquipment, true);
                log.AppendText("save " + lastEquipment + "\n");
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
                File.Copy(taskSave + content + '_' + tag, diabloSave + tag, true);
                log.AppendText(content + " is ready\n");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (listChar.SelectedItem != null)
            {
                string nick = (string)listChar.SelectedItem;
                string full = nick + ext;
                File.Copy(diabloSave + full, backSave + full, true);
                log.AppendText(nick + " is back up\n");
            }
        }

        private void BtnRecoverC_Click(object sender, RoutedEventArgs e)
        {
            if (listChar.SelectedItem != null)
            {
                string nick = (string)listChar.SelectedItem;
                string full = nick + ext;
                File.Copy(backSave + full, diabloSave + full, true);
                log.AppendText(nick + " is back up\n");
            }
        }
    }
}
