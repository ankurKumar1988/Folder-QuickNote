using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media;

namespace FolderNote
{
    /// Features to put: 
    /// expandAll; collapseAll
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            onWindowLoad();
        }

        // class variables
        public string[][] Entries;

        private void onWindowLoad()
        {
            Entries = getFolderSturcture();
            int N_files = Entries[0].Length;
            int N_folders = Entries[1].Length;
            string[] file_Notes = new string[N_files];
            string[] folder_Notes = new string[N_folders];
            
            // check if notes file exists
            if (File.Exists("FolderNotes.dat"))
            {
                // load saved data
                SavedDataClass savedData = new SavedDataClass();
                savedData = LoadFromBinaryFile("folderNotes.dat");

                // get notes for files
                for (int i = 0; i < N_files; i++)
                {
                    int fileIndex = Array.IndexOf(savedData.fileEntries, Entries[0][i]);
                    if (fileIndex > -1)
                    {
                        file_Notes[i] = savedData.fileNotes[fileIndex];
                    }
                }

                // get notes for folders
                for (int i = 0; i < N_folders; i++)
                {
                    int folderIndex = Array.IndexOf(savedData.folderEntries, Entries[1][i]);
                    if (folderIndex > -1)
                    {
                        folder_Notes[i] = savedData.folderNotes[folderIndex];
                    }
                }
            }
            
            string[][] Entries_Notes = { file_Notes, folder_Notes };
            generateExpanders(Entries, Entries_Notes);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // arrays to store notes
                int N_files = Entries[0].Length;
                int N_folders = Entries[1].Length;
                string[] file_Notes = new string[N_files];
                string[] folder_Notes = new string[N_folders];

                // loop through entries and collect notes
                for (int i = 0; i < N_files; i++)
                {
                    file_Notes[i] = ((fileStack.Children[i] as Expander).Content as TextBox).Text;
                }
                for (int i = 0; i < N_folders; i++)
                {
                    folder_Notes[i] = ((folderStack.Children[i] as Expander).Content as TextBox).Text;
                }

                // save notes
                SavedDataClass Data_to_save = new SavedDataClass();
                Data_to_save.fileEntries = Entries[0];
                Data_to_save.fileNotes = file_Notes;
                Data_to_save.folderEntries = Entries[1];
                Data_to_save.folderNotes = folder_Notes;
                SaveAsBinaryFormat(Data_to_save, "folderNotes.dat");

                MessageBox.Show("Notes saved successfully!");
            }
            catch
            {
                MessageBox.Show("Error while saving notes!");
            }
        }
        private string[][] getFolderSturcture()
        {
            // find the current working directory
            string currDirectory = Directory.GetCurrentDirectory();

            // find the file names with extension in the working directory
            string[] fileEntries = Directory.GetFiles(currDirectory);
            for (int i = 0; i < fileEntries.Length; i++)
                fileEntries[i] = Path.GetFileName(fileEntries[i]);

            // Find the folder names in the working directory
            string[] folderEntries = Directory.GetDirectories(currDirectory);
            for (int i = 0; i < folderEntries.Length; i++)
                folderEntries[i] = Path.GetFileName(folderEntries[i]);

            string[][] nameList = {fileEntries, folderEntries};
            return nameList;
        }
        private void generateExpanders(string [][] Entries, string[][] Entries_Notes)
        {
            string[] fileEntries = Entries[0];
            string[] folderEntries = Entries[1];
            string[] fileEntries_Notes = Entries_Notes[0];
            string[] folderEntries_Notes = Entries_Notes[1];

            // Add file expanders programmatically
            for (int i = 0; i < fileEntries.Length; i++)
            {
                Expander expander = new Expander
                {
                    IsExpanded = true,
                    Header = fileEntries[i],
                    FontSize = 12,
                    Margin = new System.Windows.Thickness(3),
                    Padding = new System.Windows.Thickness(3)
                };
                TextBox textBox = new TextBox
                {
                    FontWeight = new System.Windows.FontWeight(),
                    FontSize = 12,
                    Margin = new System.Windows.Thickness(20,5,5,5),
                    Padding = new System.Windows.Thickness(5),
                    TextWrapping = new System.Windows.TextWrapping(),
                    AcceptsReturn = true,
                    Text = fileEntries_Notes[i]
                };
                expander.Content = textBox;
                fileStack.Children.Add(expander);
            }

            // Add folder expanders programmatically
            for (int i = 0; i < folderEntries.Length; i++)
            {
                Expander expander = new Expander
                {
                    IsExpanded = true,
                    Header = folderEntries[i],
                    FontSize = 12,
                    Margin = new System.Windows.Thickness(3),
                    Padding = new System.Windows.Thickness(3)
                };
                TextBox textBox = new TextBox
                {
                    FontWeight = new System.Windows.FontWeight(),
                    FontSize = 12,
                    Margin = new System.Windows.Thickness(5),
                    Padding = new System.Windows.Thickness(5),
                    TextWrapping = new System.Windows.TextWrapping(),
                    AcceptsReturn = true,
                    Text = folderEntries_Notes[i]
                };
                expander.Content = textBox;
                folderStack.Children.Add(expander);
            }
        }
        private void SaveAsBinaryFormat(object objGraph, string fileName)
        {
            BinaryFormatter binFormat = new BinaryFormatter();

            using (Stream fStream = new FileStream(fileName,
                FileMode.Create, FileAccess.Write,
                FileShare.None))
            {
                binFormat.Serialize(fStream, objGraph);
            }
        }
        private SavedDataClass LoadFromBinaryFile(string fileName)
        {
            BinaryFormatter binFormat = new BinaryFormatter();

            SavedDataClass savedData = new SavedDataClass();

            using (Stream fStream = File.OpenRead(fileName))
            {
                savedData =  (SavedDataClass)binFormat.Deserialize(fStream);
            }

            return savedData;
        }
        
    }
}
