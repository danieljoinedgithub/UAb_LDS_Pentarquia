namespace testeApiDgeg
{
    //Combustiveis
    public class TipoCombustivel
    {
        public string Descritivo { get; set; }
        public string UnidadeMedida { get; set; }
        public int Id { get; set; }
        
        //Contrutor
        public TipoCombustivel(string Descritivo, string UnidadeMedida, int Id)
        {
            this.Descritivo = Descritivo;
            this.UnidadeMedida = UnidadeMedida;
            this.Id = Id;
        }
        // O Construtor Vazio
        public TipoCombustivel()
        {

        }
 
    }
}