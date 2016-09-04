namespace AssinaGravaPDF.Models
{
    public class Produto
    {
        #region "Atributos da classe"

        private int _ID_CADMAT;
        private string _DESCR_MAT_CADMAT;
        private int _QTD_CADMAT;
        private string _PRECO_UNIT;
        private string _UN_MEDIDA;

        #endregion

        #region "Propriedades da classe"

        public int ID_CADMAT
        {
            get { return _ID_CADMAT; }
            set { _ID_CADMAT = value; }
        }

        public string DESCR_MAT_CADMAT
        {
            get { return _DESCR_MAT_CADMAT; }
            set { _DESCR_MAT_CADMAT = value; }
        }

        public int QTD_CADMAT
        {
            get { return _QTD_CADMAT; }
            set { _QTD_CADMAT = value; }
        }

        public string PRECO_UNIT
        {
            get { return _PRECO_UNIT; }
            set { _PRECO_UNIT = value; }
        }

        public string UN_MEDIDA
        {
            get { return _UN_MEDIDA; }
            set { _UN_MEDIDA = value; }
        }

        #endregion
    }
}