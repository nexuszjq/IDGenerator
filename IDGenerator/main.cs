using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IDGenerator
{
    public partial class main : Form
    {
        private static string acctCreatePattern = @"^(?:\d{15,16}|\d{18,19})$";

        public main()
        {
            InitializeComponent();
        }

        private void create_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ID.Text))
            {
                MessageBox.Show("请输入卡号");
                return;
            }

            if (string.IsNullOrEmpty(this.count.Text))
            {
                MessageBox.Show("请输入生成次数");
                return;
            }

            int count = int.Parse(this.count.Text);
            if (count >= 999)
            {
                MessageBox.Show("每次只能创建小于999个卡号");
                return;
            }

            string res = string.Empty;
            long id = long.Parse(this.ID.Text);

            for (int i = 0; i < count; i++)
            {
                res += createAcctId(Convert.ToString(id)) + "\n";
                id++;
            }

            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(this.path.Text + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", FileMode.Append);
                sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(res);
                MessageBox.Show("创建成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建失败：" + ex.Message);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        private void selectPath_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择文件夹";
            dialog.SelectedPath = @"C:\";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                this.path.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// Luhn补全卡号
        /// </summary>
        /// <param name="acctId"></param>
        /// <returns></returns>
        public static string createAcctId(string acctId)
        {
            if (string.IsNullOrEmpty(acctId) || !Regex.IsMatch(acctId, acctCreatePattern))
            {
                return "";
            }
            int sum = 0;
            int lastIndex = acctId.Length < 17 ? 14 : 17;
            string preAcctId = acctId.Substring(0, lastIndex + 1);
            char[] chars = acctId.ToCharArray();
            for (int i = 0; i <= lastIndex; i++)
            {
                int unit = chars[lastIndex - i] - '0';
                if ((i & 1) == 0)
                {
                    if (unit < 5)
                    {
                        sum += unit * 2;
                    }
                    else
                    {
                        sum += unit * 2 - 9;
                    }
                }
                else
                {
                    sum += unit;
                }
            }
            int remainder = sum % 10;
            int checkCode = remainder == 0 ? 0 : 10 - remainder;
            return preAcctId + checkCode;
        }

        
    }
}
