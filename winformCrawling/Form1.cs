using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;


namespace winformCrawling
{
    public partial class Form1 : Form
    {
        private ChromeDriverService _driverService = null;
        private ChromeOptions _options = null;
        private ChromeDriver _driver = null;
        public Form1()
        {
            InitializeComponent();

            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string id = tboxID.Text;
            string pw = tboxPW.Text;

            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl("https://www.naver.com/"); //웹사이트에 접속
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            var element = _driver.FindElementByXPath("//*[@id='account']/a"); //Main 로그인 버트

            element.Click();

            Thread.Sleep(3000);

            element = _driver.FindElementByXPath("//*[@id='id']"); //ID 입력창 
            element.SendKeys(id);

            element = _driver.FindElementByXPath("//*[@id='pw']"); //PW 입력창
            element.SendKeys(pw);
           
            element = _driver.FindElementByXPath("//*[@id='log.login']");//로그인 버튼
            element.Click();
           
        }

        List<string> Lsrc = new List<string>(); // IMG URL
        int i = 0; //현재 배열 위치

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string strURL = "https://www.google.com/search?q=" + tboxSearch.Text + "&source=lnms&tbm=isch";

            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl(strURL);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            _driver.ExecuteScript("window.scrollBy(0, 10000)");

   

            Lsrc = new List<string>();
            foreach (IWebElement item in _driver.FindElementsByClassName("rg_i"))
            {
                if (item.GetAttribute("src") != null)
                    Lsrc.Add(item.GetAttribute("src"));
            }


            lblTotal.Text = "/ " + Lsrc.Count.ToString();

            this.Invoke(new Action(delegate ()
            {
                try
                {
                    foreach (string strsrc in Lsrc)
                    {
                        i++;

                        GetMapImage(Lsrc[i]);
                        tboxGo.Text = i.ToString();
                        Refresh();
                        Thread.Sleep(50);
                    }
                }
                catch (Exception)
                {
                }
            }));
        }

        private void GetMapImage(string base64String)
        {
            try
            {
                var base64Data = Regex.Match(base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;  // 정규식 검색
                var binData = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(binData))
                {
                    if (stream.Length == 0)
                    {
                        pboxMain.Load(base64String);
                        tboxGo.Text = i.ToString();
                        tboxUrl.Text = base64String;
                    }
                    else
                    {
                        var image = Image.FromStream(stream);
                        pboxMain.Image = image;
                        tboxUrl.Text = base64String;
                    }
                }
            }
            catch
            {

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void tboxUrl_TextChanged(object sender, EventArgs e)
        {

        }

        private void tboxSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnPre_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i--;

                GetMapImage(Lsrc[i]);
                tboxGo.Text = i.ToString();
            }));

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i++;

                GetMapImage(Lsrc[i]);
                tboxGo.Text = i.ToString();
            }));
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(delegate ()
            {
                i = int.Parse(tboxGo.Text);

                GetMapImage(Lsrc[i]);
                tboxGo.Text = i.ToString();
            }));
        }
    }
}
