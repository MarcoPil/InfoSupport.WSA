#Continuous Delivery in Large Complex Software Systems
.NET minor 2017 - een samenwerking van Hogeschool Utrecht en Info Support B.V.
> Het bouwen van grote complexe software systemen (denk aan de administratie van een verzekeringsmaatschappij of de website en backoffice van een grote webwinkel) geeft veel uitdagingen. Hoe ontwikkelen we binnen tijd en budget, terwijl we het geheel niet goed kunnen overzien? Hoe zorgen we er voor dat er fouten in de code komen? En misschien wel de belangrijkste uitdaging: hoe zorgen we ervoor dat het systeem flexibel goenoeg is om zich aan te passen aan een veranderende wereld en veranderende klantwensen?

Het bouwen van grote complexe software systemen vraagt een specifieke aanpak. In deze minor ligt de focus op continuous delivery. Het idee daarbij is om in een kort-cyclisch proces steeds een klein stukje van het grote systeem van idee tot productie te brengen. Belangrijk hierbij is dat de meeste stappen in het proces (build, test, deploy) geautomatiseerd kunnen worden.
In deze minor leer je hoe je zo'n proces kunt realiseren. We kijken daarbij naar alle aspecten. Je leert welke procesmethodieken en welke architecturen zich hier voor lenen. Je krijgt diepgaande kennis van de frameworks en tools die je daarbij kunnen ondersteunen. Je wordt vaardig in Test Driven Development en leert de deployment te automatiseren.

### Overzicht
* Proces
  * Scrum: agile, iteratief, incrementeel
  * Reduce waste (YAGNI): bouw alleen het hoogst noodzakelijke
* Architectuur
  * Service Oriented Architecture
  * Web Scale Architecture
    * Microservices
    * Event Driven Architecture
    * Eventual Consistency
    * CQRS
    * Domain driven design
    * Event sourcing
* Van idee naar specificatie
  * Story Mapping
  * Goede User Stories schrijven
  * Specification by Example
  * Event Storming
* Van specificatie naar implementatie
  * Test Driven Development
  * C#
  * .NET Core
  * RabbitMQ
  * SQL Server
  * Angular2
* Van implementatie naar productie
  * Docker
  * OTAP
* Tools
  * Visual Studio
  * GIT
  * Nuget
  * TFS
  * TFS Build Automation
  * Docker
* Principes
  * Shu Ha Ri
  * Practice what you preach
  * Fouten maken mag (en er van leren ook)

## Opbouw
De minor is opgebouwd uit drie blokken. Daarin gaan we incrementeel en iteratief te werk.

In **blok 1** leer je al om een (in potentie) grote applicatie van voorkant tot achterkant te bouwen. Je krijgt diepgaande kennis van C# en het .NET framework en krijgt basiskennis van de andere onderdelen die je nodig hebt.

In **blok 2** gaan we die basiskennis verdiepen en hebben we ook veel aandacht voor de architectuur en het teamwerk. Aan het eind van dit blok, heb je alle kennis om grote complexe systemen in een continuous delivery proces te realiseren.

In **blok 3** zetten we de puntje op de i en verhogen we je vaardigheid in continuous delivery en hebben we aandacht voor geadvanceerde onderwerpen en automatisering van het proces. Aan het eind van dit blok ben je in staat om met een serieus development team een _Continuous Delivery_ process op te zetten en daarmee een groot complex software systeem te ontwikkelen.

Elk blok wordt afgeloten met een toets en een case.

##Frameworks & Technieken
* Architectuur & Ontwerp
  * Web Scale Architecture
  * Onion Architecture
  * Dependency Injection
* Specificaties schrijven
  * Gherkin
  * Specflow
* Test Driven Development
  * mstest / xunit
  * MoQ
* .NET Development
  * C#
  * LINQ
  * .NET core
  * ASP.NET Core
    * MVC
    * Web API
      * Swagger / Swashbuckle
  * Entity Framework Core
  * Logging
* Deployment
  * dotnet publish
  * docker
  * docker-compose
