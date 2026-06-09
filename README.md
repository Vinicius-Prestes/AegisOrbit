# 🛰️ AegisOrbit API — Monitoramento e Sustentabilidade Orbital

O **AegisOrbit** é uma plataforma de backend de missão crítica desenvolvida em `.NET Core 8` e integrada ao banco de dados Oracle Cloud. O sistema foi projetado para atuar na vanguarda da governança espacial, realizando o rastreamento, catalogação e análise preditiva de colisões entre satélites operacionais e detritos espaciais na Órbita Baixa da Terra (LEO).

---

## 🌱 Integração com o Meio Ambiente e Sustentabilidade

Quando pensamos em meio ambiente, raramente olhamos para cima, mas o espaço ao redor da Terra é um ecossistema finito e severamente degradado por décadas de exploração irresponsável. O acúmulo de lixo cósmico ameaça desencadear a **Síndrome de Kessler** — um efeito dominó onde colisões geram novos detritos, tornando órbitas inteiras inutilizáveis por séculos.

O **AegisOrbit** integra-se diretamente à causa ambiental através de três pilares:
1. **Preservação do Patrimônio Orbital:** Trata o espaço ao redor da Terra como um recurso natural que precisa ser limpo e protegido contra a poluição por detritos.
2. **Proteção a Satélites Climáticos:** Garante a integridade de satélites ambientais cruciais que monitoram o desmatamento, camadas de ozônio, oceanos e mudanças climáticas globais, emitindo alertas preventivos para evitar que sejam destruídos.
3. **Mitigação de Impactos Atmosféricos:** Ao prever janelas de reentrada segura e gerenciar detritos, o sistema auxilia no planejamento de descartes que minimizem a liberação de compostos tóxicos na atmosfera terrestre durante a queima de materiais.

---

## 🕹️ Funcionamento dos Endpoints (O Motor do Radar)

A API expõe um conjunto de endpoints REST organizados para simular um centro de controle aeroespacial dinâmico:

* **`GET /api/orbital` (Painel Geral de Radar):** Busca e lista em tempo real todos os objetos espaciais persistidos no banco de dados Oracle. É a visão macro do tráfego orbital atual.
  
* **`POST /api/orbital/satellite` (Ingestão de Tecnologia Ativa):** Cadastra um satélite operacional na malha de monitoramento. Recebe parâmetros como massa, velocidade e operadora, armazenando as coordenadas geográficas em uma estrutura de valor imutável.
  
* **`POST /api/orbital/debris` (Catalogação de Lixo Espacial):** Registra fragmentos inertes, carcaças de foguetes antigos ou ferramentas perdidas no espaço. Permite associar a missão de origem do detrito e seu tamanho estimado em metros.
  
* **`GET /api/orbital/check-collision` (Análise de Alvo Específico):** Recebe o identificador único (Guid) de dois objetos quaisquer e calcula isoladamente a distância orbital entre eles, a probabilidade matemática exata de um impacto e o tempo estimado para a colisão.
  
* **`GET /api/orbital/threat-report` (Relatório de Ameaças Automatizado):** O cérebro do sistema. Criado para eliminar a necessidade de buscas manuais por IDs. Ele lê de forma consolidada todos os registros do banco, roda uma matriz combinatória de cruzamento de dados e gera um relatório consolidado com todos os riscos de colisão reais encontrados, ordenando-os automaticamente do risco mais crítico (urgente) para o mais baixo.

---

##  Arquitetura e Engenharia de Software

O projeto foi edificado sobre os pilares do desenvolvimento moderno de software:
* **Herança e Polimorfismo:** Classes especializadas herdam propriedades de uma entidade abstrata mãe, redefinindo comportamentos complexos de risco de reentrada.
* **Complex Properties (.NET 8):** Mapeamento avançado do Entity Framework Core 8 para embutir tipos de valor (`struct`) de forma achatada diretamente nas colunas da tabela do Oracle, otimizando a performance de leitura.
* **Resiliência:** Tratamento defensivo de exceções em loops críticos para garantir que dados corrompidos de um único detrito não interrompam o monitoramento dos demais satélites.

---

Pré-requisitos

Antes de iniciar, certifique-se de possuir os seguintes componentes instalados:

.NET 8 SDK
Oracle Database (Oracle Cloud ou ambiente local)
Git
Visual Studio 2022 ou Visual Studio Code (opcional)
1. Clonar o Repositório

Clone o projeto para sua máquina local:

git clone https://github.com/Vinicius-Prestes/AegisOrbit.git
cd AegisOrbit
2. Configurar a Conexão com o Banco

O projeto utiliza Oracle Database para persistência dos dados orbitais.

Crie um arquivo appsettings.json na raiz do projeto utilizando como base o arquivo de exemplo:

cp appsettings.Example.json appsettings.json

Configure a string de conexão com os dados da sua instância Oracle:

{
  "ConnectionStrings": {
    "OracleConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_DATABASE"
  }
}

As credenciais reais não devem ser versionadas. O arquivo appsettings.json está protegido pelo .gitignore.

3. Restaurar Dependências

Execute:

dotnet restore
4. Compilar a Aplicação

Verifique se todos os pacotes e referências estão corretos:

dotnet build
5. Executar a API

Inicie o servidor local:

dotnet run

Ao iniciar corretamente, o terminal exibirá um endereço semelhante a:

http://localhost:5138
6. Acessar a Documentação Swagger

Abra o navegador e acesse:

http://localhost:5138/swagger

A interface Swagger permite explorar e testar todos os endpoints da aplicação sem necessidade de ferramentas adicionais.

Fluxo de Teste Recomendado

Para validar rapidamente as principais funcionalidades da plataforma:

1. Registrar um Satélite

Utilize:

POST /api/orbital/satellite
2. Registrar um Detrito Espacial

Utilize:

POST /api/orbital/debris
3. Consultar Objetos Monitorados

Utilize:

GET /api/orbital
4. Gerar Relatório Completo de Ameaças

Utilize:

GET /api/orbital/threat-report

Esse endpoint realiza o cruzamento automático de todos os objetos cadastrados, identificando potenciais riscos de colisão e classificando-os por nível de criticidade.

Evidências da Aplicação

Abaixo estão algumas capturas de tela da interface Swagger demonstrando os principais endpoints e respostas da API em execução.

Cadastro de Satélites

<img width="1353" height="631" alt="image" src="https://github.com/user-attachments/assets/eae6d073-488f-41fc-8a19-90d74725f9d3" />

Cadastro de Detritos Espaciais

<img width="1351" height="686" alt="image" src="https://github.com/user-attachments/assets/673b6ef4-d47b-405e-8b94-06c8037b8056" />


Relatório de Ameaças Orbitais

<img width="1315" height="810" alt="image" src="https://github.com/user-attachments/assets/1be7db94-1630-4137-96ca-50ae5c611e2f" />


Consulta Geral de Objetos Monitorados

<img width="1331" height="834" alt="image" src="https://github.com/user-attachments/assets/37c6ce13-bdae-4c9f-8b88-43b5a30d7d64" />
