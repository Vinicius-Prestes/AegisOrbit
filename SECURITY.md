# Segurança — AegisOrbit

Este documento descreve a arquitetura de segurança, modelo de ameaças, conformidade normativa e plano de resposta a incidentes da plataforma AegisOrbit.

---

## 1. Análise de Riscos e Ameaças (Threat Modeling)

### 1.1 Identificação de Ativos Críticos

| Ativo | Classificação | Descrição |
|-------|--------------|-----------|
| Banco de dados Oracle (SpacialObjects) | Crítico | Catálogo completo de satélites operacionais e detritos rastreados. Alteração ou exclusão não autorizada pode provocar colisões reais não detectadas. |
| Endpoints REST da API | Alto | Interface pública de ingestão e consulta de dados orbitais. Ponto de entrada para qualquer agente externo. |
| String de conexão Oracle | Crítico | Credencial de acesso irrestrito ao banco de dados. Vazamento concede controle total sobre todos os dados orbitais. |
| Algoritmo de cálculo de colisão (`CollisionService`) | Alto | Lógica de negócio central. Manipulação dos parâmetros de entrada pode gerar falsos negativos (colisões críticas classificadas como LOW) ou falsos positivos, comprometendo decisões operacionais. |
| Dados de telemetria dos satélites | Alto | Posição, velocidade e frequência operacional de satélites ativos. Exposição pode comprometer missões sensíveis ou de defesa. |
| Logs e histórico de alertas (`CollisionAlertDTO`) | Médio | Registro de ameaças geradas. Adulteração pode apagar evidências de incidentes. |

---

### 1.2 Modelo de Ameaças — Vetores de Ataque

**Vetor 1 — Manipulação de Telemetria (Data Tampering)**

Um agente mal-intencionado com acesso à API pode registrar objetos com coordenadas ou velocidades falsas via `POST /api/orbital/satellite` ou `POST /api/orbital/debris`, corrompendo o banco de dados orbital. O algoritmo de cálculo de colisão processaria dados inválidos, gerando alertas incorretos ou ocultando riscos reais. Mitigação: autenticação obrigatória com perfil `Operator`, validação de limites físicos plausíveis (altitude entre 160 km e 2.000 km para LEO, velocidade entre 1 km/s e 11 km/s) e assinatura de payload.

**Vetor 2 — Interceptação de Dados em Trânsito (Man-in-the-Middle)**

A comunicação entre clientes e a API trafega sobre HTTP/HTTPS. Sem enforcement obrigatório de TLS 1.2+, uma interceptação de rede poderia expor coordenadas de satélites operacionais, incluindo possíveis ativos de defesa ou telecomunicações críticas. Mitigação: enforçar HTTPS com HSTS (HTTP Strict-Transport-Security), certificados TLS válidos e desabilitar o perfil HTTP no `launchSettings.json` em produção.

**Vetor 3 — Negação de Serviço / Abuso de Endpoints (DoS/DDoS)**

O endpoint `GET /api/orbital/threat-report` executa um cruzamento combinatório O(n²) de todos os objetos do banco. Um atacante que registre milhares de objetos via `POST` pode tornar este endpoint inutilizável por sobrecarga computacional, comprometendo o monitoramento em tempo real. Mitigação: rate limiting por IP/token, autenticação para endpoints de escrita e paginação ou limite máximo de objetos processados por requisição.

**Vetor 4 — Exposição de Credenciais (Credential Leakage)**

A string de conexão Oracle (`User Id=...;Password=...`) é armazenada em `appsettings.json`. Um commit acidental ou acesso indevido ao sistema de arquivos expõe credenciais com acesso irrestrito ao banco. Mitigação: uso de variáveis de ambiente ou gerenciadores de segredos (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault) em vez de arquivos de configuração.

**Vetor 5 — Injeção e Abuso de Parâmetros (Parameter Injection)**

Os endpoints de cadastro recebem parâmetros via query string sem validação de faixa. Valores como `altitude=0` ou `mass=-999` não são bloqueados na camada HTTP, alcançando o domínio e podendo provocar divisão por zero ou comportamentos imprevisíveis no cálculo de risco. Mitigação: Data Annotations e FluentValidation com rejeição de valores fora de limites orbitais físicos, retornando `400 Bad Request` antes de qualquer processamento.

---

## 2. Arquitetura de Segurança

### 2.1 Controles de Acesso

A arquitetura prevê autenticação baseada em **JWT Bearer Tokens** com perfis de acesso diferenciados:

| Perfil | Permissões |
|--------|------------|
| `ReadOnly` | `GET /api/orbital`, `GET /api/orbital/check-collision`, `GET /api/orbital/threat-report` |
| `Operator` | Todas as permissões `ReadOnly` + `POST /api/orbital/satellite` + `POST /api/orbital/debris` |
| `Admin` | Todas as permissões `Operator` + exclusão e edição de registros |

**Princípio do Privilégio Mínimo:** sistemas de monitoramento passivo (dashboards) recebem apenas o perfil `ReadOnly`. Somente sistemas de ingestão certificados recebem o perfil `Operator`.

**Autenticação Multifator (MFA):** para concessão de tokens `Admin`, o fluxo de autenticação deve exigir um segundo fator (TOTP ou hardware key) via um servidor de autorização (ex: Keycloak ou Azure AD B2C).

### 2.2 Proteção de Dados

**Em Trânsito:**
- TLS 1.2 como versão mínima obrigatória; TLS 1.3 preferencial.
- HSTS habilitado com `max-age=31536000; includeSubDomains`.
- Perfil HTTP desabilitado em ambiente de produção (somente HTTPS).
- Certificados gerenciados via Let's Encrypt ou autoridade interna de PKI.

**Em Repouso:**
- Credenciais de banco de dados armazenadas em gerenciador de segredos (Azure Key Vault recomendado), nunca em arquivos de configuração versionados.
- Campos sensíveis de telemetria (posição de satélites de defesa) criptografados com AES-256 antes da persistência no Oracle.
- Backups do banco criptografados com chave gerenciada pelo cliente (CMK).

**Anonimização:**
- Dados exportados para fins analíticos passam por processo de anonimização, removendo campos operador e frequência de satélites com classificação sensível.

### 2.3 Segurança da Infraestrutura

**Arquitetura Zero Trust:**
- Nenhum componente confia implicitamente em outro. Cada serviço deve apresentar credenciais válidas a cada requisição, mesmo dentro da rede interna.
- Segmentação de rede: API, banco de dados e serviços auxiliares em VLANs isoladas com regras de firewall explícitas.

**Monitoramento e Logs:**
- Todas as requisições às rotas de escrita (`POST`) são registradas com: timestamp UTC, IP de origem, token de autenticação (hash), payload resumido e resultado.
- Alertas automáticos para: falhas de autenticação consecutivas (brute-force), volume anômalo de registros por IP, erros `CollisionCalculationException` repetidos (possível injeção de dados inválidos).
- Stack recomendada: Azure Monitor + Application Insights ou Elastic Stack (ELK).

**Hardening da API:**
- Cabeçalhos de segurança HTTP: `X-Content-Type-Options`, `X-Frame-Options`, `Content-Security-Policy`.
- Remoção de cabeçalhos de versão do servidor (`Server`, `X-Powered-By`).
- Rate limiting: máximo de 100 requisições por minuto por token; 10 requisições por minuto por IP não autenticado.

---

## 3. Governança e Compliance

### 3.1 Alinhamento com ISO 27001

A ISO 27001 define um Sistema de Gestão de Segurança da Informação (SGSI) baseado em ciclos de melhoria contínua. O AegisOrbit adota os seguintes controles do Anexo A:

| Controle ISO 27001 | Implementação no AegisOrbit |
|--------------------|----------------------------|
| **A.8 — Gestão de Ativos** | Inventário de ativos críticos definido na Seção 1.1 deste documento; classificação por criticidade. |
| **A.9 — Controle de Acesso** | Perfis `ReadOnly`, `Operator`, `Admin` com JWT; MFA para acesso administrativo. |
| **A.10 — Criptografia** | TLS 1.2+ para dados em trânsito; AES-256 para campos sensíveis em repouso; CMK para backups. |
| **A.12 — Segurança Operacional** | Logs de auditoria de todas as operações de escrita; alertas automáticos para comportamento anômalo. |
| **A.14 — Segurança no Desenvolvimento** | Validação de entrada na camada HTTP; exceções customizadas; injeção de dependência para testabilidade. |
| **A.16 — Gestão de Incidentes** | Plano de resposta a incidentes descrito na Seção 4. |
| **A.18 — Conformidade** | Alinhamento com LGPD conforme Seção 3.2; revisões de conformidade semestrais. |

**Gestão de Riscos:** riscos identificados no Threat Model (Seção 1.2) são avaliados por impacto e probabilidade, priorizados e reavaliados a cada ciclo trimestral de revisão de segurança.

### 3.2 Privacidade e LGPD

A Lei Geral de Proteção de Dados (Lei 13.709/2018) se aplica ao AegisOrbit nos cenários em que dados de operadores de satélites ou coordenadas georreferenciadas de infraestrutura crítica possam ser associados a pessoas físicas ou jurídicas identificáveis.

**Base Legal de Tratamento:** o tratamento de dados de telemetria é justificado pelo legítimo interesse na prevenção de colisões orbitais e proteção de infraestrutura crítica (Art. 7º, IX, LGPD).

**Medidas adotadas:**

- **Minimização de dados:** apenas os campos estritamente necessários ao cálculo de risco orbital são coletados e armazenados.
- **Transparência:** operadores de satélites são informados, no momento do cadastro, sobre quais dados são coletados e para qual finalidade.
- **Direito ao esquecimento:** registros de satélites desativados podem ser anonimizados ou excluídos mediante solicitação formal do operador, desde que não haja restrição regulatória de retenção.
- **Encarregado de Dados (DPO):** para fins de contato com a ANPD, o projeto designa um responsável pelo tratamento de dados com canal de comunicação disponível.
- **Notificação de Incidentes:** em caso de vazamento de dados que envolva informações de operadores, a ANPD e os titulares afetados são notificados em até 72 horas, conforme Art. 48 da LGPD.

---

## 4. Plano de Resiliência e Continuidade

### 4.1 Plano de Resposta a Incidentes

O plano segue as fases NIST SP 800-61: **Preparação → Detecção → Contenção → Erradicação → Recuperação → Lições Aprendidas**.

#### Fase 1 — Preparação

- Equipe de resposta definida com responsáveis por: análise forense, comunicação externa, recuperação de banco de dados e validação de integridade orbital.
- Runbooks documentados para os cenários de incidente mais prováveis (vazamento de credenciais, injeção de dados, DDoS).
- Backups do banco Oracle realizados a cada 6 horas, retidos por 30 dias, armazenados em região geográfica separada.
- Ambiente de staging idêntico ao produção disponível para rollback imediato.

#### Fase 2 — Detecção e Análise

| Indicador de Comprometimento (IoC) | Ação Imediata |
|------------------------------------|---------------|
| Mais de 50 falhas de autenticação em 5 minutos (mesmo IP) | Alerta automático + bloqueio temporário do IP |
| `CollisionCalculationException` para > 30% dos pares em 1 hora | Alerta de possível injeção de dados inválidos em massa |
| Crescimento anômalo da tabela `SpacialObjects` (> 10× média diária) | Alerta de possível abuso de endpoint de cadastro |
| Acesso ao banco fora do horário operacional sem justificativa | Alerta crítico + notificação ao time de segurança |

#### Fase 3 — Contenção

**Curto Prazo (0–4 horas):**
1. Isolar o componente comprometido: se a API estiver sob ataque, ativar modo de manutenção e redirecionar tráfego para réplica de somente leitura.
2. Revogar todos os tokens JWT ativos e forçar nova autenticação.
3. Bloquear IPs ou tokens identificados como maliciosos no WAF (Web Application Firewall).
4. Preservar logs e snapshots do banco para análise forense — **não apagar evidências**.

**Longo Prazo (4–72 horas):**
5. Rotacionar credenciais do banco de dados Oracle e atualizar segredos no Key Vault.
6. Verificar integridade dos dados orbitais comparando com última snapshot válida.

#### Fase 4 — Erradicação

7. Identificar e corrigir a vulnerabilidade explorada (patch de código, configuração ou credencial).
8. Auditar todos os registros inseridos ou modificados durante o período de comprometimento.
9. Remover dados orbitais injetados ou corrompidos; restaurar a partir do backup mais recente íntegro se necessário.
10. Varredura de segurança completa (SAST + DAST) antes de reativar o ambiente.

#### Fase 5 — Recuperação

11. Reativar a API em modo controlado com monitoramento intensificado (janela de 48 horas).
12. Validar que todos os alertas de colisão gerados durante o período de incidente são fidedignos.
13. Comunicar operadores afetados sobre o incidente, dados potencialmente comprometidos e medidas tomadas (conforme LGPD, Art. 48).

#### Fase 6 — Lições Aprendidas

14. Relatório pós-incidente em até 5 dias úteis, documentando: linha do tempo, vetor de ataque, impacto, ações tomadas e melhorias implementadas.
15. Revisão e atualização deste plano de segurança com base nos aprendizados.
16. Treinamento da equipe se o incidente revelar lacuna de conhecimento ou processo.

---

*Documento mantido pela equipe AegisOrbit. Revisão mínima semestral ou após qualquer incidente de segurança.*
