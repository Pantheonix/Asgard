# Changelog

## 1.0.0 (2024-09-21)


### Features

* **anubis-judge0:** add nginx lb between anubis and judge0 replicas ([13fbc85](https://github.com/Pantheonix/Asgard/commit/13fbc85d6ea9a4c436484668d040932d560d7265))
* **cors:** enable cors policy for anubis, enki and quetzalcoatl ([6652dbc](https://github.com/Pantheonix/Asgard/commit/6652dbcdbfaceeb94c36369423fecb7b2682d9d5))
* **enki+anubis:** add pubsub support for eval metadata retrieval to improve performance ([35391c9](https://github.com/Pantheonix/Asgard/commit/35391c968ef2e91a89a86f20288890b866756bd7))
* **enki:** add delete problem endpoint (propagate deletion event against hermes too) ([f29eaa5](https://github.com/Pantheonix/Asgard/commit/f29eaa59984119224f0af2c2250265cbcb84e50e))


### Bug Fixes

* **enki:** allow proposer to keep the same name for an existing problem on problem update endpoint ([b1226a1](https://github.com/Pantheonix/Asgard/commit/b1226a17304bd8d334723aef6507e6b4373d78bd))
* **enki:** count items after filtering for correct pagination of problems (both published and unpublished) ([15a607b](https://github.com/Pantheonix/Asgard/commit/15a607bf6bb27f62149d8a85c0232b9f8c599e46))


### Performance Improvements

* **enki:** add authorization to GetListAsync and GetAsync methods ([be8e58b](https://github.com/Pantheonix/Asgard/commit/be8e58ba2d7b6a679aa67550428469cf1601304a))
