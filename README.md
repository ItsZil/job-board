# 1. Sprendžiamo uždavinio aprašymas

## 1.1. Sistemos paskirtis 
Sistema skirta supaprastinti darbo paieškos bei darbuotojų samdymo procesą, veikiant kaip tarpininkas tarp darbo ieškotojų ir darbdavių.

Sistema bus sudaryta iš dvejų dalių. Pirmoji dalis yra internetinė aplikacija, kuria naudosis visi sistemos naudotojai. Antra sistemos dalis yra aplikacijų programavimo sąsaja (angl. trump. API).

Internetinėje aplikacijoje bus galima peržiūrėti visus tuo metu galiojančius darbo skelbimus, kuriuos įkelia prie sistemos prisiregistravę darbdaviai. Įkeliant naują darbo skelbimą, būtina pateikti jo aprašymą, galiojimo laiką bei sulaukti administratoriaus patvirtinimo. Darbo ieškotojui norint išsiųsti gyvenimo aprašymą ir pretenduoti į poziciją, jam būtina prisiregistruoti prie sistemos.
## 1.2. Funkciniai reikalavimai
Svečias sistemoje galės:
1. Peržiūrėti galiojančius darbo skelbimus;
2. Registruotis arba prisijungti prie sistemos kaip darbo ieškotojas arba darbdavys.
   
Registruotas darbo ieškotojas galės:
1. Atsijungti nuo sistemos;
2. Kurti arba redaguoti savo asmeninį profilį, pateikiant informaciją apie savo įgūdžius, darbo patirtį, išsilavinimą;
3. Į sistemą įkelti savo gyvenimo aprašymą;
4. Pretenduoti į galiojančias darbo pozicijas:
1. Pretenduojant įkelti gyvenimo aprašymą.
2. Pretenduojant išsiųsti motyvacinį laišką;

Darbdavys galės:
1. Atsijungti nuo sistemos;
2. Sukurti darbo skelbimą:
<br>2.1. Įkelti įmonės logotipą;
<br>2.2. Pridėti darbo aprašymą;
<br>2.3. Pridėti atlyginimo rėžius;
<br>2.4. Pridėti galiojimo laiką.
4. Paskelbti sukurtą darbo skelbimą;
5. Paslėpti sukurtą darbo skelbimą;
6. Redaguoti sukurtą darbo skelbimą;
7. Peržiūrėti aplikanto sąrašą;
8. Peržiūrėti aplikanto asmeninį profilį;
9. Peržiūrėti aplikanto motyvacinį laišką;
10. Atsisiųsti aplikanto gyvenimo aprašymą;

# 2. Sistemos architektūra

Sistema sudaryta iš dvejų dalių:
- Kliento pusė (angl. front-end) – naudojant React.js;
- Serverio pusė (angl. back-end) – naudojant C#, duomenu bazė – MySQL, ORM – Entity Framework.
  
Žemiau pateiktame paveikslėlyje pavaizduota sistemos diegimo diagrama. Naudotojas, pasileidęs naršyklę, galės HTTP protokolu pasiekti sistemą talpinamą Azure serveryje. Sistemos veikimui naudojamas API, kuris vykdo duomenų mainus su duomenų bazę per ORM.

![Diegimo diagrama](https://github.com/ItsZil/job-board/assets/22817405/eec7db9d-ac67-4dec-a6a2-7edaabde4ddc)

# 2. Sistemos implementacija

# 2.1 Sistemos struktūra

Serverio pusė sudaryta iš šių kontrolerių:
- AdController - atsakingas už darbo skelbimų CRUD ir kitas operacijas;
- CandidateController - atsakingas už darbo ieškotojų CRUD ir kitas operacijas;
- EmployerController - atsakingas už darbdavių CRUD ir kitas operacijas;

Autorizacijai ir autentifikacijai naudojama JWT (JSON Web Token) technologija. Kliento pusėje saugomi duomenys apie prisijungusį naudotoją, kurie naudojami atliekant užklausas į API.
Darbo ieškotojas ir darbdavys turi skirtingą rolę, kuri saugojama JWT žetone. Ne visos sistemos operacijos yra leidžiamos darbo ieškotojui ir darbdaviui. 
Pavyzdžiui, darbdavys negali pretenduoti į darbo skelbimus, o darbo ieškotojas negali kurti darbo skelbimų.