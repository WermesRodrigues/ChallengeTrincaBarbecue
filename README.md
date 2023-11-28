Olá Sou Wermes 

Esse e meu desafio da Trinca, abaixo algumas melhorias que poderiam ser feitas 


Melhorias Arquitetura 

	->Criar camada de Services para retirar o codigo das functions...
	->Persistir Erros -> Algumas rotas nao estavam persistindo erro e nao estao criando log de exception (da para tratar exception e ja criar bugs no DevOps com Input, OutPut etc...)
	->Key Vaults Azure -> Para guardar connection string
	->Enviroment variable -> Para reconhecer ambiente de publicacao automatico (DEV, QA, STAGE, PROD)
	->Azure functions observability -> Tracking and Monitor.
	->Kubernetes e Docker -> Utilizar Docker e Kubernetes para alta disponibilidade e scalabilidade.
	->Microservices -> Isolar algumas APis com maior demanda 
	->Messageria -> Utilizar Rabbit MQ ou Event Bus Azure 
	->Unit Tests -> Criar Unit Test
	->MongoDb -> O Mongo tem mais performance na leitura, talvez em algumas aplicacoes compensa usar mongo DB.


Melhorias Features:

	-> Incluir Rateio caso a Trinca nao pague o Churras.
	-> Criar notificacoes e reminders para avisar pessoas que estao sendo convidadas em um determinado periodo antes da data prevista.
	-> Criar api de pagamento para o Rateio, conforme as pessoas vao aceitando poderia fazer o Pix antes do dia para comprar as comidas.
	-> Criar notificacoes e reminders para o pagamento de Rateio.
