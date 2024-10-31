# Changelog

## 1.0.0 (2024-09-21)


### Features

* Add logging and error handling to refresh token endpoint ([fdfc9ba](https://github.com/Pantheonix/Asgard/commit/fdfc9baee1ff9d693e6221ea337fcf824a3a94a9))


### Bug Fixes

* **odin:** remove certs dependency as they are already set in lb ([92ec8a0](https://github.com/Pantheonix/Asgard/commit/92ec8a024559c4b5aaf9f60d68eb54d449b36f44))
* **odin:** remove https redirection ([11cdfb6](https://github.com/Pantheonix/Asgard/commit/11cdfb66ff40c5c4736a6f6a337cd11eeb6f0c70))
* **odin:** use correct name for access token ([d7aabcb](https://github.com/Pantheonix/Asgard/commit/d7aabcb7e8508fb095942a24e84547b020e89442))


### Performance Improvements

* **odin:** add https support ([17d199d](https://github.com/Pantheonix/Asgard/commit/17d199d6329cea36a0f0c363a144067c190ec750))
* **odin:** add lua filter for appending access/refresh tokens from/to requests/responses ([884b168](https://github.com/Pantheonix/Asgard/commit/884b1685d1a2067f8758b2d1fb6d1418f6c2c47f))
* **odin:** increase expiry time for newly created access tokens ([9e86619](https://github.com/Pantheonix/Asgard/commit/9e86619c839023e53060f6724eb380dbaf13703e))
* **prod:** update SameSite attribute for cookies to None ([7d143ad](https://github.com/Pantheonix/Asgard/commit/7d143adde34b824adc3a90f6a062bb5453038912))
