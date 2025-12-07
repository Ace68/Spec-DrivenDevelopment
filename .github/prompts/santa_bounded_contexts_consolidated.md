# 🎄 Santa Claus Work Management -- Bounded Contexts (Versione Consolidata)

Questa versione aggiornata raggruppa i Bounded Context precedentemente
identificati in tre macro‐contesti principali, più coerenti con
l'implementazione API.

------------------------------------------------------------------------

# 🧩 Macro Bounded Context

## 1. 🎁 **Marketing**

Raggruppa i processi relativi alla raccolta, interpretazione e gestione
delle richieste dei bambini, incluse comunicazioni esterne.

### Include:

-   **Letter Management**
-   **Children Registry**
-   **Wish Processing / Order Planning**
-   **Notification & Communication**

------------------------------------------------------------------------

## 2. 🏭 **Production**

Gestisce l'intero processo di creazione dei giocattoli: produzione e
controllo qualità.

### Include:

-   **Workshop / Toy Manufacturing**
-   **Quality Assurance**

------------------------------------------------------------------------

## 3. 🛷 **Delivery**

Comprende la logistica del Natale: pianificazione del viaggio, gestione
renne, consegna.

### Include:

-   **Logistics & Delivery**
-   **Reindeer Fleet Management**
-   **Mission Control**

------------------------------------------------------------------------

# 📌 Diagramma riassuntivo (Mermaid)

``` mermaid
flowchart LR

subgraph Marketing
  A[Letter Management]
  B[Children Registry]
  C[Wish Processing]
  D[Notification & Communication]
end

subgraph Production
  E[Toy Manufacturing]
  F[Quality Assurance]
end

subgraph Delivery
  G[Logistics & Delivery]
  H[Reindeer Fleet Management]
  I[Mission Control]
end

A --> C
B --> C
C --> E
E --> F
F --> G
H --> G
G --> I
```
