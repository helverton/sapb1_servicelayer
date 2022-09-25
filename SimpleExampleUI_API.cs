using System;
using SAPbouiCOM;

namespace HelvertonSantos.Main
{
    class StartAct_Add_On
    {
        public static SAPbouiCOM.Application SboApplication;
        public static SAPbobsCOM.Company SboCompany;

        [STAThread]
        static void Main(string[] args)
        {
            SetApplication(args[0]);

            System.Windows.Forms.Application.Run();
        }

        private static void SetApplication(string connStr)
        {
            SboGuiApi sboGuiApi = new SboGuiApi();

            try
            {
                sboGuiApi.Connect(connStr);

                SboApplication = sboGuiApi.GetApplication();
                SboCompany = (SAPbobsCOM.Company)SboApplication.Company.GetDICompany();

                //Aqui pegamos uma sessão do service layer
                string SLContextSSO = SboApplication.Company.GetServiceLayerConnectionContext("https://host*:50000/b1s/v2");
                var dataSSO = SLContextSSO.Split(';');
                //B1SESSION=fs54g5s6-d5d4-a32a-888-000sa5s54s52c;;HttpOnly;path=/b1s/v2;Secure;SameSite=None;ROUTEID=.node5
                //cookies.Add(new Cookie(dataSSO[0].Split('=')[0], dataSSO[0].Split('=')[1]) { Domain = "database.ibg.lan" });
                //cookies.Add(new Cookie(dataSSO[6].Split('=')[0], dataSSO[6].Split('=')[1]) { Domain = "database.ibg.lan" });

                SboApplication.StatusBar.SetText("Conexão do add-on StartAct_Add_On realizado com sucesso.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                SboApplication.AppEvent += AppEvent;
                SboApplication.ItemEvent += ItemEvent;
                SboApplication.MenuEvent += MenuEvent;

                Menu.Add();
                SetFilters();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Não foi possivel conectar ao SAP Business One: " + ex.Message, "Erro de conexão", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                Environment.Exit(0);
            }
        }
    }
}
