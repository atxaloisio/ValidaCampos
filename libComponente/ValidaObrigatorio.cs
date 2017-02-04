using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace libComponente
{

    [ProvideProperty("Obrigatorio", typeof(Control))]
    [ProvideProperty("FraseErro", typeof(Control))]
    [ProvideProperty("IndiceCombo", typeof(ComboBox))]
    [ProvideProperty("TipoValidacao", typeof(MaskedTextBox))]
    public partial class ValidaObrigatorio : ErrorProvider, IExtenderProvider
    {
        protected Hashtable _obrigatorio;
        protected Hashtable _fraseerro;
        protected Hashtable _indicecombo;
        protected Hashtable _tipovalidacao;

        public ValidaObrigatorio()
        {
            _obrigatorio = new Hashtable();
            _fraseerro = new Hashtable();
            _indicecombo = new Hashtable();
            _tipovalidacao = new Hashtable();
            InitializeComponent();
        }

        public ValidaObrigatorio(IContainer container)
        {
            _obrigatorio = new Hashtable();
            _fraseerro = new Hashtable();
            _indicecombo = new Hashtable();
            _tipovalidacao = new Hashtable();
            container.Add(this);

            InitializeComponent();
        }

        private ReturnObrigatorio ObrigatorioExiste(object key)
        {
            ReturnObrigatorio p = (ReturnObrigatorio)_obrigatorio[key];
            if (p == null)
            {
                p = new ReturnObrigatorio();
                _obrigatorio[key] = p;
            }
            return p;
        }

        private ReturnFrase FraseErroExiste(object key)
        {
            ReturnFrase e = (ReturnFrase)_fraseerro[key];
            if (e == null)
            {
                e = new ReturnFrase();
                _fraseerro[key] = e;
            }
            return e;
        }

        private ReturnIndice IndiceComboExiste(object key)
        {
            ReturnIndice e = (ReturnIndice)_indicecombo[key];
            if (e == null)
            {
                e = new ReturnIndice();
                _indicecombo[key] = e;
            }
            return e;
        }

        private ReturnTipoValidacao TipoValidacaoExiste(object key)
        {
            ReturnTipoValidacao e = (ReturnTipoValidacao)_tipovalidacao[key];
            if (e == null)
            {
                e = new ReturnTipoValidacao();
                _tipovalidacao[key] = e;
            }
            return e;
        }

        public void SetObrigatorio(Control lControl, bool bValue)
        {
            ObrigatorioExiste(lControl).Obrigatorio = bValue;
        }

        public bool GetObrigatorio(Control lControl)
        {
            return ObrigatorioExiste(lControl).Obrigatorio;
        }

        public void SetTipoValidacao(Control lControl, TipoValidacao bValue)
        {
            TipoValidacaoExiste(lControl).tipoValidacao = bValue;
        }

        public TipoValidacao GetTipoValidacao(Control lControl)
        {
            return TipoValidacaoExiste(lControl).tipoValidacao;
        }

        public void SetFraseErro(Control lControl, string sValue)
        {
            FraseErroExiste(lControl).FraseErro = sValue;
        }

        public string GetFraseErro(Control lControl)
        {
            return FraseErroExiste(lControl).FraseErro;
        }

        public void SetIndiceCombo(Control lControl, int iValue)
        {
            IndiceComboExiste(lControl).IndiceCombo = iValue;
        }

        public int GetIndiceCombo(Control lControl)
        {
            return IndiceComboExiste(lControl).IndiceCombo;
        }

        public void ValidarCtrl(Control lControl, out bool bVer)
        {
            bool vRet = true;
                                               
            if (ObrigatorioExiste(lControl).Obrigatorio)
            {
                if (lControl is TextBox)
                {
                    if (lControl.Text == "")
                    {
                        this.SetError(lControl,
                         FraseErroExiste(lControl).FraseErro);
                        vRet = false;
                    }
                    else
                    {
                        this.SetError(lControl, string.Empty);
                    }
                }
                else if (lControl is MaskedTextBox)
                {
                    MaskedTextBox mtb = (MaskedTextBox)lControl;

                    switch (TipoValidacaoExiste(lControl).tipoValidacao)
                    {
                        case TipoValidacao.Vazio:
                            {
                                mtb.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                                if (lControl.Text == "")
                                {
                                    this.SetError(lControl,
                                     FraseErroExiste(lControl).FraseErro);
                                    vRet = false;
                                }
                                else
                                {
                                    this.SetError(lControl, string.Empty);
                                }
                            }
                            break;
                        case TipoValidacao.CEP:
                            {
                                if ((lControl.Text == "") || (lControl.Text.Length == 0))
                                {
                                    this.SetError(lControl,
                                     FraseErroExiste(lControl).FraseErro);
                                    vRet = false;
                                }
                                else
                                {
                                    if (mtb != null)
                                    {
                                        mtb.TextMaskFormat = MaskFormat.IncludeLiterals;
                                    }
                                    //Validar por expressão regular
                                    Regex re = new Regex(@"^\d{5}-\d{3}$");
                                    if (!re.IsMatch(lControl.Text))
                                    {
                                        this.SetError(lControl, "Formato do CEP inválido");
                                        vRet = false;
                                    }
                                    else
                                    {
                                        this.SetError(lControl, string.Empty);
                                    }
                                }
                            }
                            break;
                        case TipoValidacao.CPF:
                            {
                                if ((lControl.Text == "") || (lControl.Text.Length == 0))
                                {
                                    this.SetError(lControl,
                                     FraseErroExiste(lControl).FraseErro);
                                    vRet = false;
                                }
                                else
                                {
                                    if (mtb != null)
                                    {
                                        mtb.TextMaskFormat = MaskFormat.IncludeLiterals;
                                    }
                                    //Validar por expressão regular
                                    Regex re = new Regex(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$");
                                    if (!re.IsMatch(lControl.Text))
                                    {
                                        this.SetError(lControl, "Formato do CPF inválido");
                                        vRet = false;
                                    }
                                    else if (!ValidaCPF.IsCpf(lControl.Text))
                                    {
                                        this.SetError(lControl, "CPF inválido");
                                        vRet = false;
                                    }
                                    else
                                    {
                                        this.SetError(lControl, string.Empty);
                                    }
                                }
                            }
                            break;
                        case TipoValidacao.CNPJ:
                            {
                                if ((lControl.Text == "") || (lControl.Text.Length == 0))
                                {
                                    this.SetError(lControl,
                                     FraseErroExiste(lControl).FraseErro);
                                    vRet = false;
                                }
                                else
                                {
                                    if (mtb != null)
                                    {
                                        mtb.TextMaskFormat = MaskFormat.IncludeLiterals;
                                    }
                                    //Validar por expressão regular
                                    Regex re = new Regex(@"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$");
                                    if (!re.IsMatch(lControl.Text))
                                    {
                                        this.SetError(lControl, "Formato do CNPJ inválido");
                                        vRet = false;
                                    }
                                    else if (!ValidaCNPJ.IsCnpj(lControl.Text))
                                    {
                                        this.SetError(lControl, "CNPJ inválido");
                                        vRet = false;
                                    }
                                    else
                                    {
                                        this.SetError(lControl, string.Empty);
                                    }
                                }
                            }
                            break;
                        case TipoValidacao.Email:
                            {
                                if ((lControl.Text == "") || (lControl.Text.Length == 0))
                                {
                                    this.SetError(lControl,
                                     FraseErroExiste(lControl).FraseErro);
                                    vRet = false;
                                }
                                else
                                {
                                    if (mtb != null)
                                    {
                                        mtb.TextMaskFormat = MaskFormat.IncludeLiterals;
                                    }
                                    Regex re = new Regex(@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
                                    if (!re.IsMatch(lControl.Text))
                                    {
                                        this.SetError(lControl, "Formato do e-mail inválido.");
                                        vRet = false;
                                    }
                                    else
                                    {
                                        this.SetError(lControl, string.Empty);
                                    }
                                }
                            }
                            break;
                        case TipoValidacao.Telefone:
                            {
                                if ((lControl.Text == "") || (lControl.Text.Length == 0))
                                {
                                    this.SetError(lControl,
                                     FraseErroExiste(lControl).FraseErro);
                                    vRet = false;
                                }
                                else
                                {
                                    if (mtb != null)
                                    {
                                        mtb.TextMaskFormat = MaskFormat.IncludeLiterals;
                                    }
                                    //Validar por expressão regular
                                    Regex re = new Regex(@"^\(\d{2}\)\d{4}-\d{4}$");
                                    if (!re.IsMatch(lControl.Text))
                                    {
                                        this.SetError(lControl, "Telefone informado inválido. Utilize:(99) 9999-9999");
                                        vRet = false;
                                    }
                                    else
                                    {
                                        this.SetError(lControl, string.Empty);
                                    }
                                }
                            }
                            break;
                        case TipoValidacao.Celular:
                            {
                                if ((lControl.Text == "") || (lControl.Text.Length == 0))
                                {
                                    this.SetError(lControl,
                                     FraseErroExiste(lControl).FraseErro);
                                    vRet = false;
                                }
                                else
                                {
                                    if (mtb != null)
                                    {
                                        mtb.TextMaskFormat = MaskFormat.IncludeLiterals;
                                    }
                                    //Validar por expressão regular
                                    Regex re = new Regex(@"^\(\d{2}\)\d{5}-\d{4}$");
                                    if (!re.IsMatch(lControl.Text))
                                    {
                                        this.SetError(lControl, "Celular informado inválido. Utilize:(99) 99999-9999");
                                        vRet = false;
                                    }
                                    else
                                    {
                                        this.SetError(lControl, string.Empty);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }

                }
                else if (lControl is ComboBox)
                {
                    ComboBox ctl = (ComboBox)lControl;
                    if (ctl.SelectedIndex == -1)
                    {
                        this.SetError(lControl,
                         FraseErroExiste(lControl).FraseErro);
                        vRet = false;
                    }
                    else
                    {
                        this.SetError(lControl, string.Empty);
                    }
                }                                             
            }
            bVer = vRet;           
        }

        public bool Validar(bool exibeMsg = true)
        {
            bool bVer = true;
            string msg = string.Empty;
            List<string> lstMsg = new List<string>();
            bool retorno = true;

            foreach (Control ctrl in this.ContainerControl.Controls)
            {
                if (ctrl is Panel)
                {
                    foreach (Control lControl in ctrl.Controls)
                    {
                        ValidarCtrl(lControl, out bVer);
                        if (!bVer)
                        {
                            retorno = false;
                            lstMsg.Add(this.GetError(lControl));
                            //if (msg != string.Empty)
                            //{
                            //    msg = msg + "\n" + this.GetError(lControl);
                            //}
                            //else
                            //{
                            //    msg = this.GetError(lControl);
                            //}
                        }
                    }
                }
                else
                {
                    ValidarCtrl(ctrl, out bVer);
                    if (!bVer)
                    {
                        retorno = false;
                        lstMsg.Add(this.GetError(ctrl));
                        //if (msg != string.Empty)
                        //{
                        //    msg = msg + "\n" + this.GetError(ctrl);
                        //}
                        //else
                        //{
                        //    msg = this.GetError(ctrl);
                        //}
                    }
                }    
            }

            if ((!retorno) & (exibeMsg))
            {

                for (int i = lstMsg.Count; i > 0; i--)
                {
                    if (msg != string.Empty)
                    {
                        msg = msg + "\n" + lstMsg[i-1];
                    }
                    else
                    {
                        msg = lstMsg[i-1];
                    }
                }

                string Text = string.Empty;
                if (this.ContainerControl is Form)
                {
                    Text = this.ContainerControl.Text;
                }
                MessageBox.Show(msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return retorno;
        }

        bool IExtenderProvider.CanExtend(object lControl)
        {
            if ((lControl is TextBox) || (lControl is ComboBox) || (lControl is MaskedTextBox))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    partial class ReturnObrigatorio
    {
        public bool Obrigatorio;
        public ReturnObrigatorio()
        {
            Obrigatorio = false;
        }
    }

    partial class ReturnFrase
    {
        public string FraseErro;
        public ReturnFrase()
        {
            FraseErro = string.Empty;
        }
    }


    public enum TipoValidacao
    {
        Vazio,
        CEP,
        CPF,
        CNPJ,
        Email,
        Telefone,
        Celular

    }

    partial class ReturnIndice
    {
        public int IndiceCombo;
        public ReturnIndice()
        {
            IndiceCombo = -1;
        }
    }

    partial class ReturnTipoValidacao
    {
        public TipoValidacao tipoValidacao;
        public ReturnTipoValidacao()
        {
            tipoValidacao = new TipoValidacao();
        }
    }

}
