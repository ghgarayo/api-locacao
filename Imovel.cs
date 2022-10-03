using System;

namespace SistemaLocacao
{

    public class Imovel
    {
        long id;
        string endereco;
        string numero;
        string complemento;
        string bairro;
        string cidade;
        string estado;
        string proprietario;
        string valorAluguel;
        string disponivel;


        public Imovel(string e, string n, string co, string b, string ci, string uf, string p, string a)
        {
            id = DateTime.Now.Ticks; //aqui estou usando o tempo atual (em milisegundos) como id do imovel
            endereco = e;
            numero = n;
            complemento = co;
            bairro = b;
            cidade = ci;
            estado = uf;
            proprietario = p;
            valorAluguel = a;
            disponivel = "sim";
        }

        public Imovel(long i, string e, string n, string co, string b, string ci, string uf, string p, string a, string dispo)
        {
            id = i;
            endereco = e;
            numero = n;
            complemento = co;
            bairro = b;
            cidade = ci;
            estado = uf;
            proprietario = p;
            valorAluguel = a;
            disponivel = dispo;
        }

        public long GetId()
        {
            return id;
        }

        public string GetProprietario()
        {
            return proprietario;
        }

        public void SetProprietario(string p)
        {
            proprietario = p;
        }

        public string GetEndereco()
        {
            return endereco;
        }

        public void SetEndereco(string e)
        {
            endereco = e;
        }

        public string GetNumero()
        {
            return numero;
        }

        public void SetNumero(string n)
        {
            numero = n;
        }

        public string GetComplemento()
        {
            return complemento;
        }

        public void SetComplemento(string co)
        {
            complemento = co;
        }

        public string GetBairro()
        {
            return bairro;
        }

        public void setBairro(string b)
        {
            bairro = b;
        }

        public string GetCidade()
        {
            return cidade;
        }

        public void SetCidade(string ci)
        {
            cidade = ci;
        }

        public string GetEstado()
        {
            return estado;
        }

        public void SetEstado(string e)
        {
            estado = e;
        }

        public string GetAluguel()
        {
            return valorAluguel;
        }

        public void SetAluguel(string a)
        {
            valorAluguel = a;
        }

        public void setDispo(string d)
        {
            disponivel = d;
        }

        public string GetDisponibilidade()
        {
            return disponivel;
        }

        public string Serializar()
        {
            return id + "," + endereco + "," + numero + "," +
                   complemento + "," + bairro + "," +
                   cidade + "," + estado + "," + proprietario + "," + valorAluguel + "," + disponivel;
        }

        public override string ToString()
        {
            return $"Endereço: {endereco}, {numero} - {bairro} - {cidade}/{estado} \nProprietário do Imóvel: {proprietario} \nValor do Aluguel: R$ {valorAluguel} \nDisponível para locação: {disponivel}";
        }

    }

    public class BaseImoveis
    {
        string filename;
        List<Imovel> imoveis;

        public BaseImoveis(string f)
        {
            filename = f;
            imoveis = new List<Imovel>();
            CarregarImoveis();
        }

        public void CarregarImoveis()
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
                    Imovel imovel = new Imovel(long.Parse(valores[0]), valores[1],
                                               valores[2], valores[3], valores[4],
                                               valores[5], valores[6], valores[7], 
                                               valores[8], valores[9]);
                    imoveis.Add(imovel);
                }
            }
        }

        public string Serializar()
        {
            string output = "";
            foreach (var imovel in imoveis)
            {
                output += imovel.Serializar() + "\n";
            }
            return output;
        }

        public void SalvarNoArquivo()
        {
            string output = Serializar();
            File.WriteAllText(filename, output);
        }

        public void Limpar()
        {
            imoveis.Clear();
            SalvarNoArquivo();
        }

        public void AdicionarImovel(Imovel i)
        {
            foreach (var imovel in imoveis)
            {
                //lanca erro caso usuario ja esteja na lista ou email ja esteja em uso
                if (imovel.GetId() == i.GetId())
                {
                    throw new Exception($"usuario '{i.GetId()}' ja esta na lista");
                }

            }
            imoveis.Add(i);
            SalvarNoArquivo();
        }
        public Imovel BuscarImovel(long idImovel)
        {
            foreach (var imovel in imoveis)
            {
                if (imovel.GetId() == idImovel)
                {
                    return imovel;
                }
            }
            //caso nao encontre, lanca erro
            throw new Exception($"Imovel '{idImovel}' não foi encontrado na base");
        }

        public long BuscarImovelPorProprietario(string nomeProprietario)
        {
            foreach (var imovel in imoveis)
            {
                if (imovel.GetProprietario() == nomeProprietario)
                {
                    return imovel.GetId();
                }
            }
            //caso nao encontre, lanca erro
            throw new Exception($"Proprietario '{nomeProprietario}' não foi encontrado na base");
        }

        public void RemoverImovel(Imovel m)
        {
            foreach (var imovel in imoveis)
            {
                if (imovel.GetProprietario() == m.GetProprietario())
                {
                    imoveis.Remove(imovel);
                    SalvarNoArquivo();
                    return;
                }
            }
        }
        public string BuscaListaImoveis(string nomeProprietario)
        {
            string listaImoveis = "";
            foreach (var imovel in imoveis)
            {
                if (nomeProprietario == imovel.GetProprietario())
                {
                   listaImoveis += imovel.ToString() + "\n";
                }
            }
            return listaImoveis;
        }

    }

}