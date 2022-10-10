using System;

namespace SistemaLocacao
{

    public class Locacao
    {
        long id;
        long idImovel;
        long idLocatario;
        string dataLocacao;
        string tempoContrato;

        public Locacao(long i, long im, long l, string d, string t)
        {
            id = i;
            idImovel = im;
            idLocatario = l;
            dataLocacao = d;
            tempoContrato = t;
        }

        public Locacao(long im, long l, string d, string t)
        {
            id = DateTime.Now.Ticks;
            idImovel = im;
            idLocatario = l;
            dataLocacao = d;
            tempoContrato = t;
        }

        public long GetId()
        {
            return id;
        }

        public long GetIdImovel()
        {
            return idImovel;
        }

        public long GetIdLocatario()
        {
            return idLocatario;
        }

        public string GetDataLocacao()
        {
            return dataLocacao;
        }

        public string GetTempoContrato()
        {
            return tempoContrato;
        }
        public void SetTempoContrato(string tempoDeContrato)
        {
            tempoContrato = tempoDeContrato;
        }

        public void SetDataLocacao(string novaDataContrato)
        {
            dataLocacao = novaDataContrato;
        }
        
        public string Serializar()
        {
            return id + "," + idImovel + "," + idLocatario + "," + dataLocacao + "," + tempoContrato;
        }

    }

    public class BaseLocacao
    {
        string filename;
        List<Locacao> locacoes;

        public BaseLocacao(string f)
        {
            filename = f;
            locacoes = new List<Locacao>();
            CarregarLocacoes();
        }

        public void CarregarLocacoes()
        {
            if (!File.Exists(filename))
            {
                File.CreateText(filename);
            }
            string input = File.ReadAllText(filename);
            string[] linhas = input.Split("\n");

            foreach (var linha in linhas)
            {
                if (linha.Length > 0)
                {
                    string[] valores = linha.Split(",");
                    Locacao locacao = new Locacao(long.Parse(valores[0]), long.Parse(valores[1]), long.Parse(valores[2]), valores[3], valores[4]);
                    locacoes.Add(locacao);
                }
            }
        }

        public string Serializar()
        {
            string output = "";
            foreach (var locacao in locacoes)
            {
                output += locacao.Serializar() + "\n";
            }
            return output;
        }

        public void SalvarNoArquivo()
        {
            string output = Serializar();
            File.WriteAllText(filename, output);
        }

        public void AdicionarLocacao(Locacao l)
        {
            foreach (var locacao in locacoes)
            {
                if (locacao.GetId() == l.GetId())
                {
                    throw new Exception($"Contrato '{l.GetId()}' ja esta na lista");
                }

            }
            locacoes.Add(l);
            SalvarNoArquivo();
        }

        public void Limpar()
        {
            locacoes.Clear();
            SalvarNoArquivo();
        }

        public Locacao BuscarLocacaoPorContrato(long idLocacao)
        {
            foreach (var locacao in locacoes)
            {
                if (locacao.GetId() == idLocacao)
                {
                    return locacao;
                }
            }
            //caso nao encontre, lanca erro
            throw new Exception($"Imovel '{idLocacao}' não foi encontrado na base");
        }

        public Locacao BuscarLocacaoPorLocatario(long idLocatario)
        {
            
            foreach (var locacao in locacoes)
            {
                if (idLocatario == locacao.GetIdLocatario())
                {
                    return locacao;
                }
            }
            throw new Exception($"Locatario '{idLocatario}' não foi encontrado na base");
        }

        public Locacao BuscarLocacaoPorImovel(long idImovel)
        {
            foreach (var locacao in locacoes)
            {
                if (idImovel == locacao.GetIdImovel())
                {
                    return locacao;
                }
            }
            throw new Exception($"Imóvel '{idImovel}' não foi encontrado na base de locações. Disponível para locação");
        }

        public void RemoverLocacao(Locacao l)
        {
            foreach (var locacao in locacoes)
            {
                if (locacao.GetId() == l.GetId())
                {
                    locacoes.Remove(locacao);
                    SalvarNoArquivo();
                    return;
                }
            }
        }

    }

}