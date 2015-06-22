using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SalesForce4Win.SFWeb;


namespace SalesForce4Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DescribeGlobalSObjectResult[] sObjResults;
        List<Element> standardElements;
        List<Element> customElements;
        List<Element> customSettings;
        QueryResult qResult;
        private void Form1_Load(object sender, EventArgs e)
        {
            LoginForm loginform = new LoginForm();
            loginform.ShowDialog(this);

            this.Text = Connection.Login.userInfo.userName;
            standardElements = new List<Element>();
            customElements = new List<Element>();
            customSettings = new List<Element>();
            qResult = null;

            if (Connection.Login != null)
            {
                loadObjectsToolStripMenuItem_Click(null, null);
            }
            else
            {
                MessageBox.Show("Connection Error");
            }
           
        }


        private void loadTreeFromLists()
        {
            foreach (Element e in standardElements)
            {
                Tree.Nodes[0].Nodes.Add(e.ToString());
            }
            foreach (Element e in customElements)
            {
                Tree.Nodes[1].Nodes.Add(e.ToString());
            }
            foreach (Element e in customSettings)
            {
                Tree.Nodes[2].Nodes.Add(e.ToString());
            }
        }

        private void loadObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            standardElements.Clear();
            customElements.Clear();
            customSettings.Clear();
            sObjResults = Connection.SF.describeGlobal().sobjects;

            for (int i = 0; i < sObjResults.Length; i++)
            {
                if (sObjResults[i].custom == false && sObjResults[i].customSetting == false)
                {
                    standardElements.Add(new Element { Name = sObjResults[i].name });
                }
                if (sObjResults[i].custom == true && sObjResults[i].customSetting == false)
                {
                    customElements.Add(new Element { Name = sObjResults[i].name });
                }
                if (sObjResults[i].custom == true && sObjResults[i].customSetting == true)
                {
                    customSettings.Add(new Element { Name = sObjResults[i].name });
                }

            }

            loadTreeFromLists();

        }

        private List<Field> loadFieldsForObject(string objName)
        {
            DescribeSObjectResult obj = Connection.SF.describeSObject(objName);

            List<Field> fields = new List<Field>();

            for (int i = 0; i < obj.fields.Length; i++)
            {
                fields.Add(new Field { Type = obj.fields[i].type.ToString(), Name = obj.fields[i].name.ToString(), Label = obj.fields[i].label.ToString() });
            }


            return fields;
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            List<Element> listToShow = new List<Element>();
            _List.Items.Clear();
            _List.Columns.Clear();
            if (Tree.SelectedNode.Level == 0)
            {
                switch (Tree.SelectedNode.Name)
                {
                    case "standardObjectsNode":
                        listToShow = standardElements;
                        break;
                    case "customObjectsNode":
                        listToShow = customElements;
                        break;
                    case "customSettingsNode":
                        listToShow = customSettings;
                        break;
                }

                _List.Columns.Add("Objects", 300, HorizontalAlignment.Left);
                foreach (Element item in listToShow)
                {
                    _List.Items.Add(new ListViewItem(item.Name));
                }
                return;
            }
            else if (Tree.SelectedNode.Level == 1)
            {
                DescribeSObjectResult obj = Connection.SF.describeSObject(Tree.SelectedNode.Text);
                //table.DataSource = obj.fields;

                _List.Columns.Add("Label", 300, HorizontalAlignment.Left);
                _List.Columns.Add("Name", 300, HorizontalAlignment.Left);
                _List.Columns.Add("Type", 150, HorizontalAlignment.Left);
                _List.Columns.Add("Referenced to", 150, HorizontalAlignment.Left);
                //Tree.SelectedNode.Nodes.Clear();
                if (Tree.SelectedNode.Nodes.Count == 0)
                {
                    for (int i = 0; i < obj.fields.Length; i++)
                    {
                        Tree.SelectedNode.Nodes.Add(obj.fields[i].name);
                    }
                }
                string s = "";
                for (int i = 0; i < obj.fields.Length; i++)
                {
                    s = obj.fields[i].type.Equals(fieldType.reference) ? obj.fields[i].referenceTo[0].ToString() : "";
                    
                    _List.Items.Add(new ListViewItem(new string[] { obj.fields[i].label.ToString(), obj.fields[i].name.ToString(), obj.fields[i].type.ToString(), s }));
                }
                

            }

            else if (Tree.SelectedNode.Level == 2)
            {

                DescribeSObjectResult obj = Connection.SF.describeSObject(Tree.SelectedNode.Parent.Text);

                SFWeb.Field a = obj.fields.Where(o => o.name == Tree.SelectedNode.Text).First() as SFWeb.Field;
                
                _List.Columns.Add("Label", 200, HorizontalAlignment.Left);
                _List.Columns.Add("Name", 200, HorizontalAlignment.Left);
                _List.Columns.Add("Type", 100, HorizontalAlignment.Left);
                _List.Columns.Add("Referenced to", 100, HorizontalAlignment.Left);

                string s = "";
                if (a.type.Equals(fieldType.reference))
                {
                    String[] referenceTos = a.referenceTo;
                    for (int j = 0; j < referenceTos.Length; j++)
                    {
                        s += referenceTos[j] + " ";
                    }
                }

                _List.Items.Add(new ListViewItem(new string[] { a.label.ToString(), a.name.ToString(), a.type.ToString(), s}));
                    
              


            }





            /* 
            
             SFWeb.Field[] fields = obj.fields;

             // Iterate through each field and gets its properties
             resultTxt.Text = "Field Name \t Field Label \t Field Type\n";
             for (int i = 0; i < fields.Length; i++)
             {

                 SFWeb.Field field = fields[i];

                 resultTxt.Text += field.name + "\t" + field.label + "\t" + field.type + "\n";


                 // If this is a picklist field, show the picklist values

                 if (field.type.Equals(fieldType.picklist))
                 {
                     PicklistEntry[] picklistValues = field.picklistValues;

                     if (picklistValues != null)
                     {
                         //resultTxt.Text += "Picklist values:\n";                        
                         for (int j = 0; j < picklistValues.Length; j++)
                         {
                             if (picklistValues[j].label != null)
                             {
                                 //resultTxt.Text += "\tItem: " + picklistValues[j].label + "\n";
                             }
                         }
                     }
                 }


                 // If a reference field, show what it references

                 if (field.type.Equals(fieldType.reference))
                 {
                     String[] referenceTos = field.referenceTo;
                     for (int j = 0; j < referenceTos.Length; j++)
                     {
                         //resultTxt.Text += "\t" + referenceTos[j] + "\n";
                     }
                 }
             }*/
        }

        private void _List_Click(object sender, EventArgs e)
        {
           
        }

        private void _List_ItemActivate(object sender, EventArgs e)
        {
            
            //MessageBox.Show(_List.SelectedItems[0].Text + " " + _List.SelectedItems[1].Text);
        }

        private void _List_DoubleClick(object sender, EventArgs e)
        {
            Point mousePos = _List.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hitTest = _List.HitTest(mousePos);
            int columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);

            if ((columnIndex == 0 && hitTest.Item.SubItems[columnIndex].Text != string.Empty && _List.Columns[0].Text == "Objects") || (columnIndex == 3 && hitTest.Item.SubItems[columnIndex].Text != string.Empty))
            {            
                findNodeByName(hitTest.Item.SubItems[columnIndex].Text);                
            }
            
            
        }

        private void findNodeByName(string nodeName)
        {
            foreach (TreeNode node in Tree.Nodes)
            {
                foreach (TreeNode inNode in node.Nodes)
                {
                    if (inNode.Text == nodeName)
                    {
                        Tree.SelectedNode = inNode;
                        Tree.SelectedNode.Checked = true;
                        inNode.TreeView.Focus();
                    }

                }
            }
           
        }

        

        

        

    }
}

