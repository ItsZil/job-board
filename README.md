# Tūrinys

- [1. Sprendžiamo uždavinio aprašymas](#1-sprendžiamo-uždavinio-aprašymas)
  * [1.1. Sistemos paskirtis](#11-sistemos-paskirtis)
  * [1.2. Funkciniai reikalavimai](#12-funkciniai-reikalavimai)
- [2. Sistemos architektūra](#2-sistemos-architektūra)
- [3. Sistemos implementacija](#3-sistemos-implementacija)
- [3.1 Sistemos struktūra](#31-sistemos-struktūra)
- [4. Naudotojo sąsajos projektas](#4-naudotojo-sąsajos-projektas)
- [5. API specifikacija](#5-api-specifikacija)
  * [5.1 CompanyController](#51-companycontroller)
  * [5.2 AdController](#52-adcontroller)
  * [5.3 ApplicationController](#53-applicationcontroller)
- [6. Išvados](#6-išvados)

&nbsp;

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

![Diegimo diagrama](https://i.imgur.com/REsw9iY.png)

# 3. Sistemos implementacija

# 3.1 Sistemos struktūra

Serverio pusė sudaryta iš šių kontrolerių:
- AdController - atsakingas už darbo skelbimų CRUD ir kitas operacijas;
- CompanyController - atsakingas už darbdavių CRUD ir kitas operacijas;
- ApplicationController - atsakingas už kandidatūrų CRUD ir kitas operacijas;

Autorizacijai ir autentifikacijai naudojama JWT (JSON Web Token) technologija. Kliento pusėje saugomi duomenys apie prisijungusį naudotoją, kurie naudojami atliekant užklausas į API.
Darbo ieškotojas ir darbdavys turi skirtingą rolę, kuri saugojama JWT žetone. Ne visos sistemos operacijos yra leidžiamos darbo ieškotojui ir darbdaviui. 
Pavyzdžiui, darbdavys negali pretenduoti į darbo skelbimus, o darbo ieškotojas negali kurti darbo skelbimų.

# 4. Naudotojo sąsajos projektas

## 4.1 Prisijungimas

![Prisijungimas](https://i.imgur.com/Gnzy2AY.png)

![Prisijungimas](https://i.imgur.com/CB2dZze.png)

## 4.2. Pradinis puslapis (svečio)

![Pradinis puslapis (svečio)](https://i.imgur.com/WIUbAMY.png)

![Pradinis puslapis (svečio)](https://i.imgur.com/fNJecnU.png)

## 4.3. Pradinis puslapis (darbo ieškotojo)

![Pradinis puslapis (darbo ieškotojo)](https://i.imgur.com/gvBAhTH.png)

![Pradinis puslapis (darbo ieškotojo)](https://i.imgur.com/TC3AeXp.png)

## 4.4. Pradinis puslapis (darbdavio)

![Pradinis puslapis (darbdavio)](https://i.imgur.com/4YlwEFJ.png)

![Pradinis puslapis (darbdavio)](https://i.imgur.com/rJiM1cd.png)

## 4.5. Darbo skelbimų sąrašas

![Darbo skelbimų sąrašas](https://i.imgur.com/vvz9yGV.png)

![Darbo skelbimų sąrašas](https://i.imgur.com/EQX1Iq3.png)

## 4.6. Darbo skelbimo peržiūra

![Darbo skelbimo peržiūra](https://i.imgur.com/vAH2qmI.png)

![Darbo skelbimo peržiūra](https://i.imgur.com/YeLTUe1.png)

## 4.7. Pretendavimas į darbo skelbimą

![Pretendavimas į darbo skelbimą](https://i.imgur.com/5kdYW1h.png)

![Pretendavimas į darbo skelbimą](https://i.imgur.com/n93FulR.png)

## 4.8. Darbo skelbimo peržiūra (savininkas)

![Darbo skelbimo peržiūra (savininkas)](https://i.imgur.com/M2hkdts.png)

![Darbo skelbimo peržiūra (savininkas)](https://i.imgur.com/eQ8OqIz.png)

## 4.9. Darbo skelbimo redagavimas

![Darbo skelbimo redagavimas](https://i.imgur.com/2FID0Rz.png)

![Darbo skelbimo redagavimas](https://i.imgur.com/eQ8OqIz.png)

## 4.10. Įmonių peržiūra

![Įmonių peržiūra](https://i.imgur.com/TCvNQRB.png)

![Įmonių peržiūra](https://i.imgur.com/unFnthJ.png)

## 4.11. Įmonės peržiūra

![Įmonės peržiūra](https://i.imgur.com/vo4pXZO.png)

![Įmonės peržiūra](https://i.imgur.com/vMDGhUg.png)

## 4.12. Įmonės peržiūra (savininkas)

![Įmonės peržiūra (savininkas)](https://i.imgur.com/eDVI4Y6.png)

![Įmonės peržiūra (savininkas)](https://i.imgur.com/Dc7BkB4.png)

## 4.13. Įmonės redagavimas

![Įmonės redagavimas](https://i.imgur.com/RfLqCZt.png)

![Įmonės redagavimas](https://i.imgur.com/lz83tEB.png)

## 4.14. Kandidatūrų sąrašas (darbo ieškotojas)

![Kandidatūrų sąrašas (darbo ieškotojas)](https://i.imgur.com/KDkxxxf.png)

![Kandidatūrų sąrašas (darbo ieškotojas)](https://i.imgur.com/LfO01rW.png)

## 4.15. Kandidatūros peržiūra (darbo ieškotojas)

![Kandidatūros peržiūra (darbo ieškotojas)](https://i.imgur.com/lBk0rAF.png)

![Kandidatūros peržiūra (darbo ieškotojas)](https://i.imgur.com/l93Cllu.png)

## 4.16. Kandidatūros redagavimas

![Kandidatūros redagavimas](https://i.imgur.com/V2LLKzz.png)

![Kandidatūros redagavimas](https://i.imgur.com/YT8HPQM.png)

## 4.17. Kandidatūrų sąrašas (darbdavys)

![Kandidatūrų sąrašas (darbdavys)](https://i.imgur.com/H7uVZ1Q.png)

![Kandidatūrų sąrašas (darbdavys)](https://i.imgur.com/fGqdsFB.png)

## 4.18. Kandidatūros peržiūra (darbdavys)

![Kandidatūros peržiūra (darbdavys)](https://i.imgur.com/XcyhHeD.png)

![Kandidatūros peržiūra (darbdavys)](https://i.imgur.com/yo9uUVv.png)

# 5. API specifikacija

## 5.1 CompanyController

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies  | GET | Ne | Nėra  | JSON | 200 OK |

Grąžina visų įmonių sąrašą. 

Užklausa: GET api/companies

Atsakymas (200 OK):
```
[
    {
        "id": 1,
        "email": "elandes0@mtv.com",
        "companyName": "Company1",
        "companyDescription": "string",
        "industry": "string",
        "website": "string"
    },
    {
        "id": 2,
        "email": "ifishbourn1@cornell.edu",
        "companyName": "Company2",
        "companyDescription": "Bladder sphincterotomy",
        "industry": "Oil & Gas Production",
        "website": "http://cbslocal.com/morbi.jsp"
    }
]
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId  | GET | Ne | Nėra  | JSON | 200 OK, 400 Bad Request, 404 Not Found |

Grąžina vieną įmonę, kurios ID yra companyId.

Užklausa: GET api/companies/1

Atsakymas (200 OK):
```
{
    "id": 6,
    "email": "wbuzin5@example.com",
    "companyName": "Company6",
    "companyDescription": "Remov imp dev-chest cage",
    "industry": "Publishing",
    "website": "https://creativecommons.org/sociis.png"
}
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-3fb7bcca95d71386ae51d80f3aa11f5e-c984564a6bdc0361-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas (404 Not Found):
```
Company not found.
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies  | POST | Taip (admin) | JSON  | JSON | 201 Created, 400 Bad Request, 401 Unauthorized, 403 Forbidden |

Sukuria naują įmonę, naudojant duomenims iš užklausos.

Užklausa: POST api/companies
```
{
  "companyName": "Test Creation Company",
  "email": "test@gmail.com",
  "industry": "Testing",
  "password": "test",
  "companyDescription": "Test"
}
```

Atsakymas (201 Created):
```
{
    "id": 6,
    "email": "wbuzin5@example.com",
    "companyName": "Company6",
    "companyDescription": "Remov imp dev-chest cage",
    "industry": "Publishing",
    "website": "https://creativecommons.org/sociis.png"
}
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-757141b60003e3944f61d9d82675b53b-e8df1408d658b403-00",
    "errors": {
        "Password": [
            "The Password field is required."
        ]
    }
}
```

Atsakymas: 401 Unauthorized

Atsakymas: 403 Forbidden

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId  | PUT | Taip (company, admin) | JSON  | JSON | 200 OK, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found |

Atnaujina įmonę, kurios ID yra companyId, o nauji duomenys yra pateikiami užklausoje.

Užklausa: POST api/companies
```
{
  "company": "New Company Name",
  "industry": "New Industry",
  "companyDescription": "New Descritpion"
}
```

Atsakymas (200 OK):
```
{
    "id": 17,
    "email": "test@gmail.com",
    "companyName": "New Company Name",
    "companyDescription": "New Descritpion",
    "industry": "New Industry",
    "website": null
}
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-9323dcb4ed38b255b7de19a448e0db0b-111d1914d90dc2ca-00",
    "errors": {
        "CompanyDescription": [
            "The CompanyDescription field is required."
        ]
    }
}
```

Atsakymas: 401 Unauthorized

Atsakymas: 403 Forbidden

Atsakymas (404 Not Found):
```
Company not found.
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId  | DELETE | Taip (company, admin) | Nėra  | JSON | 204 No Content, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found |

Ištrina įmonę, kurios ID yra companyId.

Užklausa: DELETE api/companies/17

Atsakymas: 204 No Content

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-0999768ce7d3d6552e100a802e9348d2-618f63fafe8b1273-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas: 401 Unauthorized

Atsakymas: 403 Forbidden

Atsakymas (404 Not Found):
```
Company not found.
```

## 5.2 AdController

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId/ads  | GET | Ne | Nėra  | JSON | 200 OK, 400 Bad Request, 404 Not Found |

Grąžina visų skelbimų, priklausančių įmonei, kurios ID yra companyId, sąrašą.

Užklausa: GET api/companies/1/ads

Atsakymas (200 OK):
```
[
    {
        "id": 11,
        "title": "Civil Engineer",
        "description": "Morbi vel lectus in quam fringilla rhoncus. Mauris enim leo, rhoncus sed, vestibulum sit amet, cursus id, turpis. Integer aliquet, massa id lobortis convallis, tortor risus dapibus augue, vel accumsan tellus nisi eu orci. Mauris lacinia sapien quis libero. Nullam sit amet turpis elementum ligula vehicula consequat. Morbi a ipsum. Integer a nibh. In quis justo. Maecenas rhoncus aliquam lacus. Morbi quis tortor id nulla ultrices aliquet. Maecenas leo odio, condimentum id, luctus nec, molestie sed, justo. Pellentesque viverra pede ac diam.",
        "salaryFrom": "1981",
        "salaryTo": "3167",
        "location": "Tangzhai",
        "postedDate": "2023-07-17T00:00:00",
        "company": {
            "id": 1,
            "email": "elandes0@mtv.com",
            "companyName": "Company1",
            "companyDescription": "string",
            "industry": "string",
            "website": "string"
        }
    }
]
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-7a62ac8a6c25afeb9ecd58378624580a-86a3745a8a6d650b-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas (404 Not Found):
```
Company not found.
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId/ads/adId  | GET | Ne | Nėra  | JSON | 200 OK, 400 Bad Request, 404 Not Found |

Grąžina vieno skelbimo, kurio ID yra adId, priklausančio įmonei su ID companyId, duomenis.

Užklausa: GET api/companies/1/ads/11

Atsakymas (200 OK):
```
[
    {
        "id": 11,
        "title": "Civil Engineer",
        "description": "Morbi vel lectus in quam fringilla rhoncus. Mauris enim leo, rhoncus sed, vestibulum sit amet, cursus id, turpis. Integer aliquet, massa id lobortis convallis, tortor risus dapibus augue, vel accumsan tellus nisi eu orci. Mauris lacinia sapien quis libero. Nullam sit amet turpis elementum ligula vehicula consequat. Morbi a ipsum. Integer a nibh. In quis justo. Maecenas rhoncus aliquam lacus. Morbi quis tortor id nulla ultrices aliquet. Maecenas leo odio, condimentum id, luctus nec, molestie sed, justo. Pellentesque viverra pede ac diam.",
        "salaryFrom": "1981",
        "salaryTo": "3167",
        "location": "Tangzhai",
        "postedDate": "2023-07-17T00:00:00",
        "company": {
            "id": 1,
            "email": "elandes0@mtv.com",
            "companyName": "Company1",
            "companyDescription": "string",
            "industry": "string",
            "website": "string"
        }
    }
]
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-7a62ac8a6c25afeb9ecd58378624580a-86a3745a8a6d650b-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas (404 Not Found):
```
Ad not found.
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId/ads/adId  | POST | Taip (company, admin) | JSON  | JSON | 201 Created, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found |

Sukuria naują skelbimą, priklausančią įmonei, kurios ID yra companyId, naudojant duomenims iš užklausos.

Užklausa: POST api/companies/1/ads
```
{
  "location": "Kaunas",
  "salaryFrom": "1234",
  "salaryTo": "4321",
  "title": "Job Title",
  "description": "Ad description"
}
```

Atsakymas (201 Created):
```
{
    "id": 35,
    "title": "Job Title",
    "description": "Ad description",
    "salaryFrom": "1234",
    "salaryTo": "4321",
    "location": "Kaunas",
    "postedDate": "2023-12-02T19:19:36.238157+00:00"
}
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-7a62ac8a6c25afeb9ecd58378624580a-86a3745a8a6d650b-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas: 401 Unauthorized

Atsakymas: 403 Forbidden

Atsakymas (404 Not Found):
```
Company not found.
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId/ads/adId  | PUT | Taip (company, admin) | JSON  | JSON | 200 OK, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found |

Atnaujina skelbimą, kurio ID yra adId, priklausančią įmonei, kurios ID yra companyId, naudojant duomenims iš užklausos.

Užklausa: PUT api/companies/1/ads/35
```
{
  "location": "New location",
  "salaryFrom": "123",
  "salaryTo": "321",
  "title": "New job title",
  "description": "New description"
}
```

Atsakymas (200 OK):
```
{
    "id": 35,
    "title": "New job title",
    "description": "New description",
    "salaryFrom": "123",
    "salaryTo": "321",
    "location": "New location",
    "postedDate": "2023-12-02T19:21:11.3909401+00:00"
}
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-7a62ac8a6c25afeb9ecd58378624580a-86a3745a8a6d650b-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas: 401 Unauthorized

Atsakymas: 403 Forbidden

Atsakymas (404 Not Found):
```
Company not found.
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId/ads/adId  | DELETE | Taip (company, admin) | Nėra  | JSON | 204 No Content, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found |

Ištrina skelbimą, kurio ID yra adId, priklausančią įmonei, kurios ID yra companyId.

Užklausa: DELETE api/companies/1/ads/35

Atsakymas: 204 No Content

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-7a62ac8a6c25afeb9ecd58378624580a-86a3745a8a6d650b-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas: 401 Unauthorized

Atsakymas: 403 Forbidden

Atsakymas (404 Not Found):
```
Company not found.
```

## 5.3 ApplicationController

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId/ads/adId/applications  | GET | Taip | Nėra  | JSON | 200 OK, 400 Bad Request, 404 Not Found |

Grąžina visų kandidatūrų, priklausančių skelbimui, kurio ID yra adId, priklausančio įmonei su ID companyId, sąrašą.

Užklausa: GET api/companies/1/ads/11/applications

Atsakymas (200 OK):
```
[
    {
        "id": 1,
        "adId": 11,
        "applicationDate": "2023-09-29T22:12:45.393",
        "coverLetter": "Test cover letter",
        "phoneNumber": "+48-588-325-3178"
    },
    {
        "id": 2,
        "adId": 11,
        "applicationDate": "2023-09-29T22:13:07.693",
        "coverLetter": "Test cover letter",
        "phoneNumber": "+48-588-325-3178"
    }
]
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-7a62ac8a6c25afeb9ecd58378624580a-86a3745a8a6d650b-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas (404 Not Found):
```
Company not found.
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId/ads/adId/applications/appId  | GET | Taip | Nėra  | JSON | 200 OK, 400 Bad Request, 404 Not Found |

Grąžina kandidatūros, kurios ID yra appId, priklausančiai skelbimui, kurio ID yra adId, priklausančio įmonei su ID companyId.

Užklausa: GET api/companies/1/ads/11/applications/1

Atsakymas (200 OK):
```
{
    "id": 1,
    "applicationDate": "2023-09-29T22:12:45.393",
    "coverLetter": "Test cover letter"
}
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-7a62ac8a6c25afeb9ecd58378624580a-86a3745a8a6d650b-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas (404 Not Found):
```
Application not found.
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId/ads/adId/applications | POST | Taip (candidate, admin) | JSON  | JSON | 201 Created, 400 Bad Request, 404 Not Found |

Sukuria naują kandidatūrą, priklausančią skelbimui, kurio ID yra adId, priklausančio įmonei su ID companyId, naudojant duomenims iš užklausos.

Užklausa: POST api/companies/1/ads/11/applications
```
{
  "coverLetter": "optional cover letter"
}
```

Atsakymas (201 Created):
```
{
    "id": 31,
    "coverLetter": "optional cover letter",
    "applicationDate": "2023-12-02T19:28:02.4865391+00:00"
}
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-7a62ac8a6c25afeb9ecd58378624580a-86a3745a8a6d650b-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas (404 Not Found):
```
Ad not found.
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId/ads/adId/applications/appId | PUT | Taip (candidate, admin) | JSON  | JSON | 200 OK, 400 Bad Request, 404 Not Found |

Atnaujina kandidatūrą, kurios ID yra appId, priklausančią skelbimui, kurio ID yra adId, priklausančio įmonei su ID companyId, naudojant duomenims iš užklausos.

Užklausa: PUT api/companies/1/ads/11/applications/31
```
{
  "coverLetter": "Updated cover letter"
}
```

Atsakymas (200 OK):
```
{
    "id": 31,
    "coverLetter": "Updated cover letter",
    "applicationDate": "2023-12-02T19:28:02.487"
}
```

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-7a62ac8a6c25afeb9ecd58378624580a-86a3745a8a6d650b-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas (404 Not Found):
```
Application not found.
```

***

| URL           | Metodas | Autentifikacija | Užklausos formatas | Atsakymo formatas | Atsako kodai |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| api/companies/companyId/ads/adId/applications/appId | DELETE | Taip (candidate, admin) | Nėra  | JSON | 204 No Content, 400 Bad Request, 404 Not Found |

Ištrina kandidatūrą, kurios ID yra appId, priklausančią skelbimui, kurio ID yra adId, priklausančio įmonei su ID companyId.

Užklausa: DELETE api/companies/1/ads/11/applications/31

Atsakymas: 204 No Content

Atsakymas (400 Bad Request):
```
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-7a62ac8a6c25afeb9ecd58378624580a-86a3745a8a6d650b-00",
    "errors": {
        "companyId": [
            "The value 'x' is not valid."
        ]
    }
}
```

Atsakymas (404 Not Found):
```
Application not found.
```


# 6. Išvados

Darbo skelbimų svetainės inžinierinis projektas buvo pilnai įgyvendintas pagal iš anksto nusprestus reikalavimus. Tam atlikti buvo pasitelkta C# programavimo kalba bei HTML ir CSS žymėjimo kalbos. Taip pat buvo naudojamos ASP.NET Core, Entity Framework, Blazor ir MudBlazor technologijos, o debesyje buvo talpinama svetainė Azure App servise, bei duomenų bazė Azure SQL serveryje. Patirties su dalimi iš šių technologijų neturėta, todėl procesas nebuvo lengvas, ypač kuriant naudotojo sąsają.

Įgyvendinimo procesas praplėtė esamas žinias apie saityno taikomųjų programų projektavimą, kurios bus naudingos ateityje asmeniniams projektams bei darbo rinkoje dirbant su panašiomis sistemomis. Nors ir sistema atitinka [1 skyriuje](#12-funkciniai-reikalavimai) aprašytus reikalavimus, prieš paleidžiant į rinką, reiktų praleisti papildomo laiko jos praplėtimui bei testavimui. 