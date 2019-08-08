using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Win32;

namespace ShuffleLines
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnShuffleFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            {
                if (open.ShowDialog() == true)
                {
                    string filename = open.FileName;
                    string path = System.IO.Path.GetDirectoryName(filename);
                    List<string> linesList = File.ReadAllLines(filename).ToList();
                    List<string> shuffleList = ShuffleList(linesList);
                    File.WriteAllLines($"{path}\\{System.IO.Path.GetFileNameWithoutExtension(filename)}_traoDong.{System.IO.Path.GetExtension(filename)}", shuffleList);
                    MessageBox.Show("Đã trộn dòng thành công");
                }
            }
        }

        private List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();

            Random r = new Random();
            while (inputList.Count > 0)
            {
                var randomIndex = r.Next(0, inputList.Count);
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }

        private void BtnReFormatFile_Click(object sender, RoutedEventArgs e)
        {
            int error = 0;
            List<string> exportList = new List<string>();
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == true)
            {
                string[] lines = File.ReadAllLines(open.FileName);
                for (int i = 0; i < lines.Length; i++)
                {
                    try
                    {
                        if (lines[i].Contains("|"))
                        {
                            string[] account = lines[i].Split('|');
                            exportList.Add($"TK: {account[0]} | MK: {account[1]} | : {account[2]}");
                        }
                    }
                    catch
                    {
                        error++;
                    }
                }
            }
            
            File.AppendAllLines($"{System.IO.Path.GetDirectoryName(open.FileName)}\\{System.IO.Path.GetFileNameWithoutExtension(open.FileName)}_dinhDangLai.{System.IO.Path.GetExtension(open.FileName)}", exportList);

            if (error != 0)
            {
                MessageBox.Show($"Có {error} dòng lỗi không đúng định dạng");
            }

            MessageBox.Show("Đã xuất file thành công");
        }

        private void BtnScale_Click(object sender, RoutedEventArgs e)
        {
            _fileAccounts1.Clear();
            _fileAccounts2.Clear();
            _sortedList.Clear();
            if (IsScaleLegal(TbScale.Text))
            {
                if (!OpenFile())
                {
                    return;
                }

                SortTwoList();
                File.AppendAllText("SortedFile.txt", string.Join(Environment.NewLine, _sortedList));
                File.AppendAllText("File1Left.txt", string.Join(Environment.NewLine, _fileAccounts1));
                File.AppendAllText("File2Left.txt", string.Join(Environment.NewLine, _fileAccounts2));
                MessageBox.Show("Đã định tỉ lệ xong");
            }
            else
                MessageBox.Show("Vui lòng ghi đúng tỉ lệ");
        }

        private void SortTwoList()
        {
            int divine1 = _fileAccounts1.Count / _scaleInts[0];
            int divine2 = _fileAccounts2.Count / _scaleInts[1];
            if (divine1 <= divine2)
            {
                LoopSortList(divine1);
            }
            else
            {
                LoopSortList(divine2);
            }
        }

        private void LoopSortList(int num)
        {
            for (int k = 0; k < num; k++)
            {
                for (int i = 0; i < _scaleInts[0]; i++)
                {
                    _sortedList.Add(_fileAccounts1[0]);
                    _fileAccounts1.RemoveAt(0);
                    for (int j = 0; j < _scaleInts[1]; j++)
                    {
                        _sortedList.Add(_fileAccounts2[0]);
                        _fileAccounts2.RemoveAt(0);
                    }
                }
            }
        }

        private List<string> _fileAccounts1 = new List<string>();
        private List<string> _fileAccounts2 = new List<string>();
        private List<string> _sortedList = new List<string>();
        private int[] _scaleInts = new int[2];

        private bool OpenFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Chọn file account đầu tiên";
            if (openFile.ShowDialog() == true)
            {
                ImportAccount(openFile.FileName, ref _fileAccounts1);
            }
            else
            {
                return false;
            }

            openFile.Title = "Chọn file account thứ 2";
            if (openFile.ShowDialog() == true)
            {
                ImportAccount(openFile.FileName, ref _fileAccounts2);
            }
            else
                return false;

            return true;
        }

        private void ImportAccount(string fileName, ref List<string> accountList)
        {
            string[] lines = File.ReadAllLines(fileName);
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    if (lines[i].Contains("|"))
                    {
                        accountList.Add(lines[i]);
                    }
                }
                catch
                {

                }
            }
        }

        private bool IsScaleLegal(string scale)
        {
            if (scale.Contains(":"))
            {
                string[] arrayStrings = scale.Split(':');
                for (int i = 0; i < 2; i++)
                {
                    int n;
                    bool isNumeric = int.TryParse(arrayStrings[i], out n);
                    if (!isNumeric)
                    {
                        return false;
                    }

                    _scaleInts[i] = n;
                }

                return true;
            }
            else
                return false;
        }

        static string regexPattern = "[^0-9:]";
        private Regex _regex = new Regex(regexPattern);

        private void TbScale_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbScale.Text = _regex.Replace(TbScale.Text, "");
        }
    }
}
