# Introduzione e convenzioni
 
Il progetto fa riferimento ad uno sviluppo per gestire la preparazione dello spiedo bresciano.
 
# Istruzioni per Copilot
L'obiettivo è Garantire che il codice prodotto sia conforme alle richieste, professionale e di alta qualità
Se incontri ambiguità o mancanza di dettagli, chiedi chiarimenti prima di procedere.
 
Se invece ti chiedo di spiegarmi qualcosa, o ti sto facendo una domanda, valuta di non effettuare subito cambiamenti ma rispondere alle domande e chiarire i dubbi.
 
Quando stai applicando una modifica, evita di spiegare tutti i passaggi ma mantieni una risposta sintetica. Quando invece stai rispondendo a una domanda, includi dettagli, riferimenti e spiegazioni.
 
## Modifica di file esistenti
Se modifichi un file esistente, attieniti a quello che stai implementando. Ad esempio se stai aggiungendo un metodo, evita di rifattorizzare altre parti non legate a quel metodo.
Oppure se stai implementando una logica, evita di formattare altre porzioni di codice. Lavora insomma nel contesto di una modifica specifica, come se fosse il contenuto logico di un commit,
modifiche coerenti tra loro.
Se ritieni che alcune informazioni o modifiche aggiuntive siano rilevanti, non includerle direttamente nel codice, ma segnalale come suggerimenti o domande.
 
Mantieni gli import e `using` preesistenti a meno che non sia esplicitamente richiesto di rimuoverle.
Mantieni altre logiche a meno che non siano proprio oggetto della modifica o non sia esplicitamente richiesto di rimuoverle.
 
Se ti chiedo di modificare appsettings.json o configurazioni, per aggiungere dei dati, mantieni tutti i dati preesistenti e aggiungi quanto richiesto.
 
Se ti chiedo di estrarre una classe, un metodo, o una logica, dopo averla estratta, utilizzala al posto del codice originale e nei punti dove può essere riutilizzata
 
## Linee guida sulla scrittura di codice:
tieni un approccio professionale, aderendo agli standard come un software engineer esperto
1. Scrivi codice chiaro, leggibile e ben documentato.
2. Rispetta le convenzioni di codifica del linguaggio utilizzato.
3. Evita di introdurre complessità non necessarie.
4. Aggiungi commenti solo se necessari per spiegare scelte implementative.
5. Mantieni un approccio modulare e riutilizzabile, ove applicabile.
6. Il codice e i commenti nel codice devono essere in inglese, anche se le istruzioni sono in italiano.
 
### Commenti
I commenti vanno evitati, specialmente se ribadiscono quello che già fa il codice. I commenti servono solo per spiegare le scelte implementative o logiche complesse
 
Esempio di commento che va evitato
    salesOrder.Id = Guid.NewGuid(); // generate a new Guid
Esempio di commento che potrebbe avere senso
    salesOrder.Id = Guid.NewGuid(); // the backend is responsible of assigning unique ids