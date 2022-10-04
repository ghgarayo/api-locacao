using System;
using System.IO;

namespace SistemaLocacao
{
    class Program
    {

        /*
        
        Para efetuar os testes, você precisará instalar o Thunder Client para poder
        enviar as informações e ter um retorno do sistema       
        
        No arquivo "inputs testes.txt" você encontrará as URLs que deverão ser utilizadas
        de acordo com a função que quer testar
        
        */


        record ChavesLocatario(string nome, string email, string password);
        record ChavesImovel(string endereco, string numero, string complemento, string bairro,
                            string cidade, string uf, string proprietario, string valorAluguel);
        record ChavesLocacao(long id, long idImovel, string emailLocatario, string dataLocacao, string tempoContrato);
        record ChavesAtualizaLocatario(string email, string novoNome, string novoEmail);
        record ChavesAtualizaSenhaLocatario(string email, string novaSenha);
        record ChavesAtualizaImovel(long idImovel, string novoProprietario, string novoAluguel);
        record ChavesAtualizaLocacao(long idLocacao, string tempoContrato, string novaDataContrato);


        static BaseLocatarios baseLocatarios = new BaseLocatarios("bancoLocatarios.txt");
        static BaseImoveis baseImoveis = new BaseImoveis("bancoImoveis.txt");
        static BaseLocacao baseLocacoes = new BaseLocacao("bancoLocacoes.txt");

        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // ======================= CREATE =======================

            // **  CREATE Locatario **
            app.MapPost("/locatario/cadastrar", (ChavesLocatario u) =>
            {
                Locatario locatario = new Locatario(u.nome, u.email, u.password);
                baseLocatarios.AdicionarLocatario(locatario);
                return $"Usuário {u.nome} cadastrado com sucesso";
            });

            // ** CREATE Imóvel **
            app.MapPost("/imovel/cadastrar", (ChavesImovel i) =>
            {
                var imovel = new Imovel(i.endereco, i.numero, i.complemento, i.bairro,
                                        i.cidade, i.uf, i.proprietario, i.valorAluguel);
                baseImoveis.AdicionarImovel(imovel);
                return $"Imóvel de {i.proprietario} cadastrado com sucesso";
            });

            // ** CREATE Locação **
            app.MapPost("/locacao/cadastrar", (ChavesLocacao l) =>
             {
                 Locacao locacao = new Locacao(l.idImovel, baseLocatarios.BuscarPorEmail(l.emailLocatario).GetId(), l.dataLocacao, l.tempoContrato);
                 Imovel imovelLocacao = baseImoveis.BuscarImovel(l.idImovel);

                 if (imovelLocacao.GetDisponibilidade() == "não")
                 {
                     return $"Imóvel {imovelLocacao.GetId()} indisponível";
                 }
                 else
                 {
                     imovelLocacao.setDispo("não");
                     baseImoveis.SalvarNoArquivo();
                     baseLocacoes.AdicionarLocacao(locacao);

                     return $"Contrato {locacao.GetId()} adicionado ao sistema!";
                 }
             });

            // ======================= READ =======================

            // READ USUARIO

            // ** Busca a Lista completa de usuários ** 
            app.MapGet("/locatario/lista", () =>
            {
                return baseLocatarios.Serializar();
            });

            // ** Busca um usuário específico pelo e-mail ** 
            app.MapGet("/locatario/buscar/{email}", (string email) =>
            {
                return baseLocatarios.BuscarPorEmail(email).ToString();
            });

            // READ IMOVEL

            // ** Busca a Lista completa de imoveis ** 
            app.MapGet("/imovel/lista", () =>
            {
                return baseImoveis.Serializar();
            });

            //// ** Busca um imovel ou lista de imoveis de um proprietário pelo seu nome **
            app.MapGet("/imovel/buscar/{nomeProprietario}", (string nomeProprietario) =>
            {
                return baseImoveis.BuscaListaImoveis(nomeProprietario);
            });

            // READ LOCAÇÃO

            // ** Busca lista completa de contratos de locação **
            app.MapGet("/locacao/lista", () =>
            {
                return baseLocacoes.Serializar();
            });

            // ** Busca por ID de locação
            app.MapGet("/locacao/buscar1/{idLocacao}", (long idLocacao) =>
            {
                Locacao buscaLocacao = baseLocacoes.BuscarLocacaoPorContrato(idLocacao);
                string imprimeLocacao = "Locatário(a): " + baseLocatarios.BuscarPorId(buscaLocacao.GetIdLocatario()).GetNome() +
                                        "\nProprietário(a): " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetProprietario() +
                                        "\nEndereço: " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetEndereco() +
                                        ", " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetNumero() +
                                        " - " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetBairro() +
                                        " - " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetCidade() +
                                        "/" + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetEstado() +
                                        "\nValor do Aluguel: R$ " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetAluguel();

                return imprimeLocacao;
            });

            // ** Busca de locacao por ID de locatario
            app.MapGet("/locacao/buscar2/{idLocatario}", (long idLocatario) =>
            {
                Locacao buscaLocacao = baseLocacoes.BuscarLocacaoPorLocatario(idLocatario);
                string imprimeLocacao = "Locatário(a): " + baseLocatarios.BuscarPorId(buscaLocacao.GetIdLocatario()).GetNome() +
                        "\nProprietário(a): " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetProprietario() +
                        "\nEndereço: " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetEndereco() +
                        ", " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetNumero() +
                        " - " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetBairro() +
                        " - " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetCidade() +
                        "/" + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetEstado() +
                        "\nValor do Aluguel: R$ " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetAluguel();

                return imprimeLocacao;
            });

            // ** Busca de locacao pelo ID do imovel
            app.MapGet("/locacao/buscar3/{idImovel}", (long idImovel) =>
            {
                Locacao buscaLocacao = baseLocacoes.BuscarLocacaoPorImovel(idImovel);
                string imprimeLocacao = "Locatário(a): " + baseLocatarios.BuscarPorId(buscaLocacao.GetIdLocatario()).GetNome() +
                        "\nProprietário(a): " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetProprietario() +
                        "\nEndereço: " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetEndereco() +
                        ", " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetNumero() +
                        " - " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetBairro() +
                        " - " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetCidade() +
                        "/" + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetEstado() +
                        "\nValor do Aluguel: R$ " + baseImoveis.BuscarImovel(buscaLocacao.GetIdImovel()).GetAluguel();

                return imprimeLocacao;
            });

            // ======================= UPDATE =======================

            // ** UPDATE nome ou email locatário **
            app.MapPut("/locatario/atualiza/dados", (ChavesAtualizaLocatario cal) =>
            {
                string auxNome = "";
                string auxEmail = "";
                Locatario encontraLocatario = baseLocatarios.BuscarPorEmail(cal.email);

                if (cal.novoEmail != null)
                {
                    auxEmail = encontraLocatario.GetEmail();
                    encontraLocatario.SetEmail(cal.novoEmail);
                    baseLocatarios.SalvarNoArquivo();
                    return $"Email alterado de {auxEmail} para {cal.novoEmail}.";
                }
                else if (cal.novoNome != null)
                {
                    auxNome = encontraLocatario.GetNome();
                    encontraLocatario.SetNome(cal.novoNome);
                    baseLocatarios.SalvarNoArquivo();
                    return $"Nome alterado de {auxNome} para {cal.novoNome}.";
                }
                else
                {
                    throw new Exception("Nenhum usuário encontrado!");
                }
            });

            // ** UPDATE senha do Locatario **
            app.MapPut("/locatario/atualiza/senha", (ChavesAtualizaSenhaLocatario casl) =>
            {
                Locatario encontraLocatario = baseLocatarios.BuscarPorEmail(casl.email);
                long id = encontraLocatario.GetId();
                string nomeLocatario = encontraLocatario.GetNome();
                string emailLocatario = encontraLocatario.GetEmail();

                Locatario substituiLocatario = new Locatario(id, nomeLocatario, emailLocatario, Utilidades.GerarHash(casl.novaSenha));
                baseLocatarios.RemoverLocatario(encontraLocatario);
                baseLocatarios.AdicionarLocatario(substituiLocatario);
                return $"Senha de {nomeLocatario} atualizada!";
            });

            // ** UPDATE Imovel ** 
            app.MapPut("/imovel/atualiza/dados", (ChavesAtualizaImovel cai) =>
            {
                string auxAluguel = "";
                string auxProprietario = "";

                Imovel encontraImovel = baseImoveis.BuscarImovel(cai.idImovel);

                if (cai.novoAluguel != null)
                {
                    auxAluguel = encontraImovel.GetAluguel();
                    encontraImovel.SetAluguel(cai.novoAluguel);
                    baseImoveis.SalvarNoArquivo();
                    return $"Aluguel alterado de R${auxAluguel} para R${cai.novoAluguel}.";
                }
                else if (cai.novoProprietario != null)
                {
                    auxProprietario = encontraImovel.GetProprietario();
                    encontraImovel.SetProprietario(cai.novoProprietario);
                    baseImoveis.SalvarNoArquivo();
                    return $"Proprietário alterado de {auxProprietario} para {cai.novoProprietario}.";
                }
                else
                {
                    throw new Exception("Nenhum imóvel encontrado!");
                }
            });

            // UPDATE Locacao
            app.MapPut("/locacao/atualiza/tempocontrato", (ChavesAtualizaLocacao calo) =>
            {

                Locacao atualizaLocacao = baseLocacoes.BuscarLocacaoPorContrato(calo.idLocacao);
                atualizaLocacao.SetTempoContrato(calo.tempoContrato);
                atualizaLocacao.SetDataLocacao(calo.novaDataContrato);
                baseLocacoes.SalvarNoArquivo(); 

                return $"Contrato de locação {atualizaLocacao.GetId()} extendido por {calo.tempoContrato} meses à partir de {calo.novaDataContrato}.";
            });

            // ======================= DELETE  =======================

            // ** DELETE Locatario **
            app.MapDelete("/locatario/excluir/{email}", (string email) =>
            {
                Locatario encontraLocatario = baseLocatarios.BuscarPorEmail(email);
                baseLocatarios.RemoverLocatario(encontraLocatario);

                return $"Usuário {email} excluído do banco de dados.";
            });

            // ** DELETE Imovel **
            app.MapDelete("/imovel/excluir/{id}", (long id) =>
            {
                Imovel excluirImovel = baseImoveis.BuscarImovel(id);
                baseImoveis.RemoverImovel(excluirImovel);

                return $"Imóvel de {excluirImovel.GetProprietario()} excluído do banco de dados.";
            });

            // ** DELETE Locacao **
            app.MapDelete("/locacao/excluir/{id}", (long id) =>
            {
                Locacao excluirLocacao = baseLocacoes.BuscarLocacaoPorContrato(id);
                Imovel liberaImovel = baseImoveis.BuscarImovel(excluirLocacao.GetIdImovel());
                liberaImovel.setDispo("sim");
                baseImoveis.SalvarNoArquivo();
                baseLocacoes.RemoverLocacao(excluirLocacao);
            });

            // ** LIMPAR Banco de Dados - Locatario ** 
            app.MapDelete("/locatario/limparbanco", () => baseLocatarios.Limpar());

            // ** LIMPAR Banco de Dados - Imovel ** 
            app.MapDelete("/imovel/limparbanco", () => baseImoveis.Limpar());

            // ** LIMPAR Banco de Dados - Locacao **
            app.MapDelete("/locacao/limparbanco", () => baseLocacoes.Limpar());

            app.Run();
        }


    }
}
