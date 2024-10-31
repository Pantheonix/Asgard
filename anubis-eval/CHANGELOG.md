# Changelog

## 0.1.0 (2024-09-21)


### Features

* **anubis-judge0:** add nginx lb between anubis and judge0 replicas ([13fbc85](https://github.com/Pantheonix/Asgard/commit/13fbc85d6ea9a4c436484668d040932d560d7265))
* **anubis:** add create/get test case dapr client methods ([35702ea](https://github.com/Pantheonix/Asgard/commit/35702ea227af61eb5a90e61bea5e2bf0a480e111))
* **anubis:** add endpoint for retrieving the highest score submissions per user (and problem if specified) ([251ff99](https://github.com/Pantheonix/Asgard/commit/251ff99bcc461c143114a37a04a0dac5891bb258))
* **cors:** enable cors policy for anubis, enki and quetzalcoatl ([6652dbc](https://github.com/Pantheonix/Asgard/commit/6652dbcdbfaceeb94c36369423fecb7b2682d9d5))
* **enki+anubis:** add pubsub support for eval metadata retrieval to improve performance ([35391c9](https://github.com/Pantheonix/Asgard/commit/35391c968ef2e91a89a86f20288890b866756bd7))


### Bug Fixes

* **anubis:** add cors preflight catcher ([80f5651](https://github.com/Pantheonix/Asgard/commit/80f5651f9ef37327db4edfc0f2c6d9d0a5337729))
* **anubis:** configure CORS policy for Rocket ([f25b830](https://github.com/Pantheonix/Asgard/commit/f25b83054e91ee37a52e16075e5c4e4bb0d85f98))
* **anubis:** fix compilation error in application error mapping ([54fa5ff](https://github.com/Pantheonix/Asgard/commit/54fa5ff4165f07ebfbe4f0ed1c2b5618697f69a7))
* **anubis:** remove unused import ([5984d6c](https://github.com/Pantheonix/Asgard/commit/5984d6cde4e9a4d2a32073b325c5d330d270f6ba))
* **anubis:** split submission batch into chunks ([8bc3f87](https://github.com/Pantheonix/Asgard/commit/8bc3f87e1440a456360a62fe606f9474869f2a49))
* **anubis:** update tests PK as the composition between id and problem_id ([87c7b5b](https://github.com/Pantheonix/Asgard/commit/87c7b5bd6c7ec9653071daabe9088b7f9e6cac8b))
* **submission source code:** show submission source code iff problem has been solved previously by user ([7b60949](https://github.com/Pantheonix/Asgard/commit/7b609495c69f89db2ad79e22b5c139b2790594f1))


### Performance Improvements

* **anubis:** add is_published field to submissions dtos ([a30c434](https://github.com/Pantheonix/Asgard/commit/a30c4348ae0505b34c1f69c16b45becee7a73937))
* **anubis:** add ocaml and lua support ([a8a79eb](https://github.com/Pantheonix/Asgard/commit/a8a79ebd32b973e936fc3f8890ff2d81a51f9ad2))
* **anubis:** add problem name to get all submissions endpoint response ([096f6b7](https://github.com/Pantheonix/Asgard/commit/096f6b70551eaafaa40d1e5d5b0713f926e110f0))
* **anubis:** add problem name to get submission by id endpoint response ([94fa6c6](https://github.com/Pantheonix/Asgard/commit/94fa6c619b183165de65c715b25a1f76dd03c707))
* **anubis:** improve http errors format using json ([d47cba0](https://github.com/Pantheonix/Asgard/commit/d47cba0b128c0e2ae8be36be16b9f47bea7cd046))
