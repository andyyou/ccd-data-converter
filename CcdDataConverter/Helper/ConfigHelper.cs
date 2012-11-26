using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml.XPath;
using System.Xml;
using System.Windows.Forms;

namespace CcdDataConverter.Helper
{
    class ConfigHelper
    {
        const string _configPath = @"config\config.xml";

        #region 取得參數設定

        public void GetRelativeData(DataGridView dgv)
        {
            DataTable dtb = new DataTable("Columns");
            dtb.Columns.Add("Source", typeof(string));
            dtb.Columns.Add("Target", typeof(string));

            using (FileStream stream = new FileStream(_configPath, FileMode.Open))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();
                XPathNodeIterator node = navigator.Select("//config/relative_setting/column");
                while (node.MoveNext())
                {
                    dgv.Rows.Add(node.Current.SelectSingleNode("source").Value, node.Current.SelectSingleNode("target").Value);
                }
            }
        }

        public double GetOffsetY()
        {
            using (FileStream stream = new FileStream(_configPath, FileMode.Open))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();

                double value = Convert.ToDouble(navigator.SelectSingleNode("//offset[@name='Y']").Value);

                return value;
            }
        }

        public double GetOffsetX()
        {
            using (FileStream stream = new FileStream(_configPath, FileMode.Open))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();

                double value = Convert.ToDouble(navigator.SelectSingleNode("//offset[@name='X']").Value);

                return value;
            }
        }

        public double GetDefaultOffsetY()
        {
            using (FileStream stream = new FileStream(_configPath, FileMode.Open))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();

                double value = Convert.ToDouble(navigator.SelectSingleNode("//offset[@name='DefaultOffsetY']").Value);

                return value;
            }
        }

        public double GetRate()
        {
            using (FileStream stream = new FileStream(_configPath, FileMode.Open))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();

                double value = Convert.ToDouble(navigator.SelectSingleNode("//rate").Value);

                return value;
            }
        }

        public string GetSourceIP()
        {
            using (FileStream stream = new FileStream(_configPath, FileMode.Open))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();

                string value = navigator.SelectSingleNode("//network[@name='SourceIP']").Value;

                return value;
            }
        }

        public string GetSourcePort()
        {
            using (FileStream stream = new FileStream(_configPath, FileMode.Open))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();

                string value = navigator.SelectSingleNode("//network[@name='SourcePort']").Value;

                return value;
            }
        }

        public string GetDestIP()
        {
            using (FileStream stream = new FileStream(_configPath, FileMode.Open))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();

                string value = navigator.SelectSingleNode("//network[@name='DestIP']").Value;

                return value;
            }
        }

        public string GetDestPort()
        {
            using (FileStream stream = new FileStream(_configPath, FileMode.Open))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();

                string value = navigator.SelectSingleNode("//network[@name='DestPort']").Value;

                return value;
            }
        }        

        #endregion

        #region 儲存參數設定

        public void SaveRelativeData(DataGridView dgv)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_configPath);
            XPathNavigator navigator = document.CreateNavigator();

            // Remove old relative_setting for add new record
            if (navigator.Select("//relative_setting/*").Count > 0)
            {
                XPathNavigator first = navigator.SelectSingleNode("//relative_setting/*[1]");
                XPathNavigator last = navigator.SelectSingleNode("//relative_setting/*[last()]");
                navigator.MoveTo(first);
                navigator.DeleteRange(last);
            }

            lock (dgv)
            {
                for (int i = 0; i < dgv.Rows.Count - 1; i++)
                {
                    string source = dgv.Rows[i].Cells[0].Value.ToString();
                    string target = dgv.Rows[i].Cells[1].Value.ToString();
                    navigator.SelectSingleNode("//relative_setting").AppendChildElement(string.Empty, "column", string.Empty, null);
                    // Move to last column element then append source/target value.
                    navigator.SelectSingleNode("//relative_setting/column[last()]").AppendChildElement(string.Empty, "source", string.Empty, source);
                    navigator.SelectSingleNode("//relative_setting/column[last()]").AppendChildElement(string.Empty, "target", string.Empty, target);
                }
            }
            document.Save(_configPath); 
        }

        public void SaveOffsetY(string value)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_configPath);
            XPathNavigator navigator = document.CreateNavigator();
            navigator.SelectSingleNode("//offset[@name='Y']").SetValue(value);
            document.Save(_configPath);
        }

        public void SaveOffsetX(string value)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_configPath);
            XPathNavigator navigator = document.CreateNavigator();
            navigator.SelectSingleNode("//offset[@name='X']").SetValue(value);
            document.Save(_configPath);
        }

        public void SaveRate(string value)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_configPath);
            XPathNavigator navigator = document.CreateNavigator();
            navigator.SelectSingleNode("//rate").SetValue(value);
            document.Save(_configPath);
        }

        public void SaveSourceIP(string value)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_configPath);
            XPathNavigator navigator = document.CreateNavigator();
            navigator.SelectSingleNode("//network[@name='SourceIP']").SetValue(value);
            document.Save(_configPath);
        }

        public void SaveSourcePort(string value)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_configPath);
            XPathNavigator navigator = document.CreateNavigator();
            navigator.SelectSingleNode("//network[@name='SourcePort']").SetValue(value);
            document.Save(_configPath);
        }

        public void SaveDestIP(string value)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_configPath);
            XPathNavigator navigator = document.CreateNavigator();
            navigator.SelectSingleNode("//network[@name='DestIP']").SetValue(value);
            document.Save(_configPath);
        }

        public void SaveDestPort(string value)
        {
            XmlDocument document = new XmlDocument();
            document.Load(_configPath);
            XPathNavigator navigator = document.CreateNavigator();
            navigator.SelectSingleNode("//network[@name='DestPort']").SetValue(value);
            document.Save(_configPath);
        }

        #endregion
    }
}
