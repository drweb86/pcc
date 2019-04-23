using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ProofOfConcept
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Server _server = new Server();
        private MyToken _myToken;
        private void Form1_Load(object sender, EventArgs e)
        {
            _myToken = _server.Register();
            textBox1.Text = _myToken.Id;
            textBox2.Text = _myToken.Password;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var messages = _server.GetMessages(_myToken.Token);
            foreach (var message in messages)
            {
                MessageBox.Show(message.Message);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _server.SendMessage(_myToken.Token, int.Parse(textBox3.Text), textBox4.Text, textBox5.Text);
        }
    }

    class Server
    {
        private const string _serverUrl = "http://localhost:3000";


        public MyToken Register()
        {
            using (var wc = new WebClient())
            {
                // wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(_serverUrl + "/token", "");

                var json = new JavaScriptSerializer();
                var data = json.Deserialize<MyToken>(HtmlResult);
                return data;
            }
        }

        public ServerMessage[] GetMessages(string token)
        {
            using (var wc = new WebClient())
            {
                // wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.DownloadString(_serverUrl + "/client/"+ token+"/messages");

                var json = new JavaScriptSerializer();
                var data = json.Deserialize<ServerMessage[]>(HtmlResult);
                return data;
            }
        }

        public void SendMessage(string token, int remoteId, string remotePassword, string message)
        {
            using (var wc = new WebClient())
            {
                var json = new JavaScriptSerializer();
                var dataSend = json.Serialize(new
                {
                    id = remoteId,
                    password = remotePassword,
                    message = message,
                });

                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                string HtmlResult = wc.UploadString(_serverUrl + "/client/"+ token + "/message", dataSend);

            }
        }
    }

    class ServerMessage
    {
        public string FromToken { get; set; }
        public string ToToken { get; set; }
        public string Message { get; set; }
    }

    class MyToken
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
