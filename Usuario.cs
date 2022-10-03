using System;

namespace SistemaLocacao
{
    public class Locatario
    {
        long id;
        string nome;
        string email;
        string hash;

        public Locatario(string n, string e, string password)
        {
            id = DateTime.Now.Ticks;
            nome = n;
            email = e;
            hash = Utilidades.GerarHash(password);
        }

        //cria Locatario a partir de dados salvos na base (esse construtor eh usado apenas pela classe BaseLocatarios)
        public Locatario(long i, string n, string e, string h)
        {
            id = i;
            nome = n;
            email = e;
            hash = h;
        }

        public long GetId()
        {
            return id;
        }

        public string GetNome()
        {
            return nome;
        }

        public string GetEmail()
        {
            return email;
        }

        public string GetHash()
        {
            return hash;
        }

        public void SetNome(string n)
        {
            nome = n;
        }

        public void SetEmail(string e)
        {
            email = e;
        }

        public string Serializar()
        {
            return id + "," + nome + "," + email + "," + hash;
        }

        public override string ToString()
        {
            return $"Nome: {nome}\nEmail: {email} \nID: {id} ";
        }

    }

    public class BaseLocatarios
    {
        string filename;
        List<Locatario> locatarios;

        public BaseLocatarios(string f)
        {
            filename = f;
            locatarios = new List<Locatario>();
            CarregarLocatarios();
        }

        public void CarregarLocatarios()
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
                    Locatario locatario = new Locatario(long.Parse(valores[0]), valores[1], valores[2], valores[3]);
                    locatarios.Add(locatario);
                }
            }
        }

        public string Serializar()
        {
            string output = "";
            foreach (var locatario in locatarios)
            {
                output += locatario.Serializar() + "\n";
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
            locatarios.Clear();
            SalvarNoArquivo();
        }

        public void AdicionarLocatario(Locatario u)
        {   
            foreach (var locatario in locatarios)
            {
                //lanca erro caso Locatario ja esteja na lista ou email ja esteja em uso
                if (locatario.GetEmail() == u.GetEmail())
                {
                    throw new Exception($"Usuário '{u.GetEmail()}' já cadastrado! Tente outra opção.");
                }
            }
            locatarios.Add(u);
            SalvarNoArquivo();
        }

        //retorna Locatario que possui email indicado
        public Locatario BuscarPorEmail(string email)
        {
            foreach (var locatario in locatarios)
            {
                if (locatario.GetEmail() == email)
                {
                    return locatario;
                }
            }
            //caso nao encontre, lanca erro
            throw new Exception($"Locatario '{email}' nao foi encontrado na base");
        }

        public Locatario BuscarPorId(long idLocatario)
        {
            foreach (var locatario in locatarios)
            {
                if (locatario.GetId() == idLocatario)
                {
                    return locatario;
                }
            }
            //caso nao encontre, lanca erro
            throw new Exception($"Locatario nao foi encontrado na base");
        }


        public void RemoverLocatario(Locatario u)
        {
            foreach (var locatario in locatarios)
            {
                if (locatario.GetEmail() == u.GetEmail())
                {
                    locatarios.Remove(locatario);
                    SalvarNoArquivo();
                    return;
                }
            }
        }
    }
}

