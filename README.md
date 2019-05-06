# FormulaSim
Project to practice C# skill, purpose is to have a nice formula 1 simulator.

# Current progress

Currently at the "Race" part of the simulator. The idea is that you see one table when opening the Race view which is sorted according to
the qualifying that is connected with that race. There are supposed to be buttons to progress the race. It goes from
View with startgrid order -> stint 1, table gets updated according to the calls that have been made -> pitstops, the current progress
is caught and pitstops are added and the result is shown again in the table -> stint 2, also gets the current progress and then does
the same as stint 1 -> Finish race, returns to the RaceWeekend overview and then shows all the current results from that weekend, so
the qualifying results and race results -> Finish RaceWeekend, raceresults will be saved to the standings and such.

# Ideas

- In qualifying it also should be shown which drivers have reached the next Qualififying round through colouring the cell. Perhaps make the people who didnt went through red.
- The current way where the races will be setup is just the base of how it used to work in Excel, since it is now an application changes could be made to make it work a bit more "logically", like chassis and driver skill have an impact each stint instead of only once.
- Also very neat would be that graphs about the season can be shown, say how drivers progress over the season according to their points, maybe also the ability to filter it to an inputted amount of drivers.
- More?

# Finished parts

- Lists with Drivers, Teams, Tracks and Engines in the current F1 database. New items can be added there.
- Creating a season, here you can add everything that is going to be part of the season. Which tracks are going to be driven on, which
drivers are going to participate for which teams who use which engines. And so on.
- Qualifying functionality works now, going from Q1 to Q3 works as it should and the results of that are for now saved in a table without
relations; Qualifications.
- A whole lot more! Inquire for the me if you want to have an insight in our Trello board!

# User stories (in Dutch)
Als gebruiker wil ik coureurs, teams, motoren en circuits kunnen toevoegen zodat ze gebruikt kunnen worden voor een raceseizoen.
- Coureur bezit naam, nummer en of hij actief is.
- Team bezit naam en of deze voor seizoen actief is.
- Motor bezit naam en of deze voor seizoen in gebruik is.
- Circuit bezit naam, plaats en details (DNF, RNG en Spec.)

Als gebruiker wil ik een overzicht kunnen zien van huidige coureurs, teams, motoren en circuits en deze kunnen bewerken zodat ik er een overzicht van heb.
- Van de coureurs, teams en motoren moet ook de huidige-seizoensspecifieke details gezien kunnen worden. (vaardigheid, chassis en kracht)
- Deze seizoensspecifieke details zijn niet verplicht totdat er een seizoen gestart wordt, mits afwezig wordt dit zelf toegevoegd of ge-RNG't.

Als gebruiker wil ik een seizoen aanmaken zodat ik daarin races kan doen.
- Wordt bijgehouden hoeveelste seizoen het is.
- Actieve deelnemers worden toegevoegd aan seizoen.
- Aantal races kan bepaald worden, en welke circuits eraan toegevoegd worden.
- Laatste waarden van teams, coureurs en motoren uit vorige seizoen worden meegenomen en als startpunt opgeslagen.

Als gebruiker wil ik een raceweekend kunnen beginnen zodat ik daarmee raceweekend kan simmen.
- Coureurs moeten gekoppeld zijn aan een team en teams moeten een motor hebben.
- De gebruikte track moet degene zijn die in het seizoen aan de beurt is.
- De raceweekend wordt opgeslagen in het seizoen.

Als gebruiker wil ik een kwalificatie starten in een raceweekend zodat de gridpositie voor de race bepaald wordt.
- De raceweekend moet aangemaakt zijn, dus het is ook bekend wie waar rijdt.
- De specificatie van het circuit moet een invloed hebben op de specificatie van de chassis.
- Rijstijl van coureur heeft effect op snelheid.
- Het resultaat voor elke coureur moet worden opgeslagen.

Als gebruiker wil ik een race starten in een raceweekend zodat daar een resultaat uit komt.
- Kwalificatie moet al geweest zijn.
- Startgrid van de coureur moet op basis zijn van de bijbehorende kwalificatie.
- Als er penalties waren in een vorige raceweekend dan moeten deze toegepast worden. (?)
- Invloeden van track hebben ook weer betrekking omtrent de coureur en teamwaarden.
- Rijstijl van coureur heeft invloed op snelheid, maar ook op DNF-kans.
- De finishpositie voor elke coureur moet worden opgeslagen, daarbij ook of het een finish is of een DNF. Wanneer DNF dan ook wat voor soort. (Remmen, Crash, DSQ, etc.)

Als gebruiker wil ik de resultaten van een raceweekend automatisch verwerkt willen zien in de stand zodat deze blijft kloppen met de raceresultaten van het seizoen.
- Dit moet per seizoen gedaan worden, een nieuw seizoen is nieuwe stand maar oude resultaten blijven wel opgeslagen.
- Punten op basis van finishpositie moeten worden opgeslagen.
- Er moet ook gekeken worden of het een finish was of een DNF.
- Stand wordt voor zowel coureur als team apart berekend.
- Alleen voor coureur is er een tabel nodig met de finishpositie of DNF per raceweekend, hieruit is ook op te merken in welke plaats de race was en wat het totaal aantal punten van de coureur is.
- DNFs bepalen of er iets veranderd wordt aan de onderdelen toekenning van een coureur.(?)
- Voor zowel team als coureur moet gekeken worden hoeveel posities ze gestegen of gezakt zijn ten opzichte van de vorige raceweekend.

Als gebruiker wil ik de toekenning van onderdelen per coureur zien voor het desbetreffende seizoen zodat er daar een overzicht van is. (Toekenning is niet belangrijk nog, kan zonder.)
- Toekenning wordt automatisch geüpdatet aan de hand van de type DNFs per coureur per seizoen.
- Als een coureur over de maximum heen gaat wordt er een penalty toegekend, de grootte ervan op basis van de overtreding.

Als gebruiker wil ik deelnemers kunnen ontwikkelen in het seizoen zodat ze beter of slechter worden.
- Dit geldt voor zowel coureurs, teams als motoren.
- Het moet apart gedaan kunnen worden per onderwerp.
- Ontwikkeling verloopt willekeurig door zelf aangegeven parameters. (i.v.m. off-season ontwikkeling en eventuele aanpassingen)
- Wanneer ontwikkeling gedaan is moet dit opgeslagen worden bij de seizoensspecifieke stats van de coureurs, deelnemers en motoren.
- Alleen deelnemende coureurs mogen officieel ontwikkelen.

Als gebruiker wil ik de stats van coureurs, teams en motoren kunnen zien zodat er een logboek bijgehouden wordt van diens resultaten.
- Voor zowel coureurs, teams als motoren moet apart gezien kunnen worden hoeveel poles, winsten, podia, DNFs en dergelijke behaald zijn. 
- Ook seizoensresultaten uit het verleden moeten zichtbaar kunnen zijn als deze eraan deelgenomen heeft.
- Dit moet ook per seizoen zichtbaar zijn van de deelnemende coureurs. Denk ook aan qualificatie battles tussen teamgenoten onderling.
