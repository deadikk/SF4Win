using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesForce4Win
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            loginTxt.Text = "dmytro.strebul@globallogic.com";
            passTxt.Text = "0227533deadikDDK";
        }

        private void EnterBtn_Click(object sender, EventArgs e)
        {

            if (loginTxt.Text.Length == 0 && passTxt.Text.Length == 0)
            {
                MessageBox.Show("Please, enter login and password");
                return;
            }
            else if (loginTxt.Text.Length == 0)
            {
                MessageBox.Show("Please, enter login");
                return;
            }
            else if (passTxt.Text.Length == 0)
            {
                MessageBox.Show("Please, enter password");
                return;
            }



            Connection.SF = new SFWeb.SforceService();
            if(radioTest.Checked)
            {
                Connection.SF.Url = "https://test.salesforce.com/services/Soap/u/34.0";
            }
            if (radioProd.Checked)
            {
                Connection.SF.Url = "https://login.salesforce.com/services/Soap/u/34.0";
            }
            
            
            String username = loginTxt.Text;
            String password = passTxt.Text;
            try
            {
                Connection.Login = Connection.SF.login(username, password);
                Connection.SF.Url = Connection.Login.serverUrl;
                Connection.SF.SessionHeaderValue = new SFWeb.SessionHeader();
                Connection.SF.SessionHeaderValue.sessionId = Connection.Login.sessionId;
                this.Close();
                
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                // This is likley to be caused by bad username or password
                Connection.SF = null;
                //passTxt.Text = String.Empty;
                MessageBox.Show("Connection error. Try again.\n");
                return;
                
            }
            catch (Exception ex)
            {
                // This is something else, probably comminication
                Connection.SF = null;
                MessageBox.Show("Error text:\n" + ex);
                return;
            }        

                    
        }

        private void passTxt_MouseHover(object sender, EventArgs e)
        {
            passTxt.PasswordChar = '\0';
        }

        private void passTxt_MouseLeave(object sender, EventArgs e)
        {
            passTxt.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

       
        
    }
}
