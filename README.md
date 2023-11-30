# 1. Sprendžiamo uždavinio aprašymas

## 1.1. Sistemos paskirtis 
Sistema skirta supaprastinti darbo paieškos bei darbuotojų samdymo procesą, veikiant kaip tarpininkas tarp darbo ieškotojų ir darbdavių.

Sistema bus sudaryta iš dvejų dalių. Pirmoji dalis yra internetinė aplikacija, kuria naudosis visi sistemos naudotojai. Antra sistemos dalis yra aplikacijų programavimo sąsaja (angl. trump. API).

Internetinėje aplikacijoje bus galima peržiūrėti visus tuo metu esančius darbo skelbimus, kuriuos įkelia prie sistemos prisiregistravę darbdaviai. Įkeliant naują darbo skelbimą, būtina pateikti pavadinimą, aprašymą, atlyginimo rėžius, bei vietovę. Darbo ieškotojui norint pretenduoti į poziciją, jam būtina prisijungti prie sistemos.
## 1.2. Funkciniai reikalavimai
Svečias sistemoje galės:
1. Peržiūrėti darbo skelbimus;
2. Peržiūrėti įmones;
3. Filtruoti įmones arba darbo skelbimus pagal galimus kriterijus;
4. Prisijungti prie sistemos kaip darbo ieškotojas arba darbdavys.
   
Registruotas darbo ieškotojas galės:
1. Viską, ką svečias;
2. Atsijungti nuo sistemos;
3. Pretenduoti į darbo pozicijas;
4. Pretenduojant išsiųsti motyvacinį laišką;
5. Pamatyti savo kandidatūrų sąrašą;
6. Ištrinti savo kandidatūrą iš darbo skelbimo.

Darbdavys galės:
1. Viską, ką svečias;
2. Atsijungti nuo sistemos;
3. Sukurti darbo skelbimą:
4. Ištrinti sukurtą darbo skelbimą;
5. Redaguoti sukurtą darbo skelbimą;
7. Peržiūrėti kandidatūrų sąrašą;
8. Peržiūrėti kandidatūrą;

# 2. Sistemos architektūra

Sistema sudaryta iš dvejų dalių:
- Kliento pusė (angl. front-end) – naudojant Blazor;
- Serverio pusė (angl. back-end) – naudojant C#, duomenu bazė – Azure SQL Server, ORM – Entity Framework.
  
Žemiau pateiktame paveikslėlyje pavaizduota sistemos diegimo diagrama. Naudotojas, pasileidęs naršyklę, galės HTTPS protokolu pasiekti sistemą talpinamą Azure serveryje. Sistemos veikimui naudojamas API, kuris vykdo duomenų mainus su duomenų bazę per ORM.

![Diegimo diagrama](https://i.imgur.com/hFYG8yN.png)

# 2. Sistemos implementacija

# 2.1 Sistemos struktūra

Serverio pusė sudaryta iš šių kontrolerių:
- AdController - atsakingas už darbo skelbimų CRUD ir kitas operacijas;
- CompanyController - atsakingas už darbdavių CRUD ir kitas operacijas;
- ApplicationController - atsakingas už kandidatūrų CRUD ir kitas operacijas;

Autorizacijai ir autentifikacijai naudojama JWT (JSON Web Token) technologija. Kliento pusėje saugomi duomenys apie prisijungusį naudotoją, kurie naudojami atliekant užklausas į API.
Darbo ieškotojas ir darbdavys turi skirtingą rolę, kuri saugojama JWT žetone. Ne visos sistemos operacijos yra leidžiamos darbo ieškotojui ir darbdaviui. 
Pavyzdžiui, darbdavys negali pretenduoti į darbo skelbimus, o darbo ieškotojas negali kurti darbo skelbimų.

# 3. Naudotojo sąsajos projektas

## 3.1 Prisijungimas

![Prisijungimas](https://i.imgur.com/Gnzy2AY.png)

![Prisijungimas](https://i.imgur.com/CB2dZze.png)

## 3.2. Pradinis puslapis (svečio)

![Pradinis puslapis (svečio)](https://i.imgur.com/WIUbAMY.png)

![Pradinis puslapis (svečio)](https://i.imgur.com/fNJecnU.png)

## 3.3. Pradinis puslapis (darbo ieškotojo)

![Pradinis puslapis (darbo ieškotojo)](https://i.imgur.com/gvBAhTH.png)

![Pradinis puslapis (darbo ieškotojo)](https://i.imgur.com/TC3AeXp.png)

## 3.4. Pradinis puslapis (darbdavio)

![Pradinis puslapis (darbdavio)](https://i.imgur.com/4YlwEFJ.png)

![Pradinis puslapis (darbdavio)](https://i.imgur.com/rJiM1cd.png)

## 3.5. Darbo skelbimų sąrašas

![Darbo skelbimų sąrašas](https://i.imgur.com/vvz9yGV.png)

![Darbo skelbimų sąrašas](https://i.imgur.com/EQX1Iq3.png)

## 3.6. Darbo skelbimo peržiūra

![Darbo skelbimo peržiūra](https://i.imgur.com/vAH2qmI.png)

![Darbo skelbimo peržiūra](https://i.imgur.com/YeLTUe1.png)

## 3.7. Pretendavimas į darbo skelbimą

![Pretendavimas į darbo skelbimą](https://i.imgur.com/5kdYW1h.png)

![Pretendavimas į darbo skelbimą](https://i.imgur.com/n93FulR.png)

## 3.8. Darbo skelbimo peržiūra (savininkas)

![Darbo skelbimo peržiūra (savininkas)](https://i.imgur.com/M2hkdts.png)

![Darbo skelbimo peržiūra (savininkas)](https://i.imgur.com/eQ8OqIz.png)

## 3.9. Darbo skelbimo redagavimas

![Darbo skelbimo redagavimas](https://i.imgur.com/2FID0Rz.png)

![Darbo skelbimo redagavimas](https://i.imgur.com/eQ8OqIz.png)

## 3.10. Įmonių peržiūra

![Įmonių peržiūra](https://i.imgur.com/TCvNQRB.png)

![Įmonių peržiūra](https://i.imgur.com/unFnthJ.png)

## 3.11. Įmonės peržiūra

![Įmonės peržiūra](https://i.imgur.com/vo4pXZO.png)

![Įmonės peržiūra](https://i.imgur.com/vMDGhUg.png)

## 3.12. Įmonės peržiūra (savininkas)

![Įmonės peržiūra (savininkas)](https://i.imgur.com/eDVI4Y6.png)

![Įmonės peržiūra (savininkas)](https://i.imgur.com/Dc7BkB4.png)

## 3.13. Įmonės redagavimas

![Įmonės redagavimas](https://i.imgur.com/RfLqCZt.png)

![Įmonės redagavimas](https://i.imgur.com/lz83tEB.png)

## 3.14. Kandidatūrų sąrašas (darbo ieškotojas)

![Kandidatūrų sąrašas (darbo ieškotojas)](https://i.imgur.com/KDkxxxf.png)

![Kandidatūrų sąrašas (darbo ieškotojas)](https://i.imgur.com/LfO01rW.png)

## 3.15. Kandidatūros peržiūra (darbo ieškotojas)

![Kandidatūros peržiūra (darbo ieškotojas)](https://i.imgur.com/lBk0rAF.png)

![Kandidatūros peržiūra (darbo ieškotojas)](https://i.imgur.com/l93Cllu.png)

## 3.16. Kandidatūros redagavimas

![Kandidatūros redagavimas](https://i.imgur.com/V2LLKzz.png)

![Kandidatūros redagavimas](https://i.imgur.com/YT8HPQM.png)

## 3.17. Kandidatūrų sąrašas (darbdavys)

![Kandidatūrų sąrašas (darbdavys)](https://i.imgur.com/H7uVZ1Q.png)

![Kandidatūrų sąrašas (darbdavys)](https://i.imgur.com/fGqdsFB.png)

## 3.18. Kandidatūros peržiūra (darbdavys)

![Kandidatūros peržiūra (darbdavys)](https://i.imgur.com/XcyhHeD.png)

![Kandidatūros peržiūra (darbdavys)](https://i.imgur.com/yo9uUVv.png)